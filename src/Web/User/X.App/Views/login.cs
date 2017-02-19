using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using X.App.Com;
using X.Core.Utility;

namespace X.App.Views
{
    public class login : xview
    {
        public string uk { get; set; }
        public string url { get; set; }

        private int isin = 0;

        protected override string GetParmNames
        {
            get
            {
                return "uk-url";
            }
        }

        protected override bool needus
        {
            get
            {
                return false;
            }
        }

        protected override void InitView()
        {
            base.InitView();
            if (!string.IsNullOrEmpty(uk))
            {
                cu = DB.x_user.FirstOrDefault(o => o.ukey == uk);
                if (cu != null)
                {
                    Context.Response.SetCookie(new System.Web.HttpCookie("ukey", uk));
                    isin = 1;
                }
            }
        }
        protected override void InitDict()
        {
            base.InitDict();
            if (isin == 0)
            {
                var code = Tools.GetRandRom(16, 3);
                var qr = new QrEncoder();
                var cd = qr.Encode("http://" + cfg.domain + "/wx/login-" + code + ".html");
                var rd = new GraphicsRenderer(new FixedModuleSize(15, QuietZoneModules.Two));

                using (var ms = new MemoryStream())
                {
                    rd.WriteToStream(cd.Matrix, ImageFormat.Jpeg, ms);
                    dict.Add("qrcode", Convert.ToBase64String(ms.ToArray()));
                }
                dict.Add("code", code);
            }
            dict["url"] = Secret.FormBase64(url);
            dict.Add("isin", isin);
        }
    }
}
