using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net.Security;
using System.Text;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using X.Core.Utility;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading;
using System.Security.Authentication;
using System.IO.Compression;

namespace X.Wx.App
{
    public class Http
    {
        public delegate void OutLogHandler(string log);
        public event OutLogHandler OutLog;

        /// <summary>
        /// 获取或设置请求与回应的超时时间,默认3秒.
        /// </summary>
        int timeOut { get; set; }

        /// <summary>
        /// 获取或设置请求cookie
        /// </summary>
        public string Cookies = null;

        public Http() : this("", 30) { }
        public Http(string ck, int tout)
        {
            timeOut = tout;
            Cookies = ck;
        }

        byte[] get(string url)
        {
            var rsp = SocketHttp(url, null, null);
            if (rsp == null) return null;
            else return rsp.Body;
        }

        byte[] post(string url, byte[] data) { return post(url, data, ""); }
        byte[] post(string url, byte[] data, string boundary)
        {
            var ct = "Content-Type: application/x-www-form-urlencoded; charset=UTF-8";//"Content-Type: multipart/form-data;charset=utf-8";
            if (!string.IsNullOrEmpty(boundary)) ct = "Content-Type: multipart/form-data;charset=utf-8;boundary=" + boundary;
            ct += "\r\nContent-Length: " + data.Length;
            var rsp = SocketHttp(url, data, ct);
            if (rsp == null) return null;
            return rsp.Body;
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
            return "";
            //if (Cookies == null) return "";
            //var cks = Cookies.GetCookies(new Uri("http://qq.com"));
            //return cks[name]?.Value;
        }

        public class Resp
        {
            public bool err { get; set; }
            public string msg { get; set; }
            public object data { get; set; }

        }
        #endregion

        /// <summary>
        /// HTTP回应包装
        /// </summary>
        class HttpResponse
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

        enum HttpMethod
        {
            GET, POST
        }

        bool ValidateServerCertificate(
                object sender,
                X509Certificate certificate,
                X509Chain chain,
                SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        HttpResponse SocketHttp(string url, byte[] data, string content_type)
        {
            HttpResponse rsp = null;

            var uri = new Uri(url);
            var tcp = new TcpClient(uri.Host, uri.Port);
            if (!tcp.Connected) return rsp;

            Stream ms = tcp.GetStream();

            ms.WriteTimeout = 5 * 1000;
            ms.ReadTimeout = timeOut * 1000;

            if (url.StartsWith("https://"))
            {
                var ssl = new SslStream(tcp.GetStream(),
                    false,
                    new RemoteCertificateValidationCallback(ValidateServerCertificate),
                    null);

                ssl.AuthenticateAsClient(uri.Host, null, SslProtocols.Tls, false);

                if (!ssl.IsAuthenticated) return rsp;
                ms = ssl;
            }

            byte[] buff = getHeader(data == null ? "GET" : "POST", uri, content_type);  //生成协议包
            ms.Write(buff, 0, buff.Length);
            if (data != null) { ms.Write(data, 0, data.Length); ms.Write(Encoding.UTF8.GetBytes("\r\n"), 0, 2); }
            ms.Flush();

            try
            {
                rsp = ReadResponse(ms);
                OutLog?.Invoke("socket->" + url + "\r\n" + rsp.Header + Encoding.UTF8.GetString(rsp.Body));
            }
            catch (Exception ex)
            {
                OutLog?.Invoke("socket->" + url + "\r\n错误：" + ex.Message);
            }
            return rsp;
        }

        byte[] getHeader(string md, Uri u, string ct)
        {
            var head = md + " " + u.PathAndQuery + @" HTTP/1.1
Host: " + u.Host + @"
Connection: keep-alive
Pragma: no-cache
Cache-Control: no-cache
Cookie: " + Cookies + (string.IsNullOrEmpty(ct) ? "" : @"
" + ct) + @"
Accept: */*
User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Maxthon/4.9.3.1000 Chrome/39.0.2146.0 Safari/537.36
DNT: 1
Referer: https://wx.qq.com/
Accept-Encoding: gzip,deflate
Accept-Language: zh-CN

";
            return Encoding.UTF8.GetBytes(head);
        }

        HttpResponse ReadResponse(Stream sm)
        {
            var header = ReadHeaderProcess(sm);

            if (header.StartsWith("HTTP/1.1 100")) return ReadResponse(sm);

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
                    while (inread < buff.Length) inread += sm.Read(buff, inread, buff.Length - inread);
                }
            }
            else
            {
                start = header.ToUpper().IndexOf("TRANSFER-ENCODING: CHUNKED");
                if (start > 0) buff = ChunkedReadResponse(sm);
                else buff = SpecialReadResponse(sm);//例外
            }
            if (header.Contains("gzip")) buff = unZip(buff);
            return new HttpResponse(header, buff);
        }

        byte[] unZip(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                var czs = new GZipStream(ms, CompressionMode.Decompress);
                var obuff = new MemoryStream();
                byte[] block = new byte[1024];
                while (true)
                {
                    int bytesRead = czs.Read(block, 0, block.Length);
                    if (bytesRead <= 0)
                        break;
                    else
                        obuff.Write(block, 0, bytesRead);
                }
                czs.Close();
                obuff.Dispose();
                return obuff.ToArray();
            }
        }
        string ReadHeaderProcess(Stream sm)
        {
            var sb = new StringBuilder();

            while (true)
            {
                int read = sm.ReadByte();
                if (read == -1) break;

                byte b = (byte)read;
                sb.Append((char)b);
                string temp = sb.ToString();
                if (temp.EndsWith("\r\n\r\n")) break;
            }

            var reg = new Regex("Set-Cookie: ([^;]+)[^\\n]+");
            foreach (Match m in reg.Matches(sb.ToString())) Cookies += m.Groups[1].Value + ";";

            return sb.ToString();
        }
        byte[] ChunkedReadResponse(Stream sm)
        {
            ArraySegmentList<byte> arraySegmentList = new ArraySegmentList<byte>();
            int chunked = GetChunked(sm);
            while (chunked > 0)
            {
                byte[] buff = new byte[chunked];

                int inread = sm.Read(buff, 0, buff.Length);
                while (inread < buff.Length)
                {
                    inread += sm.Read(buff, inread, buff.Length - inread);
                }
                arraySegmentList.Add(new ArraySegment<byte>(buff));
                if (sm.ReadByte() != -1) sm.ReadByte();

                chunked = GetChunked(sm);
            }
            return arraySegmentList.ToArray();
        }
        int GetChunked(Stream sm)
        {
            int chunked = 0;
            var bulider = new StringBuilder();
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
                catch { break; }
            }
            return chunked;
        }
        byte[] SpecialReadResponse(Stream sm)
        {
            var array = new ArrayList();
            var bulider = new StringBuilder();
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
                if (temp.ToUpper().Contains("</HTML>")) break;
                if (DateTime.Now.Subtract(now).TotalSeconds >= timeOut) break;//超时30秒则跳出
            }
            byte[] bytes = new byte[length];
            int index = 0;
            for (int i = 0; i < array.Count; i++)
            {
                byte[] temp = (byte[])array[i];
                Array.Copy(temp, 0, bytes, index, temp.Length);
                index += temp.Length;
            }
            return bytes;
        }

    }
}