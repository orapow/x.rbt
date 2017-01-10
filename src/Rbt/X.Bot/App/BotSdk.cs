using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using X.Core.Utility;

namespace X.Bot.App
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

    ///// <summary>
    ///// Http通讯类
    ///// </summary>
    //public class Wc
    //{
    //    int timeout = 0;
    //    CookieContainer cookie;

    //    public CookieContainer Cookies { get { return cookie; } }

    //    public Wc() : this(null, 30) { }
    //    public Wc(CookieContainer cks, int tout)
    //    {
    //        if (cks == null) cookie = new CookieContainer();
    //        else cookie = cks;
    //        timeout = tout;
    //    }

    //    byte[] get(string url)
    //    {
    //        GC.Collect();

    //        var req = (HttpWebRequest)WebRequest.Create(url);
    //        req.ServicePoint.ConnectionLimit = 512;

    //        #region head
    //        req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
    //        req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Maxthon/4.9.3.1000 Chrome/39.0.2146.0 Safari/537.36";
    //        req.Headers.Set("Pragma", "no-cache");
    //        req.Method = "GET";
    //        req.ReadWriteTimeout = 30 * 1000;
    //        req.KeepAlive = true;
    //        req.Timeout = timeout * 1000;
    //        req.CookieContainer = Cookies;  //启用cookie
    //        #endregion

    //        HttpWebResponse rsp = null;
    //        try
    //        {
    //            rsp = (HttpWebResponse)req.GetResponse();

    //            int count = (int)rsp.ContentLength;
    //            byte[] buf = new byte[count];
    //            using (var rsp_st = rsp.GetResponseStream())
    //            {
    //                int offset = 0;
    //                while (count > 0)  //读取返回数据
    //                {
    //                    int n = rsp_st.Read(buf, offset, count);
    //                    if (n == 0) break;
    //                    count -= n;
    //                    offset += n;
    //                }
    //            }

    //            return buf;
    //        }
    //        catch
    //        {
    //            throw;
    //        }
    //        finally
    //        {
    //            if (rsp != null) rsp.Close();
    //            req.Abort();
    //            req = null;
    //        }

    //    }

    //    byte[] post(string url, byte[] data) { return post(url, data, ""); }
    //    byte[] post(string url, byte[] data, string boundary)
    //    {
    //        GC.Collect();

    //        if (url.Contains("https://")) ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) => { return true; });

    //        var req = (HttpWebRequest)WebRequest.Create(url);
    //        req.Accept = "application/json, text/plain, */*";
    //        req.ServicePoint.ConnectionLimit = 512;
    //        req.ServicePoint.Expect100Continue = false;
    //        req.Method = "POST";
    //        req.KeepAlive = true;
    //        req.ContentType = "application/json;charset=UTF-8";
    //        req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Maxthon/4.9.3.1000 Chrome/39.0.2146.0 Safari/537.36";
    //        req.ContentLength = data.Length;
    //        req.CookieContainer = Cookies;
    //        req.Headers.Set("Pragma", "no-cache");

    //        if (!string.IsNullOrEmpty(boundary)) req.ContentType = "multipart/form-data;charset=utf-8;boundary=" + boundary;

    //        HttpWebResponse rsp = null;
    //        try
    //        {
    //            using (var req_st = req.GetRequestStream()) req_st.Write(data, 0, data.Length);

    //            rsp = (HttpWebResponse)req.GetResponse();

    //            int count = (int)rsp.ContentLength;
    //            byte[] buf = new byte[count];
    //            using (var rsp_st = rsp.GetResponseStream())
    //            {
    //                int offset = 0;
    //                while (count > 0)  //读取返回数据
    //                {
    //                    int n = rsp_st.Read(buf, offset, count);
    //                    if (n == 0) break;
    //                    count -= n;
    //                    offset += n;
    //                }
    //            }
    //            return buf;
    //        }
    //        catch { throw; }
    //        finally
    //        {
    //            if (rsp != null) rsp.Close();
    //            req.Abort();
    //            req = null;
    //        }

    //    }

    //    #region 封装了PostData, GetSrc 和 GetFile 方法
    //    /// <summary>
    //    /// 向指定的 URL POST 数据，并返回页面
    //    /// </summary>
    //    /// <param name="url">POST URL</param>
    //    /// <param name="body">POST 的 数据</param>
    //    /// <returns>页面的源文件</returns>
    //    public Resp PostStr(string url, string body)
    //    {
    //        try
    //        {
    //            return new Resp()
    //            {
    //                data = Encoding.UTF8.GetString(post(url, Encoding.UTF8.GetBytes(body)))
    //            };
    //        }
    //        catch (Exception ex)
    //        {
    //            return new Resp() { err = true, msg = ex.Message };
    //        }
    //    }
    //    /// <summary>
    //    /// 获得指定 URL 的源文件
    //    /// </summary>
    //    /// <param name="url">页面 URL</param>
    //    /// <returns>页面的源文件</returns>
    //    public Resp GetStr(string url)
    //    {
    //        try
    //        {
    //            return new Resp() { data = Encoding.UTF8.GetString(get(url)) };
    //        }
    //        catch (Exception ex)
    //        {
    //            return new Resp() { err = true, msg = ex.Message };
    //        }
    //    }

    //    /// <summary>
    //    /// 添加cookie
    //    /// </summary>
    //    /// <param name="name"></param>
    //    /// <param name="value"></param>
    //    public void SetCookie(string name, string value)
    //    {
    //        cookie.Add(new Cookie() { Name = name, Value = value });
    //    }

    //    public class Resp
    //    {
    //        public bool err { get; set; }
    //        public string msg { get; set; }
    //        public object data { get; set; }

    //    }
    //    #endregion

    //}

}
