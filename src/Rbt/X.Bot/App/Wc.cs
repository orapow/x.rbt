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

        public delegate void SetCodeHandler(string uin);
        public event SetCodeHandler SetCode;

        public User cu { get; private set; }

        private Login lg = null;
        private Wx wx = null;
        private Tcp tc = null;

        public Wc(TcpClient sc)
        {

            cu = new User();
            lg = new Login();
            lg.FormClosed += Lg_FormClosed;

            tc = new Tcp(sc);
            tc.NewMsg += Tc_NewMsg;
            tc.Closed += Tc_Closed;

        }

        private void Lg_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            if (e.CloseReason == System.Windows.Forms.CloseReason.UserClosing && !lg.isLoged)
            {
                tc.Send(new msg() { act = "quit" });
                tc.Quit();
                Exit?.Invoke(tc.code);
            }
        }

        public void Run()
        {
            tc.Start();
        }

        public static void Start()
        {
            new Thread(o =>
            {
                var pi = new ProcessStartInfo("cl.exe", "--script-encoding=gbk --debug=false script.js " + Guid.NewGuid().ToString());
                //pi.CreateNoWindow = true;
                //pi.WindowStyle = ProcessWindowStyle.Hidden;
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
        public void Quit(int t)
        {
            if (lg.Visible) lg.Quit();
            if (t == 1)
            {
                tc.Send(new msg() { act = "quit" });
                Thread.Sleep(2 * 1000);
            }
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
                    SetCode?.Invoke(m.body);
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
                    //if (wx != null && wx.Visible) wx.OutLog(m.body);
                    Debug.WriteLine("log->" + m.body);
                    break;
                case "quit":
                    Quit(1);
                    break;
                case "loadcontact":
                    Debug.WriteLine("contact->" + m.body.Length);
                    break;
            }
        }

        public class User
        {
            public string uin { get; set; }
            public string nickname { get; set; }
            public string headimg { get; set; }
        }
    }
}
