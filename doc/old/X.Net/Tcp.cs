using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using X.Core.Utility;

namespace X.Net
{
    /// <summary>
    /// 通讯类
    /// </summary>
    public class Tcp
    {
        #region 公开属性
        public string code { get; set; }
        #endregion

        #region 公开事件
        /// <summary>
        /// 新消息事件
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="tc">连接对象</param>
        public delegate void NewMsgHandler(string from, string body, Tcp tc);
        public event NewMsgHandler NewMsg;

        /// <summary>
        /// 断开事件
        /// </summary>
        /// <param name="tc">连接对象</param>
        public delegate void ClosedHandler(Tcp tc);
        public event ClosedHandler Closed;
        #endregion

        #region 私有字段
        TcpClient tc = null;
        int stop = 0;// 0 正常 1 退出中
        List<byte> data = new List<byte>();
        string key = "";
        #endregion

        #region 公开方法
        public Tcp(TcpClient tc, string key)
        {
            this.tc = tc;
            this.key = key;
        }
        public Tcp(string ip, int port, string key)
        {
            this.key = key;
            tc = new TcpClient();
            try { tc.Connect(ip, port); } catch (SocketException) { exit(2); }

            new Thread(o =>
            {
                while (stop == 0) { send(new msg() { act = "heart" }); Thread.Sleep(15 * 1000); }

            }).Start();

        }

        /// <summary>
        /// 开始
        /// </summary>
        public void Start()
        {
            stop = 0;
            data.Clear();

            //接收消息
            new Thread(Accept).Start();

            //处理消息
            new Thread(Prase).Start();

        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="m"></param>
        public void Send(object m)
        {
            send(new msg() { act = "msg", body = Serialize.ToJson(m) });
        }

        private void send(msg m)
        {
            if (tc.Client == null || tc.Client.Connected == false) { exit(2); return; }

            try
            {

                var body = Secret.MD5(Serialize.ToJson(m) + key) + Serialize.ToJson(m);
                var data = Encoding.UTF8.GetBytes(body);

                Console.WriteLine("send->" + body + "(" + data.Length + "字节)");
                Debug.WriteLine("send->" + body + "(" + data.Length + "字节)");

                var dt = new List<byte>();
                dt.AddRange(Encoding.UTF8.GetBytes("x.rbt"));
                dt.AddRange(BitConverter.GetBytes(data.Length));
                dt.AddRange(Encry.Encode(data, key));

                tc.Client.Send(dt.ToArray());

            }
            catch (Exception ex)
            {
                Console.WriteLine("send->err." + ex.Message);
                Debug.WriteLine("send->err." + ex.Message);
                exit(1);
            }
        }

        /// <summary>
        /// 退出
        /// </summary>
        public void Quit()
        {
            exit(3);
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 接收消息
        /// </summary>
        void Accept()
        {
            while (stop == 0)
            {
                if (tc.Client == null) { exit(2); break; }

                if (tc.Available == 0) { Thread.Sleep(500); continue; }

                var d = new byte[tc.Available];
                tc.Client.Receive(d);

                lock (data) data.AddRange(d);
            }
        }

        /// <summary>
        /// 解析消息
        /// </summary>
        void Prase()
        {
            string str = "";
            int hlen = 9;

            while (stop == 0)
            {
                if (data.Count < hlen) { Thread.Sleep(500); continue; }

                byte[] head = null;
                lock (data) head = data.Take(5).ToArray();
                if (Encoding.UTF8.GetString(head) != "x.rbt") { lock (data) data.RemoveRange(0, hlen); continue; }

                var len = BitConverter.ToInt32(data.Skip(5).Take(4).ToArray(), 0) + 32;
                if (data.Count() < len) continue;

                byte[] msg = null;
                lock (data) msg = data.Skip(9).Take(len).ToArray();
                lock (data) data.RemoveRange(0, len + 9);

                try
                {
                    str = Encoding.UTF8.GetString(Encry.Decode(msg, key));

                    Console.WriteLine("parse->" + str);
                    Debug.WriteLine("parse->" + str);

                    var sign = str.Substring(0, 32);
                    var json = str.Substring(32);
                    if (sign != Secret.MD5(json + key)) throw new Exception("签名验证失败");
                    var m = Serialize.FromJson<msg>(json);
                    switch (m.act)
                    {
                        case "heart":
                            continue;
                        case "exit":
                            exit(1);
                            break;
                        case "msg":
                            NewMsg?.Invoke(m.from, m.body, this);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message == "Decode Error") exit(2);
                    Console.WriteLine("parse->err." + ex.Message);
                    Debug.WriteLine("parse->err." + ex.Message);
                }
            }
        }

        /// <summary>
        /// 退出
        /// 1、对方退出
        /// 2、我方退出
        /// 3、外部调用
        /// </summary>
        /// <param name="t"></param>
        void exit(int t)
        {
            stop = 1;
            if (t == 3) send(new msg() { act = "exit" });

            Thread.Sleep(2000);
            tc.Close();

            if (t < 3) Closed?.Invoke(this);
        }
        #endregion

    }

    /// <summary>
    /// 消息类
    /// </summary>
    class msg //: DynamicObject
    {
        /// <summary>
        /// heart 
        /// msg
        /// exit
        /// </summary>
        public string act { get; set; }
        public string from { get; set; }
        public string err { get; set; }
        public string body { get; set; }

        public override string ToString()
        {
            return Serialize.ToJson(this);
        }
    }

}
