using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using X.Bot.App;
using X.Core.Utility;

namespace X.Bot
{
    public partial class Login : Base
    {
        public string nickname { get; set; }
        public string headimg { get; set; }
        public string ukey { get; set; }

        string code = "";

        public Login()
        {
            InitializeComponent();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            loadQrCode();
        }

        private void doLogin(string code)
        {
            ((Action)(() =>
            {

                var rsp = BotSdk.Login(code);
                if (!rsp.issucc) { loadQrCode(); return; }

                headimg = rsp.headimg;
                nickname = rsp.nickname;
                ukey = rsp.ukey;

                Invoke((Action)(() =>
                {
                    pb_headimg.ImageLocation = headimg;
                    lb_tip.Text = nickname + " 已登陆\r\n正在进入，请稍后...";
                    Thread.Sleep(2 * 1000);
                    Close();
                }));

            })).BeginInvoke(null, null);
        }

        private void loadQrCode()
        {
            code = Tools.GetRandRom(16, 3);
            var qr = new QrEncoder();
            var cd = qr.Encode("http://rbt.tunnel.qydev.com/wx/login-" + code + ".html");
            var rd = new GraphicsRenderer(new FixedModuleSize(15, QuietZoneModules.Two));

            using (var ms = new MemoryStream())
            {
                rd.WriteToStream(cd.Matrix, ImageFormat.Jpeg, ms);
                pb_headimg.Image = Image.FromStream(ms);
            }

            doLogin(code);
        }

        private void Login_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (string.IsNullOrEmpty(headimg)) BotSdk.Cancel(code);
        }
    }
}
