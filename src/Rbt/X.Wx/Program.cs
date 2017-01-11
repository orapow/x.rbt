using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using System.Linq;
using X.Core.Utility;
using X.Wx.App;
using System.Text.RegularExpressions;
using System.Configuration;
using X.Data;
using System.Web;

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
        static Wc.Contact user = null;
        static bool stop = false;
        static List<ReplyResp.Reply> repes = null;

        /// <summary>
        /// lgid ip:port ukey
        /// 1 127.0.0.1:9999
        /// </summary>
        /// <param name="args"></param>
        [STAThreadAttribute]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                Sdk.Init(ConfigurationManager.AppSettings["app-key"]);
            }
            catch
            {
                return;
            }

            lg = new Login();

            //微信线程
            startWx();

            Application.Run(lg);

            if (lg.headimg == null)
                Application.Exit(); //Application.Run(new Main(lg.nickname, lg.headimg, lg.ukey));

            Sdk.WxLogin(uin, nickname, headimg);

            main = new Main(wx, user, lg.headimg);

            //获取自动回复
            loadReply();

            //群发线程
            sendMsg();

            //头像线程
            loadHeadimg();

            Application.Run(main);

        }

        /// <summary>
        /// 获取用户头像
        /// </summary>
        private static void loadHeadimg()
        {
            new Thread(o =>
            {
                while (!stop)
                {

                }
                stop = true;
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
                while (!stop)
                {
                    var st = DateTime.Now.ToString("HH:mm");
                    outLog("群发@" + st + "->开始获取");
                    var rsp = Sdk.LoadMsg(uin);
                    if (stop) break;
                    outLog("群发@" + st + "->开始发送，共 " + rsp.items.Count() + " 个群发");
                    foreach (var m in rsp.items)
                    {
                        if (m.touser == null) continue;
                        wx.Send(m.touser, m.type, m.content);
                        if (stop) break;
                        Thread.Sleep(5 * 1000);
                    }
                    outLog("群发@" + st + "结束");
                    Thread.Sleep(10 * 1000);
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
                Thread.Sleep(3 * 1000);
                outLog("正在获取回复");
                var rsp = Sdk.LoadReply();
                if (rsp.issucc) repes = rsp.items;
                outLog("自动回复获取" + (rsp.issucc ? "成功" : "失败"));
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

        private static void Wx_ContactLoaded(List<Wc.Contact> cts)
        {
            contacts = cts;
            var rsp = Sdk.ContactSync(Serialize.ToJson(cts), uin);
            if (!rsp.issucc) outLog("通讯录同步失败");
        }

        static void Wx_Loged(Wc.Contact u)
        {
            nickname = u.NickName;
            uin = u.Uin + "";
            user = u;
            lg.SetLoged();

            Thread.Sleep(2 * 1000);
            lg.Quit();
        }

        static void Wx_OutLog(string log)
        {
            Debug.WriteLine("log@" + DateTime.Now.ToString("HH:mm:ss.fff") + "->" + log);
        }

        static void Wx_LogonOut()
        {
            stop = true;
            Thread.Sleep(5 * 1000);
            Environment.Exit(0);
        }

        static void Wx_NewMsg(Wc.Msg m)
        {
            new Thread(p =>
            {
                var msg = p as Wc.Msg;

                var name = m.FromUserName;
                var u = contacts.FirstOrDefault(o => o.UserName == m.FromUserName);
                if (u != null) name = u.NickName;

                var cot = Tools.RemoveHtml(m.Content);

                if (m.FromUserName[1] == '@' && u != null)
                {
                    var c = cot.Split(':');
                    var su = u?.MemberList.FirstOrDefault(o => o.UserName == c[0]);
                    if (su != null) name = su.NickName + "@" + u.NickName;
                    cot = c[1];
                }

                switch (msg.MsgType)
                {
                    case 1:
                        outLog(name + "->" + cot);
                        break;
                    case 42:
                        outLog(name + "->名片");
                        break;
                    case 43:
                        outLog(name + "->视频");
                        break;
                    case 3:
                        outLog(name + "->图片");
                        break;
                    case 34:
                        outLog(name + "->语音");
                        break;
                    case 37:
                        //wx.VerifyUser();自动同意
                        outLog(name + "->请求加好友");
                        break;
                    case 1000:
                        break;
                    case 10002:
                        break;
                }

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

                if (rep != null) wx.Send(new List<string>() { m.FromUserName }, rep.type, rep.content);

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
