using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Text;
using X.Core.Utility;

namespace X.Lpw.App
{
    public class Sdk
    {
        public delegate void OutLogHandler(string log);
        public static event OutLogHandler OutLog;

        static string gateWay;//网关
        static int city_id = 0;
        static T doapi<T>(string api, object o) where T : Resp
        {
            return (T)doapi<T>(api, o, true);
        }
        static T doapi<T>(string api, object o, bool scr) where T : Resp
        {
            var wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            string json = "";
            var r = Activator.CreateInstance<T>();
            var post = Serialize.ToJson(o);
            try
            {
                var ps = o as Dictionary<string, string>;
                var url = api.StartsWith("http://") ? api : gateWay + api;
                if (ps == null) json = wc.UploadString(url, scr ? Secret.Rc4.Encrypt(post) : post);
                else
                {
                    var parms = new NameValueCollection();
                    foreach (var k in ps.Keys) { parms.Add(Secret.Rc4.Encrypt(k), Secret.Rc4.Encrypt(ps[k])); }
                    json = Encoding.UTF8.GetString(wc.UploadValues(url, parms));
                }
                if (string.IsNullOrEmpty(json)) throw new Exception("服务器返回空");
                OutLog?.Invoke("sdk@" + api + "\r\n->" + post + "\r\n->" + json);
            }
            catch (Exception ex)
            {
                var msg = ex.Message + "\r\n" + ex.StackTrace;
                if (ex is WebException) using (var sr = new System.IO.StreamReader(((WebException)ex).Response.GetResponseStream())) msg = sr.ReadToEnd();
                OutLog?.Invoke("sdk@" + api + "\r\n->" + post + "\r\n->" + msg);
                r.RMessage = ex.Message;
            }

            var rp = Serialize.FromJson<T>(json);
            if (rp != null) return rp;

            return r;
        }

        public static void Init()
        {
            gateWay = Rbt.cfg.GateWay;
            city_id = Rbt.cfg.City_Id;
        }

        /// <summary>
        /// 获取消息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static MsgResp LoadMsg(List<string> names, string wxno, int stat)
        {
            return doapi<MsgResp>("/robot/regist/get", new { build_name = names, city_id = city_id + "", status = stat, weixin_id = wxno });
        }
        /// <summary>
        /// 提交报备
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static SubmitResp Submit(object m, string uid)
        {
            return doapi<SubmitResp>("/robot/regist/submit", new { regist_user_id = uid, data = m, city_id = city_id });//new Dictionary<string, string>() { { "regist_id", uid + "" }, { "data", Serialize.ToJson(m) }, { "city_id", city_id + "" } });// new { regist_id = uid + "", data = m, city_id = city_id });
        }
        /// <summary>
        /// 设置转发状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static StatusResp SetStatus(int id, int stat)
        {
            return doapi<StatusResp>("/robot/regist/upstatus", new { regist_id = id + "", stat = stat });
        }
        /// <summary>
        /// 获取城市列表
        /// </summary>
        /// <returns></returns>
        public static CityResp LoadCity()
        {
            return doapi<CityResp>("http://ldgl.loupan.com:8092/ld/InitCity", null, false);
        }
    }

    public class StatusResp : Resp
    {
        public string url { get; set; }
    }
    public class SubmitResp : Resp
    {
        public Msg Data { get; set; }
    }

    public class MsgResp : Resp
    {
        public List<Msg> Data { get; set; }
        public page Page { get; set; }
    }

    public class page
    {
        public int RAllCnt { get; set; }
        public int RIndex { get; set; }
    }

    public class CityResp : Resp
    {
        public List<City> Data { get; set; }
        public page Page { get; set; }
    }

    public class City
    {
        public int id { get; set; }
        public string name { get; set; }
        public override string ToString()
        {
            return name;
        }
    }

    public class Msg
    {
        /// <summary>
        /// 城市id
        /// </summary>
        public int city_id { get; set; }
        /// <summary>
        /// 楼盘名
        /// </summary>
        public string build_name { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string data { get; set; }
        /// <summary>
        /// 二维码地址
        /// </summary>
        public string short_url { get; set; }
        /// <summary>
        /// 发送人
        /// </summary>
        public string regist_user_id { get; set; }
        /// <summary>
        /// 报备id
        /// </summary>
        public int regist_id { get; set; }
    }

    public class Resp
    {
        public string RCode { get; set; }
        public string RMessage { get; set; }
    }
}
