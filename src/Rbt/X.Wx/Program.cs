using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using System.Linq;
using X.Core.Utility;
using X.Wx.App;
using System.Configuration;
using X.Core.Plugin;

namespace X.Wx
{
    class Program
    {
        static Wc wx = null;
        static string nickname;
        static string headimg;
        static string uin = "";
        static Login lg = null;
        static Main main = null;
        static List<Wc.Contact> contacts = null;
        static bool contactdone = false;
        static bool stop = false;
        static List<ReplyResp.Reply> repes = null;

        [STAThreadAttribute]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Loger.Init();

            System.Net.ServicePointManager.DefaultConnectionLimit = 512;

            var key = ConfigurationManager.AppSettings["app-key"];
            if (string.IsNullOrEmpty(key))
            {
                var ak = new Akey();
                Application.Run(ak);
                if (string.IsNullOrEmpty(ak.key)) return;

                var cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (key == null) cfg.AppSettings.Settings.Add("app-key", ak.key);
                else cfg.AppSettings.Settings["app-key"].Value = ak.key;
                cfg.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

                key = ak.key;
            }

            if (!Sdk.Init(key)) return;

            lg = new Login();

            //微信线程
            startWx();

            Application.Run(lg);

            if (string.IsNullOrEmpty(headimg)) return;

            Sdk.WxLogin(uin, nickname, headimg);

            main = new Main(wx, headimg);
            main.LoadReply += Main_LoadReply;

            //获取自动回复
            loadReply();

            //群发线程
            sendMsg();

            //头像线程
            loadHeadimg();

            Application.Run(main);
            stop = true;

            if (wx != null) wx.Quit();

        }

        private static void Main_LoadReply()
        {
            loadReply();
        }

        /// <summary>
        /// 获取用户头像
        /// </summary>
        private static void loadHeadimg()
        {
            new Thread(o =>
            {
                var t = 0;
                while (!stop)
                {
                    if (!contactdone) continue;
                    var rsp = Sdk.LoadNoimg(uin);
                    if (rsp == null || !rsp.issucc || rsp.items == null || rsp.items.Count == 0) break;

                    var us = wx.GetHeadImage(rsp.items);
                    if (us.Count == 0) { Thread.Sleep(5 * 1000); continue; }

                    if (us.Count(c => c.Value != "1") > 0)
                    {
                        t = 0;
                        Sdk.SetHeadimg(us, uin);
                        outLog("同步头像");
                    }
                    else
                    {
                        t++;
                        if (t >= 5)
                        {
                            outLog("头像同步完成");
                            break;
                        }
                    }
                }
            }).Start();
        }

        /// <summary>
        /// 群发消息
        /// </summary>
        private static void sendMsg()
        {
            new Thread(o =>
            {
                Thread.Sleep(10 * 1000);
                var t = 0;
                while (!stop)
                {
                    Thread.Sleep(5 * 1000);
                    t++;
                    if (t < 10) continue;
                    if (DateTime.Now.Hour < 8 || DateTime.Now.Hour > 21) continue;
                    t = 0;
                    var st = DateTime.Now.ToString("HH:mm");
                    outLog("群发@" + st + "->开始获取");
                    var rsp = Sdk.LoadMsg(uin);
                    if (stop) break;
                    if (rsp.items == null || rsp.items.Count() == 0) { outLog("群发@" + st + "->无内容"); continue; }
                    outLog("群发@" + st + "->开始发送，" + rsp.items.Count() + "个");
                    foreach (var m in rsp.items)
                    {
                        if (m.touser == null || m.touser.Count() == 0) continue;
                        outLog("群发@" + st + "->发送给：，" + m.touser.Count + "个用户（" + m.content + "）");
                        wx.Send(m.touser, m.type, m.content);
                        if (stop) break;
                        Thread.Sleep(5 * 1000);
                    }
                    outLog("群发@" + st + "->结束");
                }
            }).Start();
        }

