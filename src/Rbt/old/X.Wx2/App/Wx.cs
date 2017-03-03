using X.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using X.Core.Utility;
using System.Data.Linq;

namespace X.Wx.App
{
    public class Wx
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id"></param>
        public Wx(long id)
        {
            db = new RbtDBDataContext() { DeferredLoadingEnabled = true };
            lg = db.x_logon.FirstOrDefault(o => o.logon_id == id);
            ukey = lg.x_user.ukey;
            baseRequest = new BaseRequest();
            wc = new Wc();
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

                op = new Wc(wc.Cookies, 5);

                lg.status = 6;//初始化完成
                db.SubmitChanges();

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
            new Thread(o =>
            {
                var hw = new Wc(wc.Cookies, 5);
                var haserr = false;
                var page = 1;
                while (!isquit)
                {
                    haserr = false;
                    page = 1;
                    var cs = db.x_contact.Where(c => c.uin == user.Uin && c.headimg == null).Skip((page - 1) * 10).Take(10);
                    foreach (var c in cs)
                    {
                        var rp = hw.GetFile(gateway + "/cgi-bin/mmwebwx-bin/webwxget" + (c.username[1] == '@' ? "headimg" : "icon") + "?username=" + c.username);
                        outLog("getimg->" + c.nickname + "(" + c.contact_id + ") 获取" + (rp.err ? "失败" : "成功"));
                        if (!rp.err) c.headimg = "data:img/jpg;base64," + Convert.ToBase64String(rp.data as byte[]);
                        else { haserr = true; break; }
                        page++;
                    };
                    try
                    {
                        db.SubmitChanges();
                    }
                    catch { }
                    if (haserr) Thread.Sleep(5 * 60 * 1000);
                    else Thread.Sleep(5 * 1000);
                    if (db.x_contact.Count(c => c.uin == user.Uin && c.headimg == null) == 0) { outLog("图片同步完成"); break; }
                };

            }).Start();

        }

        /// <summary>
        /// 退出方法
        /// </summary>
        public void Quit()
        {
            exit(0);
        }

