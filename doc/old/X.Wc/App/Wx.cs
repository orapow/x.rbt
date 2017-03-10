using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using X.Core.Utility;
using System.IO;
using System.Web;

namespace X.Wc.App
{
    public class Wx
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id"></param>
        public Wx()
        {
            baseRequest = new BaseRequest();
            wc = new Http();
        }

        #region 公开方法
        /// <summary>
        /// 开始运行
        /// </summary>
        public void Run()
        {
            isquit = false;

            try
            {
                loadQrcode();
                waitFor(1, 0);

                wxLogin();
                wxInit();
                wxStatusNotify();

                op = new Http(wc.Cookies, 5);

                loadContact();
            }
            catch (Exception ex)
            {
                outLog("wx.Run->" + ex.Message);
                exit(1);
            }

            new Thread(o =>
            {
                while (!isquit) SyncCheck();

            }).Start();

            ///加载用户图片
            //new Thread(o =>
            //{
            //    var hw = new Wc(wc.Cookies, 5);
            //    var haserr = false;
            //    var page = 1;
            //    while (!isquit)
            //    {
            //        haserr = false;
            //        page = 1;
            //        var cs = db.x_contact.Where(c => c.uin == user.Uin && c.headimg == null).Skip((page - 1) * 10).Take(10);
            //        foreach (var c in cs)
            //        {
            //            var rp = hw.GetFile(gateway + "/cgi-bin/mmwebwx-bin/webwxget" + (c.username[1] == '@' ? "headimg" : "icon") + "?username=" + c.username);
            //            outLog("getimg->" + c.nickname + "(" + c.contact_id + ") 获取" + (rp.err ? "失败" : "成功"));
            //            if (!rp.err) c.headimg = "data:img/jpg;base64," + Convert.ToBase64String(rp.data as byte[]);
            //            else { haserr = true; break; }
            //            page++;
            //        };
            //        try
            //        {
            //            db.SubmitChanges();
            //        }
            //        catch { }
            //        if (haserr) Thread.Sleep(5 * 60 * 1000);
            //        else Thread.Sleep(5 * 1000);
            //        if (db.x_contact.Count(c => c.uin == user.Uin && c.headimg == null) == 0) { outLog("图片同步完成"); break; }
            //    };

            //}).Start();

        }

        /// <summary>
        /// 退出方法
        /// </summary>
        public void Quit()
        {
            if (user == null) exit(1);
            else
            {
                logonOut();
                exit(0);
            }
        }

        /// <summary>
        /// 获取用户头像
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public string GetHeadImage(string username)
        {
            var hw = new Http(wc.Cookies, 3);
            var rp = hw.GetFile(gateway + "/cgi-bin/mmwebwx-bin/webwxget" + (username[1] == '@' ? "headimg" : "icon") + "?username=" + username);
            if (!rp.err) return Convert.ToBase64String(rp.data as byte[]);
            else { outLog("GetHeadImage->" + rp.msg); return ""; }
        }

        public void Send(List<string> touser, int type, string content)
        {
            var mmid = "0";
            if (type == 2) mmid = uploadImg(content);
            if (string.IsNullOrEmpty(mmid)) { outLog("send->图片上传失败"); return; }

            var i = 1;
            foreach (var u in touser)
            {
                var rt = false;
                if (type == 1) rt = sendText(u, content);
                else if (type == 2) rt = sendImg(u, mmid);
                i++;
                if (!rt) Thread.Sleep(Tools.GetRandNext(5000, 30000));
                else if (i == 5) { Thread.Sleep(Tools.GetRandNext(3000, 8000)); i = 1; }
                else Thread.Sleep(Tools.GetRandNext(1500, 3000));
            }
            outLog("send->发送完成，共发给" + touser.Count() + "人");
        }

        /// <summary>
        /// 设置用户备注
        /// </summary>
        /// <param name="username"></param>
        /// <param name="mk"></param>
        public void SetRemark(string username, string mk)
        {
            string url = String.Format("{0}/cgi-bin/mmwebwx-bin/webwxoplog", gateway);
            var o = new
            {
                BaseRequest = baseRequest,
                CmdId = 2,
                RemarkName = mk,
                UserName = username
            };

            var rsp = op.PostStr(url, Serialize.ToJson(o));
            outLog("SetRemark->" + Serialize.ToJson(rsp));

        }