        /// <summary>
        /// 运行微信
        /// </summary>
        private static void startWx()
        {
            new Thread(o =>
            {
                wx = new Wc();
                wx.LoadQr += Wx_LoadQr; ;
                wx.Scaned += Wx_Scaned;
                wx.Loged += Wx_Loged;
                wx.LogonOut += Wx_LogonOut;
                wx.OutLog += Wx_OutLog; ;
                wx.NewMsg += Wx_NewMsg;
                wx.ContactLoaded += Wx_ContactLoaded;
                wx.Run();

            }).Start();
        }

        /// <summary>
        /// 加载回复
        /// </summary>
        private static void loadReply()
        {
            new Thread(o =>
            {
                outLog("正在获取回复");
                var rsp = Sdk.LoadReply(uin);
                repes = rsp?.items;
                outLog("自动回复获取" + (rsp != null && rsp.issucc ? "成功" : "失败"));
            }).Start();
        }

        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="msg"></param>
        static void outLog(string msg)
        {
            ((Action)(() =>
            {
                if (main != null) main.OutLog(msg);
            })).BeginInvoke(null, null);
        }

        private static void Wx_ContactLoaded(List<Wc.Contact> cts, string gname, bool isdone)
        {
            if (main == null) return;
            main.SetContact(cts, gname);
            if (string.IsNullOrEmpty(gname))
            {
                contacts = cts;
                outLog("同步主通讯录");
            }
            else
            {
                var gp = contacts.FirstOrDefault(o => o.UserName == gname);
                gp.MemberCount = cts.Count();
                gp.MemberList = cts;
                outLog("同步群成员：" + gp.NickName + " " + gp.MemberCount + "人");
            }

            var rsp = Sdk.ContactSync(Serialize.ToJson(cts), uin, gname);
            if (!rsp.issucc) outLog("通讯录同步失败");
            contactdone = isdone;
        }

        static void Wx_Loged(Wc.Contact u)
        {
            outLog(u.NickName + "已经登陆");
            nickname = u.NickName;
            uin = u.Uin + "";
            lg.SetLoged();

            Thread.Sleep(2 * 1000);
            lg.Quit();
        }

        static void Wx_OutLog(string log)
        {
            //if(log.StartsWith("synccheck->")&&log.Contains())
            Debug.WriteLine("log@" + DateTime.Now.ToString("HH:mm:ss.fff") + "->" + log);
        }

        static void Wx_LogonOut()
        {
            outLog("正在退出...");
            stop = true;
            Thread.Sleep(5 * 1000);
            Environment.Exit(0);
        }

