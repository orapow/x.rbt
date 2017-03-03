using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Net.Sockets;
using System.Threading;
using X.Core.Utility;
using X.Wx.App;

namespace X.Wx
{
    class Program
    {
        static App.Wx wx = null;
        static App.Tc tc = null;
        static string ukey = "";

        static long lgid = 0;
        /// <summary>
        /// lgid ip:port ukey
        /// 1 127.0.0.1:9999
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            if (args.Length != 3) { Console.WriteLine("参数不正确，格式 lgid ip:port ukey"); return; }

            ukey = args[2];
            lgid = int.Parse(args[0]);

            new Thread(o =>
            {
                var svr = args[1].Split(':');
                tc = new Tc(svr[0], int.Parse(svr[1]));
                tc.Closed += Tc_Closed;
                tc.NewMsg += Tc_NewMsg;
                tc.Run(ukey, lgid);
            }).Start();

            Thread.Sleep(500);

            new Thread(o =>
            {
                wx = new App.Wx(lgid);
                wx.LoadQr += Wx_LoadQr;
                wx.Scaned += Wx_Scaned;
                wx.Loged += Wx_Loged;
                wx.Logout += Wx_Logout;
                wx.NewMsg += Wx_NewMsg;
                wx.Run();
            }).Start();
        }

        static void Tc_NewMsg(msg m)
        {
            Debug.WriteLine("tc_newmsg->" + Serialize.ToJson(m));
            dynamic bd = Serialize.FromJson<mbody>(m.body);
            switch (m.act)
            {
                case "exit":
                    wx.Quit();
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
                    wx.Send(Serialize.FromJson<List<string>>(m.body, "touser"), (int)bd.type, bd.body);
                    //foreach (var u in bd.touser) { wx.Send(u.ToString(), (int)bd.type, bd.body); }
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

        static void Tc_Closed()
        {
            if (wx != null) wx.Quit();
        }

        static void Wx_NewMsg(App.Wx.Msg m)
        {
            tc.Send(new msg
            {
                act = "newmsg",
                body = Serialize.ToJson(m)
            });
        }

        static void Wx_Logout()
        {
            try
            {
                tc.Send(new msg() { act = "logout" });
                tc.Quit();
            }
            catch { }
            Environment.Exit(0);
        }

        static void Wx_Loged()
        {
            tc.Send(new msg() { act = "loged" });
        }

        static void Wx_Scaned(string hdimg)
        {
            tc.Send(new msg()
            {
                act = "scaned",
                body = hdimg
            });
        }

        static void Wx_LoadQr(string qrcode)
        {
            tc.Send(new msg()
            {
                act = "qrback",
                body = qrcode
            });
        }

        static void Wx_ContactSynced()
        {
            tc.Send(new msg() { act = "contactsynced" });
        }

        class mbody : DynamicObject
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
