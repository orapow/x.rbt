using Rbt.Svr.App;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Threading;

namespace X.Svr
{
    public partial class Rbt : ServiceBase
    {
        List<Tcp> tcps = null;
        bool stop = true;
        int tcp_port = 0;

        public Rbt()
        {
            InitializeComponent();
            tcps = new List<Tcp>();
            int.TryParse(ConfigurationManager.AppSettings.Get("tcp_port"), out tcp_port);
        }

        protected override void OnStart(string[] args)
        {
            stop = false;

            ((Action)(delegate ()
            {
                var svr = new TcpListener(IPAddress.Parse("127.0.0.1"), tcp_port);
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

        }

        protected override void OnStop()
        {
            stop = true;
            lock (tcps) foreach (var c in tcps) if (c != null) c.Quit();
            tcps.Clear();
        }

        private void Tcp_NewMsg(msg m, Tcp tc)
        {
            if (m == null) throw new Exception("消息格式不正确，解析失败！");

            if (string.IsNullOrEmpty(m.from + "") || string.IsNullOrEmpty(m.to + "") || string.IsNullOrEmpty(m.act))
            {
                tc.Send(new msg() { err = "缺少必要参数，请检查（from,act,to）参数是否有值！", act = "err" });// throw new Exception("缺少必要参数，请检查（ukey,lgid,act）参数是否有值！");
                tc.Quit();
                return;
            }

            switch (m.act)
            {
                case "loadqr":
                    lock (tcps)
                    {
                        var t = tcps.FirstOrDefault(o => o.code == "@" + m.body);
                        if (t == null) break;
                        {
                            t.Quit();
                            tcps.Remove(t);
                        }
                    }
                    lock (newwx) newwx.Enqueue(new wxlog() { lgid = long.Parse(m.body), ukey = m.from });
                    break;
                case "hands":
                    tc.code = m.from;
                    break;
            }
        }

        void Tcp_Closed(Tcp ct)
        {
            lock (tcps) tcps.Remove(ct);
        }
    }
}
