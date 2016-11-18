using Rbt.Svr;
using Rbt.Svr.App;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using X.Core.Utility;
using X.Data;

namespace Test
{


    public partial class Form1 : Form
    {
        RbtDBDataContext db = null;
        List<Wx> wxs = null;
        List<Tcp> tcps = null;
        bool stop = true;
        Queue<long> newwx = null;
        int tcp_port = 9999;
        public Form1()
        {
            InitializeComponent();
            db = new RbtDBDataContext() { DeferredLoadingEnabled = true };
            wxs = new List<Wx>();
            tcps = new List<Tcp>();
            newwx = new Queue<long>();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new Thread(t =>
            {

                ServicePointManager.DefaultConnectionLimit = int.MaxValue;

                stop = false;

                ((Action)(delegate ()
                {

                    var svr = new TcpListener(IPAddress.Any, tcp_port);
                    svr.Start();

                    while (!stop)
                    {
                        var tcp = new Tcp(svr.AcceptTcpClient());
                        tcp.NewMsg += Tcp_NewMsg;
                        tcp.Closed += Tcp_Closed;
                        tcp.Start();
                        lock (tcps) tcps.Add(tcp);
                    }

                })).BeginInvoke(null, null);

                ((Action)(delegate ()
                {
                    while (!stop)
                    {
                        if (newwx.Count == 0) { Thread.Sleep(2000); continue; }

                        var id = newwx.Dequeue();
                        var wx = wxs.FirstOrDefault(o => o.lgid == id);
                        if (wx != null) { Wx_Logout(wx); wx.Quit(); }

                        new Thread(o =>
                        {
                            wx = new Wx((long)o);
                            if (wx == null) return;
                            wx.LoadQr += Wx_LoadQr;
                            wx.Scaned += Wx_Scaned;
                            wx.Loged += Wx_Loged;
                            wx.NewMsg += Wx_NewMsg;
                            wx.Logout += Wx_Logout;
                            lock (wxs) wxs.Add(wx);
                            wx.Run();
                        }).Start(id);
                    }

                })).BeginInvoke(null, null);

            }).Start();
        }

        void Wx_NewMsg(Wx.Msg m, string ukey)
        {
            var tc = tcps.FirstOrDefault(o => o.ukey == ukey);
            dynamic msg = new msg();
            msg.act = "newmsg";
            msg.msg = m.Content;
            tc.Send(msg);
        }

        void Wx_Loged(Wx w)
        {
            var tc = tcps.FirstOrDefault(o => o.ukey == w.ukey);
            dynamic msg = new msg();
            msg.act = "loged";
            tc.Send(msg);
        }

        void Wx_Scaned(string hdimg, string uk)
        {
            var tc = tcps.FirstOrDefault(o => o.ukey == uk);
            dynamic msg = new msg();
            msg.act = "scaned";
            msg.headimg = hdimg;
            tc.Send(msg);
        }

        void Wx_LoadQr(string qr, string uk)
        {
            var tc = tcps.FirstOrDefault(o => o.ukey == uk);
            dynamic msg = new msg();
            msg.act = "qrcode";
            msg.qrcode = qr;
            tc.Send(msg);
        }

        void Wx_Logout(Wx wx)
        {
            lock (wxs) wxs.Remove(wx);
        }

        void Tcp_Closed(Tcp ct)
        {
            lock (tcps) tcps.Remove(ct);
        }

        void Tcp_NewMsg(string json, Tcp tc)
        {
            dynamic msg = Serialize.FromJson<msg>(json);
            if (msg == null) throw new Exception("消息格式不正确，解析失败！");

            if (string.IsNullOrEmpty(msg.ukey + "") || string.IsNullOrEmpty(msg.lgid + "") || string.IsNullOrEmpty(msg.act)) throw new Exception("缺少必要参数，请检查（ukey,lgid,act）参数是否有值！");

            string uk = msg.ukey + "";
            var user = db.x_user.FirstOrDefault(o => o.ukey == uk);
            if (user == null) throw new Exception("用户未登陆");

            long id = long.Parse(msg.lgid + "");
            var lg = db.x_logon.FirstOrDefault(o => o.logon_id == id);

            if (lg.user_id != user.user_id) throw new Exception("用户越权，登陆器不属于你");

            if (msg.act == "loadqr")
            {
                tc.ukey = user.ukey;
                lock (newwx) newwx.Enqueue(lg.logon_id);
            }
        }
    }
}
