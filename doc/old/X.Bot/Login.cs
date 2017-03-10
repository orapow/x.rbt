using System;
using System.Drawing;
using System.Threading;
using X.Bot.App;

namespace X.Bot
{
    public partial class Login : Base
    {
        public Image headimg { get { return pb_headimg.Image; } }

        public bool isLoged = false;

        public Login()
        {
            InitializeComponent();
        }

        public void Quit()
        {
            Invoke((Action)(() =>
            {
                Close();
            }));
        }

        public void SetHeadimg(string hdimg)
        {
            Invoke((Action)(() =>
            {
                pb_headimg.Image = base64ToImage(hdimg);
                pb_headimg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
                lb_tip.Text = "已扫描\r\n请在手机上确认";
                Activate();
            }));
        }

        public void SetQrcode(string qrurl)
        {
            Invoke((Action)(() =>
            {
                try
                {
                    var rsp = Sdk.LoadQr(qrurl);
                    if (!rsp.issucc) SetQrcode(qrurl);
                    else
                    {
                        pb_headimg.Image = base64ToImage(rsp.msg);
                        pb_headimg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
                        lb_tip.Text = "请使用微信扫描二维码";
                        Activate();
                    }
                }
                catch
                {
                    Close();
                }
            }));
        }

        public void SetSucc()
        {
            isLoged = true;
            Invoke((Action)(() =>
            {
                lb_tip.Text = "已登陆\r\n正在初始化...";
            }));
            Thread.Sleep(2 * 1000);
            Invoke((Action)(() =>
            {
                Close();
            }));
        }
    }
}
