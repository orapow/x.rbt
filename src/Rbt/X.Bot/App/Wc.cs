using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using X.Core.Utility;

namespace X.Bot.App
{
    public class Wc
    {
        public delegate void NewWxHandler(Wc wx);
        public event NewWxHandler NewWx;

        public delegate void ExitHandler(string uin);
        public event ExitHandler Exit;

        public User cu { get; private set; }

        private Login lg = null;
        private Wx wx = null;
        private Tcp tc = null;

        public Wc(TcpClient sc)
        {

            cu = new User();
            lg = new Login();

            tc = new Tcp(sc);
            tc.NewMsg += Tc_NewMsg;
            tc.Closed += Tc_Closed;

        }

        public void Run()
        {
            tc.Start();
        }

        public static void Start()
        {
            new Thread(o =>
            {
                var pi = new ProcessStartInfo("cl.exe", "--script-encoding=gbk script.js " + Guid.NewGuid().ToString());
                var p = new Process();
                p.StartInfo = pi;
                p.Start();

            }).Start();
        }

        public void Show()
        {
            if (wx == null) wx = new Wx(cu.nickname, cu.headimg);
            wx.Show();
            wx.Activate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t">
        /// 0 关闭连接
        /// 1 退出微信
        /// </param>
        public void Quit()
        {
            tc.Send(new msg() { act = "quit" });
            Thread.Sleep(2 * 1000);
            tc.Quit();
        }

        private void Tc_Closed(Tcp tc)
        {
            if (lg.Visible) lg.Close();
            Exit?.Invoke(cu.uin);
        }

        private void Tc_NewMsg(msg m, Tcp tc)
        {
            switch (m.act)
            {
                case "setcode":
                    if (m.body.Length == 36) new Thread(o => { lg.ShowDialog(); }).Start();
                    tc.Send(new msg() { act = "ok" });
                    break;
                case "setuser":
                    cu = Serialize.FromJson<User>(m.body);
                    if (lg.Visible) lg.SetSucc();
                    NewWx?.Invoke(this);
                    break;
                case "qrcode":
                    lg.SetQrcode(m.body);
                    break;
                case "headimg":
                    lg.SetHeadimg(m.body);
                    cu.headimg = m.body;
                    break;
                case "log":
                    Debug.WriteLine("log->" + m.body);
                    break;
                case "quit":
                    tc.Quit();
                    Exit?.Invoke(cu.uin);
                    break;
            }
        }

        //public void SetTc(Tcp tc)
        //{
        //    Tc = tc;
        //    //tc.NewMsg += Tc_NewMsg;
        //    //tc.Closed += Tc_Closed;
        //    //tc.Start();
        //}

        //private void Tc_Closed(Tcp tc)
        //{
        //    //tc.Send(new msg() { act = "quit" });
        //}

        //private void Tc_NewMsg(msg m, Tcp tc)
        //{
        //    //if (m.act == "login") tc.code = m.body;

        //    if (m.act == "qrcode") lg.SetQrcode(m.body);
        //    else if (m.act == "headimg")
        //    {
        //        lg.SetHeadimg(m.body);
        //        cu.headimg = m.body;
        //    }
        //    else if (m.act == "setuser")
        //    {
        //        var u = Serialize.FromJson<User>(m.body);
        //        cu.uin = u.uin;
        //        cu.nickname = u.nickname;
        //        tc.code = cu.uin;

        //        lg.SetSucc();

        //        Wx = new Wx(cu.nickname, cu.headimg);
        //        Wx.Show();
        //    }

        //    Debug.WriteLine(m.act + "->" + m.body);
        //}

        public class User
        {
            public string uin { get; set; }
            public string nickname { get; set; }
            public string headimg { get; set; }
        }
    }
}
