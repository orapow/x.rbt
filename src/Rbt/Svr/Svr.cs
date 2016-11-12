using X.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Rbt.Svr;

namespace X.Rbt.App
{
    public class Service
    {
        RbtDBDataContext db = new RbtDBDataContext() { DeferredLoadingEnabled = true };
        List<Wx> clients = new List<Wx>();
        bool stop = true;
        public static void Start()
        {
            db.x_logon.DeleteAllOnSubmit(db.x_logon);
            db.SubmitChanges();

            int max = 1;
            new Thread(() =>
            {
                stop = false;

                while (!stop)
                {

                    var ct = clients.Count(o => o.status == 3);
                    if (ct >= max) { Thread.Sleep(5 * 1000); continue; }

                    var cts = db.x_logon.Where(o => o.status == 1).Take(max).ToList();
                    foreach (var c in cts) newWx(c.logon_id);

                    Thread.Sleep(5 * 1000);

                    ct = clients.Count(o => o.status == 3);
                    if (ct >= max) { Thread.Sleep(5 * 1000); continue; }

                    var list = new List<x_logon>();
                    for (var i = 0; i < max - ct; i++) list.Add(new x_logon() { code = Guid.NewGuid().ToString(), status = 1 });
                    db.x_logon.InsertAllOnSubmit(list);
                    db.SubmitChanges();

                }

            }).Start();

        }

        static void newWx(long id)
        {
            new Thread(o =>
            {
                var wx = new Wx((long)o);
                if (wx == null) return;

                wx.Logout += Wx_Logout;
                clients.Add(wx);

                wx.Run();

            })
            { IsBackground = true }.Start(id);

        }

        private static void Wx_Logout(Wx wx)
        {
            lock (clients) clients.Remove(wx);
        }

        public static void Stop()
        {
            stop = true;
            lock (clients) foreach (var c in clients) if (c != null) c.Quit();
            clients.Clear();
        }

    }
}
