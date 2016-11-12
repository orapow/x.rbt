using X.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using X.Core.Utility;
using Rbt.Svr.App;

namespace Rbt.Svr
{
    public class Wx
    {

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id"></param>
        public Wx(long id)
        {
            db = new RbtDBDataContext();
            lg = db.x_logon.FirstOrDefault(o => o.logon_id == id);
            code = lg.code;
            baseRequest = new BaseRequest();
            wc = new Wc();
        }

        #region 公开方法
        /// <summary>
        /// 开始运行
        /// 1、拉取二维码 loadQrcode
        /// 2、等待扫描和登陆 waitFor
        /// 3、初始化 wxInit
        /// 4、微信状态 wxStatusNotify
        /// 5、获取联系人和群聊 loadContact
        /// 6、循环发送心跳包 wxSync 间隔2秒
        /// </summary>
        public void Run()
        {
            isquit = false;

            while (!isquit)
            {
                var lr = loadQrcode();
                if (!lr) continue;

                Thread.Sleep(500);

                var wr = waitFor(1, 0);
                if (wr != 1) continue;

                if (string.IsNullOrEmpty(redirecturl)) continue;

                break;
            }

            if (!login()) { Exit(1); return; }

            if (!wxInit()) { Exit(1); return; };

            wxStatusNotify();
            //loadContact();

            lg.status = 6;//初始化完成
            db.SubmitChanges();

            while (!isquit) { SyncCheck(); Thread.Sleep(2 * 1000); }

        }

        /// <summary>
        /// 退出方法
        /// </summary>
        public void Quit()
        {
            Exit(0);
        }

        #endregion

        #region 公开属性

        public string code { get; private set; }

        public delegate void LoadQrHandler(string qrcode);
        public event LoadQrHandler LoadQr;

        public delegate void ScanedHandler(string headimg);
        public event ScanedHandler Scaned;

        public delegate void LogedHandler(Wx wx);
        public event LogedHandler Logded;

        public delegate void LogoutHandler(Wx wx);
        public event LogoutHandler Logout;

        #endregion

        #region 私有变量
        RbtDBDataContext db = null;
        x_logon lg = null;
        Wc wc = null;
        Tcp tcp = null;
        BaseRequest baseRequest = null;
        Contact user = null;//当前用户信息
        SyncKey _syncKey;
        static readonly DateTime BaseTime = new DateTime(1970, 1, 1);
        string uuid = "";
        string redirecturl = "";
        string passticket = "";
        string gateway = "";
        string synckey
        {
            get
            {
                string val = String.Empty;
                foreach (var item in _syncKey.List) val += String.Format("{0}_{1}|", item.Key, item.Val);
                return val.TrimEnd('|');
            }
        }
        bool isquit = false;
        #endregion

        #region 私有方法
        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        static long getcurrentseconds()
        {
            return (long)(DateTime.UtcNow - BaseTime).TotalMilliseconds;
        }

        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="msg"></param>
        void outLog(string msg)
        {
            Debug.WriteLine(lg.logon_id + "->" + msg);
        }

        /// <summary>
        /// 加载二维码
        /// </summary>
        /// <returns></returns>
        bool loadQrcode()
        {
            try
            {

                outLog("loaduuid");

                var rsp = wc.GetStr("https://login.weixin.qq.com/jslogin?appid=wx782c26e4c19acffb&redirect_uri=https%3A%2F%2Fwx2.qq.com%2Fcgi-bin%2Fmmwebwx-bin%2Fwebwxnewloginpage&fun=new&lang=zh_CN&_=" + getcurrentseconds());
                if (rsp.err) return false;

                outLog("loaduuid->" + Serialize.ToJson(rsp));

                var reg = new Regex(@"QRLogin.uuid = ""(\S+?)""");
                var m = reg.Match(rsp.data + "");
                if (!m.Success) return false;

                uuid = m.Groups[1].Value;
                lg.status = 2;//已获取UUID
                lg.uuid = uuid;
                db.SubmitChanges();

                Thread.Sleep(500);

                outLog("qrcode");
                rsp = wc.GetFile(String.Format("https://login.weixin.qq.com/qrcode/{0}?t=webwx&_={1}", uuid, getcurrentseconds()));
                outLog("qrcode->" + Serialize.ToJson(rsp));

                if (rsp.err) return false;

                lg.qrcode = "data:img/jpg;base64," + Convert.ToBase64String(rsp.data as byte[]);
                lg.status = 3;//已获取二维码
                db.SubmitChanges();

                LoadQr?.Invoke(lg.qrcode);

                return true;

            }
            catch { return false; }
        }

