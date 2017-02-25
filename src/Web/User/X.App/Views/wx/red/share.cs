using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using X.Core.Utility;

namespace X.App.Views.wx.red
{
    public class share : _red
    {
        protected override void InitDict()
        {
            base.InitDict();

            var get = r.x_red_get.FirstOrDefault(o => o.get_op == cu.openid);

            if (r.status == 1)
            {
                var bmp = new Bitmap(902, 1202);
                var g = Graphics.FromImage(bmp);

                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.Clear(Color.Transparent);

                using (var ms = new MemoryStream())
                {
                    var data = Tools.GetHttpFile("http://" + cfg.domain + "/img/wx/b1.png");
                    ms.Write(data, 0, data.Length);
                    var img = Image.FromStream(ms);
                    g.DrawImage(img, new Rectangle(0, 0, bmp.Width - 1, bmp.Height - 1), new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
                    img.Dispose();
                }

                using (var ms = new MemoryStream())
                {
                    var qr = new QrEncoder();
                    var cd = qr.Encode("http://" + cfg.domain + "/wx/red/show-" + rid + "-" + get?.red_get_id + ".html");
                    var rd = new GraphicsRenderer(new FixedModuleSize(15, QuietZoneModules.Two));
                    rd.WriteToStream(cd.Matrix, ImageFormat.Jpeg, ms);
                    var img = Image.FromStream(ms);
                    g.DrawImage(img, new Rectangle(306, 632, 290, 290), new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
                    img.Dispose();
                }

                using (var ms = new MemoryStream())
                {
                    var data = Tools.GetHttpFile(Mp.head_img);
                    ms.Write(data, 0, data.Length);
                    var img = Image.FromStream(ms);
                    g.DrawImage(img, new Rectangle(326, 90, 240, 240), new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
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
}
