using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Text;
using X.Core.Utility;

namespace X.Wx.App
{
    public class Sdk
    {
        public delegate void OutLogHandler(string log);
        public static event OutLogHandler OutLog;

        static string uin = "";
        static string gateway = "http://rbt.80xc.com/api/";

        static T doapi<T>(string api, Dictionary<string, string> values) where T : Resp
        {
            var wc = new Http("uin=" + uin, 30);
            var r = Activator.CreateInstance<T>();
            var post = "";
            try
            {
                foreach (var k in values) post += k.Key + "=" + System.Web.HttpUtility.UrlEncode(k.Value) + "&";
                post = post.TrimEnd('&');
                var rsp = wc.PostStr(gateway + api, post);
                var json = rsp.data as string;
                if (string.IsNullOrEmpty(json)) throw new Exception("服务器返回空");
                OutLog?.Invoke("sdk@" + api + "->" + post + "\r\n" + json);
                return Serialize.FromJson<T>(json);
            }
            catch (Exception ex)
            {
                r.issucc = false;
                r.msg = ex.Message;
                OutLog?.Invoke("sdk@err:" + api + "->" + post + "\r\n" + ex.Message);
            }
            return r;
        }

        public static void Init(string u)
        {
            uin = u;
        }
        public static void LoadConfig() { }
        public static void LoadMsg() { }
        public static void LoadReply() { }
        public static void LoadPickup() { }

        public class Resp
        {
            public bool issucc { get; set; }
            public string msg { get; set; }
        }

        public class Config
        {

        }

        public class Msg
        {
            /// <summary>
            /// 消息类型
            /// 1、文字
            /// 2、图片
            /// </summary>
            public int type { get; set; }
            /// <summary>
            /// 用户列表
            /// </summary>
            public List<string> touser { get; set; }
            /// <summary>
            /// 消息内容
            /// </summary>
            public string content { get; set; }
        }
        public class Reply
        {
            public int tp { get; set; }
            /// <summary>
            /// 匹配方式
            /// 1、以关键字开头
            /// 2、以关键字结尾
            /// 3、全字匹配
            /// 4、包含关键字
            /// 5、正则匹配
            /// </summary>
            public int match { get; set; }
            /// <summary>
            /// 消息类型
            /// 1、文字
            /// 2、图片
            /// </summary>
            public int type { get; set; }
            public string keys { get; set; }
            public string content { get; set; }
            public List<string> users { get; set; }
        }
        public class Pickup
        {
            public List<string> users { get; set; }
            /// <summary>
            /// 匹配方式
            /// 1、以关键字开头
            /// 2、以关键字结尾
            /// 3、全字匹配
            /// 4、包含关键字
            /// 5、正则匹配
            /// </summary>
            public int match { get; set; }
            public string keys { get; set; }
            public string api { get; set; }
            public Dictionary<string, object> parms { get; set; }
        }
    }
}