        public void Send(List<string> touser, int type, string content)
        {
            var mmid = "0";
            if (type == 2) mmid = uploadImg(content);
            if (string.IsNullOrEmpty(mmid)) { outLog("send->图片上传失败"); return; }

            foreach (var u in touser)
            {
                if (type == 1) sendText(u, content);
                else if (type == 2) sendImg(u, mmid);
                Thread.Sleep(100);
            }

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

        #region 公开属性
        public long lgid { get { return lg.logon_id; } }
        public string ukey { get; private set; }
        public string qrcode { get; private set; }
        public string headimg { get; private set; }
        #endregion

        #region 公开事件
        public delegate void LoadQrHandler(string qrcode);
        public event LoadQrHandler LoadQr;

        public delegate void ScanedHandler(string hdimg);
        public event ScanedHandler Scaned;

        public delegate void LogedHandler();
        public event LogedHandler Loged;

        public delegate void LogoutHandler();
        public event LogoutHandler Logout;

        public delegate void NewMsgHandler(Msg m);
        public event NewMsgHandler NewMsg;

        #endregion

        #region 私有变量
        RbtDBDataContext db = null;
        x_logon lg = null;
        Wc wc = null;
        Wc op = null;
        BaseRequest baseRequest = null;
        Contact user = null;//当前用户信息
        SyncKey _syncKey;
        List<Contact> conts = null;
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
        int filecount = 0;
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
            Debug.WriteLine("log@" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":->" + msg);
            //OutLog?.Invoke((lg.uin > 0 ? lg.uin.Value : lg.logon_id) + "->" + msg);
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
            lg.uuid = uuid;

            outLog("uuid->" + uuid);

            rsp = wc.GetFile(String.Format("https://login.weixin.qq.com/qrcode/{0}?_={1}", uuid, getcurrentseconds()));

            if (rsp.err) throw new Exception("qrcode获取失败->" + Serialize.ToJson(rsp));

            lg.qrcode = qrcode = "data:img/jpg;base64," + Convert.ToBase64String(rsp.data as byte[]);
            lg.status = 3;//已获取二维码
            db.SubmitChanges();

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

            if (c >= 5 || isquit) throw new Exception("wait 已退出");

            string url = String.Format("https://login.weixin.qq.com/cgi-bin/mmwebwx-bin/login?loginicon=true&uuid={0}&tip=0&r={1}&_={2}", uuid, ~getcurrentseconds(), getcurrentseconds());
            var rsp = wc.GetStr(url);

            outLog("wait->" + t + "->" + c + "->" + Serialize.ToJson(rsp));

            if (rsp.err) waitFor(t, c + 1);

            var str = rsp.data + "";

            if (str.Contains("code=201"))
            {
                var img = str.Split('\'')[1].TrimEnd('\'');
                lg.headimg = headimg = img;
                lg.status = 4;//已扫描
                db.SubmitChanges();

                Scaned?.Invoke(headimg);

                waitFor(2, 0);
            }

            if (str.Contains("code=200"))
            {
                var reg = new Regex("window.redirect_uri=\"(\\S+?)\"");
                redirecturl = reg.Match(str).Groups[1].Value;
                if (!String.IsNullOrEmpty(redirecturl)) gateway = "https://" + new Uri(redirecturl).Host;
                lg.status = 5;//已登陆
                db.SubmitChanges();
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

            lg.uin = baseRequest.Uin = Convert.ToInt64(m.Groups[3].Value);
            db.SubmitChanges();

            outLog("loged->" + lg.uin);

            Loged?.Invoke();

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

            lg.nickname = user.NickName;
            db.SubmitChanges();

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
            conts = Serialize.FromJson<List<Contact>>(rsp.data + "", "MemberList").Where(o => o.KeyWord != "gh_").ToList();

            var nks = new Dictionary<string, int>();

            var gps = toContact(conts, nks, 0);

            rsp = wc.PostStr(gateway + "/cgi-bin/mmwebwx-bin/webwxbatchgetcontact?type=ex&r=" + getcurrentseconds(),//群组
                Serialize.ToJson(new
                {
                    BaseRequest = baseRequest,
                    Count = gps.Count,
                    List = gps
                }));

            if (rsp.err) { outLog("群组获取失败"); return; }
            conts = Serialize.FromJson<List<Contact>>(rsp.data + "", "ContactList");

            foreach (var c in conts)
            {
                var d = db.x_contact.FirstOrDefault(o => o.username == c.UserName);
                d.membercount = c.MemberCount;
                d.roomid = c.EncryChatRoomId;
                toContact(c.MemberList, nks, d.contact_id);
            }

            outLog("通讯录获取完成！");

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="cts"></param>
        /// <param name="nks"></param>
        /// <param name="t"></param>
        List<object> toContact(List<Contact> list, Dictionary<string, int> nks, long gpid)
        {
            var cts = new List<x_contact>();
            var gps = new List<object>();
            foreach (var c in list)
            {
                var n = c.NickName;
                if (!string.IsNullOrEmpty(c.RemarkName)) n = c.RemarkName;
                if (gpid > 0) n += gpid;
                n += user.Uin;
                if (nks.ContainsKey(n)) { c.RemarkName = c.NickName + (++nks[n]).ToString("000"); }//SetRemark(c.UserName, c.RemarkName);
                else { nks.Add(n, 0); c.RemarkName = n; }

                var no = Secret.MD5(n);
                var ct = db.x_contact.FirstOrDefault(t => t.no == no && t.uin == user.Uin);
                if (ct == null) ct = new x_contact() { uin = user.Uin, no = no };

                ct.user_id = lg.user_id;
                ct.remarkname = c.RemarkName;
                ct.nickname = Tools.RemoveHtml(c.NickName);
                ct.flag = c.UserName[1] == '@' ? 2 : 1;
                ct.group_id = gpid;
                ct.membercount = c.MemberCount;
                ct.signature = c.Signature;
                ct.username = c.UserName;
                ct.imgurl = c.HeadImgUrl;

                var m = new Regex("(\\d{11})").Match(c.NickName);
                if (m.Success) ct.tel = m.Groups[1].Value;
                if (ct.contact_id == 0) cts.Add(ct);

                if (c.UserName[1] == '@') gps.Add(new
                {
                    EncryChatRoomId = c.EncryChatRoomId,
                    UserName = c.UserName
                });

            }
            if (cts.Count > 0) db.x_contact.InsertAllOnSubmit(cts);
            try
            {
                db.SubmitChanges(ConflictMode.ContinueOnConflict);
                db.SubmitChanges(ConflictMode.FailOnFirstConflict);
            }
            catch (ChangeConflictException)
            {
                foreach (ObjectChangeConflict occ in db.ChangeConflicts)
                {
                    occ.Resolve(RefreshMode.KeepChanges);
                }
            }
            catch (Exception ex)
            {
                outLog("toContact->err:" + ex.Message);
            }
            return gps;
        }

        /// <summary>
        /// 同步检测
        /// </summary>
        void SyncCheck()
        {

            var url = String.Format("{0}/cgi-bin/mmwebwx-bin/synccheck?r={1}&sid={2}&uin={3}&skey={4}&deviceid={5}&synckey={6}&_{7}", gateway, getcurrentseconds(), baseRequest.Sid, baseRequest.Uin, baseRequest.Skey, baseRequest.DeviceID, synckey, getcurrentseconds());
            var rsp = wc.GetStr(url);

            outLog("synccheck->" + Serialize.ToJson(rsp));

            if (rsp.err) { return; throw new Exception("心跳同步失败->" + Serialize.ToJson(rsp)); }

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
                outLog("msg->" + user.Uin + "->" + m.Content);

                if (m.Content.Contains("你的朋友验证请求，") && m.Content.Contains("可以开始聊天了"))
                {
                    var rp = db.x_reply.Where(r => r.user_id == lg.user_id);

                }

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
        void sendText(string ToUserName, string Content)
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
            outLog("SendMsg->" + Serialize.ToJson(rsp));

        }

        /// <summary>
        /// 发送图片
        /// </summary>
        /// <param name="ToUserName"></param>
        /// <param name="url"></param>
        void sendImg(string ToUserName, string mmid)
        {
            //var mmid = uploadImg(ToUserName, url);
            //if (string.IsNullOrEmpty(mmid)) return;

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
            if (!(rsp.data+"").Contains("\"Ret\": 0")) Thread.Sleep(5 * 1000);

            outLog("SendImg->" + Serialize.ToJson(rsp));

        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        string uploadImg(string img)
        {
            var fs = op.GetFile(img);
            if (fs.err) return "";

            string url = gateway.Replace("https://", "https://file.") + "/cgi-bin/mmwebwx-bin/webwxuploadmedia?f=json";
            var o = new
            {
                UploadType = 2,
                BaseRequest = baseRequest,
                ClientMediaId = getcurrentseconds(),
                TotalLen = (fs.data as byte[]).Length,
                StartPos = 0,
                DataLen = (fs.data as byte[]).Length,
                MediaType = 4
            };

            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("id", "WU_FILE_" + filecount++);
            dict.Add("name", img.Substring(img.LastIndexOf('/') + 1));
            dict.Add("type", Tools.GetMimeType(img.Substring(img.LastIndexOf('.') + 1)));
            dict.Add("lastModifiedDate", "Wed Sep 07 2016 10:38:12 GMT+0800");//DateTime.Now.ToString("r")
            dict.Add("size", (fs.data as byte[]).Length + "");
            dict.Add("mediatype", "pic");
            dict.Add("uploadmediarequest", Serialize.ToJson(o));
            dict.Add("webwx_data_ticket", wc.GetCookie("webwx_data_ticket"));
            dict.Add("pass_ticket", passticket);

            var rsp = op.PostFile(url, dict, fs.data as byte[]);
            outLog("UploadImg->" + Serialize.ToJson(rsp));

            if (rsp.err) return "";

            return Serialize.FromJson<string>(rsp.data + "", "MediaId");

        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="c"></param>
        void exit(int c)
        {
            outLog("exit");
            isquit = true;

            if (lg != null)
            {
                lg.uuid = null;
                lg.qrcode = null;
                lg.status = 1;

                try
                {
                    db.SubmitChanges();
                    db.Dispose();
                }
                catch (Exception ex)
                {
                    outLog("exit->err:" + ex.Message);
                }
            }
            if (c == 1) Logout?.Invoke();
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