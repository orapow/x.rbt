using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using X.Wx.App;
using static X.Wx.App.Wc;

namespace X.Wx
{
    public partial class Main : Base
    {
        public delegate void LoadReplyHandler();
        public event LoadReplyHandler LoadReply;

        public Main(Wc w, Image hd) : base(w)
        {
            InitializeComponent();

            if (w.user == null) Environment.Exit(0);

            User = w.user;
            pb_head.Image = hd;
            ni_tip.Text = Text = User.NickName;
            var bmp = (Bitmap)hd;
            Icon = Icon.FromHandle(bmp.GetHicon());
            ni_tip.Icon = Icon;
        }

        public void SetContact(List<Contact> contacts)
        {
            Invoke((Action)(() =>
            {
                contacts = contacts.OrderBy(o => o.UserName[1] == '@' ? 0 : 1).ToList();
                var nds = new List<TreeNode>();
                var imgs = new ImageList();
                foreach (var c in contacts)
                {
                    var tn = new TreeNode(string.IsNullOrEmpty(c.RemarkName) ? c.NickName : c.RemarkName);
                    tn.Tag = c;
                    if (c.MemberList != null)
                    {
                        foreach (var m in c.MemberList)
                        {
                            var stn = new TreeNode(string.IsNullOrEmpty(m.RemarkName) ? m.NickName : m.RemarkName);
                            stn.Tag = m;
                            tn.Nodes.Add(stn);
                        }
                    }
                    nds.Add(tn);
                }
                tv_contact.Nodes.AddRange(nds.ToArray());
            }));
        }

        public void OutLog(string log)
        {
            try
            {
                Invoke((Action)(() =>
                {
                    if (tb_log.Lines.Length > 65535) tb_log.Text = "";
                    tb_log.AppendText(DateTime.Now.ToString("HH:mm") + " " + log + "\r\n");
                }));
            }
            catch { }
        }

        private void pb_close_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void ni_tip_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Visible) Hide();
            else
            {
                Show();
                Activate();
            }
        }

        private void 退出QToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isquit = true;
            Close();
        }

        private void 刷新回复ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadReply?.Invoke();
        }

        private void 管理中心GToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://rbt.80xc.com/login-" + Sdk.user.uk + ".html");
        }

        private void pb_setting_Click(object sender, EventArgs e)
        {
            var pt = PointToScreen(pb_setting.Location);
            cms_menu.Show();
            cms_menu.Left = pt.X;
            cms_menu.Top = pt.Y + pb_setting.Height;
            toolStripMenuItem2.Enabled = false;
        }

        private void lb_title_DoubleClick(object sender, EventArgs e)
        {
            WindowState = WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Show();
            Activate();
        }

        private void cms_menu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            toolStripMenuItem2.Enabled = true;
        }

        private void pb_head_Click(object sender, EventArgs e)
        {
            sp_c.Panel1Collapsed = !sp_c.Panel1Collapsed;
        }

        private void tv_contact_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var tn = tv_contact.SelectedNode;
            if (tn == null) return;
            sp_p.Panel1Collapsed = false;
            gp_msg.Tag = tv_contact.SelectedNode.Tag;
            gp_msg.Text = tn.Text;
        }

        private void pb_msg_close_Click(object sender, EventArgs e)
        {
            gp_msg.Tag = null;
            sp_p.Panel1Collapsed = true;
        }

        private void bt_send_Click(object sender, EventArgs e)
        {
            var ct = gp_msg.Tag as Contact;
            if (ct != null)
                wx.Send(new List<string>() { ct.UserName }, 1, tb_msg.Text);
            tb_msg.Text = "";
            MessageBox.Show("已发送");
        }

        private void bt_send_pic_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "图片文件|*.jpg;*.png;*.gif";
            if (ofd.ShowDialog() != DialogResult.OK) return;

            var ct = gp_msg.Tag as Contact;
            if (ct != null)
                wx.Send(new List<string>() { ct.UserName }, 2, ofd.FileName);
            MessageBox.Show("已发送");

        }
    }
}
