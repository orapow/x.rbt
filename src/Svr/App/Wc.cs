using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Rbt.Svr.App
{
    public class Wc
    {
        // Cookie 容器
        private CookieContainer cookie;
        int timeout = 60;
        string guid = "";
        public Wc()
        {
            cookie = new CookieContainer();
            guid = Guid.NewGuid().ToString();
        }


        byte[] get(string url)
        {

            var req = (HttpWebRequest)WebRequest.Create(url);
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Maxthon/4.9.3.1000 Chrome/39.0.2146.0 Safari/537.36";
            req.Referer = "https://wx2.qq.com/?&lang=zh_CN";
            req.Headers.Set("Pragma", "no-cache");
            req.Headers.Set("X-DevTools-Emulate-Network-Conditions-Client-Id", guid);
            req.Method = "GET";
            req.Timeout = timeout * 1000;
            req.CookieContainer = cookie;  //启用cookie

            var rsp = (HttpWebResponse)req.GetResponse();
            var rsp_st = rsp.GetResponseStream();

            int count = (int)rsp.ContentLength;
            int offset = 0;
            byte[] buf = new byte[count];
            while (count > 0)  //读取返回数据
            {
                int n = rsp_st.Read(buf, offset, count);
                if (n == 0) break;
                count -= n;
                offset += n;
            }

            rsp_st.Close();
            rsp.Close();

            return buf;

        }

        byte[] post(string url, string body)
        {

            byte[] data = Encoding.UTF8.GetBytes(body);

            var req = (HttpWebRequest)WebRequest.Create(url);
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Maxthon/4.9.3.1000 Chrome/39.0.2146.0 Safari/537.36";
            req.Referer = "https://wx2.qq.com/?&lang=zh_CN";
            req.Headers.Set("Pragma", "no-cache");
            req.Headers.Set("X-DevTools-Emulate-Network-Conditions-Client-Id", Guid.NewGuid().ToString());
            req.Method = "POST";
            req.Timeout = timeout * 1000;
            req.ContentLength = data.Length;

            Stream req_st = req.GetRequestStream();
            req_st.Write(data, 0, data.Length);
            req.CookieContainer = cookie;  //启用cookie

            var rsp = (HttpWebResponse)req.GetResponse();
            var rsp_st = rsp.GetResponseStream();

            int count = (int)rsp.ContentLength;
            int offset = 0;
            byte[] buf = new byte[count];
            while (count > 0)  //读取返回数据
            {
                int n = rsp_st.Read(buf, offset, count);
                if (n == 0) break;
                count -= n;
                offset += n;
            }

            req_st.Close();
            rsp_st.Close();
            rsp.Close();

            return buf;

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
                    data = Encoding.UTF8.GetString(post(url, body))
                };
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

        public class Resp
        {
            public bool err { get; set; }
            public string msg { get; set; }
            public object data { get; set; }

        }
        #endregion

    }
}