        /// <summary>
        /// 等待中。。。
        /// 1、扫描
        /// 2、登陆
        /// </summary>
        /// <param name="t"></param>
        /// <returns>
        /// 0 超时
        /// 1 已登陆
        /// </returns>
        int waitFor(int t, int c)
        {
            if (c >= 10) return 0;

            outLog("wait->" + t + "->" + c);
            string url = String.Format("https://login.weixin.qq.com/cgi-bin/mmwebwx-bin/login?loginicon=true&uuid={0}&tip=0&r={1}&_={2}", uuid, ~getcurrentseconds(), getcurrentseconds());
            var rsp = wc.GetStr(url);
            outLog("wait->" + t + "->" + c + "->" + Serialize.ToJson(rsp));

            if (rsp.err) return waitFor(t, c + 1);

            var str = rsp.data + "";

            if (str.Contains("code=201"))
            {
                var img = str.Split('\'')[1].TrimEnd('\'');
                lg.headimg = img;
                lg.status = 4;//使用中
                db.SubmitChanges();
                return waitFor(2, 0);
            }

            if (str.Contains("code=200"))
            {
                var reg = new Regex(@"window.redirect_uri=""(\S+?)""");
                redirecturl = reg.Match(str).Groups[1].Value;
                if (!String.IsNullOrEmpty(redirecturl)) gateway = "https://" + new Uri(redirecturl).Host;
                lg.status = 5;//已登陆
                db.SubmitChanges();
                return 1;
            }

            return waitFor(t, c + 1);

        }

        /// <summary>
        /// 获取用户登陆凭证
        /// </summary>
        bool login()
        {
            outLog("login");
            var rsp = wc.GetStr(redirecturl + "&fun=new&version=v2");
            outLog("login->" + Serialize.ToJson(rsp));

            if (rsp.err) return false;

            Regex reg = new Regex(@"<skey>(\S+?)</skey><wxsid>(\S+?)</wxsid><wxuin>(\d+)</wxuin><pass_ticket>(\S+?)</pass_ticket>");
            passticket = String.Empty;
            baseRequest = new BaseRequest();

            var m = reg.Match(rsp.data + "");
            if (!m.Success) return false;

            baseRequest.Skey = m.Groups[1].Value;
            baseRequest.Sid = m.Groups[2].Value;
            passticket = HttpUtility.UrlDecode(m.Groups[4].Value, Encoding.UTF8);

            lg.uin = baseRequest.Uin = Convert.ToInt64(m.Groups[3].Value);
            db.SubmitChanges();

            return true;
        }

        /// <summary>
        /// 登陆后初始化
        /// </summary>
        bool wxInit()
        {
            outLog("init");

            string url = String.Format("{0}/cgi-bin/mmwebwx-bin/webwxinit?pass_ticket={1}&skey={2}&r={3}", gateway, passticket, baseRequest.Skey, getcurrentseconds());
            var rsp = wc.PostData(url, Serialize.ToJson(new { BaseRequest = baseRequest }));

            outLog("init->" + Serialize.ToJson(rsp));

            if (rsp.err) return false;

            user = Serialize.FromJson<Contact>(rsp.data + "", "User");
            _syncKey = Serialize.FromJson<SyncKey>(rsp.data + "", "SyncKey");

            return true;
        }

