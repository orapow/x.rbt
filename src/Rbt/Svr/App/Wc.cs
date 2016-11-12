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
    public class Wc
    {
        // Cookie 容器
        private CookieContainer cookie;
        int timeout = 60; //超时 分钟
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
        /// 上传文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="dict"></param>
        /// <param name="fi"></param>
        /// <returns></returns>
        public Resp PostFile(string url, Dictionary<string, string> dict, FileInfo fi)
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

            #region
            //------WebKitFormBoundaryeR4fLqVKn7HbtWSt
            //Content - Disposition: form - data; name = "id"

            //WU_FILE_0
            //------WebKitFormBoundaryeR4fLqVKn7HbtWSt
            //Content - Disposition: form - data; name = "name"

            //untitled1.png
            //------WebKitFormBoundaryeR4fLqVKn7HbtWSt
            //Content - Disposition: form - data; name = "type"

            //image / png
            //------WebKitFormBoundaryeR4fLqVKn7HbtWSt
            //Content - Disposition: form - data; name = "lastModifiedDate"

            //2016 / 11 / 8 下午10: 20:16
            //     ------WebKitFormBoundaryeR4fLqVKn7HbtWSt
            //Content - Disposition: form - data; name = "size"

            //5126
            //------WebKitFormBoundaryeR4fLqVKn7HbtWSt
            //Content - Disposition: form - data; name = "mediatype"

            //pic
            //------WebKitFormBoundaryeR4fLqVKn7HbtWSt
            //Content - Disposition: form - data; name = "uploadmediarequest"

            //{ "UploadType":2,"BaseRequest":{ "Uin":269785315,"Sid":"q64ER3s7XP4zK3mQ","Skey":"@crypt_618f1218_96ac7ed2e46f41c0df9ed8ae7b25e29a","DeviceID":"e361467430833727"},"ClientMediaId":1478614816970,"TotalLen":5126,"StartPos":0,"DataLen":5126,"MediaType":4,"FromUserName":"@094ac46c504ea6d2cdb72d46ae4f94de","ToUserName":"filehelper","FileMd5":"1178e646540a10c5f78cf873525bab91"}
            //            ------WebKitFormBoundaryeR4fLqVKn7HbtWSt
            //Content - Disposition: form - data; name = "webwx_data_ticket"

            //gSfJvnKy4GKvRmS6efapzhfm
            //------WebKitFormBoundaryeR4fLqVKn7HbtWSt
            //Content - Disposition: form - data; name = "pass_ticket"

            //undefined
            //------WebKitFormBoundaryeR4fLqVKn7HbtWSt
            //Content - Disposition: form - data; name = "filename"; filename = "untitled1.png"
            //Content - Type: image / png


            //  ------WebKitFormBoundaryeR4fLqVKn7HbtWSt--
            #endregion

            return PostData(url, body.ToString());

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