using Rbt.Svr.App;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Dynamic;
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

        void Tcp_Closed(Tcp ct)
        {
            lock (tcps) tcps.Remove(ct);
        }

        void Tcp_NewMsg(string json, Tcp ct)
        {
            dynamic msg = Serialize.FromJson<msg>(json);
            if (msg == null || string.IsNullOrEmpty(msg.ukey + "") || string.IsNullOrEmpty(msg.act)) { ct.Quit(); return; }

            if (msg.act == "loadqr" && !string.IsNullOrEmpty(msg.id + ""))
            {
                lock (newwx) newwx.Enqueue((long)msg.id);
            }

        }

        void Wx_Logout(Wx wx)
        {
            lock (wxs) wxs.Remove(wx);
        }

    }
}
