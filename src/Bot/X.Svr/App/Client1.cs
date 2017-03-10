using System;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Rbt.Svr.App
{
    public class Client
    {
        /// <summary>
        /// 客户端名
        /// </summary>
        public string id { get; set; }

        public delegate void NewMsgHandler(Msg m);
        public event NewMsgHandler NewMsg;

        public delegate void CloseHandler(string name);
        public event CloseHandler Closed;

        TcpClient tc = null;
        bool stop = false;

        public Client(TcpClient t)
        {
            tc = t;
            new Thread(o => { MsgAccept(); }).Start();
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg">消息内容</param>
        /// <param name="act">
        /// 动作：
        /// hold 握手
        /// msg 消息
        /// end 结束
        /// </param>
        public void SendMsg(string txt, string act)
        {
            SendMsg(txt, act, id);
        }

        public void SendMsg(string txt, string act, string from)
        {
            SendMsg(new Msg() { act = act, id = id, from = from, text = txt });
        }

        public void SendMsg(Msg m)
        {
            try
            {
                tc.Client.Send(Encoding.UTF8.GetBytes(m.ToString()));
            }
            catch
            {
                tc.Close();
                tc = null;
                Closed?.Invoke(id);
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            stop = true;
            SendMsg("", "--close--");
            tc.Client.Disconnect(false);
            tc.Close();
            Closed?.Invoke(id);
        }

        private void MsgAccept()
        {
            while (true)
            {
                if (stop) break;
                if (tc == null || tc.Client == null || !tc.Connected) break;
                if (tc.Available == 0) { Thread.Sleep(200); continue; }

                var data = new byte[tc.Available];
                tc.Client.Receive(data);

                string json = Encoding.UTF8.GetString(data);
                Console.WriteLine("msg->" + json);

                var reg = new Regex("({[\\S][^}]+})");
                Match m = reg.Match(json);
                while (m.Success)
                {
                    var msg = Com.FromJson<Msg>(m.Groups[1].Value);
                    if (msg == null || !msg.Valid() || msg.act == "end") { Close(); }

                    NewMsg?.Invoke(msg);

                    m = m.NextMatch();
                };
            }
        }

    }

    public class Msg
    {
        string scret = "3PeJif1y9Q98eIQnAG7m0AM7HF5oIWV6aiVpVWPhEM6FMCm9LcePvo55nrCEgBeazsI5i7dxcHNEpq3KFGh5u2BUSeCB8FHo0gd";

        /// <summary>
        /// hold
        /// msg
        /// end
        /// </summary>
        public string act { get; set; }
        public string from { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public string id { get; set; }
        public string text { get; set; }
        public string sign { get; set; }

        public override string ToString()
        {
            sign = MD5(from + text + scret);
            return Com.ToJson(this);
        }

        public bool Valid()
        {
            return sign == MD5(from + text + scret);
        }

        string MD5(string txt)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(Encoding.UTF8.GetBytes(txt));
            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < t.Length; i++) sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            return sb.ToString();
        }
    }
}
