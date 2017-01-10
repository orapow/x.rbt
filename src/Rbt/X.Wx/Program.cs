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

namespace X.Wx
{
    class Program
    {
        static Wc wx = null;
        static string nickname;
        static string headimg;
        static string uin = "";
        static Login lg = null;
        static Log log = null;
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
            
            Sdk.Init(ConfigurationManager.AppSettings["app-key"]);

            lg = new Login();

            //微信线程
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

            Application.Run(lg);

            if (lg.headimg == null) Application.Exit(); //Application.Run(new Main(lg.nickname, lg.headimg, lg.ukey));

            //获取自动回复
            new Thread(o =>
            {
                var rsp = Sdk.LoadReply();
                if (rsp.issucc) repes = rsp.replies;
                else outLog("自动回复获取失败，无法自动回复");
            }).Start();

            //群发线程
            //new Thread(o =>
            //{
            //    while (!stop)
            //    {
            //        var rsp = Sdk.LoadMsg(uin);
            //        if (stop) break;
            //        if (rsp == null || rsp.items == null)
            //        {
            //            Thread.Sleep(10 * 1000);
            //            continue;
            //        }
            //        foreach (var m in rsp.items)
            //        {
            //            if (m.tousers == null) continue;
            //            wx.Send(m.tousers, m.type, m.content);
            //            if (stop) break;
            //            Thread.Sleep(5 * 1000);
            //        }
            //    }
            //    stop = true;
            //}).Start();

            //头像线程
            new Thread(o =>
            {
                //while (!stop)
                //{
                //    rep r = null;
                //    lock (repqueue) r = repqueue.Dequeue();
                //    if (r == null)
                //    {
                //        Thread.Sleep(10 * 1000);
                //        continue;
                //    }

                //    var rsp = Sdk.LoadReply(r.type, r.str, r.fromuser);
                //    if (stop) break;
                //    if (rsp.type == 0 || string.IsNullOrEmpty(rsp.content)) continue;

                //    wx.Send(new List<string>() { r.fromuser }, rsp.type, rsp.content);
                //    Thread.Sleep(5 * 1000);

                //}
                //stop = true;
            }).Start();

            log = new Log(wx, user, lg.headimg);
            Application.Run(log);

        }

        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="msg"></param>
        static void outLog(string msg)
        {
            ((Action)(() =>
            {
                if (log != null) log.OutLog(msg);
            })).BeginInvoke(null, null);
        }

        private static void Wx_ContactLoaded(List<Wc.Contact> cts)
        {
            contacts = cts;
            Sdk.ContactSync(Serialize.ToJson(cts), uin);
            lg.Quit();
        }

        static void Wx_Loged(Wc.Contact u)
        {
            nickname = u.NickName;
            uin = u.Uin + "";
            user = u;
            lg.SetLoged();
        }

        static void Wx_OutLog(string log)
        {
            //outLog(log);
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

                if (Regex.IsMatch(m.Content, "你已添加了[\\S+]，现在可以开始聊天了。$")) return;
                //if (Regex.IsMatch(m.Content, "\"^([^\"]+)\"撤回了一条消息$")) return; //<sysmsg type="revokemsg"><revokemsg><session>zk-520-mj</session><oldmsgid>1651334833</oldmsgid><msgid>7582253229712101098</msgid><replacemsg><![CDATA["橙子兄弟" 撤回了一条消息]]></replacemsg></revokemsg></sysmsg>
                if (m.MsgType != 1) return;

                if (Regex.IsMatch(m.Content, "^你被\"([^\"]+)\"移出了群聊$")) return;
                if (Regex.IsMatch(m.Content, "\"^([^\"]+)\"邀请你加入了群聊")) return;

                var name = m.FromUserName;
                var u = contacts.FirstOrDefault(o => o.UserName == m.FromUserName);
                if (u != null) name = u.NickName;

                if (u.UserName[1] == '@')
                {
                    var c = m.Content.Split(':');
                    var su = u.MemberList.FirstOrDefault(o => o.UserName == c[0]);
                    if (su != null) name = u.NickName + ":" + su.NickName;
                    outLog(name + "->" + c[1]);
                }
                else outLog(name + "->" + m.Content);

                if (repes == null || repes.Count == 0) return;
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
