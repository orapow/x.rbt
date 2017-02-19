using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;

namespace X.App.Views.app.red
{
    public class qrcode : xview
    {
        public string id { get; set; }
        protected override string GetParmNames
        {
            get
            {
                return "id";
            }
        }
        protected override void InitDict()
        {
            base.InitDict();

            var bmp = new Bitmap(512, 512);
            var g = Graphics.FromImage(bmp);

            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.Clear(Color.Transparent);

            using (var ms = new MemoryStream())
            {
                var qr = new QrEncoder();
                var cd = qr.Encode("http://" + cfg.domain + "/wx/red/share-" + id + ".html");
                var rd = new GraphicsRenderer(new FixedModuleSize(15, QuietZoneModules.Two));
                rd.WriteToStream(cd.Matrix, ImageFormat.Jpeg, ms);
                var img = Image.FromStream(ms);
                g.DrawImage(img, new Rectangle(0, 0, 512, 512), new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
                img.Dispose();
            }

            g.Dispose();

            var base64 = "";
            using (var ms = new MemoryStream())
            {
                var ci = ImageCodecInfo.GetImageEncoders().FirstOrDefault(o => o.MimeType == "image/png");
                bmp.Save(ms, ImageFormat.Png);
                base64 = Convert.ToBase64String(ms.ToArray());
            }

            bmp.Dispose();

            dict.Add("img", base64);

        }
    }
}
