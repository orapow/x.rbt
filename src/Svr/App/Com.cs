using System;
using System.Collections.Generic;
using System.Text;
using X.Json;
using X.Json.Linq;

namespace Rbt.Svr.App
{
    public class Com
    {
        #region Json
        private const string DateTimeFormat = "yyyy-MM-dd HH:mm";

        public static string ToJson(object o) { return ToJson(o, false); }
        public static string ToJson(object o, bool ignore)
        {
            if (o == null) return "";
            var jss = new JsonSerializerSettings();
            if (ignore) jss.NullValueHandling = NullValueHandling.Ignore;
            jss.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            jss.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            return JsonConvert.SerializeObject(o, jss);
        }

        public static T FromJson<T>(string json)
        {
            if (string.IsNullOrEmpty(json)) return default(T);
            var jss = new JsonSerializerSettings();
            jss.NullValueHandling = NullValueHandling.Ignore;
            jss.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            jss.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            return JsonConvert.DeserializeObject<T>(json, jss);
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="json">json数据</param>
        /// <param name="node">解析节点</param>
        /// <returns></returns>
        public static T FromJson<T>(string json, string node)
        {
            if (string.IsNullOrEmpty(json)) return default(T);
            var jss = new JsonSerializerSettings();
            jss.NullValueHandling = NullValueHandling.Ignore;
            jss.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            var jo = JObject.Parse(json).GetValue(node);
            if (jo == null) return Activator.CreateInstance<T>();
            return jo.ToObject<T>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Dictionary<string, string> JsonToDict(string json)
        {
            var dict = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(json)) return dict;

            var jo = JObject.Parse(json);
            var n = jo.GetEnumerator();
            while (n.MoveNext())
            {
                dict.Add(n.Current.Key, n.Current.Value.ToString());
            }
            return dict;
        }

        #endregion

        #region Base64
        public static string ToBase64(string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(bytes);
        }

        public static string FormBase64(string code)
        {
            var bytes = Convert.FromBase64String(code);
            return Encoding.UTF8.GetString(bytes);
        }
        #endregion
    }
}
