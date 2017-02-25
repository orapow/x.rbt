using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using X.Wx.App;
using static X.Wx.App.Wc;
using X.Core.Utility;
using System.Drawing.Imaging;

namespace X.Wx
{
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public partial class Main : Base
    {
        public delegate void LoadReplyHandler();
        public event LoadReplyHandler LoadReply;

        string headimg = "";

        public Main(Wc w, string hd) : base(w)
        {
            InitializeComponent();

            if (w.user == null) Environment.Exit(0);

            User = w.user;
            pb_head.Image = base64ToImage(hd);
            ni_tip.Text = Text = User.NickName;
            var bmp = (Bitmap)pb_head.Image;
            Icon = Icon.FromHandle(bmp.GetHicon());
            ni_tip.Icon = Icon;

            wb.Navigate("http://rbt.80xc.com/rbt/chat.html");
            wb.ObjectForScripting = this;

            headimg = hd;
            sp_p.SplitterDistance = 283;
            sp_p.Panel2Collapsed = true;
        }

        public void SetMsg(object m)
        {
            try
            {
                Invoke((Action)(() =>
                {
                    wb.Document.InvokeScript("newmsg", new object[] { Serialize.ToJson(m) });
                }));
            }
            catch { }
        }

        public void SetContact(List<Contact> contacts, string gpname)
        {
            Invoke((Action)(() =>
            {
                TreeNodeCollection ptn = null;
                if (string.IsNullOrEmpty(gpname))
                {
                    contacts = contacts.OrderBy(o => o.UserName[1] == '@' ? 0 : 1).ToList();
                    ptn = tv_contact.Nodes;
                }
                else ptn = tv_contact.Nodes.Find(gpname, false)[0].Nodes;

                foreach (var c in contacts)
                {
                    var tn = new TreeNode(string.IsNullOrEmpty(c.RemarkName) ? c.NickName : c.RemarkName);
                    tn.Name = c.UserName;
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
                    ptn.Add(tn);
                }

            }));
        }

        public void ShowBox(string un)
        {
            var tn = tv_contact.Nodes.Find(un, true)[0];
            if (tn == null) return;
            sp_p.Panel2Collapsed = false;
            gp_msg.Tag = tn.Tag;
            gp_msg.Text = tn.Text;
        }

        public void OutLog(string log)
        {
            try
            {
                Invoke((Action)(() =>
                {
                    wb.Document.InvokeScript("showlog", new object[] { log });
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
            sp_p.Panel2Collapsed = false;
            gp_msg.Tag = tv_contact.SelectedNode.Tag;
            gp_msg.Text = tn.Text;
        }

        private void pb_msg_close_Click(object sender, EventArgs e)
        {
            gp_msg.Tag = null;
            sp_p.Panel2Collapsed = true;
        }

        private void bt_send_Click(object sender, EventArgs e)
        {
            Send(tb_msg.Text, 1);
            tb_msg.Text = "";
        }

        private void bt_send_pic_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "图片文件|*.jpg;*.png;*.gif";
            if (ofd.ShowDialog() != DialogResult.OK) return;
            Send(ofd.FileName, 2);
        }

        void Send(string cot, int type)
        {
            var ct = gp_msg.Tag as Contact;
            if (ct != null) wx.Send(new List<string>() { ct.UserName }, type, cot);

            object m = null;
            if (ct.UserName[1] == '@')
            {
                m = new
                {
                    body = type == 1 ? cot : "<img src='file:///" + cot + "'/>",
                    u = new
                    {
                        name = User.NickName,
                        img = headimg,
                        id = User.UserName
                    },
                    r = new
                    {
                        name = ct.NickName,
                        id = ct.UserName
                    }
                };
            }
            else
            {
                m = new
                {
                    body = type == 1 ? cot : "<img src='file:///" + cot + "'/>",
                    u = new
                    {
                        name = User.NickName,
                        img = headimg,
                        id = User.UserName
                    }
                };
            }
            wb.Document.InvokeScript("sendmsg", new object[] { Serialize.ToJson(m) });
        }
    }
}
