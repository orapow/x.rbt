using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using static X.Wx.App.Wc;

namespace X.Bot
{
    public partial class Wx : Base
    {
        public string Uin { get; private set; }

        //public delegate void LoadReplyHandler();
        //public event LoadReplyHandler LoadReply;

        public Wx(string nk, string hm)
        {
            InitializeComponent();
            pb_head.Image = base64ToImage(hm);
            Text = nk;
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
            //isquit = true;
            Close();
        }

        private void 刷新回复ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //LoadReply?.Invoke();
        }

        private void pb_setting_Click(object sender, EventArgs e)
        {

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
            //var ct = gp_msg.Tag as Contact;
            //if (ct != null)
            //    wx.Send(new List<string>() { ct.UserName }, 1, tb_msg.Text);
            //tb_msg.Text = "";
            //MessageBox.Show("已发送");
        }

        private void bt_send_pic_Click(object sender, EventArgs e)
        {
            //var ofd = new OpenFileDialog();
            //ofd.Filter = "图片文件|*.jpg;*.png;*.gif";
            //if (ofd.ShowDialog() != DialogResult.OK) return;

            //var ct = gp_msg.Tag as Contact;
            //if (ct != null)
            //    wx.Send(new List<string>() { ct.UserName }, 2, ofd.FileName);
            //MessageBox.Show("已发送");
        }
    }
}
