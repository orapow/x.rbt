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
using X.Core.Utility;
using X.Web;

namespace X.App.Views.wx.red
{
    public class share : _wx
    {
        public int rid { get; set; }
        protected override string GetParmNames
        {
            get
            {
                return "rid";
            }
        }
        protected override void InitDict()
        {
            base.InitDict();

            var r = DB.x_red.FirstOrDefault(o => o.red_id == rid);
            if (r == null) throw new XExcep("红包不存在");

            dict.Add("bao", r);

            var u = DB.x_user.FirstOrDefault(o => o.user_id == r.user_id);
            dict.Add("u", u);

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
                var cd = qr.Encode("http://" + cfg.domain + "/wx/red/show-" + rid + "-" + cu.user_id + ".html");
                var rd = new GraphicsRenderer(new FixedModuleSize(15, QuietZoneModules.Two));
                rd.WriteToStream(cd.Matrix, ImageFormat.Jpeg, ms);
                var img = Image.FromStream(ms);
                g.DrawImage(img, new Rectangle(306, 632, 290, 290), new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
                img.Dispose();
            }

            using (var ms = new MemoryStream())
            {
                var data = Tools.GetHttpFile(cu.headimg);
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
