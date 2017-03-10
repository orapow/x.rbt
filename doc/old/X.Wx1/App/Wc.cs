using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using X.Core.Utility;

namespace X.Wx.App
{
    /// <summary>
    /// Http通讯类
    /// </summary>
    public class Wc
    {
        // Cookie 容器
        private CookieContainer cookie;
        int timeout = 60; //超时 秒钟
        string guid = "";

        public CookieContainer Cookies { get { return cookie; } }

        public Wc()
        {
            cookie = new CookieContainer();
            guid = Guid.NewGuid().ToString();
        }

        byte[] get(string url)
        {
            GC.Collect();

            var req = (HttpWebRequest)WebRequest.Create(url);
            req.ServicePoint.ConnectionLimit = 512;

            #region head
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Maxthon/4.9.3.1000 Chrome/39.0.2146.0 Safari/537.36";
            req.Referer = "https://wx2.qq.com/?&lang=zh_CN";
            req.Headers.Set("Pragma", "no-cache");
            req.Method = "GET";
            req.ReadWriteTimeout = timeout * 1000;
            req.KeepAlive = true;
            req.Timeout = timeout * 1000;
            req.CookieContainer = cookie;  //启用cookie
            #endregion

            var rsp = (HttpWebResponse)req.GetResponse();
            try
            {
                int count = (int)rsp.ContentLength;
                byte[] buf = new byte[count];
                using (var rsp_st = rsp.GetResponseStream())
                {
                    int offset = 0;
                    while (count > 0)  //读取返回数据
                    {
                        int n = rsp_st.Read(buf, offset, count);
                        if (n == 0) break;
                        count -= n;
                        offset += n;
                    }
                }
                return buf;
            }
            catch
            {
                throw;
            }
            finally
            {
                rsp.Close();
                req.Abort();
                req = null;
            }


        }

        byte[] post(string url, string body)
        {
            GC.Collect();

            byte[] data = Encoding.UTF8.GetBytes(body);

            var req = (HttpWebRequest)WebRequest.Create(url);
            req.ServicePoint.ConnectionLimit = 512;

            #region header
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Maxthon/4.9.3.1000 Chrome/39.0.2146.0 Safari/537.36";
            req.Referer = "https://wx2.qq.com/?&lang=zh_CN";
            req.Headers.Set("Pragma", "no-cache");
            req.Method = "POST";
            req.ReadWriteTimeout = timeout * 1000;
            req.KeepAlive = true;
            req.Timeout = timeout * 1000;
            req.ContentLength = data.Length;
            req.CookieContainer = cookie;  //启用cookie
            #endregion

            try
            {
                using (var req_st = req.GetRequestStream()) req_st.Write(data, 0, data.Length);
            }
            catch
            {
                req.Abort();
                throw;
            }
            finally { }

            var rsp = (HttpWebResponse)req.GetResponse();

            try
            {
                int count = (int)rsp.ContentLength;
                byte[] buf = new byte[count];
                using (var rsp_st = rsp.GetResponseStream())
                {
                    int offset = 0;
                    while (count > 0)  //读取返回数据
                    {
                        int n = rsp_st.Read(buf, offset, count);
                        if (n == 0) break;
                        count -= n;
                        offset += n;
                    }
                }
                return buf;
            }
            catch { throw; }
            finally
            {
                rsp.Close();
                req.Abort();
                req = null;
            }

        }

        #region 封装了PostData, GetSrc 和 GetFile 方法
        /// <summary>
        /// 向指定的 URL POST 数据，并返回页面
        /// </summary>
        /// <param name="url">POST URL</param>
        /// <param name="body">POST 的 数据</param>
        /// <returns>页面的源文件</returns>
        public Resp PostStr(string url, string body)
        {
            try
            {
                return new Resp()
                {
                    data = Encoding.UTF8.GetString(post(url, body))
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
        public Resp PostData(string url, Dictionary<string, string> dict, FileInfo fi)
        {
            var tk = "------WebKitFormBoundary" + Tools.GetRandRom(16, 3);
            var body = new StringBuilder();
            foreach (var k in dict.Keys)
            {
                body.Append(tk + "\r\n");
                body.Append("Content - Disposition: form - data; name = \"id\"\r\n\r\n");
                body.Append(dict[k] + "\r\n");
            }
            body.Append(tk + "\r\n");
            body.Append("Content - Disposition: form - data; name = \"filename\"; filename = \"" + fi.Name + "\"\r\n");
            body.Append("Content - Type: image / png\r\n");
            body.Append("\r\n\r\n");
            body.Append(tk + "\r\n");

            return PostStr(url, body.ToString());

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
                return new Resp() { data = Encoding.UTF8.GetString(get(url)) };
            }
            catch (Exception ex)
            {
                return new Resp() { err = true, msg = ex.Message };
            }
        }
        /// <summary>
        /// 从指定的 URL 下载文件到本地
        /// </summary>
        /// <param name="uriString">文件 URL</param>
        /// <returns></returns>
        public Resp GetFile(string url)
        {
            try
            {
                return new Resp() { data = get(url) };
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