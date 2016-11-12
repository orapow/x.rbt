using Rbt.Svr.App;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Threading;
using X.Core.Utility;
using X.Data;

namespace Rbt.Svr
{
    public partial class RbtServer : ServiceBase
    {
        RbtDBDataContext db = null;
        List<Wx> wxs = null;
        List<Tcp> tcps = null;
        bool stop = true;
        Queue<long> newwx = null;
        int tcp_port = 0;
        int max_client = 0;

        public RbtServer()
        {
            InitializeComponent();
            db = new RbtDBDataContext() { DeferredLoadingEnabled = true };
            wxs = new List<Wx>();
            tcps = new List<Tcp>();
            newwx = new Queue<long>();
            int.TryParse(ConfigurationManager.AppSettings.Get("tcp_port"), out tcp_port);
            int.TryParse(ConfigurationManager.AppSettings.Get("max_client"), out max_client);
        }

        protected override void OnStart(string[] args)
        {

            stop = false;
            new Thread(() =>
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

            }).Start();
            new Thread(() =>
            {
                while (!stop)
                {
                    if (newwx.Count == 0) { Thread.Sleep(2000); continue; }

                    var id = newwx.Dequeue();
                    new Thread(o =>
                    {
                        var wx = new Wx((long)o);
                        if (wx == null) return;
                        wx.LoadQr += Wx_LoadQr;
                        wx.Scaned += Wx_Scaned;
                        wx.Loged += Wx_Loged;
                        wx.Logout += Wx_Logout; ;

                        lock (wxs) wxs.Add(wx);

                        wx.Run();

                    }).Start(id);

                }

            }).Start();

        }

        protected override void OnStop()
        {
            stop = true;
            lock (wxs) foreach (var c in wxs) if (c != null) c.Quit();
            wxs.Clear();

            lock (tcps) foreach (var c in tcps) if (c != null) c.Quit();
            tcps.Clear();
        }

        void Wx_Loged(Wx w)
        {
            var tc = tcps.FirstOrDefault(o => o.ukey == wx.ukey);
            dynamic msg = new msg();
            msg.act = "loged";
            tc.Send(msg);
        }

        void Wx_Scaned(Wx w)
        {
            var tc = tcps.FirstOrDefault(o => o.ukey == w.ukey);
            dynamic msg = new msg();
            msg.act = "scaned";
            msg.headimg = w.headimg;
            tc.Send(msg);
        }

        void Wx_LoadQr(Wx w)
        {
            var tc = tcps.FirstOrDefault(o => o.ukey == w.ukey);
            dynamic msg = new msg();
            msg.act = "qrcode";
            msg.qrcode = w.qrcode;
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

            long id = long.Parse(msg.lgid + "");
            var lg = db.x_logon.FirstOrDefault(o => o.logon_id == id);

            if (lg == null || string.IsNullOrEmpty(lg.x_user.ukey)) throw new Exception(lg == null ? "登陆器不存在，请重新发起！" : "用户未登陆，请重新登陆");
            if (lg.x_user.ukey != msg.ukey) throw new Exception("用户越权，登陆器不属于你");

            if (msg.act == "loadqr")
            {
                tc.ukey = lg.x_user.ukey;
                lock (newwx) newwx.Enqueue((long)msg.id);
            }
        }

    }
}
