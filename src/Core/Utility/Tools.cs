using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using X.Core.Plugin;

namespace X.Core.Utility
{
    /// <summary>
    /// 工具类
    /// </summary>
    public class Tools
    {
        #region 私有变量
        private static Random rnd = null;
        private static string num = "0123456789";
        private static string letter = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static string str = "!@#$%^&*()-=_+[]\\{}|;':\",./<>?";
        #endregion

        #region 获取客户端数据
        /// <summary>
        /// 获取客户端IP
        /// </summary>
        /// <returns></returns>
        public static string GetClientIP()
        {
            string result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (null == result || result == String.Empty)
            {
                result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            if (null == result || result == String.Empty)
            {
                result = HttpContext.Current.Request.UserHostAddress;
            }
            return result;
        }

        /// <summary>
        /// 获取IP地址(在线IP库)
        /// </summary>
        /// <returns></returns>
        public static string GetIPpos(string ip)
        {
            try
            {
                var dt = DateTime.Now;
                Uri uri = new Uri("http://whois.pconline.com.cn/ip.jsp?ip=" + ip);
                WebRequest req = (HttpWebRequest)WebRequest.Create(uri);
                req.Timeout = 2000;

                WebResponse res = (WebResponse)(req.GetResponse());
                StreamReader rs = new StreamReader(res.GetResponseStream(), Encoding.GetEncoding("GB2312"));
                string s = rs.ReadToEnd();

                rs.Close();
                req.Abort();
                res.Close();
                return s;
            }
            catch
            {
                return "";
            }
        }
        #endregion

        #region 格林尼治时间
        /// <summary>
        /// 获取格林尼治时间
        /// </summary>
        /// <param name="time">
        /// 为空时获取当前时间
        /// </param>
        /// <returns></returns>
        public static string GetGreenTime(string time)
        {
            DateTime dt = DateTime.MinValue;
            if (!string.IsNullOrEmpty(time)) DateTime.TryParse(time, out dt);
            if (dt == DateTime.MinValue) dt = DateTime.Now;
            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return (dt - startTime).TotalMilliseconds.ToString("F0").Substring(0, 10);
        }
        /// <summary>
        /// 格林尼治转本地时间
        /// </summary>
        /// <param name="time">
        /// 要转换的日期字符串
        /// </param>
        /// <returns></returns>
        public static string FromGreenTime(string time, string format)
        {
            if (string.IsNullOrEmpty(time)) return "";
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(time + (time.Length == 10 ? "0000000" : "0000"));
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow).ToString(format); //得到转换后的时间
        }
        public static string FromGreenTime(string time)
        {
            return FromGreenTime(time, "yyyy-MM-dd HH:mm:ss");
        }
        #endregion

        #region 随机数
        /// <summary>
        /// 获取随机数
        /// </summary>
        /// <param name="bit"></param>
        /// <returns></returns>
        public static string GetRandRom(int bit)
        {
            return GetRandRom(bit, 1);
        }

        /// <summary>
        /// 获取随机数
        /// </summary>
        /// <param name="bit">位数</param>
        /// <param name="type">
        /// 类型：
        /// 1、数字（默认）
        /// 2、字母
        /// 3、数字字母
        /// 4、特殊字符+数字+字母
        /// </param>
        /// <returns></returns>
        public static string GetRandRom(int bit, int type)
        {
            if (rnd == null) rnd = new Random((int)DateTime.Now.Ticks);
            var rngstr = num;
            switch (type)
            {
                case 1:
                default:
                    rngstr = num;
                    break;
                case 2:
                    rngstr = letter;
                    break;
                case 3:
                    rngstr = num + letter;
                    break;
                case 4:
                    rngstr = num + letter + str;
                    break;
            }
            var r = "";
            for (var i = 0; i < bit; i++)
            {
                r += rngstr[rnd.Next(0, rngstr.Length - 1)];
            }
            return r;
        }
        /// <summary>
        /// 获取随数
        /// 最小0
        /// </summary>
        /// <param name="max">最大数</param>
        /// <returns></returns>
        public static int GetRandNext(int max)
        {
            return GetRandNext(0, max);
        }
        /// <summary>
        /// 获取随机数
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        public static int GetRandNext(int min, int max)
        {
            if (rnd == null) rnd = new Random((int)DateTime.Now.Ticks);
            return rnd.Next(min, max);
        }
        #endregion

        #region Http Post Get
        /// <summary>
        /// 提交Http数据
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="data">要提交的数据</param>
        /// <param name="mothod">提交方式</param>
        /// <returns></returns>
        public static string PostHttpData(string url, string data, string mothod, string cert, string pwd)
        {
            try
            {
                Debug.WriteLine("Url->" + url);
                var rq = (HttpWebRequest)HttpWebRequest.Create(url);
                if (!string.IsNullOrEmpty(cert))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) =>
                    {
                        if (errors == SslPolicyErrors.None) return true;
                        return false;
                    });
                    rq.ClientCertificates.Add(new X509Certificate2(cert, pwd, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet));
                }

