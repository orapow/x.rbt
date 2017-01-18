using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using X.Core.Utility;

namespace X.Bot.App
{
    public class Wc
    {
        public Wx Wx { get; private set; }
        public Tcp Tc { get; private set; }
        public User cu { get; private set; }

        private Login lg = null;


        public Wc()
        {
            cu = new User();
            lg = new Login();
        }

        public void Run()
        {
            new Thread(o =>
            {
                var pi = new ProcessStartInfo("cl.exe", "--output-encoding='gb2312' script.js");
                var p = new Process();
                p.StartInfo = pi;
                p.Start();
                p.WaitForExit();
            }).Start();

            lg.ShowDialog();
        }

        public void ShowUi()
        {
            Wx.Show();
            Wx.Activate();
        }

        public void SetTc(Tcp tc)
        {
            Tc = tc;
            tc.NewMsg += Tc_NewMsg;
            tc.Closed += Tc_Closed;
            tc.Start();
        }

        private void Tc_Closed(Tcp tc)
        {
            //tc.Send(new msg() { act = "quit" });
        }

        private void Tc_NewMsg(msg m, Tcp tc)
        {
            //if (m.act == "login") tc.code = m.body;

            if (m.act == "qrcode") lg.SetQrcode(m.body);
            else if (m.act == "headimg")
            {
                lg.SetHeadimg(m.body);
                cu.headimg = m.body;
            }
            else if (m.act == "setuser")
            {
                var u = Serialize.FromJson<User>(m.body);
                cu.uin = u.uin;
                cu.nickname = u.nickname;

                

                Wx = new Wx(cu.nickname, cu.headimg);
                Wx.Show();
            }

            Debug.WriteLine(m.act + "->" + m.body);
        }

        public class User
        {
            public string uin { get; set; }
            public string nickname { get; set; }
            public string headimg { get; set; }
        }
    }
}
