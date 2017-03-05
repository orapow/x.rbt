using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace X.Wx.App
{
    public class Curl
    {
        public static string cookie { get; private set; }
        static string bin = AppDomain.CurrentDomain.BaseDirectory + "curl.exe";

        static string getparms()
        {
            return "-A \"Mozilla / 5.0(Windows NT 10.0; WOW64) AppleWebKit / 537.36(KHTML, like Gecko) Maxthon / 4.9.3.1000 Chrome / 39.0.2146.0 Safari / 537.36\" -H \"Accept-Encoding:gzip,deflate; Connection:keep-alive; Content-Type: text/html; charset=utf-8;\r\n Cookie:" + cookie + "\"";
        }

        public static string GetCookie(string name) { return ""; }

        private static Resp curl(string url) { return curl(url, "", false); }
        private static Resp curl(string url, bool isfile) { return curl(url, "", isfile); }
        private static Resp curl(string url, string body) { return curl(url, body, false); }
        private static Resp curl(string url, string body, bool isfile)
        {
            var cfn = Guid.NewGuid().ToString();

            var ps = url + getparms() + " -D tmp\\" + cfn + ".h" + " -o tmp\\" + cfn + ".d";
            if (!string.IsNullOrEmpty(body)) ps += " -d '" + body + "'";

            var psi = new ProcessStartInfo(bin, ps);
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            var p = new Process();
            p.StartInfo = psi;
            p.Start();
            p.WaitForExit();

            var cf = AppDomain.CurrentDomain.BaseDirectory + "tmp\\" + cfn;
            if (File.Exists(cf + ".h"))
            {
                var txt = File.ReadAllText(cf + ".h");
                File.Delete(cf + ".h");
                var reg = new Regex("Set-Cookie: ([^=]+)=([^;]+)");
                foreach (Match m in reg.Matches(txt)) cookie += m.Groups[1] + "=" + m.Groups[2] + "; ";
            }

            var r = new Resp() { err = false };

            if (File.Exists(cf + ".d"))
            {
                if (isfile) r.data = File.ReadAllBytes(cf + ".d");
                else r.data = File.ReadAllText(cf + ".d");
                File.Delete(cf + ".d");
            }
            else r.err = true;

            if (r.data == null || r.data + "" == "") r.err = true;

            return r;

        }

        public static Resp GetStr(string url)
        {
            return curl(url);
        }

        public static Resp GetFile(string url)
        {
            return curl(url, true);
        }

        public static Resp PostStr(string url, string body)
        {
            return curl(url, body);
        }

        public static Resp PostFile(string url, Dictionary<string, string> dict, byte[] file)
        {
            return new Resp();
        }

        public class Resp
        {
            public bool err { get; set; }
            public string msg { get; set; }
            public object data { get; set; }

        }

    }
}
