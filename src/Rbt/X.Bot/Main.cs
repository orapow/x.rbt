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
using System.Drawing;

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

            //var bmp = new Bitmap(pb_head.Image);

            //ni_tip.Icon = Icon.FromHandle(bmp.GetHicon());
            ni_tip.Text = Text + " " + lb_vip.Text;

            //bmp.Dispose();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            new Thread(o =>
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
            }).Start();

        }

        private void RunWx(TcpClient tc)
        {
            new Thread(o =>
            {
                var wx = new Wc(o as TcpClient);

                wx.NewWx += Wx_NewWx;
                wx.Exit += Wx_Exit;
                wx.Run();

                Invoke((Action)(() => { var us = new Wu(cms_user, wx); fp_wxs.Controls.Add(us); tip_info.SetToolTip(us, "正在获取信息"); }));

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
                Activate();
            }));
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.ApplicationExitCall) return;

            var dr = MessageBox.Show("是否退出程序？\r\n是：退出程序，微信号不会退出。\r\n否：窗口最小化。", "消息提示", MessageBoxButtons.YesNoCancel);
            if (dr == DialogResult.Yes)
            {
                stop = true;
                foreach (var c in fp_wxs.Controls)
                {
                    var w = c as Wu;
                    if (w == null) continue;
                    w.wx.Quit(0);
                }
            }
            else if (dr == DialogResult.No)
            {
                e.Cancel = true;
                ni_tip.ShowBalloonTip(2 * 1000, "消息提示", "窗体已经最小化，可从这里双击打开。", ToolTipIcon.Info);
                Hide();
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void pb_newwx_Click(object sender, EventArgs e)
        {
            Wc.Start();
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
            us.wx.Quit(1);
            fp_wxs.Controls.Remove(us);
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
            var dr = MessageBox.Show("是否退出程序和微信号？\r\n是：退出程序并退出微信。\r\n否：仅退出程序。", "消息提示", MessageBoxButtons.YesNoCancel);
            if (dr == DialogResult.Cancel) return;
            stop = true;
            foreach (var c in fp_wxs.Controls)
            {
                var w = c as Wu;
                if (w == null) continue;
                w.wx.Quit(dr == DialogResult.Yes ? 1 : 0);
            }
            Application.Exit();
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

        private void tsm_open_Click(object sender, EventArgs e)
        {
            var us = cms_user.SourceControl as Wu;
            if (us == null) return;
            us.Show();
        }

        private void ni_tip_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            Activate();
        }

        private void cms_notify_Opening(object sender, CancelEventArgs e)
        {
            cms_mi_showmain.Visible = !Visible;
        }
    }
}
