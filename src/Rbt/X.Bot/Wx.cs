using System.Drawing;

namespace X.Bot
{
    public partial class Wx : Base
    {
        public Image headimg { get { return pb_headimg.Image; } }

        public Wx()
        {
            InitializeComponent();
        }

        public void SetHeadimg(string hdimg)
        {
            pb_headimg.Image = base64ToImage(hdimg);
            lb_tip.Text = "已扫描\r\n请在手机上确认";
        }

        public void SetQrcode(string qrcode)
        {
            pb_headimg.Image = base64ToImage(qrcode);
            lb_tip.Text = "请使用微信扫描二维码";
        }
    }
}
