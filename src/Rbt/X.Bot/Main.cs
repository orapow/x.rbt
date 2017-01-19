using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using System.Linq;
using X.Bot.App;
using X.Bot.Ctrls;

namespace X.Bot
{
    public partial class Main : Base
    {
        #region 私有变量
        TcpListener svr = null;
        //List<Wc> wxs = null;

        bool stop = false;
        int port = 0; //服务端口
        #endregion

        public Main()
        {
            InitializeComponent();
            ni_tip.Visible = true;

            int.TryParse(ConfigurationManager.AppSettings["port"], out port);

            if (port == 0) Application.Exit();

            Text = Sdk.user.nk;
            pb_head.ImageLocation = Sdk.user.img;
            lb_vip.Text = "Vip用户(" + Sdk.user.dt + ")";

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ((Action)(delegate ()
            {
                svr = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
                svr.Start();

                while (!stop)
                {
                    if (!svr.Pending()) { Thread.Sleep(2 * 1000); continue; }
                    var tc = svr.AcceptTcpClient();
                    if (stop) break;
                    RunWx(tc);
                }

            })).BeginInvoke(null, null);

        }

        private void RunWx(TcpClient tc)
        {
            new Thread(o =>
            {
                var wx = new Wc(o as TcpClient);

                wx.NewWx += Wx_NewWx;
                wx.Exit += Wx_Exit;
                wx.Run();

                Invoke((Action)(() => { var us = new Wu(cms_user, "") { wx = wx }; fp_wxs.Controls.Add(us); tip_info.SetToolTip(us, "已登陆，正在获取信息"); }));

            }).Start(tc);
        }

        private void Wx_Exit(string uin)
        {
            Invoke((Action)(() =>
            {
                var ct = fp_wxs.Controls.Find("id:" + uin, false);
                if (ct.Length == 0) return;
                fp_wxs.Controls.Remove(ct[0]);
            }));
        }

        private void Wx_NewWx(Wc wx)
        {
            Invoke((Action)(() =>
            {
                Wu us = null;
                foreach (var c in fp_wxs.Controls)
                {
                    var w = c as Wu;
                    if (w == null) continue;
                    if (w.wx.cu.uin == wx.cu.uin) { us = w; break; }
                }
                if (us != null)
                {
                    us.Name = "id:" + wx.cu.uin;
                    us.Image = base64ToImage(wx.cu.headimg);
                    us.nickname = wx.cu.nickname;
                    us.SizeMode = PictureBoxSizeMode.StretchImage;
                    tip_info.SetToolTip(us, us.nickname);
                }
            }));
        }

        //private void Tc_Closed(Tcp tc)
        //{
        //    Debug.WriteLine(tc.code + "close");
        //    //throw new NotImplementedException();
        //}

        //private void Tc_NewMsg(msg m, Tcp tc)
        //{
        //    var wx = wxs.FirstOrDefault(o => o.tc.code == tc.code);
        //    if (wx == null)
        //    {
        //        wx = new Wc() { tc = tc };
        //        lock (wxs) wxs.Add(wx);
        //        wx.Run();
        //    }
        //}

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
        }

        private void pb_newwx_Click(object sender, EventArgs e)
        {
            //fp_wxs.Controls.Add(new Wu(cms_user, ""));
            Wc.Start();

            //    if (string.IsNullOrEmpty(wxbin)) MessageBox.Show("请设置微信客户端路径！");

            //    new Thread(o =>
            //    {
            //        var psi = new ProcessStartInfo(wxbin, "127.0.0.1:" + port + " " + nk);
            //        psi.CreateNoWindow = true;
            //        psi.UseShellExecute = false;
            //        var p = new Process();
            //        p.StartInfo = psi;
            //        p.Start();
            //        p.WaitForExit();
            //    }).Start();

            //    wl = new Wx();
            //    wl.FormClosed += wl_FormClosed;
            //    wl.ShowDialog();

        }

        private void cms_user_Opening(object sender, CancelEventArgs e)
        {
            var cms = sender as ContextMenuStrip;
            var us = cms.SourceControl as Wu;
            if (us != null) cms_user_name.Text = us.nickname + " " + us.Name;
        }

        private void cm_us_exit_Click(object sender, EventArgs e)
        {
            //var us = cms_user.SourceControl as Wu;
            //var tc = wxs.FirstOrDefault(o => o.code == us.uin);
            //if (tc != null) tc.Send(new { act = "quit" });
        }

        private void bt_usercenter_Click(object sender, EventArgs e)
        {
            Process.Start("http://rbt.80xc.com/login-" + Sdk.user.uk + ".html");
        }

        private void cms_mi_showmain_Click(object sender, EventArgs e)
        {
            Show();
        }

        private void cms_mi_exit_Click(object sender, EventArgs e)
        {
            //stop = true;
            //if (svr != null) { svr.Stop(); }
            //lock (tcps) foreach (var t in tcps) t.Send(new { act = "quit" });
            //Application.Exit();
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

        private void Tcp_NewMsg(msg m, Tcp tc)
        {
            //dynamic bd = Serialize.FromJson<msg>(body);
            //string act = bd.act;
            //switch (act)
            //{
            //    case "loged":
            //        Invoke((Action)(() =>
            //        {
            //            if (wl != null) wl.Close();
            //            tc.code = bd.uin;
            //            var us = new Wu(cms_user, bd.uin);
            //            us.Image = base64ToImage(bd.headimg);
            //            us.nickname = bd.nickname;
            //            fp_wxs.Controls.Add(us);
            //        }));
            //        break;
            //    case "outlog":
            //        break;
            //    case "exit":
            //        Invoke((Action)(() =>
            //        {
            //            fp_wxs.Controls.RemoveByKey("id:" + bd.uin);
            //        }));
            //        break;
            //    case "newmsg":
            //        break;
            //    case "scaned":
            //        if (wl != null) Invoke((Action)(() => { wl.SetHeadimg(bd.headimg); }));
            //        break;
            //    case "qrback":
            //        if (wl != null) Invoke((Action)(() => { wl.SetQrcode(bd.qrcode); }));
            //        break;
            //    case "contactsynced":
            //        break;
            //}
        }

        private void pb_head_Click(object sender, EventArgs e)
        {
            var pt = PointToScreen(pb_head.Location);
            cms_notify.Show();
            cms_notify.Left = pt.X;
            cms_notify.Top = pt.Y + pb_head.Height;
            cms_mi_showmain.Visible = false;
        }

        private void fp_wxs_MouseDown(object sender, MouseEventArgs e)
        {
            base.OnMouseDown(e);
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }
        //class msg : DynamicObject
        //{
        //    Dictionary<string, object> parms = new Dictionary<string, object>();
        //    public override bool TryGetMember(GetMemberBinder binder, out object result)
        //    {
        //        if (parms.ContainsKey(binder.Name)) result = parms[binder.Name];
        //        else result = null;
        //        return true;
        //    }
        //    public override bool TrySetMember(SetMemberBinder binder, object value)
        //    {
        //        if (parms.ContainsKey(binder.Name)) parms[binder.Name] = value;
        //        else parms.Add(binder.Name, value);
        //        return true;
        //    }
        //}


    }
}
