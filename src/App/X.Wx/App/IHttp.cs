using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
/************************************************************************/
/* Author:huliang
 * Email:huliang@yahoo.cn
 * 注意：转载请注明出处
/************************************************************************/

namespace LiangHu
{
    /// <summary>
    /// HTTP协议头包装
    /// </summary>
    public class HttpHeader
    {
        public HttpHeader() : this("") { }
        public HttpHeader(string url) { this.Url = url; }

        public string Url { get; set; }
        public string Host { get; set; }
        public string Accept { get; set; }
        public string Referer { get; set; }
        public string Cookies { get; set; }
        public string Body { get; set; }

        Dictionary<string, string> m_Others = new Dictionary<string, string>();

        public string this[string key]
        {
            get
            {
                return m_Others.ContainsKey(key) ? m_Others[key] : null;
            }
            set
            {
                Add(key, value);
            }
        }

        public void Add(string key, string value)
        {
            switch (key.ToUpper())
            {
                case "URL":
                    this.Url = value;
                    break;
                case "HOST":
                    this.Host = value;
                    break;
                case "ACCEPT":
                    this.Accept = value;
                    break;
                case "REFERER":
                    this.Referer = value;
                    break;
                case "BODY":
                    this.Body = value;
                    break;
                default:
                    if (!m_Others.ContainsKey(key))
                    {
                        m_Others.Add(key, value);
                    }
                    else
                    {
                        m_Others[key] = value;
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// HTTP回应包装
    /// </summary>
    public class HttpResponse
    {
        internal HttpResponse(string header, byte[] body)
        {
            this.Header = header;
            this.Body = body;
        }
        //暂未将回应HTTP协议头转换为HttpHeader类型
        public string Header { get; private set; }
        public byte[] Body { get; private set; }
    }

    /// <summary>
    /// HttpHelper
    /// </summary>
    public static class HttpHelper
    {
        /// <summary>
        /// 提交方法
        /// </summary>
        enum HttpMethod
        {
            GET, POST
        }

        #region HttpWebRequest & HttpWebResponse

        /// <summary>
        /// Get方法
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="cookieContainer">Cookies存储器</param>
        /// <param name="encoding">返回内容的编码格式</param>
        /// <param name="others">其他需要补充的HTTP协议头</param>
        /// <returns>请求结果</returns>
        public static string Get(string url,
            CookieContainer cookieContainer,
            Encoding encoding,
            Dictionary<string, string> others = null)
        {
            return InternalHttp(HttpMethod.GET, url, null, cookieContainer, encoding, others);
        }


        /// <summary>
        /// Post方法
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="body">Post内容</param>
        /// <param name="cookieContainer">Cookies存储器</param>
        /// <param name="encoding">返回内容编码</param>
        /// <param name="others">其他需要补充的HTTP协议头</param>
        /// <returns>请求结果</returns>
        public static string Post(string url,
            byte[] body,
            CookieContainer cookieContainer,
            Encoding encoding,
            Dictionary<string, string> others = null)
        {
            return InternalHttp(HttpMethod.POST, url, body, cookieContainer, encoding, others);
        }

        /// <summary>
        /// Http操作
        /// </summary>
        /// <param name="method">请求方式</param>
        /// <param name="url">请求地址</param>
        /// <param name="bytes">提交的数据</param>
        /// <param name="cookieContainer">Cookies存储器</param>
        /// <param name="encoding">返回内容编码</param>
        /// <param name="others">其他需要补充的HTTP协议头</param>
        /// <returns>请求结果</returns>
        static byte[] InternalHttp(HttpMethod method,
            string url,
            byte[] bytes,
            CookieContainer cookieContainer,
            Encoding encoding,
            Dictionary<string, string> others = null)
        {
            byte[] result = null;

            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException("请求地址不能为空");
            if (method == HttpMethod.POST && bytes == null) throw new ArgumentNullException("提交的数据不能为空");
            if (cookieContainer == null) throw new ArgumentNullException("Cookies存储器不能为空");

            try
            {
                var req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = method.ToString();
                req.UserAgent = "Mozilla/5.0 (Windows NT 6.1;MSIE 9.0;)";
                req.CookieContainer = cookieContainer;

                if (method == HttpMethod.POST)
                {
                    req.ContentType = "application/x-www-form-urlencoded";
                    req.ContentLength = bytes.Length;
                    using (Stream stream = req.GetRequestStream())
                    {
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Flush();
                    }
                }
                if (others != null)
                {
                    foreach (KeyValuePair<string, string> pair in others) req.Headers.Add(pair.Key, pair.Value);
                }
                var response = (HttpWebResponse)req.GetResponse();
                if (response.StatusCode != HttpStatusCode.OK) return result;

                using (var rd = new StreamReader(response.GetResponseStream(), encoding))
                {
                    result = rd.();
                }
            }
            catch
            {
                throw;
            }
            return result;
        }

        #endregion

        #region Socket

        static bool ValidateServerCertificate(
                 object sender,
                 X509Certificate certificate,
                 X509Chain chain,
                 SslPolicyErrors sslPolicyErrors)
        {
            /*
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;
            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);
            return false;
            */
            return true;
        }

        public static HttpResponse Get(IPEndPoint endpoint,
            HttpHeader header)
        {
            return Get(endpoint, header, null);
        }

        public static HttpResponse Get(IPEndPoint endpoint,
            HttpHeader header,
            X509CertificateCollection x509certs)
        {
            return InternalSslSocketHttp(HttpMethod.GET, endpoint, header, x509certs);
        }

        public static HttpResponse Post(IPEndPoint endpoint,
            HttpHeader header)
        {
            return Post(endpoint, header, null);
        }

        public static HttpResponse Post(IPEndPoint endpoint,
            HttpHeader header,
            X509CertificateCollection x509certs)
        {
            return InternalSslSocketHttp(HttpMethod.POST, endpoint, header, x509certs);
        }

        static HttpResponse InternalSslSocketHttp(HttpMethod method,
            IPEndPoint endpoint,
            HttpHeader header,
            X509CertificateCollection x509certs)
        {
            HttpResponse response = null;
            try
            {
                TcpClient tcp = new TcpClient();
                tcp.Connect(endpoint);
                if (tcp.Connected)
                {
                    byte[] buff = ParseHttpHeaderToBytes(method, header);  //生成协议包
                    if (x509certs != null)
                    {
                        using (SslStream ssl = new SslStream(tcp.GetStream(),
                                                false,
                                                new RemoteCertificateValidationCallback(ValidateServerCertificate),
                                                null))
                        {
                            ssl.AuthenticateAsClient("ServerName",
                                x509certs,
                                SslProtocols.Tls,
                                false);
                            if (ssl.IsAuthenticated)
                            {
                                ssl.Write(buff);
                                ssl.Flush();
                                response = ReadResponse(ssl);
                            }
                        }
                    }
                    else
                    {
                        using (NetworkStream ns = tcp.GetStream())
                        {
                            ns.Write(buff, 0, buff.Length);
                            ns.Flush();
                            response = ReadResponse(ns);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return response;
        }

        class TaskArguments
        {
            public TaskArguments(CancellationTokenSource cancelSource, Stream sm)
            {
                this.CancelSource = cancelSource;
                this.Stream = sm;
            }
            public CancellationTokenSource CancelSource { get; private set; }
            public Stream Stream { get; private set; }
        }

        private static HttpResponse ReadResponse(Stream sm)
        {
            HttpResponse response = null;
            CancellationTokenSource cancelSource = new CancellationTokenSource();
            Task<string> myTask = Task.Factory.StartNew<string>(
                new Func<object, string>(ReadHeaderProcess),
                new TaskArguments(cancelSource, sm),
                cancelSource.Token);
            if (myTask.Wait(3 * 1000)) //尝试3秒时间读取协议头
            {
                string header = myTask.Result;
                if (!string.IsNullOrEmpty(header))
                {
                    if (header.StartsWith("HTTP/1.1 100"))
                    {
                        return ReadResponse(sm);
                    }
                    byte[] buff = null;
                    int start = header.ToUpper().IndexOf("CONTENT-LENGTH");
                    int content_length = -1;  //fix bug
                    if (start > 0)
                    {
                        string temp = header.Substring(start, header.Length - start);
                        string[] sArry = Regex.Split(temp, "\r\n");
                        content_length = Convert.ToInt32(sArry[0].Split(':')[1]);
                        if (content_length > 0)
                        {
                            buff = new byte[content_length];
                            int inread = sm.Read(buff, 0, buff.Length);
                            while (inread < buff.Length)
                            {
                                inread += sm.Read(buff, inread, buff.Length - inread);
                            }
                        }
                    }
                    else
                    {
                        start = header.ToUpper().IndexOf("TRANSFER-ENCODING: CHUNKED");
                        if (start > 0)
                        {
                            buff = ChunkedReadResponse(sm);
                        }
                        else
                        {
                            buff = SpecialReadResponse(sm);//例外
                        }
                    }
                    response = new HttpResponse(header, buff);
                }
            }
            else
            {
                cancelSource.Cancel(); //超时的话，别忘记取消任务哦
            }
            return response;
        }

        static string ReadHeaderProcess(object args)
        {
            TaskArguments argument = args as TaskArguments;
            StringBuilder bulider = new StringBuilder();
            if (argument != null)
            {
                Stream sm = argument.Stream;
                while (!argument.CancelSource.IsCancellationRequested)
                {
                    try
                    {
                        int read = sm.ReadByte();
                        if (read == -1) break;

                        byte b = (byte)read;
                        bulider.Append((char)b);
                        string temp = bulider.ToString();
                        if (temp.EndsWith("\r\n\r\n")) break;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        break;
                    }
                }
            }
            return bulider.ToString();
        }

        class ArraySegmentList<T>
        {
            List<ArraySegment<T>> m_SegmentList = new List<ArraySegment<T>>();
            public ArraySegmentList() { }

            int m_Count = 0;
            public void Add(ArraySegment<T> arraySegment)
            {
                m_Count += arraySegment.Count;
                m_SegmentList.Add(arraySegment);
            }

            public T[] ToArray()
            {
                T[] array = new T[m_Count];
                int index = 0;
                for (int i = 0; i < m_SegmentList.Count; i++)
                {
                    ArraySegment<T> arraySegment = m_SegmentList[i];
                    Array.Copy(arraySegment.Array,
                        0,
                        array,
                        index,
                        arraySegment.Count);
                    index += arraySegment.Count;
                }
                return array;
            }
        }
        static byte[] ChunkedReadResponse(Stream sm)
        {
            ArraySegmentList<byte> arraySegmentList = new ArraySegmentList<byte>();
            int chunked = GetChunked(sm);
            while (chunked > 0)
            {
                byte[] buff = new byte[chunked];
                try
                {
                    int inread = sm.Read(buff, 0, buff.Length);
                    while (inread < buff.Length)
                    {
                        inread += sm.Read(buff, inread, buff.Length - inread);
                    }
                    arraySegmentList.Add(new ArraySegment<byte>(buff));
                    if (sm.ReadByte() != -1) sm.ReadByte();
                }
                catch
                {
                    break;
                }
                chunked = GetChunked(sm);
            }
            return arraySegmentList.ToArray();
        }

        static int GetChunked(Stream sm)
        {
            int chunked = 0;
            StringBuilder bulider = new StringBuilder();
            while (true)
            {
                try
                {
                    int read = sm.ReadByte();
                    if (read == -1) break;

                    byte b = (byte)read;
                    bulider.Append((char)b);
                    string temp = bulider.ToString();
                    if (temp.EndsWith("\r\n"))
                    {
                        chunked = Convert.ToInt32(temp.Trim(), 16);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    break;
                }
            }
            return chunked;
        }

        /*
         * 注意：该方法仅供测试，实际使用时请根据需要定制
         */
        static byte[] SpecialReadResponse(Stream sm)
        {
            ArrayList array = new ArrayList();
            StringBuilder bulider = new StringBuilder();
            int length = 0;
            DateTime now = DateTime.Now;
            while (true)
            {
                byte[] buff = new byte[1024 * 10];
                int len = sm.Read(buff, 0, buff.Length);
                if (len > 0)
                {
                    length += len;
                    byte[] reads = new byte[len];
                    Array.Copy(buff, 0, reads, 0, len);
                    array.Add(reads);
                    bulider.Append(Encoding.Default.GetString(reads));
                }
                string temp = bulider.ToString();
                if (temp.ToUpper().Contains("</HTML>"))
                {
                    break;
                }
                if (DateTime.Now.Subtract(now).TotalSeconds >= 30)
                {
                    break;//超时30秒则跳出
                }
            }
            byte[] bytes = new byte[length];
            int index = 0;
            for (int i = 0; i < array.Count; i++)
            {
                byte[] temp = (byte[])array[i];
                Array.Copy(temp, 0, bytes,
                    index, temp.Length);
                index += temp.Length;
            }
            return bytes;
        }

        #endregion

        #region  Helper

        /// <summary>
        /// 将HTTP协议头转换为Bytes数据
        /// </summary>
        /// <param name="method">HTTP方法</param>
        /// <param name="header">HTTP协议头</param>
        /// <returns>Bytes数据</returns>
        static byte[] ParseHttpHeaderToBytes(HttpMethod method, HttpHeader header)
        {
            var bulider = new StringBuilder();
            if (method.Equals(HttpMethod.POST))
            {
                bulider.AppendLine(string.Format("POST {0} HTTP/1.1", header.Url));
                bulider.AppendLine("Content-Type: application/x-www-form-urlencoded");
            }
            else
            {
                bulider.AppendLine(string.Format("GET {0} HTTP/1.1", header.Url));
            }

            if (!string.IsNullOrEmpty(header.Host)) bulider.AppendLine(string.Format("Host: {0}", header.Host));
            bulider.AppendLine("User-Agent: Mozilla/5.0 (Windows NT 6.1; IE 9.0)");

            if (!string.IsNullOrEmpty(header.Referer)) bulider.AppendLine(string.Format("Referer: {0}", header.Referer));
            bulider.AppendLine("Connection: keep-alive");

            if (!string.IsNullOrEmpty(header.Accept)) bulider.AppendLine(string.Format("Accept: {0}", header.Accept));
            else bulider.AppendLine("Accept: */*");

            if (!string.IsNullOrEmpty(header.Cookies)) bulider.AppendLine(string.Format("Cookie: {0}", header.Cookies));

            if (method.Equals(HttpMethod.POST))
            {
                bulider.AppendLine(string.Format("Content-Length: {0}\r\n", Encoding.Default.GetBytes(header.Body).Length));
                bulider.Append(header.Body);
            }
            else
            {
                bulider.Append("\r\n");
            }
            return Encoding.Default.GetBytes(bulider.ToString());
        }

        /// <summary>
        /// 从Url中提取Host信息
        /// </summary>
        /// <param name="url">Url</param>
        /// <returns>Host信息</returns>
        public static string GetHost(string url)
        {
            string host = string.Empty;
            try
            {
                Uri uri = new Uri(url);
                host = uri.Host;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return host;
        }

        /// <summary>
        /// 通过Host获取IP地址
        /// </summary>
        /// <param name="host">Host</param>
        /// <returns>IP地址</returns>
        public static IPAddress GetAddress(string host)
        {
            IPAddress address = IPAddress.Any;
            try
            {
                IPAddress[] alladdress = Dns.GetHostAddresses(host);
                if (alladdress.Length > 0) address = alladdress[0];
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return address;
        }

        /// <summary>
        /// 从HTTP返回头协议中取Set-Cookie信息（即Cookies）
        /// </summary>
        /// <param name="responseHeader">HTTP返回头协议</param>
        /// <returns>Cookies</returns>
        public static string GetCookies(string responseHeader)
        {
            StringBuilder cookies = new StringBuilder();
            using (StringReader reader = new StringReader(responseHeader))
            {
                string strLine = reader.ReadLine();
                while (strLine != null)
                {
                    if (strLine.StartsWith("Set-Cookie:"))
                    {
                        string temp = strLine.Remove(0, 12);
                        if (!temp.EndsWith(";"))
                        {
                            temp = temp + ";";
                        }
                        cookies.Append(temp);
                    }
                    strLine = reader.ReadLine();
                }
            }
            return cookies.ToString();
        }

        /// <summary>
        /// 从HTTP返回头协议中去Location地址(一般出现在301跳转)
        /// </summary>
        /// <param name="responseHeader">HTTP返回头协议</param>
        /// <returns>Location地址</returns>
        public static string GetLocation(string responseHeader)
        {
            string result = string.Empty;
            using (StringReader reader = new StringReader(responseHeader))
            {
                string strLine = reader.ReadLine();
                while (strLine != null)
                {
                    if (strLine.StartsWith("Location:"))
                    {
                        result = strLine.Remove(0, 10);
                    }
                    strLine = reader.ReadLine();
                }
            }
            return result;
        }

        #endregion
    }
}