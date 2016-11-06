using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using X.Json;
using X.Json.Linq;

namespace X.Core.Utility
{
    public class Serialize
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

        #region XML
        public static string ToXml(object o)
        {
            string xmlString = string.Empty;
            using (MemoryStream ms = new MemoryStream())
            {
                var xmlSerializer = new XmlSerializer(o.GetType());
                var set = new XmlWriterSettings();
                set.OmitXmlDeclaration = true;
                var ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                var writer = XmlWriter.Create(ms, set);
                xmlSerializer.Serialize(writer, o, ns);
                xmlString = Encoding.UTF8.GetString(ms.ToArray());
            }
            return xmlString;
        }
        public static T FormXml<T>(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(new MemoryStream(Encoding.Unicode.GetBytes(xml)));
        }
        #endregion
    }
}
