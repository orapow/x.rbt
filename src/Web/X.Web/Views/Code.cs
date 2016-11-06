using System.Drawing;
using X.Core.Cache;
using X.Core.Plugin;
using X.Core.Utility;

namespace X.Web.Views
{
    public class Code : View
    {
        protected virtual int len { get { return 4; } }
        /// <summary>
        /// 
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public string ts { get; set; }
        protected override string GetParmNames
        {
            get
            {
                return "key-ts";
            }
        }
        protected override void Validate()
        {
            base.Validate();
            //Validator.Require("key", key);
        }
        public override byte[] GetResponse()
        {
            GetPageParms();
            var c = "";
            var _ts = CacheHelper.Get<string>("code.key.ts" + ts);
            if (_ts == null) c = Tools.GetRandRom(len);
            CacheHelper.Save("code." + key, c);
            CacheHelper.Save("code.key.ts", ts, 1);
            Context.Response.ContentType = "image/gif";
            return CreateImage(c);
        }
        /// <summary>
        /// 创建随机码图片
        /// </summary>
        /// <param name="randomcode">随机码</param>
        private byte[] CreateImage(string randomcode)
        {
            var randAngle = 70;
            var mapwidth = (int)(randomcode.Length * 16);
            var map = new Bitmap(mapwidth, 28);
            var graph = Graphics.FromImage(map);
            graph.Clear(Color.AliceBlue);
            graph.DrawRectangle(new Pen(Color.Black, 0), 0, 0, map.Width - 1, map.Height - 1);
            var blackPen = new Pen(Color.LightGray, 0);
            for (var i = 0; i < 50; i++)
            {
                var x = Tools.GetRandNext(0, map.Width);
                var y = Tools.GetRandNext(0, map.Height);
                graph.DrawRectangle(blackPen, x, y, 1, 1);
            }

            var chars = randomcode.ToCharArray();

            var format = new StringFormat(StringFormatFlags.NoClip);
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            var c = new Color[] { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Purple };
            var font = new string[] { "Verdana", "Microsoft Sans Serif", "Comic Sans MS", "Arial", "宋体" };

            for (var i = 0; i < chars.Length; i++)
            {
                var cindex = Tools.GetRandNext(7);
                var findex = Tools.GetRandNext(5);
                var f = new Font(font[findex], 16, FontStyle.Bold);
                Brush b = new SolidBrush(c[cindex]);
                var dot = new Point(12, 12);
                var angle = Tools.GetRandNext(0, randAngle);
                graph.TranslateTransform(dot.X, dot.Y);
                graph.RotateTransform(angle);
                graph.DrawString(chars[i].ToString(), f, b, 2, 6, format);
                graph.RotateTransform(-angle);
                graph.TranslateTransform(2, -dot.Y);
            }

            var ms = new System.IO.MemoryStream();
            map.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            var data = ms.ToArray();
            graph.Dispose();
            map.Dispose();
            ms.Close();
            return data;
        }
    }
}
