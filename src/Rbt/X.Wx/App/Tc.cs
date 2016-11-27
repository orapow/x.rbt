using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using X.Core.Utility;

namespace X.Wx.App
{
    public class Tc
    {
        public delegate void ClosedHandler();
        public event ClosedHandler Closed;

        public delegate void NewMsgHandler(msg m);
        public event NewMsgHandler NewMsg;

        TcpClient tc = null;
        string ip = "";
        int port = 0;
        bool stop = false;
        string ukey = "";
        long lgid = 0;
        List<byte> data = new List<byte>();

        public Tc(string i, int p)
        {
            tc = new TcpClient();
            ip = i;
            port = p;
        }

        public void Run(string uk, long id)
        {
            try
            {
                tc.Connect(ip, port);
                ukey = uk;
                lgid = id;

                tc.Client.Send(Encoding.UTF8.GetBytes(uk + ":" + id + ":" + Secret.MD5(uk + id + "x.rbt")));

                new Thread(o => { recProc(); }).Start();

                while (!stop)
                {
                    if (tc.Available == 0) { Thread.Sleep(500); continue; }

                    var d = new byte[tc.Available];
                    tc.Client.Receive(d);

                    lock (data) data.AddRange(d);
                }

            }
            catch
            {
                exit(1);
            }
        }

        public void Send(msg m)
        {
            if (tc.Client == null || tc.Client.Connected == false) { exit(1); return; }

            try
            {
                m.to = ukey;
                m.from = "@" + lgid;
                if (string.IsNullOrEmpty(m.to)) m.to = ukey;
                var body = Encoding.UTF8.GetBytes(m.ToString());
                var data = new List<byte>();
                data.AddRange(Encoding.UTF8.GetBytes("x.rbt"));
                data.AddRange(BitConverter.GetBytes((ushort)body.Length));
                data.AddRange(body);
                tc.Client.Send(data.ToArray());
            }
            catch (Exception ex)
            {
                Debug.WriteLine("tc.send->" + ex.Message);
                exit(1);
            }

        }

        public void Quit()
        {
            exit(0);
        }

        /// <summary>
        /// 处理消息
        /// </summary>
        void recProc()
        {
            try
            {
                while (!stop)
                {

                    if (data.Count < 7) { Thread.Sleep(500); continue; }

                    var head = data.Take(5).ToArray();
                    if (Encoding.UTF8.GetString(head) != "x.rbt") continue;

                    var len = BitConverter.ToUInt16(data.Take(2).ToArray(), 0);
                    var body = Encoding.UTF8.GetString(data.Skip(7).Take(len).ToArray());
                    lock (data) data.RemoveRange(0, len + 7);

                    NewMsg?.Invoke(Serialize.FromJson<msg>(body));

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("tc.recProc->" + ex.Message);
                exit(1);
            }
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="t"></param>
        void exit(int t)
        {
            stop = true;
            if (t == 1) Closed?.Invoke();
        }

    }

    public class msg //: DynamicObject
    {
        public string to { get; set; }
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