        static void Wx_NewMsg(Wc.Msg m)
        {
            new Thread(p =>
            {
                if (contacts == null || m == null) return;
                var msg = p as Wc.Msg;
                var name = m.FromUserName;

                var u = contacts.FirstOrDefault(o => o.UserName == m.FromUserName);
                if (u == null) { outLog("收到无法识别的消息：" + m.Content); return; }

                var cot = Tools.RemoveHtml(m.Content);
                var ur = "";

                if (m.FromUserName[1] == '@' && cot[0] == '@')
                {
                    var ct = cot.Split(':');
                    ur = ct[0];
                    cot = ct[1];
                }

                object mg = null;

                if (msg.MsgType != 1) main.OutLog(cot);

                switch (msg.MsgType)
                {
                    case 42:
                        cot = "名片";
                        break;
                    case 43:
                        cot = "视频";
                        break;
                    case 3:
                        cot = "图片";
                        break;
                    case 34:
                        cot = "语音";
                        break;
                    case 37:
                        //wx.VerifyUser();//自动同意
                        cot = "请求加好友";
                        break;
                    case 47:
                        cot = "自定义表情";
                        break;
                    case 1000:
                        break;
                    case 10002:
                        break;
                }

                if (!string.IsNullOrEmpty(ur))
                {
                    var su = u?.MemberList.FirstOrDefault(o => o.UserName == ur);
                    if (su != null)
                    {
                        var img = wx.GetHeadImage(new List<string>() { u.UserName, su.UserName });
                        mg = new
                        {
                            body = cot,
                            u = new { name = su.NickName, img = img != null && img.ContainsKey(su.UserName) ? img[su.UserName] : "", id = su.UserName },
                            r = new { name = u.NickName, img = img != null && img.ContainsKey(u.UserName) ? img[u.UserName] : "", id = u.UserName }
                        };
                    }
                }
                if (mg == null)
                {
                    var img = wx.GetHeadImage(new List<string>() { u.UserName });
                    mg = new
                    {
                        body = cot,
                        u = new { name = u.NickName, img = img != null && img.ContainsKey(u.UserName) ? img[u.UserName] : "", id = u.UserName }
                    };
                }

                main.SetMsg(mg);

                if (repes == null || repes.Count == 0) return;
                ReplyResp.Reply rep = null;
                foreach (var r in repes)
                {
                    switch (r.tp)
                    {
                        case 1:
                            if (cot.Contains("我通过了你的朋友验证请求，现在我们可以开始聊天了")) rep = r;
                            outLog("匹配到 被添加 自动回复，发送了" + (rep.type == 1 ? "文本" : "图片") + "内容：" + rep.content);
                            //被添加
                            break;
                        case 2://群内新成员
                            rep = r;
                            outLog("匹配到 群内新成员 自动回复，发送了" + (rep.type == 1 ? "文本" : "图片") + "内容：" + rep.content);
                            break;
                        case 3://默认回复
                            break;
                        case 4://关键字回复
                            if (r.match == 1 && r.keys.Contains(cot)) rep = r;
                            else if (r.match == 2 && r.keys.Count(o => cot.StartsWith(o)) > 0) rep = r;
                            else if (r.match == 3 && r.keys.Count(o => cot.EndsWith(o)) > 0) rep = r;
                            else if (r.match == 4 && r.keys.Count(o => cot.Contains(o)) > 0) rep = r;
                            if (rep != null) outLog("匹配到 关键字(" + string.Join(" ", r.keys) + ") 自动回复，发送了" + (rep.type == 1 ? "文本" : "图片") + "内容：" + rep.content);
                            break;
                    }
                    if (rep != null) break;
                }

                if (rep != null && m.FromUserName != wx.user.UserName) wx.Send(new List<string>() { m.FromUserName }, rep.type, rep.content);

                //if (name == "橙子兄弟") wx.Send(new List<string>() { m.FromUserName }, 3, HttpUtility.HtmlDecode(m.Content));

                //if (Regex.IsMatch(m.Content, "你已添加了[\\S+]，现在可以开始聊天了。$")) return;
                //if (Regex.IsMatch(m.Content, "\"^([^\"]+)\"撤回了一条消息$")) return; //<sysmsg type="revokemsg"><revokemsg><session>zk-520-mj</session><oldmsgid>1651334833</oldmsgid><msgid>7582253229712101098</msgid><replacemsg><![CDATA["橙子兄弟" 撤回了一条消息]]></replacemsg></revokemsg></sysmsg>
                //if (m.MsgType != 1) return;

                //if (Regex.IsMatch(m.Content, "^你被\"([^\"]+)\"移出了群聊$")) return;
                //if (Regex.IsMatch(m.Content, "\"^([^\"]+)\"邀请你加入了群聊")) return;

                //var r = repes.FirstOrDefault(o => o.users.Contains(m.FromUserName));
                //if (r == null) return;

            }).Start(m);
        }

        static void Wx_Scaned(string hdimg)
        {
            headimg = hdimg;
            lg.SetHeadimg(hdimg);
        }

        static void Wx_LoadQr(string qrcode)
        {
            lg.SetQrcode(qrcode);
        }

    }
}