        /// <summary>
        /// 群内加好友
        /// </summary>
        /// <param name="username"></param>
        public void ToFriend(string username, string hello)
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

            var rsp = op.PostStr(url, Serialize.ToJson(o));
            outLog("ToFriend->" + Serialize.ToJson(rsp));

        }

        /// <summary>
        /// 删除群成员
        /// </summary>
        /// <param name="username"></param>
        public void OutMember(string groupname, string username)
        {
            string url = String.Format("{0}/cgi-bin/mmwebwx-bin/webwxupdatechatroom?fun=delmember", gateway);

            var o = new
            {
                BaseRequest = baseRequest,
                ChatRoomName = groupname,
                DelMemberList = username
            };

            var rsp = op.PostStr(url, Serialize.ToJson(o));
            outLog("OutMember->" + Serialize.ToJson(rsp));

        }

        /// <summary>
        /// 添加群成员
        /// </summary>
        /// <param name="username"></param>
        public void InMember(string groupname, List<string> userlist)
        {
            string url = String.Format("{0}/cgi-bin/mmwebwx-bin/webwxupdatechatroom?fun=addmember", gateway);

            var o = new
            {
                AddMemberList = string.Join(",", userlist),
                BaseRequest = baseRequest,
                ChatRoomName = groupname
            };

            var rsp = op.PostStr(url, Serialize.ToJson(o));
            outLog("InMember->" + Serialize.ToJson(rsp));

        }

        /// <summary>
        /// 创建群聊
        /// </summary>
        /// <param name="username"></param>
        public void NewGroup(string groupname, List<string> userlist)
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

            var rsp = op.PostStr(url, Serialize.ToJson(o));
            outLog("NewGroup->" + Serialize.ToJson(rsp));

        }

        /// <summary>
        /// 取消聊天置顶
        /// </summary>
        /// <param name="username"></param>
        public void ToTop(string username)
        {
            string url = String.Format("{0}/cgi-bin/mmwebwx-bin/webwxoplog", gateway);

            var o = new
            {
                BaseRequest = baseRequest,
                CmdId = 3,
                OP = 0,
                UserName = username
            };

            var rsp = op.PostStr(url, Serialize.ToJson(o));
            outLog("ToTop->" + Serialize.ToJson(rsp));

        }

        /// <summary>
        /// 取消聊天置顶
        /// </summary>
        /// <param name="username"></param>
        public void UnTop(string username)
        {
            string url = String.Format("{0}/cgi-bin/mmwebwx-bin/webwxoplog", gateway);

            var o = new
            {
                BaseRequest = baseRequest,
                CmdId = 3,
                OP = 1,
                UserName = username
            };

            var rsp = op.PostStr(url, Serialize.ToJson(o));
            outLog("UnTop->" + Serialize.ToJson(rsp));

        }

        #endregion

        #region 公开事件
        public delegate void LoadQrHandler(string qrcode);
        public event LoadQrHandler LoadQr;

        public delegate void ScanedHandler(string hdimg);
        public event ScanedHandler Scaned;

        public delegate void LogedHandler(Contact user);
        public event LogedHandler Loged;

        public delegate void LogonOutHandler();
        public event LogonOutHandler LogonOut;

        public delegate void NewMsgHandler(Msg m);
        public event NewMsgHandler NewMsg;

        public delegate void LogerHandler(string log);
        public event LogerHandler OutLog;

        public delegate void ContactLoadedHandler(List<Contact> contacts);
        public event ContactLoadedHandler ContactLoaded;

        #endregion

        #region 私有变量
        Http wc = null;
        Http op = null;
        BaseRequest baseRequest = null;
        Contact user = null;//当前用户信息
        SyncKey _syncKey;
        List<Contact> contacts = null;
        //static readonly DateTime BaseTime = new DateTime(1970, 1, 1);
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
        int filecount = 0;
        #endregion

        #region 私有方法
        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        static long getcurrentseconds()
        {
            return long.Parse(Tools.GetGreenTime("")); //(long)(DateTime.UtcNow - BaseTime).TotalMilliseconds;
        }

        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="msg"></param>
        void outLog(string msg)
        {
            Console.WriteLine("log@" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":->" + msg);
            Debug.WriteLine("log@" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":->" + msg);

            OutLog?.Invoke("log@" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "->" + msg);
        }

        /// <summary>
        /// 加载二维码
        /// </summary>
        /// <returns></returns>
        void loadQrcode()
        {
            var rsp = wc.GetStr("https://login.weixin.qq.com/jslogin?appid=wx782c26e4c19acffb&fun=new&lang=zh_CN&_=" + getcurrentseconds());//&redirect_uri=https%3A%2F%2Fwx2.qq.com%2Fcgi-bin%2Fmmwebwx-bin%2Fwebwxnewloginpage
            if (rsp.err) throw new Exception("uuid获取失败" + Serialize.ToJson(rsp));

            var reg = new Regex("\"(\\S+?)\"");
            var m = reg.Match(rsp.data + "");
            if (rsp.err) throw new Exception("uuid获取失败->" + Serialize.ToJson(rsp));

            uuid = m.Groups[1].Value;
            outLog("uuid->" + uuid);

            rsp = wc.GetFile(string.Format("https://login.weixin.qq.com/qrcode/{0}?_={1}", uuid, getcurrentseconds()));

            if (rsp.err) throw new Exception("qrcode获取失败->" + Serialize.ToJson(rsp));
            var qrcode = Convert.ToBase64String(rsp.data as byte[]);

            outLog("qrcode->" + qrcode);
            LoadQr?.Invoke(qrcode);
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
        void waitFor(int t, int c)
        {

            if (c >= 2 || isquit) throw new Exception("wait 已退出");

            string url = String.Format("https://login.weixin.qq.com/cgi-bin/mmwebwx-bin/login?loginicon=true&uuid={0}&tip=0&r={1}&_={2}", uuid, ~getcurrentseconds(), getcurrentseconds());
            var rsp = wc.GetStr(url);

            outLog("wait->" + t + "->" + c + "->" + Serialize.ToJson(rsp));

            if (rsp.err) waitFor(t, c + 1);

            var str = rsp.data + "";

            if (str.Contains("code=201"))
            {
                var img = str.Split('\'')[1].TrimEnd('\'').Replace("data:img/jpg;base64,", "");
                //lg.headimg = headimg = img;
                //lg.status = 4;//已扫描
                //db.SubmitChanges();
                Scaned?.Invoke(img);
                waitFor(2, 0);
            }

            if (str.Contains("code=200"))
            {
                var reg = new Regex("window.redirect_uri=\"(\\S+?)\"");
                redirecturl = reg.Match(str).Groups[1].Value;
                if (!String.IsNullOrEmpty(redirecturl)) gateway = "https://" + new Uri(redirecturl).Host;
                //lg.status = 5;//已登陆
                //db.SubmitChanges();
                return;
            }

            waitFor(t, c + 1);

        }

        /// <summary>
        /// 获取用户登陆凭证
        /// </summary>
        void wxLogin()
        {
            var rsp = wc.GetStr(redirecturl + "&fun=new&version=v2");

            outLog("login->" + Serialize.ToJson(rsp));

            if (rsp.err) if (rsp.err) throw new Exception("登陆失败->" + Serialize.ToJson(rsp));

            Regex reg = new Regex(@"<skey>(\S+?)</skey><wxsid>(\S+?)</wxsid><wxuin>(\d+)</wxuin><pass_ticket>(\S+?)</pass_ticket>");
            passticket = String.Empty;
            baseRequest = new BaseRequest();

            var m = reg.Match(rsp.data + "");

            baseRequest.Skey = m.Groups[1].Value;
            baseRequest.Sid = m.Groups[2].Value;
            passticket = HttpUtility.UrlDecode(m.Groups[4].Value, Encoding.UTF8);

            baseRequest.Uin = Convert.ToInt64(m.Groups[3].Value);
            outLog("loged->" + baseRequest.Uin);

        }

        /// <summary>
        /// 登陆后初始化
        /// </summary>
        void wxInit()
        {

            string url = String.Format("{0}/cgi-bin/mmwebwx-bin/webwxinit?pass_ticket={1}&skey={2}&r={3}", gateway, passticket, baseRequest.Skey, getcurrentseconds());
            var rsp = wc.PostStr(url, Serialize.ToJson(new { BaseRequest = baseRequest }));

            outLog("init->" + Serialize.ToJson(rsp));

            if (rsp.err) throw new Exception("初始化失败->" + Serialize.ToJson(rsp));

            user = Serialize.FromJson<Contact>(rsp.data + "", "User");
            _syncKey = Serialize.FromJson<SyncKey>(rsp.data + "", "SyncKey");

            //lg.nickname = user.NickName;
            //db.SubmitChanges();

            Loged?.Invoke(user);
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
            var rsp = wc.PostStr(url, Serialize.ToJson(o));
            outLog("notify->" + Serialize.ToJson(rsp));
        }

        /// <summary>
        /// 获取通讯录
        /// </summary>
        void loadContact()
        {
            string url = String.Format("{0}/cgi-bin/mmwebwx-bin/webwxgetcontact?pass_ticket={1}&skey={2}&r={3}", gateway, passticket, baseRequest.Skey, getcurrentseconds());
            var rsp = wc.GetStr(url);
            if (rsp.err) return;
            contacts = Serialize.FromJson<List<Contact>>(rsp.data + "", "MemberList").Where(o => o.KeyWord != "gh_").ToList();

            var nks = new Dictionary<string, int>();
            var qgps = from g in contacts
                       where g.UserName[1] == '@'
                       select g;  //toContact(contacts, null, nks, 0);

            for (var i = 1; i <= Math.Ceiling(qgps.Count() / 50.0); i++)
            {
                var gs = contacts.Skip((i - 1) * 50)
                    .Take(50)
                    .Select(o => new
                    {
                        EncryChatRoomId = o.EncryChatRoomId,
                        UserName = o.UserName
                    });

                if (gs.Count() == 0) break;

                rsp = wc.PostStr(gateway + "/cgi-bin/mmwebwx-bin/webwxbatchgetcontact?type=ex&r=" + getcurrentseconds(),//群组
                    Serialize.ToJson(new
                    {
                        BaseRequest = baseRequest,
                        Count = gs.Count(),
                        List = gs.ToList()
                    }));

                if (rsp.err) { outLog("群组获取失败"); break; }

                foreach (var c in Serialize.FromJson<List<Contact>>(rsp.data + "", "ContactList"))
                {
                    var g = contacts.FirstOrDefault(o => o.UserName == c.UserName);
                    g = c;
                }
            }

            outLog("通讯录获取完成！");
            ContactLoaded?.Invoke(contacts);
        }

        /// <summary>
        /// 同步检测
        /// </summary>
        void SyncCheck()
        {

            var url = String.Format("{0}/cgi-bin/mmwebwx-bin/synccheck?r={1}&sid={2}&uin={3}&skey={4}&deviceid={5}&synckey={6}&_{7}", gateway, getcurrentseconds(), baseRequest.Sid, baseRequest.Uin, baseRequest.Skey, baseRequest.DeviceID, synckey, getcurrentseconds());
            var rsp = wc.GetStr(url);

            outLog("synccheck->" + Serialize.ToJson(rsp));

            var reg = new Regex("{retcode:\"(\\d+)\",selector:\"(\\d+)\"}");
            var m = reg.Match(rsp.data + "");

            var rt = int.Parse(m.Groups[1].Value);
            var sel = int.Parse(m.Groups[2].Value);

            if (isquit || rt != 0) exit(1);
            else if (sel == 2 || sel == 4 || sel == 6) wxSync();

        }

        /// <summary>
        /// 消息同步
        /// </summary>
        void wxSync()
        {

            string url = String.Format("{0}/cgi-bin/mmwebwx-bin/webwxsync?sid={1}&skey={2}&pass_ticket={3}", gateway, baseRequest.Sid, baseRequest.Skey, passticket);
            var o = new
            {
                BaseRequest = baseRequest,
                SyncKey = _syncKey,
                rr = getcurrentseconds()
            };
            var rsp = wc.PostStr(url, Serialize.ToJson(o));

            outLog("sync->" + Serialize.ToJson(rsp));

            if (rsp.err) { throw new Exception("消息获取失败->" + Serialize.ToJson(rsp)); }

            _syncKey = Serialize.FromJson<SyncKey>(rsp.data + "", "SyncKey");

            var msglist = Serialize.FromJson<List<Msg>>(rsp.data + "", "AddMsgList");

            foreach (var m in msglist)
            {
                if (m.FromUserName == user.UserName) continue;



                //outLog("msg->" + user.Uin + "->" + m.Content);

                //if (m.Content.Contains("你的朋友验证请求，") && m.Content.Contains("可以开始聊天了"))
                //{
                //    var rp = db.x_reply.Where(r => r.user_id == lg.user_id);
                //}

                //var rps = db.x_reply.FirstOrDefault(o => o.keys)

                NewMsg?.Invoke(m);
            }

            // Debug.WriteLine(user.Uin + "收到消息->" + m.MsgId + "--->>>" + m.Content);

        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="ToUserName"></param>
        /// <param name="Content"></param>
        bool sendText(string ToUserName, string Content)
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

            var rsp = op.PostStr(url, Serialize.ToJson(o));
            var rp = Serialize.FromJson<BaseResponse>(rsp.data + "", "BaseResponse");
            outLog("SendMsg->" + Serialize.ToJson(rsp));
            return rp.Ret == "0";
        }

        /// <summary>
        /// 发送图片
        /// </summary>
        /// <param name="ToUserName"></param>
        /// <param name="url"></param>
        bool sendImg(string ToUserName, string mmid)
        {
            string api = String.Format("{0}/cgi-bin/mmwebwx-bin/webwxsendmsgimg?fun=async&f=json&pass_ticket={1}", gateway, passticket);
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

            var rsp = op.PostStr(api, Serialize.ToJson(o));
            var rp = Serialize.FromJson<BaseResponse>(rsp.data + "", "BaseResponse");
            outLog("SendImg->" + Serialize.ToJson(rsp));
            return rp.Ret == "0";

        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="imgaddr"></param>
        /// <returns></returns>
        string uploadImg(string url)
        {
            var fs = op.GetFile(url);
            if (fs.err) return "";
            var data = fs.data as byte[];//File.ReadAllBytes(imgaddr);
            string api = gateway.Replace("https://", "https://file.") + "/cgi-bin/mmwebwx-bin/webwxuploadmedia?f=json";
            var o = new
            {
                UploadType = 2,
                BaseRequest = baseRequest,
                ClientMediaId = getcurrentseconds(),
                TotalLen = data.Length,
                StartPos = 0,
                DataLen = data.Length,
                MediaType = 4
            };

            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("id", "WU_FILE_" + filecount++);
            dict.Add("name", url.Substring(url.LastIndexOf('/') + 1));
            dict.Add("type", Tools.GetMimeType(url.Substring(url.LastIndexOf('.') + 1)));
            dict.Add("lastModifiedDate", DateTime.Now.ToString("r"));//DateTime.Now.ToString("r")
            dict.Add("size", data.Length + "");
            dict.Add("mediatype", "pic");
            dict.Add("uploadmediarequest", Serialize.ToJson(o));
            dict.Add("webwx_data_ticket", wc.GetCookie("webwx_data_ticket"));
            dict.Add("pass_ticket", passticket);

            var rsp = op.PostFile(api, dict, data);
            outLog("UploadImg->" + Serialize.ToJson(rsp));

            if (rsp.err) return "";

            return Serialize.FromJson<string>(rsp.data + "", "MediaId");

        }

        /// <summary>
        /// 登出
        /// </summary>
        void logonOut()
        {
            if (op == null) return;
            string api = String.Format("{0}/cgi-bin/mmwebwx-bin/webwxlogout?redirect=0&type=1&skey={1}", gateway, baseRequest.Skey);
            var o = new
            {
                sid = baseRequest.Sid,
                uin = user.Uin
            };
            op.PostStr(api, Serialize.ToJson(o));
            outLog("logout");
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="c"></param>
        void exit(int c)
        {
            isquit = true;
            outLog("exit");
            logonOut();
            if (c == 1) LogonOut?.Invoke();
        }

        #endregion

        /// <summary>
        /// 消息
        /// </summary>
        public class Msg
        {
            public string MsgId { get; set; }
            public string FromUserName { get; set; }
            public string ToUserName { get; set; }
            public int MsgType { get; set; }
            public string Content { get; set; }
        }

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
        class BaseResponse
        {
            public string Ret { get; set; }
            public string ErrMsg { get; set; }
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
        public class Contact
        {
            public long Uin { get; set; }
            public string Alias { get; set; }
            public string UserName { get; set; }
            public string NickName { get; set; }
            public string KeyWord { get; set; }
            public int MemberCount { get; set; }
            public List<Contact> MemberList { get; set; }
            public string Signature { get; set; }
            public string RemarkName { get; set; }
            public string DisplayName { get; set; }
            public string HeadImgUrl { get; set; }
            public string EncryChatRoomId { get; set; }
        }
        #endregion

    }
}