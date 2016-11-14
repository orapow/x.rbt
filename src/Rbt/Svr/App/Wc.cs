using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using X.Core.Utility;

namespace Rbt.Svr
{
    /// <summary>
    /// Http通讯类
    /// </summary>
    public class Wc : WebClient
    {
        // Cookie 容器
        private CookieContainer cookie = null;
        int timeout = 10; //超时 秒
        string guid = "";

        public Wc()
        {
            guid = Guid.NewGuid().ToString();
            Encoding = Encoding.UTF8;
            Proxy = null;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var req = base.GetWebRequest(address) as HttpWebRequest;
            req.Timeout = timeout * 1000;
            req.CookieContainer = cookie;
            //req.KeepAlive = false;
            req.Proxy = null;
            return req;
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #region 封装了PostData, GetSrc 和 GetFile 方法
        /// <summary>
        /// 向指定的 URL POST 数据，并返回页面
        /// </summary>
        /// <param name="url">POST URL</param>
        /// <param name="body">POST 的 数据</param>
        /// <returns>页面的源文件</returns>
        public Resp PostData(string url, string body)
        {
            try
            {
                return new Resp()
                {
                    data = UploadString(url, body)
                };
            }
            catch (Exception ex)
            {
                return new Resp() { err = true, msg = ex.Message };
            }
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="dict"></param>
        /// <param name="fi"></param>
        /// <returns></returns>
        public Resp PostFile(string url, Dictionary<string, string> dict, FileInfo fi)
        {
            var tk = "------WebKitFormBoundary" + Tools.GetRandRom(16, 3);

            var data = new List<byte>();

            #region 头部
            var hd = new StringBuilder();
            foreach (var k in dict.Keys)
            {
                hd.Append(tk + "\r\n");
                hd.Append("Content - Disposition: form - data; name = \"id\"\r\n\r\n");
                hd.Append(dict[k] + "\r\n");
            }
            hd.Append(tk + "\r\n");
            hd.Append("Content - Disposition: form - data; name = \"filename\"; filename = \"" + fi.Name + "\"\r\n");
            hd.Append("Content - Type: image / png\r\n");
            hd.Append("\r\n\r\n");
            hd.Append(tk + "\r\n");
            data.AddRange(Encoding.UTF8.GetBytes(hd.ToString()));
            #endregion

            using (var fs = fi.OpenRead())
            {
                var buf = new byte[fs.Length];
                fs.Read(buf, 0, buf.Length);
            }
            data.AddRange(data);

            return new Resp() { data = UploadData(url, "POST", data.ToArray()) };

        }
        /// <summary>
        /// 获得指定 URL 的源文件
        /// </summary>
        /// <param name="url">页面 URL</param>
        /// <returns>页面的源文件</returns>
        public Resp GetFile(string url)
        {
            try
            {
                return new Resp() { data = Convert.ToBase64String(DownloadData(url)) };
            }
            catch (Exception ex)
            {
                return new Resp() { err = true, msg = ex.Message };
            }
        }
        /// <summary>
        /// 获得指定 URL 的源文件
        /// </summary>
        /// <param name="url">页面 URL</param>
        /// <returns>页面的源文件</returns>
        public Resp GetStr(string url)
        {
            try
            {
                return new Resp() { data = DownloadString(url) };
            }
            catch (Exception ex)
            {
                return new Resp() { err = true, msg = ex.Message };
            }
        }
        /// <summary>
        /// 获取Cookie
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetCookie(string name)
        {
            if (cookie == null) return "";
            var cks = cookie.GetCookies(new Uri("http://qq.com"));
            return cks[name]?.Value;
        }

        public class Resp
        {
            public bool err { get; set; }
            public string msg { get; set; }
            public object data { get; set; }

        }
        #endregion

    }
}