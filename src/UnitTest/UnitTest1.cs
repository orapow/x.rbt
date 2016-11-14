using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using X.Core.Utility;
using System.Threading;
using System.Text;
using Rbt.Svr;
using System.Text.RegularExpressions;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        static long getcurrentseconds()
        {
            return (long)(DateTime.UtcNow - DateTime.MinValue).TotalMilliseconds;
        }
        [TestMethod]
        public void TestMethod1()
        {
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            var s = 0;
            var txt = new StringBuilder();
            for (var i = 0; i <= 5; i++)
            {
                new Thread(o =>
                {
                    try
                    {
                        var wc = new Wc();
                        lock (txt)
                        {
                            var rsp = wc.GetStr("https://login.weixin.qq.com/jslogin?appid=wx782c26e4c19acffb&fun=new&lang=zh_CN&_=" + Tools.GetGreenTime(""));
                            var reg = new Regex("\"(\\S+?)\"");
                            var m = reg.Match(rsp.data + "");
                            txt.AppendLine(o + "->" + m.Groups[1].Value + "->" + wc.GetFile("https://login.weixin.qq.com/qrcode/" + m.Groups[1].Value + "?t=webwx&_=" + Tools.GetGreenTime("")).data);
                            string url = String.Format("https://login.weixin.qq.com/cgi-bin/mmwebwx-bin/login?loginicon=true&uuid={0}&tip=0&r={1}&_={2}", m.Groups[1].Value, "", getcurrentseconds());
                            new Thread(y => { wc.GetStr(url); }).Start();
                        }
                    }
                    catch (Exception ex)
                    {
                        lock (txt) txt.AppendLine(o + "->err," + ex.Message);
                    }
                    finally { s++; }
                }).Start(i);
                //Thread.Sleep(2 * 1000);
            }
            while (s <= 5) ;
            System.IO.File.WriteAllText("d:\\s.txt", txt.ToString());

            //for (long i = 1; i < 4; i++)
            //{
            //    new Thread(o =>
            //    {
            //        new Wx(i).Run();
            //    }).Start();
            //    Thread.Sleep(2000);
            //}

        }
    }
}
