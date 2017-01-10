using System;
using System.Drawing;
using System.Threading;

namespace X.Wx
{
    public partial class Login : Base
    {
        public Image headimg { get { return pb_headimg.Image; } }

        public Login()
        {
            InitializeComponent();
        }

        public void Quit()
        {
            Invoke((Action)(() =>
            {
                isquit = false;
                this.Close();
            }));
        }

        public void SetHeadimg(string hdimg)
        {
            Invoke((Action)(() =>
            {
                pb_headimg.Image = base64ToImage(hdimg);
                lb_tip.Text = "已扫描\r\n请在手机上确认";
            }));
        }

        public void SetQrcode(string qrcode)
        {
            Invoke((Action)(() =>
            {
                pb_headimg.Image = base64ToImage(qrcode);
                lb_tip.Text = "请使用微信扫描二维码";
            }));
        }

        public void SetLoged()
        {
            Invoke((Action)(() =>
            {
                lb_tip.Text = "已登陆\r\n正在同步通讯录...";
            }));
        }

    }
}
