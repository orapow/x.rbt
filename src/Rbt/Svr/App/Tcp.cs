using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using X.Core.Utility;

namespace Rbt.Svr.App
{
    public class Tcp
    {
        public string ukey { get; set; }

        public delegate void NewMsgHandler(string content, Tcp ct);
        public event NewMsgHandler NewMsg;

        public delegate void ClosedHandler(Tcp ct);
        public event ClosedHandler Closed;

        #region
        TcpClient tc = null;
        bool stop = false;
        List<byte> data = new List<byte>();
        bool ready = false;
        #endregion

        #region
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
        /// <param name="msg"></param>
        public void Send(msg msg)
        {
            var frame = new List<byte>();
            frame.Add(128 | 1);

            var b2 = 0 << 7;
            var cot = Encoding.UTF8.GetBytes(msg.ToString());
            if (cot.Length <= 125) frame.Add((byte)(b2 + cot.Length));
            else if (cot.Length < 65535)
            {
                frame.Add((byte)(b2 + 126));
                frame.AddRange(BitConverter.GetBytes((ushort)cot.Length).Reverse());
            }
            else
            {
                frame.Add((byte)(b2 + 127));
                frame.AddRange(BitConverter.GetBytes((ulong)cot.Length).Reverse());
            }

            frame.AddRange(cot);

            tc.Client.Send(frame.ToArray());

        }
        /// <summary>
        /// 退出
        /// </summary>
        public void Quit()
        {
            Exit(0);
        }
        #endregion

        #region
        /// <summary>
        /// 接收消息
        /// </summary>
        void Accept()
        {
            while (!stop)
            {
                if (tc.Available == 0) { Thread.Sleep(500); continue; }

                var d = new byte[tc.Available];
                tc.Client.Receive(d);

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
            while (!stop)
            {
                if (data.Count == 0) { Thread.Sleep(500); continue; }

                var dh = data.Take(2).ToArray();
                lock (data) { data.RemoveRange(0, 2); }

                var fd = new Frame();
                fd.opcode = dh[0] & 15;

                if (fd.opcode == 8) { Quit(); return; }//断开

                fd.mask = (dh[1] >> 7) == 1;

                var idx = 2;
                ulong len = (uint)dh[1] & 127;
                if (len > 125)
                {
                    var max = len == 126 ? 2 : 8;
                    idx += max;
                    var dl = data.Take(max).Reverse().ToArray();
                    lock (data) { data.RemoveRange(0, max); }
                    len = (max == 2 ? BitConverter.ToUInt16(dl, 0) : BitConverter.ToUInt64(dl, 0));
                }
                fd.len = len;

                if (fd.mask)
                {
                    fd.mkey = data.Take(4).ToArray(); idx += 4;
                    lock (data) { data.RemoveRange(0, 4); }
                }

                var dt = data.Take((int)len).ToArray();
                lock (data) { data.RemoveRange(0, (int)len); }

                if (fd.mask) for (var i = 0; i < dt.Length; i++) dt[i] = (byte)(dt[i] ^ fd.mkey[i % 4]);

                fd.data = Encoding.UTF8.GetString(dt);

                Debug.WriteLine("newmsg->" + Serialize.ToJson(fd));

                NewMsg?.Invoke(fd.data, this);

            }
        }
        /// <summary>
        /// 握手
        /// </summary>
        /// <param name="data"></param>
        void Hands(byte[] data)
        {

            var head = Encoding.UTF8.GetString(data);

            var key = "";
            var r = new Regex(@"Sec\-WebSocket\-Key:(.*?)\r\n"); //查找"Abc"
            var m = r.Match(head); //设定要查找的字符串
            if (m.Groups.Count != 0) key = Regex.Replace(m.Value, @"Sec\-WebSocket\-Key:(.*?)\r\n", "$1").Trim();

            if (string.IsNullOrEmpty(key)) { Exit(1); return; }

            string rspkey = Convert.ToBase64String(SHA1.Create().ComputeHash(Encoding.ASCII.GetBytes(key + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11")));

            var rsp = new StringBuilder();
            rsp.Append("HTTP/1.1 101 Switching Protocols" + Environment.NewLine);
            rsp.Append("Connection: Upgrade" + Environment.NewLine);
            rsp.Append("Upgrade: websocket" + Environment.NewLine);
            rsp.Append("Sec-WebSocket-Accept: " + rspkey + Environment.NewLine + Environment.NewLine);

            tc.Client.Send(Encoding.UTF8.GetBytes(rsp.ToString()));

            ready = true;

            Debug.WriteLine("hands->OK");

        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="t"></param>
        void Exit(int t)
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
    public class msg : DynamicObject
    {
        Dictionary<string, object> parms = new Dictionary<string, object>();
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (parms.ContainsKey(binder.Name)) result = parms[binder.Name];
            else result = null;
            return true;
        }
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (parms.ContainsKey(binder.Name)) parms[binder.Name] = value;
            else parms.Add(binder.Name, value);
            return true;
        }
        public override string ToString()
        {
            return Serialize.ToJson(parms);
        }
    }

    class Frame
    {
        /// <summary>
        /// 0、后续帧
        /// 1、文本帧
        /// 2、二进制帧
        /// 8、连接关闭
        /// 9、ping
        /// 10、pong
        /// </summary>
        public int opcode { get; set; }
        /// <summary>
        /// 是否有掩码
        /// </summary>
        public bool mask { get; set; }
        /// <summary>
        /// 数据长度
        /// </summary>
        public ulong len { get; set; }
        /// <summary>
        /// 掩码
        /// </summary>
        public byte[] mkey { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public string data { get; set; }
    }

}