                rq.Method = mothod;
                rq.KeepAlive = true;

                using (var wr = new StreamWriter(rq.GetRequestStream()))
                {
                    wr.WriteLine(data);
                }

                var rp = (HttpWebResponse)rq.GetResponse();
                string resp = string.Empty;
                var rspstr = "";
                using (var reader = new StreamReader(rp.GetResponseStream()))
                {
                    rspstr = reader.ReadToEnd(); ;
                }
                return rspstr;
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
                throw;
            }
        }

        /// <summary>
        /// 提交Http数据
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="data">要提交的数据</param>
        /// <returns></returns>
        public static string PostHttpData(string url, string data)
        {
            return PostHttpData(url, data, "POST", "", "");
        }

        /// <summary>
        /// 提交Http数据
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="parms">
        /// 参数
        /// </param>
        /// <returns></returns>
        public static string PostHttpData(string url, Dictionary<string, string> parms)
        {
            return PostHttpData(url, "POST", parms);
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="url">服务器地址</param>
        /// <param name="data">文件内容</param>
        /// <param name="filename">文件名</param>
        /// <returns></returns>
        public static string PostHttpFile(string url, byte[] data, string filename)
        {
            var wc = new WebClient();
            try
            {
                string boundary = "----------" + DateTime.Now.Ticks.ToString("x");
                var header = Encoding.UTF8.GetBytes("--" + boundary + "\r\nContent-Disposition: form-data; name=\"Filedata\"; filename=\"" + filename + "\"\r\nContent-Type: application/octet-stream\r\n\r\n");
                var footer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");

                var post = new List<byte>();
                post.AddRange(header);
                post.AddRange(data);
                post.AddRange(footer);

                wc.Headers.Add("Content-Type", string.Format("multipart/form-data;boundary={0}", boundary));
                var back = wc.UploadData(url, "POST", post.ToArray());
                return Encoding.UTF8.GetString(back);
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
                throw;
            }
            finally { wc.Dispose(); }

            #region 另一种写法
            //var request = (HttpWebRequest)WebRequest.Create(url);
            //request.Method = "POST";

            //string boundary = "----" + DateTime.Now.Ticks.ToString("x");
            //var header = Encoding.UTF8.GetBytes("----" + boundary + "\r\nContent-Disposition: form-data; name=\"media\"; filename=\"" + filename + "\"\r\nContent-Type: application/octet-stream\r\n\r\n");
            //var footer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
            //request.ContentLength = header.Length + data.Length + footer.Length;

            //request.ContentType = string.Format("multipart/form-data;boundary={0}", boundary);
            //request.KeepAlive = true;

            //var ms = request.GetRequestStream();

            //ms.Write(header, 0, header.Length);
            //ms.Write(data, 0, data.Length);
            //ms.Write(footer, 0, footer.Length);

            //var response = (HttpWebResponse)request.GetResponse();

            //using (Stream responseStream = response.GetResponseStream())
            //{
            //    using (StreamReader myStreamReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8")))
            //    {
            //        return myStreamReader.ReadToEnd();
            //    }
            //}
            #endregion

        }
        /// <summary>
        /// 提交Http数据
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="mothod">方式</param>
        /// <param name="parms">参数</param>
        /// <returns></returns>
        public static string PostHttpData(string url, string mothod, Dictionary<string, string> parms)
        {
            var wc = new WebClient();
            try
            {
                var PostVars = new System.Collections.Specialized.NameValueCollection();
                var uri = new Uri(url);
                if (parms != null) foreach (var k in parms.Keys) PostVars.Add(k, parms[k]);
                var data = wc.UploadValues(uri, mothod.ToUpper(), PostVars);
                var str_back = Encoding.UTF8.GetString(data);
                Console.WriteLine(str_back);
                return str_back;
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
                throw;
            }
            finally { wc.Dispose(); }
        }

        /// <summary>
        /// 获取Http数据
        /// </summary>
        /// <param name="url">url</param>
        /// <returns></returns>
        public static string GetHttpData(string url, Encoding encd)
        {
            var wc = new WebClient();
            StreamReader sr = null;
            try
            {
                wc = new WebClient();
                sr = new StreamReader(wc.OpenRead(url), encd);
                var data = sr.ReadToEnd();

                return data;
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
                throw;
            }
            finally
            {
                if (sr != null) sr.Close();
                if (wc != null) wc.Dispose();
            }
        }

        public static string GetHttpData(string url) { return GetHttpData(url, Encoding.UTF8); }
        #endregion

        #region 读写文件
        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="cont"></param>
        public static void SaveFile(string file, string cont)
        {
            try
            {
                File.WriteAllText(file, cont);
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }

        /// <summary>
        /// 读取文件所有行
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string[] ReadFileLines(string path)
        {
            try
            {
                return File.ReadAllLines(path);
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
            return null;
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReadFile(string path)
        {
            try
            {
                return File.ReadAllText(path);
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
            return "";
        }
        #endregion
    }
}
