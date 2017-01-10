using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using X.Core.Utility;

namespace X.Wc.App
{
    public class BotSdk
    {
        static string uk = "";

        private static string gateway = "http://rbt.tunnel.qydev.com/api/";

        static string doapi(string api, Dictionary<string, string> values)
        {
            var wc = new WebClient();
            if (!string.IsNullOrEmpty(uk)) wc.Headers.Add("Cookie", "ukey=" + uk);
            var fs = new NameValueCollection();
            if (values != null) foreach (var k in values.Keys) fs.Add(k, values[k]);
            Debug.WriteLine("doapi-post->" + api + "->" + Serialize.ToJson(values));
            string json = "";
            try
            {
                var data = wc.UploadValues(gateway + api, fs);
                json = Encoding.UTF8.GetString(data);
            }
            catch { }
            Debug.WriteLine("doapi-back->" + api + "->" + json);
            //if (json.Contains("0x0006") && !onlogin && api != "login" && api != "cancel") { onlogin = true; Logout?.Invoke(); onlogin = false; return null; }
            //else 
            return json;
        }

        public static void Cancel(string code)
        {
            doapi("cancel", new Dictionary<string, string>() { { "code", code } });
        }

        public static LoginResp Login(string key)
        {
            var json = doapi("login", new Dictionary<string, string>() { { "code", key } });
            var rsp = Serialize.FromJson<LoginResp>(json);
            if (rsp == null) rsp = new LoginResp();
            if (rsp.issucc) { uk = rsp.ukey; }
            return rsp;
        }

        public static void LogonOut()
        {
            doapi("logout", null);
        }

        public static LoadMsgResp LoadMsg(string us)
        {
            var json = doapi("msg.load", new Dictionary<string, string>() { { "uins", us } });
            return Serialize.FromJson<LoadMsgResp>(json);
        }

        public class Resp
        {
            public bool issucc { get; set; }
            public string msg { get; set; }
        }
        public class LoginResp : Resp
        {
            public string headimg { get; set; }
            public string nickname { get; set; }
            public string ukey { get; set; }
        }
        public class LoadMsgResp : Resp
        {
            public class Msg
            {
                public int id { get; set; }
                public int status { get; set; }
                public string headimg { get; set; }
                public string nickname { get; set; }
            }
            public List<Msg> items { get; set; }
        }

    }
}
