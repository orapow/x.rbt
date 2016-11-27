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
using X.Core.Utility;
using X.Data;

namespace Rbt.Svr
{
    public partial class RbtServer : ServiceBase
    {
        RbtDBDataContext db = null;
        List<Tcp> tcps = null;
        bool stop = true;
        Queue<wxlog> newwx = null;
        int tcp_port = 0;
        int max_client = 0;

        public RbtServer()
        {
            InitializeComponent();
            db = new RbtDBDataContext() { DeferredLoadingEnabled = true };
            tcps = new List<Tcp>();
            newwx = new Queue<wxlog>();
            int.TryParse(ConfigurationManager.AppSettings.Get("tcp_port"), out tcp_port);
            int.TryParse(ConfigurationManager.AppSettings.Get("max_client"), out max_client);
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
                    tcp.NewMsg += Tcp_NewMsg; ;
                    tcp.Closed += Tcp_Closed;
                    tcp.Start();
                    lock (tcps) tcps.Add(tcp);
                }

            })).BeginInvoke(null, null);

            ((Action)(delegate ()
            {

                while (!stop)
                {
                    if (newwx.Count == 0) { Thread.Sleep(500); continue; }

                    wxlog wxl = null;// newwx.Dequeue();
                    lock (newwx) wxl = newwx.Dequeue();
                    if (wxl == null) continue;

                    var user = db.x_user.FirstOrDefault(o => o.ukey == wxl.ukey);
                    if (user == null) throw new Exception("用户未登陆");

                    var lg = db.x_logon.FirstOrDefault(o => o.logon_id == wxl.lgid);
                    if (lg == null) continue;// throw new Exception("登陆器不存在");
                    if (lg.user_id != user.user_id) continue; //throw new Exception("用户越权，登陆器不属于你");

                    new Thread(o =>
                    {
                        var psi = new ProcessStartInfo(AppDomain.CurrentDomain.BaseDirectory + "x.wx.exe", wxl.lgid + " " + "127.0.0.1:" + tcp_port + " " + wxl.ukey);
                        psi.CreateNoWindow = true;
                        psi.UseShellExecute = false;
                        var p = new Process();
                        p.StartInfo = psi;
                        p.Start();
                        p.WaitForExit();

                    }).Start();

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
                tc.Send(new msg() { err = "缺少必要参数，请检查（from,act,to）参数是否有值！", act = "err" }, m.from);// throw new Exception("缺少必要参数，请检查（ukey,lgid,act）参数是否有值！");
                tc.Quit();
                return;
            }

            if (m.to != "@svr")//直接转发
            {
                var wx = tcps.FirstOrDefault(o => o.code == m.to);
                if (wx != null) wx.Send(m, m.to);
            }
            else
            {
                switch (m.act)
                {
                    case "loadqr":
                        lock (tcps)
                        {
                            var t = tcps.FirstOrDefault(o => o.code == "@" + m.body);
                            if (t != null)
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
        }

        void Tcp_Closed(Tcp ct)
        {
            lock (tcps) tcps.Remove(ct);
        }

        class wxlog
        {
            public long lgid { get; set; }
            public string ukey { get; set; }
        }
    }
}
