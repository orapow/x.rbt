using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using X.Core.Utility;
using static X.Wx.App.Wc;

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
        private List<ReplyResp.Reply> repies = null;

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
                var p = new Process();
                p.StartInfo = pi;
                p.Start();

            }).Start();
        }

        public void Show()
        {
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
                if (wx != null) wx.Close();
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
                    wx = new Wx(cu, tc);
                    repies = Sdk.LoadReply(cu.uin).items;
                    break;
                case "newmsg":
                    var wm = Serialize.FromJson<Wxm>(m.body);
                    if (wm == null) break;
                    if (string.IsNullOrEmpty(wm.text)) break;

                    wx.OutLog("收到消息->" + wm.fr.text + (wm.rm.name[1] == '@' ? "@" + wm.rm.text : "") + "：\r\n\t" + wm.text.Replace("  ", "").Replace("&nbsp;", "").Replace("<br>", "\r\n\t").Trim());

                    if (repies == null) break;

                    ReplyResp.Reply rep = null;
                    foreach (var r in repies)
                    {
                        switch (r.tp)
                        {
                            case 1:
                                if (wm.text.Contains("我通过了你的朋友验证请求，现在我们可以开始聊天了")) rep = r;
                                wx.OutLog("匹配到 被添加 自动回复，发送了" + (rep.type == 1 ? "文本" : "图片") + "内容：" + rep.content);
                                //被添加
                                break;
                            case 2://群内新成员
                                wx.OutLog("匹配到 群内新成员 自动回复，发送了" + (rep.type == 1 ? "文本" : "图片") + "内容：" + rep.content);
                                break;
                            case 3://默认回复
                                break;
                            case 4://关键字回复
                                if (r.match == 1 && r.keys.Contains(wm.text)) rep = r;
                                else if (r.match == 2 && r.keys.Count(o => wm.text.StartsWith(o)) > 0) rep = r;
                                else if (r.match == 3 && r.keys.Count(o => wm.text.EndsWith(o)) > 0) rep = r;
                                else if (r.match == 4 && r.keys.Count(o => wm.text.Contains(o)) > 0) rep = r;
                                if (rep != null) wx.OutLog("匹配到 关键字(" + string.Join(" ", r.keys) + ") 自动回复，发送了" + (rep.type == 1 ? "文本" : "图片") + "内容：" + rep.content);
                                break;
                        }
                        if (rep != null) break;
                    }

                    if (rep != null) tc.Send(new msg() { act = "send", body = Serialize.ToJson(new { to = wm.fr.name, rep.type, rep.content }) });

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
                    Quit(1);
                    break;
                case "contact":
                    wx.SetContact(Serialize.FromJson<List<Contact>>(m.body));
                    break;
            }
        }

        class Wxm
        {
            public from fr { get; set; }
            public room rm { get; set; }
            public string text { get; set; }
            public class room
            {
                public string name { get; set; }
                public string text { get; set; }
            }
            public class from
            {
                public string name { get; set; }
                public string text { get; set; }
            }
        }

        public class User
        {
            public string uin { get; set; }
            public string username { get; set; }
            public string nickname { get; set; }
            public string headimg { get; set; }
        }
    }
}