        /// <summary>
        /// 更新状态提示
        /// </summary>
        void wxStatusNotify()
        {
            string url = String.Format("{0}/cgi-bin/mmwebwx-bin/webwxstatusnotify?lang=zh_CN&pass_ticket={1}", gateway, passticket);
            var o = new
            {
                BaseRequest = baseRequest,
                Code = 3,
                FromUserName = user.UserName,
                ToUserName = user.UserName,
                ClientMsgId = getcurrentseconds()
            };
            wc.PostData(url, Serialize.ToJson(o));
        }

        /// <summary>
        /// 加载通讯录
        /// </summary>
        void loadContact()
        {
            string url = String.Format("{0}/cgi-bin/mmwebwx-bin/webwxgetcontact?pass_ticket={1}&skey={2}&r={3}", gateway, passticket, baseRequest.Skey, getcurrentseconds());
            var json = wc.GetStr(url);
            //userlist = Com.FromJson<List<Contact>>(json, "MemberList");
            //grouplist = userlist.Where(o => o.NickName.StartsWith("@@")).ToList();
        }

        /// <summary>
        /// 同步检测
        /// </summary>
        void SyncCheck()
        {
            outLog("synccheck");

            var url = String.Format("{0}/cgi-bin/mmwebwx-bin/synccheck?r={1}&sid={2}&uin={3}&skey={4}&deviceid={5}&synckey={6}&_{7}", gateway, getcurrentseconds(), baseRequest.Sid, baseRequest.Uin, baseRequest.Skey, baseRequest.DeviceID, synckey, getcurrentseconds());
            var rsp = wc.GetStr(url);

            outLog("synccheck->" + Serialize.ToJson(rsp));

            if (rsp.err) { return; }

            var reg = new Regex("{retcode:\"(\\d+)\",selector:\"(\\d+)\"}");
            var m = reg.Match(rsp.data + "");
            if (!m.Success) { Exit(1); return; }

            var rt = int.Parse(m.Groups[1].Value);
            var sel = int.Parse(m.Groups[2].Value);

            if (isquit || rt != 0) Exit(1);
            else if (sel == 2) wxSync();

        }

