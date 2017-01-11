using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using X.Core.Utility;
using System.Web;

namespace X.Wx.App
{
    public class Wc
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id"></param>
        public Wc()
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
            }
            catch (Exception ex)
            {
                outLog("wx.Run->" + ex.Message);
                exit(1);
            }

            new Thread(o =>
            {
                loadContact();

            }).Start();

            Thread.Sleep(3 * 1000);

            new Thread(o =>
            {
                while (!isquit)
                {
                    SyncCheck();
                    Thread.Sleep(2 * 1000);
                }

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

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="touser"></param>
        /// <param name="type"></param>
        /// <param name="content"></param>
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
                else if (type == 3) rt = sendText(u, content, 42);
                i++;
                if (!rt) Thread.Sleep(Tools.GetRandNext(5000, 30000));
                else if (i == 5) { Thread.Sleep(Tools.GetRandNext(3000, 8000)); i = 1; }
                else Thread.Sleep(Tools.GetRandNext(1500, 3000));
            }
            outLog("send->发送完成，共发给" + touser.Count() + "人");
        }

        /// <summary>
        /// 通过好友验证
        /// </summary>
        public void VerifyUser(int p, int[] sl, string vul)
        {
            var url = string.Format("https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxverifyuser?r={0}&pass_ticket={1}", getcurrentseconds(), passticket);
            var o = new
            {
                BaseRequest = baseRequest,
                Opcode = p,
                SceneList = sl,
                SceneListCount = sl.Length,
                VerifyContent = "",
                VerifyUserList = vul,
                skey = baseRequest.Skey
            };

            var rsp = op.PostStr(url, Serialize.ToJson(o));
            outLog("VerifyUser->" + Serialize.ToJson(rsp));
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
            //Console.WriteLine("log@" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":->" + msg);
            //Debug.WriteLine("log@" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":->" + msg);
            OutLog?.Invoke(msg);
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
                Scaned?.Invoke(img);
                waitFor(2, 0);
            }

            if (str.Contains("code=200"))
            {
                var reg = new Regex("window.redirect_uri=\"(\\S+?)\"");
                redirecturl = reg.Match(str).Groups[1].Value;
                if (!String.IsNullOrEmpty(redirecturl)) gateway = "https://" + new Uri(redirecturl).Host;
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

            rsp.data = Tools.RemoveHtml(rsp.data + "");

            contacts = Serialize.FromJson<List<Contact>>(rsp.data + "", "MemberList").Where(o => o.KeyWord != "gh_" && o.UserName[0] == '@').ToList();

            var nks = new Dictionary<string, int>();

            foreach (var c in contacts.Where(o => string.IsNullOrEmpty(o.RemarkName) && o.UserName[1] != '@'))
            {
                if (nks.ContainsKey(c.NickName)) { nks[c.NickName]++; c.RemarkName = c.NickName + nks[c.NickName].ToString("000"); SetRemark(c.UserName, c.RemarkName); }
                else nks.Add(c.NickName, 0);
            }

            new Thread(() =>
            {
                var qgps = contacts.Where(o => o.UserName[1] == '@');
                for (var i = 1; i <= Math.Ceiling(qgps.Count() / 50.0); i++)
                {
                    var gs = qgps.Skip((i - 1) * 50)
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
                        g.MemberList.AddRange(c.MemberList);
                    }
                }
                ContactLoaded?.Invoke(contacts);
            }).Start();

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
            if (rsp.err) return;

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

            //outLog("sync->" + Serialize.ToJson(rsp));

            if (rsp.err) { throw new Exception("消息获取失败->" + Serialize.ToJson(rsp)); }

            _syncKey = Serialize.FromJson<SyncKey>(rsp.data + "", "SyncKey");

            var msglist = Serialize.FromJson<List<Msg>>(rsp.data + "", "AddMsgList");

            foreach (var m in msglist)
            {
                if (m.FromUserName == user.UserName) continue;
                NewMsg?.Invoke(m);
            }

        }

        bool sendText(string ToUserName, string Content) { return sendText(ToUserName, Content, 1); }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="ToUserName"></param>
        /// <param name="Content"></param>
        bool sendText(string ToUserName, string Content, int type)
        {
            string url = String.Format("{0}/cgi-bin/mmwebwx-bin/webwxsendmsg?pass_ticket={1}", gateway, passticket);
            Content = Content.Replace("<br/>", "\n");
            var o = new
            {
                BaseRequest = baseRequest,
                Msg = new
                {
                    Type = type,
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
            /// <summary>
            /// 1、文本消息
            /// 42、名片 <?xml version="1.0"?><msg bigheadimgurl ="http://wx.qlogo.cn/mmhead/Q3auHgzwzM53BeiaunvP5HiciayBKET65sJBDy5QIWxNMPxu0TKb1t6SQ/0" smallheadimgurl="http://wx.qlogo.cn/mmhead/Q3auHgzwzM53BeiaunvP5HiciayBKET65sJBDy5QIWxNMPxu0TKb1t6SQ/132" username="gh_eda6e1a8b86f" nickname="微购友阿" shortpy="WGYA" alias="youa-wego" imagestatus="3" scene="17" province="湖南" city="长沙" sign="" sex="0" certflag="24" certinfo="湖南友谊阿波罗商业股份有限公司" brandIconUrl="http://mmsns.qpic.cn/mmsns/QXEF1VWnQtaibv3IChtmU31tKDVvQg3Zib7KS8lYCKahcXmWaky1ssNg/0" brandHomeUrl="" brandSubscriptConfigUrl="" brandFlags="0" regionCode="CN_Hunan_Changsha" /></msg>
            /// 43、视频 <?xml version="1.0"?><msg><videomsg aeskey="e6fc5c70fef644f2a55a5adc365847f1" cdnthumbaeskey="e6fc5c70fef644f2a55a5adc365847f1" cdnvideourl="306f0201000468306602010002041431d65b02032dcf5902042855d33a0204587245e50444645f323134305f617570766964656f5f303136643866376630326235376232395f313438333837373232395f3230303730383038303131373937346431336538343935320201000201000400" cdnthumburl="306f0201000468306602010002041431d65b02032dcf5902042855d33a0204587245e50444645f323134305f617570766964656f5f303136643866376630326235376232395f313438333837373232395f3230303730383038303131373937346431336538343935320201000201000400" length="4985440" playlength="91" cdnthumblength="19110" cdnthumbwidth="272" cdnthumbheight="480" fromusername="zk-520-mj" md5="f2a76f49be6b7f12b005fe117f759003" newmd5="facb3747aa963c69b275f0b4d4c23367" isad="0" /></msg> 封面https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxgetmsgimg?&MsgID=7644433703673069160&skey=%40crypt_65a9d629_3eaa6430758c1b7ab8193a2bf9d237df&type=slave 视频 https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxgetvideo?msgid=7644433703673069160&skey=%40crypt_65a9d629_3eaa6430758c1b7ab8193a2bf9d237df
            /// 3、图片 <?xml version="1.0"?><msg><img aeskey="d9be5a1797de4c5983a44eb522560589" encryver="0" cdnthumbaeskey="d9be5a1797de4c5983a44eb522560589" cdnthumburl="30500201000449304702010002044aa5f15a02033d3bc902045aff03b70204586f621f0425617570696d675f646535646234326439313361646531395f313438333639343632323938370201000201000400" cdnthumblength="8636" cdnthumbheight="120" cdnthumbwidth="68" cdnmidheight="0" cdnmidwidth="0" cdnhdheight="0" cdnhdwidth="0" cdnmidimgurl="30500201000449304702010002044aa5f15a02033d3bc902045aff03b70204586f621f0425617570696d675f646535646234326439313361646531395f313438333639343632323938370201000201000400" length="49695" md5="2a8524f1fe50a11bf5adfdd158f77e16" /></msg> https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxgetmsgimg?&MsgID=5404623435463498538&skey=%40crypt_65a9d629_e5556306fe9f3d3df85d819541c90df5&type=primary
            /// 34、语音 <msg><voicemsg endflag="1" length="2067" voicelength="1189" clientmsgid="416539626635343661393932656138001917280106175d1b5edf762104" fromusername="zk-520-mj" downcount="0" cancelflag="0" voiceformat="4" forwardflag="0" bufid="219897043082412579" /></msg> https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxgetvoice?msgid=1628355296153004216&skey=@crypt_65a9d629_e5556306fe9f3d3df85d819541c90df5
            /// 37、加好友 <msg fromusername="wxid_vmck44cm4fu622" encryptusername="v1_fe106ba8a609cb7f49fd6f05fb53210bd6b28dd103966a9eb814fff9b5f756f6a692db444c93415626a2cbd751adedc5@stranger" fromnickname="黄珊" content="换群" shortpy="HS" imagestatus="3" scene="30" country="CN" province="Beijing" city="Haidian" sign="找房网" percard="1" sex="2" alias="zfw609253" weibo="" weibonickname="" albumflag="0" albumstyle="0" albumbgimgid="" snsflag="1" snsbgimgid="http://mmsns.qpic.cn/mmsns/0pygn8iaZdEf2AuwhK1OfNDK7lialh5qH6zibD6nbNBgvPjrEvy1iblvMslq4q9syfOrUuXc3IDbVEw/0" snsbgobjectid="12345344171967328503" mhash="c1d4fae9b7def70ad3901f4917c4393d" mfullhash="c1d4fae9b7def70ad3901f4917c4393d" bigheadimgurl="http://wx.qlogo.cn/mmhead/ver_1/OI0HZYp5KMQ6hfAad1MdkaLZTYgPrfvVUymUiahUdBzhDLubc8sJKmoM7jtCxOkTjKHrlHKnEG7KqOOCxgmCAVB8hCt8jibiadI7uw43TyqTIc/0" smallheadimgurl="http://wx.qlogo.cn/mmhead/ver_1/OI0HZYp5KMQ6hfAad1MdkaLZTYgPrfvVUymUiahUdBzhDLubc8sJKmoM7jtCxOkTjKHrlHKnEG7KqOOCxgmCAVB8hCt8jibiadI7uw43TyqTIc/96" ticket="v2_5d02ba8d70d7b0696543edb0140385aa682bac119bcc990d431173506482c7e6a6e3914dbb78638c2310047a31b39a673b038ae564aeda93356f4e9eb13c8086@stranger" opcode="2" googlecontact="" qrticket="" chatroomusername="" sourceusername="" sourcenickname=""><brandlist count="0" ver="674411725"></brandlist></msg>
            /// 1000、好友同意
            /// 10002 系统消息
            /// </summary>
            public int MsgType { get; set; }
            public string Content { get; set; }
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
        #endregion

    }
}