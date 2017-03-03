using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Threading;
using X.Data;
using X.Net;

namespace Rbt.Svr
{
    public partial class RbtServer : ServiceBase
    {
        List<Tcp> tcps = null;
        bool stop = true;
        Queue<wxlog> newwx = null;
        int tcp_port = 0;

        public RbtServer()
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

                //while (!stop)
                //{
                //    var tcp = new Tcp(svr.AcceptTcpClient(),);
                //    tcp.NewMsg += Tcp_NewMsg;
                //    tcp.Closed += Tcp_Closed;
                //    tcp.Start();
                //    lock (tcps) tcps.Add(tcp);
                //}

            })).BeginInvoke(null, null);

            #region 登陆器
            //((Action)(delegate ()
            //{
            //    var db = new RbtDBDataContext() { DeferredLoadingEnabled = true };
            //    while (!stop)
            //    {
            //        if (newwx.Count == 0) { Thread.Sleep(500); continue; }

            //        wxlog wxl = null;// newwx.Dequeue();
            //        lock (newwx) wxl = newwx.Dequeue();
            //        if (wxl == null) continue;

            //        var user = db.x_user.FirstOrDefault(o => o.ukey == wxl.ukey);
            //        if (user == null) throw new Exception("用户未登陆");

            //        var w = tcps.FirstOrDefault(o => o.code == "@" + wxl.lgid);
            //        if (w != null) w.Send(new msg() { act = "exit", from = "@svr" });

            //        new Thread(gid =>
            //        {
            //            var lg = db.x_logon.FirstOrDefault(g => g.logon_id == (long)gid);
            //            if (lg == null) return;// throw new Exception("登陆器不存在");
            //            if (lg.user_id != user.user_id) return; //throw new Exception("用户越权，登陆器不属于你");

            //            var psi = new ProcessStartInfo(AppDomain.CurrentDomain.BaseDirectory + "x.wx.exe", wxl.lgid + " " + "127.0.0.1:" + tcp_port + " " + wxl.ukey);
            //            psi.CreateNoWindow = true;
            //            psi.UseShellExecute = false;
            //            var p = new Process();
            //            p.StartInfo = psi;
            //            p.Start();
            //            p.WaitForExit();

            //            lg.status = 1;
            //            lg.uuid = null;
            //            lg.qrcode = null;
            //            db.SubmitChanges();

            //        }).Start(wxl.lgid);
            //    }

            //    db.Dispose();

            //})).BeginInvoke(null, null);
            #endregion

            ((Action)(delegate ()
            {
                //Thread.Sleep(15 * 1000);
                //var db = new RbtDBDataContext() { DeferredLoadingEnabled = true };
                //while (!stop)
                //{
                //    var ms = db.x_msg.Where(o => o.next_time <= DateTime.Now && o.status == 1);
                //    if (ms.Count() == 0) { if (stop) break; Thread.Sleep(10 * 1000); continue; }
                //    foreach (var m in ms)
                //    {
                        

                //        m.last_time = m.next_time;
                //        if (m.way != 3) m.status = 2;
                //        else m.next_time = DateTime.Now.AddMinutes(m.tcfg.Value);

                //        //Dictionary<string, long?> lgs = null;
                //        //List<x_contact> us = null;

                //        //if (string.IsNullOrEmpty(m.uids))
                //        //{
                //        //    lgs = db.x_logon.Where(o => o.user_id == m.user_id && o.status == 6 && o.uin > 0).ToDictionary(o => "@" + o.logon_id, o => o.uin);
                //        //    us = db.x_contact.Where(o => o.user_id == m.user_id).ToList();
                //        //}
                //        //else
                //        //{
                //        //    us = db.x_contact.Where(o => Serialize.FromJson<List<long>>(System.Web.HttpUtility.HtmlDecode(m.uids)).Contains(o.contact_id)).ToList();
                //        //    lgs = db.x_logon.Where(o => us.GroupBy(u => u.uin).Select(u => u.Key).Contains(o.uin) && o.status == 6).ToDictionary(o => "@" + o.logon_id, o => o.uin);
                //        //}
                //        //foreach (var k in lgs.Keys)
                //        //{
                //        //    var tc = tcps.FirstOrDefault(o => o.code == k);
                //        //    if (tc == null) continue;
                //        //    tc.Send(new msg()
                //        //    {
                //        //        act = "sendmsg",
                //        //        body = Serialize.ToJson(new
                //        //        {
                //        //            body = m.content,
                //        //            type = m.type,
                //        //            touser = us.Where(o => o.uin == lgs[k]).Select(o => o.username).ToList()
                //        //        }),
                //        //        from = "@svr"
                //        //    });
                //        //}
                //    }
                //    try
                //    {
                //        db.SubmitChanges(ConflictMode.ContinueOnConflict);
                //        db.SubmitChanges(ConflictMode.FailOnFirstConflict);
                //    }
                //    catch (ChangeConflictException)
                //    {
                //        foreach (ObjectChangeConflict occ in db.ChangeConflicts)
                //        {
                //            occ.Resolve(RefreshMode.KeepChanges);
                //        }
                //    }
                //    catch { }
                //}
                //db.Dispose();

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

            if (m.to != "@svr")//直接转发
            {
                var wx = tcps.FirstOrDefault(o => o.code == m.to);
                if (wx != null) wx.Send(m);
                else tc.Send(new msg() { act = "err", err = m.to + "已经断开连接。" });
            }
            else
            {
                switch (m.act)
                {
                    case "newmsg":

                        break;
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
