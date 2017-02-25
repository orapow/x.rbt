using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using X.Core.Utility;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using X.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Collections;
using X.App.Com;
using System.Web;
using System.IO;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        static long getcurrentseconds()
        {
            return (long)(DateTime.UtcNow - DateTime.MinValue).TotalMilliseconds;
        }
        [TestMethod]
        public void TestMethod1()
        {
            //var dt = Encry.Encode(Encoding.UTF8.GetBytes("<title style=\"asdfqewrqweq: sadfa;\">C#正则删除HTML标签 - HOT SUMMER - 博客园</title>"));
            ////Thread.Sleep(60 * 1000);
            //dt = Encry.Decode(dt);
            //Console.WriteLine("dt->" + Encoding.UTF8.GetString(dt));
            //Console.WriteLine(Tools.RemoveHtml("<title style=\"asdfqewrqweq: sadfa;\">C#正则删除HTML标签 - HOT SUMMER - 博客园</title>"));

            //var msg = Wx.Msg.Get("wx55bfd5ad6998a826", "<xml><Encrypt><![CDATA[UzHfJ6kvqVjYZndF6i5Xqo3bLesQvYVTs8hWcEt6Slmbqj/ILjsu++ih9lP1Y9AodrxsW5NnyB6kM8TCcsdU6CDNRZe/Q7fZKftcM4oy6c0iFQtS8B+xXi+ntSl+i0Vp0/J3HTmz0MIsZf19VEuRNaemduh1oeldjaCYJ3zRSAHroL8obfzVPRpkr9binPfwo+YmuzUB7SlHMcHjWx+dwdBza63qnZvD62GfeDFfka9Mq0t53/9SKRnZTpXLHp/bNIgvbt/e9Ev1GnSjq6UW7lwzJsO2CbK+dZn6DUS2ZIN3ObvKZPfNw4q/sAuG6MP8cH+EPakmn+lLiZ+3L1SDgX2AT0eCefLAbP7OaSluXvEq5Js/8PW/9zkjVint0T774Vfs/KbqjjyeNLxtAFrgvGLSUN2Y1Yf27oHJKYLQWugX1cYFKqyCLO/YfvI9cdu373bvudoOToDJsPcDPceyxQ==]]></Encrypt><MsgSignature><![CDATA[9b4317209a8766c7aee6de6e6bfd04343153be4d]]></MsgSignature><TimeStamp><![CDATA[1487900361]]></TimeStamp><Nonce><![CDATA[ivo18KY55]]></Nonce></xml>", "9b4317209a8766c7aee6de6e6bfd04343153be4d", "ivo18KY55", "1487900361");
            HttpContext.Current = new HttpContext(new HttpRequest("", "http://localhost", ""),
                                                  new HttpResponse(new StringWriter(new StringBuilder()))); //
            var st = Wx.Pay.PayToOpenid("wxc6982f28ed963521", "1344240501", "o84gaswfHg0XzHBeJzAjY4PdyXi4", "44", 1, "d:\\", "gzkdzMh0AsfpDKJTjTCZPYq2QKphK5Kmss");

        }

        public class SslTcpClient
        {
            private static Hashtable certificateErrors = new Hashtable();

            // 验证服务器证书的回调函数。
            public static bool ValidateServerCertificate(
                  object sender,
                  X509Certificate certificate,
                  X509Chain chain,
                  SslPolicyErrors sslPolicyErrors)
            {

                if (sslPolicyErrors == SslPolicyErrors.None)
                    return true;

                Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

                // 不能通过的服务器证书验证的，返回false，阻止建立连接。
                return false;

            }
            public static void RunClient(string machineName, string serverName)
            {
                // 1 建立普通tcp连接。
                TcpClient client = new TcpClient(machineName, 443);
                Console.WriteLine("Client connected.");
                // 2 构造SslStream类的对象.
                SslStream sslStream = new SslStream(
                    client.GetStream(),
                    false,
                    new RemoteCertificateValidationCallback(ValidateServerCertificate),
                    null
                    );
                // The server name must match the name on the server certificate.
                try
                {
                    // 3 发起ssl握手，试图建立ssl连接. 
                    sslStream.AuthenticateAsClient(serverName, null, SslProtocols.Tls, false);
                }
                catch (AuthenticationException e)
                {
                    Console.WriteLine("Exception: {0}", e.Message);
                    if (e.InnerException != null)
                    {
                        Console.WriteLine("Inner exception: {0}", e.InnerException.Message);
                    }
                    Console.WriteLine("Authentication failed - closing the connection.");
                    client.Close();
                    return;
                }
                // Encode a test message into a byte array.
                // Signal the end of the message using the "<EOF>".
                byte[] messsage = Encoding.UTF8.GetBytes("GET " + machineName + "/jslogin?appid=wx782c26e4c19acffb&fun=new&lang=zh_CN&_=1487150460 HTTP/1.0\r\n\r\n");
                // 发送ssl加密数据。. 
                sslStream.Write(messsage);
                sslStream.Flush();
                // Read message from the server.
                string serverMessage = ReadMessage(sslStream);
                Console.WriteLine("Server says: {0}", serverMessage);
                // Close the client connection.
                client.Close();
                Console.WriteLine("Client closed.");
            }
            static string ReadMessage(SslStream sslStream)
            {
                byte[] buffer = new byte[2048];
                StringBuilder messageData = new StringBuilder();
                int bytes = -1;
                do
                {
                    bytes = sslStream.Read(buffer, 0, buffer.Length);
                    Decoder decoder = Encoding.UTF8.GetDecoder();
                    char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                    decoder.GetChars(buffer, 0, bytes, chars, 0);
                    messageData.Append(chars);
                } while (bytes != 0);

                return messageData.ToString();
            }
        }


        class Secret
        {
            static byte[] getKey()
            {
                var dt = DateTime.Now;
                if (dt.Second >= 50) dt = dt.AddSeconds(-30);
                var key = X.Core.Utility.Secret.MD5(dt.ToString("yyyymmddHHMMmm") + "+80xc.com+");
                Console.WriteLine("key->" + key);
                return Encoding.UTF8.GetBytes(key);
            }

            public static byte[] Decode(byte[] data)
            {
                var k1 = getKey();
                var k2 = data.Take(32);
                if (!Enumerable.SequenceEqual(k1, k2)) throw new Exception("Decode Error");

                var i = 0;
                for (var j = 0; j < data.Length; j++) data[j] = (byte)(data[j] ^ k1[i++ % k1.Length]);

                return data.Skip(32).ToArray();
            }

            public static byte[] Encode(byte[] data)
            {
                var key = getKey();
                var i = 0;
                var dts = data.ToList();
                for (var j = 0; j < dts.Count; j++) dts[j] = (byte)(dts[j] ^ key[i++ % key.Length]);
                dts.InsertRange(0, key.ToList());
                return dts.ToArray();
            }
        }
    }
}
