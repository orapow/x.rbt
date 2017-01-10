using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using X.Bot.App;
using X.Bot.Ctrls;
using X.Core.Utility;
using X.Net;

namespace X.Bot
{
    public partial class Main : Base
    {
        #region 私有变量
        TcpListener svr = null;
        List<Tcp> tcps = null;
        Wx wl = null;

        bool stop = false;
        string wxbin = "";
        string uk = "";//用户身份Key
        string nk = "";//通信加密Key
        int port = 0; //服务端口
        string nickname = "";
        string headimg = "";
        #endregion

        public Main(string n, string hd, string uk)
        {
            InitializeComponent();
            ni_tip.Visible = true;
            tcps = new List<Tcp>();

            wxbin = ConfigurationManager.AppSettings["wxbin"];
            nk = ConfigurationManager.AppSettings["net-key"];
            int.TryParse(ConfigurationManager.AppSettings["port"], out port);

            if (port == 0 || string.IsNullOrEmpty(nk)) Application.Exit();

            nickname = n;
            headimg = hd;
            this.uk = uk;

            Init();
        }

        private void Init()
        {
            Text = nickname;
            pb_head.ImageLocation = headimg;
        }

        private void Main_Load(object sender, EventArgs e)
        {
            ((Action)(delegate ()
            {
                svr = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
                svr.Start();

                while (!stop)
                {
                    try
                    {
                        var tcp = new Tcp(svr.AcceptTcpClient(), NetKey);
                        if (stop) break;
                        tcp.NewMsg += Tcp_NewMsg; ; ;
                        tcp.Closed += Tcp_Closed; ;
                        tcp.Start();
                        lock (tcps) tcps.Add(tcp);
                    }
                    catch { }
                }

            })).BeginInvoke(null, null);

            ((Action)(() =>
            {
                while (!stop)
                {
                    Thread.Sleep(10 * 1000);
                    if (tcps.Count() == 0) continue;
                    var uins = "";
                    foreach (var c in tcps.Where(o => o.code != null)) uins += c.code + ",";
                    var ms = BotSdk.LoadMsg(uins.TrimEnd(','));
                    if (ms == null || ms.items == null) continue;
                }
            })).BeginInvoke(null, null);

        }
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                tr_check.Enabled = false;
                e.Cancel = true;
                Hide();
            }
        }

        private void pb_newwx_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(wxbin)) MessageBox.Show("请设置微信客户端路径！");

            new Thread(o =>
            {
                var psi = new ProcessStartInfo(wxbin, "127.0.0.1:" + port + " " + nk);
                psi.CreateNoWindow = true;
                psi.UseShellExecute = false;
                var p = new Process();
                p.StartInfo = psi;
                p.Start();
                p.WaitForExit();
            }).Start();

            wl = new Wx();
            wl.FormClosed += wl_FormClosed;
            wl.ShowDialog();

        }
        private void wl_FormClosed(object sender, FormClosedEventArgs e)
        {
            wl = null;
        }
        private void cms_user_Opening(object sender, CancelEventArgs e)
        {
            var cms = sender as ContextMenuStrip;
            var us = cms.SourceControl as Wu;
            if (us != null) cms_user_name.Text = us.nickname + " " + us.Name;
        }
        private void cm_us_exit_Click(object sender, EventArgs e)
        {
            var us = cms_user.SourceControl as Wu;
            var tc = tcps.FirstOrDefault(o => o.code == us.uin);
            if (tc != null) tc.Send(new { act = "quit" });
        }
        private void bt_usercenter_Click(object sender, EventArgs e)
        {
            Process.Start("http://rbt.tunnel.qydev.com/login-" + uk + ".html");
        }
        private void cms_mi_showmain_Click(object sender, EventArgs e)
        {
            Show();
        }
        private void cms_mi_exit_Click(object sender, EventArgs e)
        {
            stop = true;
            if (svr != null) { svr.Stop(); }
            lock (tcps) foreach (var t in tcps) t.Send(new { act = "quit" });
            Application.Exit();
        }

        //private void connectSvr()
        //{
        //    var svr = ConfigurationManager.AppSettings["svr"] ?? "";
        //    var ps = svr.Split(':');
        //    if (ps.Length == 2)
        //    {
        //        var spt = 0;
        //        int.TryParse(ps[1], out spt);
        //        if (string.IsNullOrEmpty(ps[0]) || spt == 0) MessageBox.Show("服务器参数配置错误");
        //        else
        //        {
        //            tc = new Tcp(ps[0], spt, nk);
        //            tc.NewMsg += Tc_NewMsg;
        //            tc.Closed += Tc_Closed;
        //            tc.Start();

        //            Invoke((Action)(() =>
        //            {
        //                lsb_tip.Image = Properties.Resources.dot_g;
        //                lsb_tip.Text = "已连接";
        //                lsb_tip.ToolTipText = "服务器已连接。";
        //            }));
        //        }
        //    }
        //}

        //private void Tc_Closed(Tcp tc)
        //{
        //    Invoke((Action)(() =>
        //    {
        //        lsb_tip.Image = Properties.Resources.dot_r;
        //        lsb_tip.Text = "未连接";
        //        lsb_tip.ToolTipText = "服务器已断开，任务将无法执行，正在等待重连...";
        //    }));
        //}

        //private void Tc_NewMsg(string from, string body, Tcp tc)
        //{
        //    throw new NotImplementedException();
        //}

        private void Tcp_NewMsg(string from, string body, Tcp tc)
        {
            dynamic bd = Serialize.FromJson<msg>(body);
            string act = bd.act;
            switch (act)
            {
                case "loged":
                    Invoke((Action)(() =>
                    {
                        if (wl != null) wl.Close();
                        tc.code = bd.uin;
                        var us = new Wu(cms_user, bd.uin);
                        us.Image = base64ToImage(bd.headimg);
                        us.nickname = bd.nickname;
                        fp_wxs.Controls.Add(us);
                    }));
                    break;
                case "outlog":
                    break;
                case "exit":
                    Invoke((Action)(() =>
                    {
                        fp_wxs.Controls.RemoveByKey("id:" + bd.uin);
                    }));
                    break;
                case "newmsg":
                    break;
                case "scaned":
                    if (wl != null) Invoke((Action)(() => { wl.SetHeadimg(bd.headimg); }));
                    break;
                case "qrback":
                    if (wl != null) Invoke((Action)(() => { wl.SetQrcode(bd.qrcode); }));
                    break;
                case "contactsynced":
                    break;
            }
        }
        private void Tcp_Closed(Tcp tc)
        {
            tcps.Remove(tc);
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
