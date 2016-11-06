using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace Top.Api
{
    public class ConvertWebPage2Image
    {
        private void MethodName()
        {
            string url = "http://www.8888012.com/Chart/pk10.aspx";
            
            Bitmap x = GetBitmap(url, 800, 840);
            string FileName = DateTime.Now.ToString("yyyyMMddhhmmss");

            x.Save(FileName + ".jpg");
        }

        public Bitmap GetBitmap(string Url, int ImageWidth, int ImageHeight)
        {
            WebBrowser MyBrowser = new WebBrowser();
            MyBrowser.ScrollBarsEnabled = false;
            MyBrowser.Width = ImageWidth;
            MyBrowser.Height = ImageHeight;

            MyBrowser.Navigate(Url);
            while (MyBrowser.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }

            Bitmap myBitmap = new Bitmap(ImageWidth, ImageHeight);
            Rectangle DrawRect = new Rectangle(0, 0, ImageWidth, ImageHeight);
            MyBrowser.DrawToBitmap(myBitmap, DrawRect);
            return myBitmap;
        }
    }


}
