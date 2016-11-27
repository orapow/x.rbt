using System;
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
            //if (args.Length == 0) args = "1 127.0.0.1:9999 494a16fc4261955fb704049d48ff18e3".Split(' ');
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
            //switch (m.act)
            //{
            //    case "send":

            //        break;
            //}
        }

        static void Tc_Closed()
        {
            if (wx != null) wx.Quit();
        }

        static void Wx_NewMsg(App.Wx.Msg m)
        {
            if (m.Content == "发图片")
            {
                var mid = wx.UploadImg(m.FromUserName, "d:\\s.jpg");
                if (string.IsNullOrEmpty(mid)) wx.SendMsg(m.FromUserName, "文件ID获取失败");
                else wx.SendImg(m.FromUserName, mid);
            }
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

    }
}
