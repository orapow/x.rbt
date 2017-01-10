using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using X.Core.Utility;

namespace X.Wx.App
{
    public class Sdk
    {
        static string key = "";//应用key
        static string uk = "";//用户key
        static string gateway = "http://rbt.tunnel.qydev.com/api/";//网关
        class api
        {
            public string name;
            public object ps;
        }
        static api last;

        static T doapi<T>(string api, Dictionary<string, string> values)
        {
            var wc = new WebClient();
            last = new Sdk.api() { name = api, ps = values };
            if (!string.IsNullOrEmpty(uk)) wc.Headers.Add("Cookie", "ukey=" + uk);
            var fs = new NameValueCollection();
            if (values != null) foreach (var k in values.Keys) fs.Add(k, values[k]);
            Debug.WriteLine("doapi:" + api + "|post->" + Serialize.ToJson(values));
            string json = "";
            try
            {
                var data = wc.UploadValues(gateway + api, fs);
                json = Encoding.UTF8.GetString(data);
                Debug.WriteLine("doapi:" + api + "|back->->" + json);
            }
            catch (Exception ex) { Debug.WriteLine("doapi:" + api + "|err->" + ex.Message); }
            finally
            {
                last = null;
                wc.Dispose();
            }
            if (json.Contains("0x0006"))
            {
                Check();
                if (last != null)
                    return doapi<T>(last.name, last.ps as Dictionary<string, string>);
                else
                    return default(T);
            }
            else return Serialize.FromJson<T>(json);
        }

        public static void Init(string k)
        {
            key = k;
            Check();
        }
        public static void Check()
        {
            var rsp = doapi<Resp>("check", new Dictionary<string, string>() { { "akey", key } });
            if (rsp.issucc) uk = rsp.msg;
        }
        public static MsgResp LoadMsg(string uins)
        {
            var rsp = doapi<MsgResp>("msg.load", new Dictionary<string, string>() { { "uins", uins } });
            return rsp ?? null;
        }
        public static ReplyResp LoadReply()
        {
            var rsp = doapi<ReplyResp>("reply.load", null);
            return rsp ?? new ReplyResp();
        }
        public static void ContactSync(string data, string uin)
        {
            doapi<Resp>("contact.sync", new Dictionary<string, string>() { { "data", data }, { "uin", uin } });
        }

    }
    public class Resp
    {
        public bool issucc { get; set; }
        public string msg { get; set; }
    }
    public class MsgResp : Resp
    {
        public class Msg
        {
            public int type { get; set; }
            public List<string> tousers { get; set; }
            public string content { get; set; }
        }
        public List<Msg> items { get; set; }
    }
    public class ReplyResp : Resp
    {
        public class Reply
        {
            public int tp { get; set; }
            public int match { get; set; }
            public int type { get; set; }
            public string[] keys { get; set; }
            public string content { get; set; }
            public string[] users { get; set; }
        }
        public List<Reply> replies { get; set; }
    }
}
