using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace X.Wx.Controls
{
    public partial class XContact : UserControl
    {
        public bool ShowBag { get; set; }
        public int MsgCount { get; set; }
        public string LastMsg { get; set; }
        public bool Selected { get; set; }

        public App.Wx.Contact User { get; }
        public string NickName { get; }

        public XContact() : this(null) { }

        private bool isloading = false;

        public XContact(App.Wx.Contact u)
        {
            InitializeComponent();
            lb_msg.Text = "";
            if (u != null)
            {
                User = u;
                lb_name.Text = u.NickName;
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            BackColor = Color.FromArgb(0x3A, 0x3F, 0x45);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (!Selected) BackColor = Color.Transparent;
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            var xp = this.Parent.Parent as XPanel;
            if (xp.Current != null && xp.Current != this) { xp.Current.Selected = false; xp.Current.BackColor = Color.Transparent; }

            Selected = true;
            BackColor = Color.FromArgb(0x3A, 0x3F, 0x45);
            xp.Current = this;
        }

        public void LoadImage()
        {
            if (isloading) return;
            isloading = true;
            if (pb_image.Image != null) return;
            ((Action)(() =>
            {
                var img = User.LoadImage();
                if (string.IsNullOrEmpty(img)) return;
                var fr = this.FindForm() as BaseForm;
                if (fr != null) pb_image.Image = fr.base64ToImage(img);
                isloading = false;
            })).BeginInvoke(null, null);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (MsgCount > 0)
            {
                if (ShowBag) { lb_bag.Visible = true; lb_dot.Visible = false; lb_msg.Text = LastMsg; lb_bag.Text = MsgCount > 99 ? "..." : MsgCount + ""; }
                else { lb_bag.Visible = false; lb_dot.Visible = true; lb_msg.Text = "[" + MsgCount + "条]" + LastMsg; }
            }
            base.OnPaint(e);
        }

    }
}
