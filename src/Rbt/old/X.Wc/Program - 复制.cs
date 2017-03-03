using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Threading;
using X.Core.Utility;
using X.Net;
using X.Wc.App;

namespace X.Wc
{
    class Program
    {
        static string ip = "";
        static int port = 0;
        static string nk = "";
        static Wx wx = null;
        static Tcp tc = null;
        static string nickname;
        static string headimg;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">
        /// ip:port code
        /// </param>
        static void Main(string[] args)
        {
            var svr = ConfigurationManager.AppSettings["svr"] ?? "";
            if (svr == null && args.Length >= 1) { svr = args[0]; }

            var ps = svr.Split(':');
            if (ps.Length != 2) { Console.WriteLine("服务器配置参数不正确"); Environment.Exit(0); }

            ip = ps[0];
            int.TryParse(ps[1], out port);
            nk = ConfigurationManager.AppSettings["net-key"];

            if (string.IsNullOrEmpty(ip) || port == 0 || string.IsNullOrEmpty(nk)) { Console.WriteLine("缺少参数或参数不正确"); Environment.Exit(0); }

            RunTcp();

            new Thread(o =>
            {
                wx = new Wx();
                wx.LoadQr += Wx_LoadQr; ;
                wx.Scaned += Wx_Scaned;
                wx.Loged += Wx_Loged;
                wx.LogonOut += Wx_LogonOut;
                wx.OutLog += Wx_OutLog; ;
                wx.NewMsg += Wx_NewMsg;
                wx.Run();

            }).Start();

        }

        private static void Wx_Loged(Wx.Contact user)
        {
            nickname = user.NickName;
            if (tc == null) return;
            tc.code = user.Uin + "";
            tc.Send(new
            {
                act = "loged",
                uin = tc.code,
                headimg = headimg,
                nickname = user.NickName
            });
        }

        private static void Wx_OutLog(string log)
        {
            if (tc == null) return;
            tc.Send(new
            {
                act = "outlog",
                log = log
            });
        }

        private static void Wx_LogonOut()
        {
            if (tc == null) return;
            try
            {
                tc.Send(new { act = "exit", uin = tc.code });
                tc.Quit();
            }
            catch { }
            Environment.Exit(0);
        }

        static void Wx_NewMsg(Wx.Msg m)
        {
            if (tc == null) return;
            tc.Send(new
            {
                act = "newmsg",
                msg = Serialize.ToJson(m)
            });
        }

        static void Wx_Scaned(string hdimg)
        {
            headimg = hdimg;
            if (tc == null) return;
            tc.Send(new
            {
                act = "scaned",
                headimg = hdimg
            });
        }

        static void Wx_LoadQr(string qrcode)
        {
            if (tc == null) return;
            tc.Send(new
            {
                act = "qrback",
                qrcode = qrcode
            });
        }

        static void Wx_ContactSynced()
        {
            if (tc == null) return;
            tc.Send(new { act = "contactsynced" });
        }

        static void RunTcp()
        {
            try
            {
                tc = new Tcp(ip, port, nk);
                tc.NewMsg += Tc_NewMsg;
                tc.Closed += Tc_Closed;
                tc.Start();
            }
            catch { }
        }

        private static void Tc_Closed(Tcp tc)
        {
        }

        private static void Tc_NewMsg(string from, string body, Tcp tc)
        {
            dynamic bd = Serialize.FromJson<msg>(body);
            string act = bd.act;
            switch (act)
            {
                case "quit":
                    wx.Quit(); tc.Quit();
                    break;
                case "setmark":
                    wx.SetRemark(bd.username, bd.remark);
                    break;
                case "inmember":
                    wx.InMember(bd.gpname, bd.users.ToList<string>());
                    break;
                case "newgroup":
                    wx.NewGroup(bd.gpname, bd.users.ToList<string>());
                    break;
                case "outmember":
                    wx.OutMember(bd.gpname, bd.username);
                    break;
                case "sendmsg":
                    wx.Send(Serialize.FromJson<List<string>>(body, "touser"), (int)bd.type, bd.body);
                    break;
                case "tofriend":
                    wx.ToFriend(bd.touser, bd.hello);
                    break;
                case "totop":
                    wx.ToTop(bd.username);
                    break;
                case "untop":
                    wx.UnTop(bd.username);
                    break;
            }
        }

        class msg : DynamicObject
        {
            Dictionary<string, object> parms = new Dictionary<string, object>();
            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                if (parms.ContainsKey(binder.Name)) result = parms[binder.Name];
                else result = null;
                return true;
            }
            public override bool TrySetMember(SetMemberBinder binder, object value)
            {
                if (parms.ContainsKey(binder.Name)) parms[binder.Name] = value;
                else parms.Add(binder.Name, value);
                return true;
            }
        }
    }
}
