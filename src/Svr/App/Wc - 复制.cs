using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;

namespace Rbt.Svr.App
{
    /// <summary>
    /// 支持 Session 和 Cookie 的 WebClient。
    /// </summary>
    public class Wc : WebClient
    {
        // Cookie 容器
        private CookieContainer cookie;

        /// <summary>
        /// 创建一个新的 WebClient 实例。
        /// </summary>
        public Wc()
        {
            cookie = new CookieContainer();
        }

        /// <summary>
        /// 坑爹之用
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// 返回带有 Cookie 的 HttpWebRequest。
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        protected override WebRequest GetWebRequest(Uri address)
        {
            var req = base.GetWebRequest(address);
            req.Timeout = 30000;
            if (req is HttpWebRequest)
            {
                var hr = req as HttpWebRequest;
                hr.CookieContainer = cookie;
            }
            return req;
        }

        #region 封装了PostData, GetSrc 和 GetFile 方法
        /// <summary>
        /// 向指定的 URL POST 数据，并返回页面
        /// </summary>
        /// <param name="uriString">POST URL</param>
        /// <param name="postString">POST 的 数据</param>
        /// <returns>页面的源文件</returns>
        public string PostData(string uriString, string postString)
        {
            try
            {
                byte[] postData = Encoding.GetEncoding("utf-8").GetBytes(postString);
                this.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                byte[] responseData = this.UploadData(uriString, "POST", postData);
                string srcString = Encoding.GetEncoding("utf-8").GetString(responseData);
                return srcString.Replace("\t", "").Replace("\r", "").Replace("\n", "");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("PostErr->" + ex.Message);
                return "";
            }
        }

        /// <summary>
        /// 获得指定 URL 的源文件
        /// </summary>
        /// <param name="uriString">页面 URL</param>
        /// <returns>页面的源文件</returns>
        public string GetStr(string uriString)
        {
            try
            {
                string srcString = DownloadString(uriString);
                return srcString.Replace("\t", "").Replace("\r", "").Replace("\n", "");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("GetStrErr->" + ex.Message);
                return "";
            }
        }

        /// <summary>
        /// 从指定的 URL 下载文件到本地
        /// </summary>
        /// <param name="uriString">文件 URL</param>
        /// <returns></returns>
        public byte[] GetFile(string urlString)
        {
            try
            {
                return DownloadData(urlString);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("GetFileErr->" + ex.Message);
                return null;
            }
        }
        #endregion
    }
}