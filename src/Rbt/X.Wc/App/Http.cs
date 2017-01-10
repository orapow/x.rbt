﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using X.Core.Utility;

namespace X.Wc.App
{
    /// <summary>
    /// Http通讯类
    /// </summary>
    public class Http
    {
        int timeout = 0;
        CookieContainer cookie;

        public CookieContainer Cookies { get { return cookie; } }

        public Http() : this(null, 30) { }
        public Http(CookieContainer cks, int tout)
        {
            //hcl = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            if (cks == null) cookie = new CookieContainer();
            else cookie = cks;
            //if (cks != null) hcl.CookieContainer = cks;
            //hcl.UseCookies = true;
            //hcl.UseDefaultCredentials = true;
            //hcl.UseProxy = true;
            //hc = new HttpClient(hcl);
            //hc.Timeout = new TimeSpan(tout * 10000000);
            timeout = tout;
        }

        byte[] get(string url)
        {
            GC.Collect();

            var req = (HttpWebRequest)WebRequest.Create(url);
            req.ServicePoint.ConnectionLimit = 512;

            #region head
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Maxthon/4.9.3.1000 Chrome/39.0.2146.0 Safari/537.36";
            req.Headers.Set("Pragma", "no-cache");
            req.Method = "GET";
            req.ReadWriteTimeout = 30 * 1000;
            req.KeepAlive = true;
            req.Timeout = timeout * 1000;
            req.CookieContainer = Cookies;  //启用cookie
            #endregion

            HttpWebResponse rsp = null;
            try
            {
                rsp = (HttpWebResponse)req.GetResponse();

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
                if (rsp != null) rsp.Close();
                req.Abort();
                req = null;
            }

        }

        byte[] post(string url, byte[] data) { return post(url, data, ""); }
        byte[] post(string url, byte[] data, string boundary)
        {
            //hc.CancelPendingRequests();
            //return await hc.PostAsync(url, new ByteArrayContent(data)).Result.Content.ReadAsByteArrayAsync();

            GC.Collect();

            if (url.Contains("https://")) ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) => { return true; });

            var req = (HttpWebRequest)WebRequest.Create(url);
            req.Accept = "application/json, text/plain, */*";
            req.ServicePoint.ConnectionLimit = 512;
            req.ServicePoint.Expect100Continue = false;
            req.Method = "POST";
            req.KeepAlive = true;
            req.ContentType = "application/json;charset=UTF-8";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Maxthon/4.9.3.1000 Chrome/39.0.2146.0 Safari/537.36";
            req.ContentLength = data.Length;
            req.CookieContainer = Cookies;
            req.Headers.Set("Pragma", "no-cache");

            if (!string.IsNullOrEmpty(boundary)) req.ContentType = "multipart/form-data;charset=utf-8;boundary=" + boundary;

            HttpWebResponse rsp = null;
            try
            {
                using (var req_st = req.GetRequestStream()) req_st.Write(data, 0, data.Length);

                rsp = (HttpWebResponse)req.GetResponse();

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
                if (rsp != null) rsp.Close();
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
                    data = Encoding.UTF8.GetString(post(url, Encoding.UTF8.GetBytes(body)))
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
        public Resp PostFile(string url, Dictionary<string, string> dict, byte[] file)
        {
            var tk = "WebKitFormBoundary" + Tools.GetRandRom(16, 3);
            var boundary = "\r\n--" + tk;

            var sr = new MemoryStream();
            var bd = Encoding.UTF8.GetBytes(boundary + "\r\n");

            foreach (var k in dict.Keys)
            {
                var item = Encoding.UTF8.GetBytes(string.Format("Content-Disposition:form-data;name=\"{0}\"\r\nContent-Type:text/plain\r\n\r\n{1}", k, dict[k]));
                sr.Write(bd, 0, bd.Length);
                sr.Write(item, 0, item.Length);
            }

            sr.Write(bd, 0, bd.Length);
            var fi = Encoding.UTF8.GetBytes(string.Format("Content-Disposition:form-data;name=\"{0}\";filename=\"{1}\"\r\nContent-Type:{2}\r\n\r\n", "filename", dict["name"], dict["type"]));
            sr.Write(fi, 0, fi.Length);
            sr.Write(file, 0, file.Length);

            var endbd = Encoding.UTF8.GetBytes(boundary + "--\r\n");
            sr.Write(endbd, 0, endbd.Length);

            try
            {
                return new Resp() { data = Encoding.UTF8.GetString(post(url, sr.ToArray(), tk)) };
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
            if (Cookies == null) return "";
            var cks = Cookies.GetCookies(new Uri("http://qq.com"));
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