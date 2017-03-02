using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using X.Wx.Controls;

namespace X.Wx
{
    public partial class Main : Base
    {
        App.Wx wx = null;
        App.Wx.Contact user = null;

        public Main(App.Wx w, App.Wx.Contact u, Image hd)
        {
            InitializeComponent();
            ShowTitle = false;
            wx = w;
            user = u;
            pb_head.Image = hd;
            lb_name.Text = u.NickName;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            wx.Quit();
            base.OnClosing(e);
        }

        private void Wx_OutLog(string log)
        {
            try
            {
                this.Invoke((Action)(() =>
                {
                    textBox1.AppendText(log + "\r\n");
                }), null);
            }
            catch { }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            Text = user.NickName;
            wx.OutLog += Wx_OutLog;
            wx.ContactLoaded += Wx_ContactLoaded;
            wx.Logout += Wx_Logout;
            xPanel1.VerticalScroll.Value = 90;
        }

        private void Wx_ContactLoaded(List<App.Wx.Contact> contacts)
        {
            var cts = new List<XContact>();
            foreach (var c in contacts)
            {
                c.SetWx(wx);
                var ct = new XContact(c);
                ct.Click += Ct_Click;
                cts.Add(ct);
            }
            Invoke((Action)(() =>
            {
                xPanel1.AddContactRange(cts);
            }));
        }

        private void Ct_Click(object sender, EventArgs e)
        {
            var ct = sender as XContact;
            lb_nickname.Text = ct.User.NickName;
        }

        private void Wx_Logout()
        {
            Application.Exit();
        }

        private void bt_send_Click(object sender, EventArgs e)
        {
            if (xPanel1.Current == null) wx.Send(xPanel1.list.Where(o => o.User.UserName[1] == '@').Take(50).Select(o => o.User.UserName).ToList(), 1, tb_msg.Text);
            else wx.Send(new List<string>() { xPanel1.Current.User.UserName }, 1, tb_msg.Text);
        }

        private void bt_sendimg_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "图片文件|*.jpg;*.png;*.bmp:*.gif";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (xPanel1.Current == null) wx.Send(xPanel1.list.Where(o => o.User.UserName[1] == '@').Take(50).Select(o => o.User.UserName).ToList(), 2, ofd.FileName);
                else wx.Send(new List<string>() { xPanel1.Current.User.UserName }, 2, ofd.FileName);
            }
            xPanel1.VerticalScroll.Value = 100;
        }

    }
}