        /// <summary>
        /// 消息同步
        /// </summary>
        void wxSync()
        {
            outLog("sync");

            string url = String.Format("{0}/cgi-bin/mmwebwx-bin/webwxsync?sid={1}&skey={2}&pass_ticket={3}", gateway, baseRequest.Sid, baseRequest.Skey, passticket);
            var o = new
            {
                BaseRequest = baseRequest,
                SyncKey = _syncKey,
                rr = getcurrentseconds()
            };
            var rsp = wc.PostData(url, Serialize.ToJson(o));

            outLog("sync->" + Serialize.ToJson(rsp));

            if (rsp.err) { Exit(1); return; }

            _syncKey = Serialize.FromJson<SyncKey>(rsp.data + "", "SyncKey");

            var msglist = Serialize.FromJson<List<Msg>>(rsp.data + "", "AddMsgList");

            foreach (var m in msglist) if (m.FromUserName != user.UserName) outLog("msg->" + user.Uin + "->" + m.Content);// Debug.WriteLine(user.Uin + "收到消息->" + m.MsgId + "--->>>" + m.Content);

        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="ToUserName"></param>
        /// <param name="Content"></param>
        void wxSendMsg(string ToUserName, string Content)
        {
            string url = String.Format("{0}/cgi-bin/mmwebwx-bin/webwxsendmsg?pass_ticket={1}", gateway, passticket);
            Content = Content.Replace("<br/>", "\n");
            var o = new
            {
                BaseRequest = baseRequest,
                Msg = new
                {
                    Type = 1,
                    Content = Content,
                    FromUserName = user.UserName,
                    ToUserName = ToUserName,
                    LocalID = getcurrentseconds(),
                    ClientMsgId = getcurrentseconds()
                }
            };

            var rsp = wc.PostData(url, Serialize.ToJson(o));

            outLog(Serialize.ToJson(rsp));

        }

        /// <summary>
        /// 发送图片
        /// </summary>
        /// <param name="ToUserName"></param>
        /// <param name="mmid"></param>
        void wxSendImg(string ToUserName, string mmid)
        {
            string url = String.Format("{0}/cgi-bin/mmwebwx-bin/webwxsendmsgimg?fun=async&f=json&pass_ticket={1}", gateway, passticket);
            var o = new
            {
                BaseRequest = baseRequest,
                Msg = new
                {
                    Type = 3,
                    MediaId = mmid,
                    FromUserName = user.UserName,
                    ToUserName = ToUserName,
                    LocalID = getcurrentseconds(),
                    ClientMsgId = getcurrentseconds()
                },
                Scene = 0
            };

            var rsp = wc.PostData(url, Serialize.ToJson(o));
            outLog("sendimg->" + Serialize.ToJson(rsp));

        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        string wxUpload(string img)
        {
            var fi = new FileInfo(img);
            if (!fi.Exists) return "";

            const string url = "https://file.wx2.qq.com/cgi-bin/mmwebwx-bin/webwxuploadmedia?f=json";
            var o = new
            {
                BaseRequest = baseRequest,
                ClientMediaId = getcurrentseconds(),
                TotalLen = fi.Length,
                StartPos = 0,
                DataLen = fi.Length,
                MediaType = 4
            };

            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("id", "WU_FILE_0");
            dict.Add("name", fi.Name);
            dict.Add("type", "image/jpeg");
            dict.Add("lastModifiedDate", fi.LastWriteTime.ToString());
            dict.Add("size", fi.Length + "");
            dict.Add("mediatype", "pic");
            dict.Add("uploadmediarequest", Serialize.ToJson(o));
            dict.Add("webwx_data_ticket", wc.GetCookie("webwx_data_ticket"));

            var rsp = wc.PostFile(url, dict, fi);
            outLog("upimg->" + Serialize.ToJson(rsp));

            if (rsp.err) return "";

            return Serialize.FromJson<string>(rsp.data + "", "MediaId");

        }

        /// <summary>
        /// 设置用户备注
        /// </summary>
        /// <param name="username"></param>
        /// <param name="mk"></param>
        void wxRemark(string username, string mk)
        {
            string url = String.Format("{0}/cgi-bin/mmwebwx-bin/webwxoplog", gateway);
            var o = new
            {
                BaseRequest = baseRequest,
                CmdId = 2,
                RemarkName = mk,
                UserName = username
            };

            var rsp = wc.PostData(url, Serialize.ToJson(o));

            outLog(Serialize.ToJson(rsp));
        }

        /// <summary>
        /// 群内加好友
        /// </summary>
        /// <param name="username"></param>
        void toFriend(string username, string hello)
        {
            string url = String.Format("{0}/cgi-bin/mmwebwx-bin/webwxverifyuser?r={1}", gateway, getcurrentseconds());

            var list = new List<object>();
            list.Add(new { Value = username, VerifyUserTicket = "" });

            var o = new
            {
                BaseRequest = baseRequest,
                Opcode = 2,
                SceneList = new int[] { 33 },
                SceneListCount = 1,
                VerifyContent = hello,
                VerifyUserList = Serialize.ToJson(list),
                VerifyUserListSize = list.Count,
                skey = baseRequest.Skey
            };

            var rsp = wc.PostData(url, Serialize.ToJson(o));

            outLog(Serialize.ToJson(rsp));
        }

        /// <summary>
        /// 删除群成员
        /// </summary>
        /// <param name="username"></param>
        void outMember(string groupname, string username)
        {
            string url = String.Format("{0}/cgi-bin/mmwebwx-bin/webwxupdatechatroom?fun=delmember", gateway);

            var o = new
            {
                BaseRequest = baseRequest,
                ChatRoomName = groupname,
                DelMemberList = username
            };

            var rsp = wc.PostData(url, Serialize.ToJson(o));

            outLog(Serialize.ToJson(rsp));
        }

        /// <summary>
        /// 添加群成员
        /// </summary>
        /// <param name="username"></param>
        void inMember(string groupname, List<string> userlist)
        {
            string url = String.Format("{0}/cgi-bin/mmwebwx-bin/webwxupdatechatroom?fun=addmember", gateway);

            var o = new
            {
                AddMemberList = string.Join(",", userlist),
                BaseRequest = baseRequest,
                ChatRoomName = groupname
            };

            var rsp = wc.PostData(url, Serialize.ToJson(o));

            outLog(Serialize.ToJson(rsp));
        }

        /// <summary>
        /// 创建群聊
        /// </summary>
        /// <param name="username"></param>
        void newGroup(string groupname, List<string> userlist)
        {
            string url = String.Format("{0}/cgi-bin/mmwebwx-bin/webwxcreatechatroom?r={1}", gateway, getcurrentseconds());

            var list = new List<object>();
            foreach (var u in userlist) list.Add(new { UserName = u });

            var o = new
            {
                BaseRequest = baseRequest,
                MemberCount = list.Count,
                MemberList = Serialize.ToJson(list),
                Topic = groupname
            };

            var rsp = wc.PostData(url, Serialize.ToJson(o));

            outLog(Serialize.ToJson(rsp));
        }

        /// <summary>
        /// 取消聊天置顶
        /// </summary>
        /// <param name="username"></param>
        void toTop(string username)
        {
            string url = String.Format("{0}/cgi-bin/mmwebwx-bin/webwxoplog", gateway);

            var o = new
            {
                BaseRequest = baseRequest,
                CmdId = 3,
                OP = 0,
                UserName = username
            };

            var rsp = wc.PostData(url, Serialize.ToJson(o));

            outLog(Serialize.ToJson(rsp));
        }

        /// <summary>
        /// 取消聊天置顶
        /// </summary>
        /// <param name="username"></param>
        void unTop(string username)
        {
            string url = String.Format("{0}/cgi-bin/mmwebwx-bin/webwxoplog", gateway);

            var o = new
            {
                BaseRequest = baseRequest,
                CmdId = 3,
                OP = 1,
                UserName = username
            };

            var rsp = wc.PostData(url, Serialize.ToJson(o));

            outLog(Serialize.ToJson(rsp));
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="c"></param>
        void Exit(int c)
        {
            outLog("exit");

            isquit = true;

            if (lg != null)
            {
                lg.uuid = null;
                lg.uin = null;
                lg.qrcode = null;
                lg.headimg = null;
                lg.status = 1;
            }

            try
            {
                db.SubmitChanges();
                db.Dispose();
            }
            catch { }

            if (c == 1) Logout?.Invoke(this);

        }

        #endregion

        #region 私有类
        /// <summary>
        /// 请求基础参数
        /// </summary>
        class BaseRequest
        {
            public BaseRequest()
            {
                DeviceID = "e" + getcurrentseconds();// "e84617712" + DateTime.Now.ToString("fffffff");
            }
            public long Uin { get; set; }
            public string Sid { get; set; }
            public string Skey { get; set; }
            public string DeviceID { get; set; }
        }
        /// <summary>
        /// 同步Key
        /// </summary>
        class SyncKey
        {
            public class KeyValuePair
            {
                public int Key { get; set; }
                public int Val { get; set; }
            }
            public int Count { get; set; }
            public IList<KeyValuePair> List { get; set; }
        }
        /// <summary>
        /// 联系人实体
        /// </summary>
        class Contact
        {
            public long Uin { get; set; }
            public string UserName { get; set; }
            public string NickName { get; set; }
            /// <summary>
            /// 1-好友， 2-群组， 3-公众号
            /// </summary>
            public int ContactFlag { get; set; }
            public int MemberCount { get; set; }
            public List<Contact> MemberList { get; set; }
            public string Signature { get; set; }
            public string RemarkName { get; set; }
            public string HeadImgUrl { get; set; }
            public string EncryChatRoomId { get; set; }
        }
        /// <summary>
        /// 消息关体
        /// </summary>
        class Msg
        {
            public string MsgId { get; set; }
            public string FromUserName { get; set; }
            public string ToUserName { get; set; }
            public int MsgType { get; set; }
            public string Content { get; set; }
        }
        #endregion

    }
}