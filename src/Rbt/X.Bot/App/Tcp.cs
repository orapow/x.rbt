using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using X.Core.Utility;

namespace X.Bot.App
{
    /// <summary>
    /// WebSocket通讯类
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
        public delegate void NewMsgHandler(msg m, Tcp tc);
        public event NewMsgHandler NewMsg;

        /// <summary>
        /// 断开事件
        /// </summary>
        /// <param name="tc">连接对象</param>
        public delegate void ClosedHandler(Tcp tc);
        public event ClosedHandler Closed;
        #endregion

        #region 私有属性
        TcpClient tc = null;
        bool stop = false;
        List<byte> data = new List<byte>();
        bool ready = false;
        #endregion

        #region 公开方法
        public Tcp(TcpClient tc)
        {
            this.tc = tc;
        }
        /// <summary>
        /// 开始
        /// </summary>
        public void Start()
        {
            stop = false;
            data.Clear();
            ready = false;
            //接收消息
            new Thread(Accept).Start();
            //处理消息
            new Thread(Prase).Start();

        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="m"></param>
        public void Send(msg m)
        {
            m.from = "svr";
            if (tc.Client == null || tc.Client.Connected == false) { exit(1); return; }

            var data = new List<byte>();
            data.Add(128 | 1);
            var b2 = 0 << 7;
            var cot = Encoding.UTF8.GetBytes(m.ToString());
            if (cot.Length <= 125) data.Add((byte)(b2 + cot.Length));
            else if (cot.Length < 65535)
            {
                data.Add((byte)(b2 + 126));
                data.AddRange(BitConverter.GetBytes((ushort)cot.Length).Reverse());
            }
            else
            {
                data.Add((byte)(b2 + 127));
                data.AddRange(BitConverter.GetBytes((ulong)cot.Length).Reverse());
            }
            data.AddRange(cot);
            try
            {
                tc.Client.Send(data.ToArray());
            }
            catch
            {
                exit(1);
            }

        }
        /// <summary>
        /// 退出
        /// </summary>
        public void Quit()
        {
            exit(0);
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 接收消息
        /// </summary>
        void Accept()
        {
            while (!stop)
            {
                if (tc.Client == null) exit(1);

                if (tc.Available == 0)
                {
                    Thread.Sleep(500);
                    continue;
                }

                var d = new byte[tc.Available];
                try
                {
                    tc.Client.Receive(d);
                }
                catch
                {
                    exit(1);
                }
                if (!ready) Hands(d);
                else lock (data) data.AddRange(d);
            }
        }
        /// <summary>
        /// 解析消息
        /// </summary>
        void Prase()
        {
            var list = new List<byte>();

            string str = "";

            while (!stop)
            {
                if (data.Count == 0) { Thread.Sleep(500); continue; }

                var dh = data.Take(2).ToArray();
                lock (data) { data.RemoveRange(0, 2); }

                var opcode = dh[0] & 15;

                if (opcode == 8) { exit(1); return; }//断开

                var mask = (dh[1] >> 7) == 1;

                ulong len = (uint)dh[1] & 127;
                if (len > 125)
                {
                    var max = len == 126 ? 2 : 8;
                    var dl = data.Take(max).Reverse().ToArray();
                    lock (data) { data.RemoveRange(0, max); }
                    len = (max == 2 ? BitConverter.ToUInt16(dl, 0) : BitConverter.ToUInt64(dl, 0));
                }

                var mkey = new byte[4];
                if (mask)
                {
                    mkey = data.Take(4).ToArray();
                    lock (data) { data.RemoveRange(0, 4); }
                }
                var dt = data.Take((int)len).ToArray();
                lock (data) { data.RemoveRange(0, (int)len); }

                if (mask) for (var i = 0; i < dt.Length; i++) dt[i] = (byte)(dt[i] ^ mkey[i % 4]);

                str = Encoding.UTF8.GetString(dt);


                try
                {
                    var m = Serialize.FromJson<msg>(str);
                    if (m == null) return;

                    if (m.act == "setcode") { code = m.body; }

                    NewMsg?.Invoke(m, this);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("tcp.prase->" + ex.Message);
                    exit(1);
                }
            }
        }
        /// <summary>
        /// 握手
        /// </summary>
        /// <param name="data"></param>
        void Hands(byte[] data)
        {

            var body = Encoding.UTF8.GetString(data);
            var key = "";
            var r = new Regex(@"Sec\-WebSocket\-Key:(.*?)\r\n");
            var m = r.Match(body);
            if (m.Groups.Count != 0) key = Regex.Replace(m.Value, @"Sec\-WebSocket\-Key:(.*?)\r\n", "$1").Trim();

            if (string.IsNullOrEmpty(key)) { exit(1); return; }

            string rspkey = Convert.ToBase64String(SHA1.Create().ComputeHash(Encoding.ASCII.GetBytes(key + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11")));

            var rsp = new StringBuilder();
            rsp.Append("HTTP/1.1 101 Switching Protocols" + Environment.NewLine);
            rsp.Append("Connection: Upgrade" + Environment.NewLine);
            rsp.Append("Upgrade: websocket" + Environment.NewLine);
            rsp.Append("Sec-WebSocket-Accept: " + rspkey + Environment.NewLine + Environment.NewLine);

            tc.Client.Send(Encoding.UTF8.GetBytes(rsp.ToString()));

            ready = true;

        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="t"></param>
        void exit(int t)
        {
            try
            {
                stop = true;
                tc.Close();

                if (t == 1) Closed?.Invoke(this);

            }
            catch { }
        }
        #endregion

    }

    /// <summary>
    /// 消息类
    /// </summary>
    public class msg //: DynamicObject
    {
        public string from { get; set; }
        public string act { get; set; }
        public string err { get; set; }
        public string body { get; set; }

        public override string ToString()
        {
            return Serialize.ToJson(this);
        }
    }

}
