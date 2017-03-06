using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using System.Linq;
using X.Core.Utility;
using X.GetFans.App;
using X.Core.Plugin;
using System.IO;
using System.Text.RegularExpressions;

namespace X.GetFans
{
    class Program
    {
        static Wc wx = null;
        static string nickname;
        static string headimg;
        static string uin = "";
        static Login lg = null;
        static Main main = null;
        static bool stop = false;
        static bool ctdone = false;
        static List<Wc.Contact> contacts = null;
        static Queue<Msg> msg_qu = null;
        static List<Ct> cts = null;

        [STAThreadAttribute]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Loger.Init();
            System.Net.ServicePointManager.DefaultConnectionLimit = 512;

            lg = new Login();

            Rbt.LoadConfig();

            msg_qu = new Queue<Msg>();
            cts = new List<Ct>();

            //微信线程
            startWx();
            //发送活动规则
            sendRule();

            Application.Run(lg);

            if (string.IsNullOrEmpty(headimg)) return;

            main = new Main(wx, headimg);

            msgProcc();//消息队列处理

            Application.Run(main);
            stop = true;

            if (wx != null) wx.Quit();
        }

        /// <summary>
        /// 消息处理
        /// </summary>
        private static void msgProcc()
        {
            new Thread(o =>
            {
                while (!stop)
                {
                    if (msg_qu.Count() == 0) { Thread.Sleep(1500); continue; }
                    Msg m = null;
                    lock (msg_qu) m = msg_qu.Dequeue();
                    if (m == null) { Thread.Sleep(1500); continue; }
                    var r = wx.Send(m.username, m.tp, m.content);
                    Thread.Sleep(Tools.GetRandNext(500, 2000));
                }
            }).Start();
        }

        /// <summary>
        /// 规则发送
        /// </summary>
        private static void sendRule()
        {
            new Thread(o =>
            {
                while (!stop)
                {
                    if (cts.Count() == 0) { Thread.Sleep(1500); continue; }

                    lock (cts)
                    {
                        var ct = cts.FirstOrDefault(c => c.nicks > 0 && (c.nicks >= Rbt.cfg.newct || c.lst.AddSeconds(Rbt.cfg.tosec) <= DateTime.Now));
                        if (ct == null) continue;

                        var co = contacts.FirstOrDefault(c => c.UserName == ct.gname);
                        if (co == null || co.MemberCount >= Rbt.cfg.full_ct) continue;

                        lock (msg_qu)
                        {
                            if (!string.IsNullOrEmpty(Rbt.cfg.in_txt))
                            {
                                msg_qu.Enqueue(new Msg()
                                {
                                    content = Rbt.cfg.in_txt,
                                    tp = 1,
                                    username = ct.gname
                                });
                            }
                            if (!string.IsNullOrEmpty(Rbt.cfg.sh_txt))
                            {
                                msg_qu.Enqueue(new Msg()
                                {
                                    content = Rbt.cfg.sh_txt,
                                    tp = 1,
                                    username = ct.gname
                                });
                            }
                            if (!string.IsNullOrEmpty(Rbt.cfg.sh_pic))
                            {
                                msg_qu.Enqueue(new Msg()
                                {
                                    content = Rbt.cfg.sh_pic,
                                    tp = 2,
                                    username = ct.gname
                                });
                            }
                        }
                        ct.lst = DateTime.Now;
                        ct.nicks = 0;
                    }
                }
            }).Start();
        }

        /// <summary>
        /// 运行微信
        /// </summary>
        private static void startWx()
        {
            new Thread(o =>
            {
                wx = new Wc();
                wx.LoadQr += Wx_LoadQr; ;
                wx.Scaned += Wx_Scaned;
                wx.Loged += Wx_Loged;
                wx.LogonOut += Wx_LogonOut;
                wx.OutLog += Wx_OutLog; ;
                wx.NewMsg += Wx_NewMsg;
                wx.ContactLoaded += Wx_ContactLoaded;
                wx.Run();

            }).Start();
        }
        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="msg"></param>
        private static void outLog(string msg)
        {
            ((Action)(() =>
            {
                if (main != null) main.OutLog(msg);
            })).BeginInvoke(null, null);
        }

        /// <summary>
        /// 微信通讯录加载事件
        /// </summary>
        /// <param name="cts"></param>
        /// <param name="gname"></param>
        /// <param name="isdone"></param>
        private static void Wx_ContactLoaded(Wc.Contact ct, bool isdone)
        {
            if (main == null) return;
            main.SetContact(ct);

            if (string.IsNullOrEmpty(ct.UserName))
            {
                contacts = ct.MemberList;
                outLog("同步主通讯录");
            }
            else
            {
                var idx = contacts.FindIndex(o => o.UserName == ct.UserName);
                if (idx >= 0) contacts[idx] = ct;
                outLog("同步群：" + ct.NickName + " " + ct.MemberCount + "人");
            }

            if (isdone)
            {
                if (!ctdone) { outLog("通讯录同步完成"); if (string.IsNullOrEmpty(Rbt.cfg.gpname)) main.ShowSetting(); }
                else outLog("通讯录已经更新");
                ctdone = true;
            }
        }
        static void Wx_Loged(Wc.Contact u)
        {
            outLog(u.NickName + "已经登陆");
            nickname = u.NickName;
            uin = u.Uin + "";
            lg.SetLoged();
            Thread.Sleep(2 * 1000);
            lg.Quit();
        }
        static void Wx_OutLog(string log)
        {
            if (Rbt.cfg.isdebug)
            {
                try
                {
                    File.AppendAllText(Application.StartupPath + "\\log_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt", "log@" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff") + "\r\n" + log + "\r\n\r\n");
                }
                catch { }
            }
            Debug.WriteLine("log@" + DateTime.Now.ToString("HH:mm:ss.fff") + "->" + log);
        }
        static void Wx_LogonOut()
        {
            outLog("正在退出...");
            stop = true;
            Thread.Sleep(5 * 1000);
            Environment.Exit(0);
        }
        static void Wx_NewMsg(Wc.Msg m)
        {
            new Thread(p =>
            {
                if (contacts == null || m == null) return;
                var msg = p as Wc.Msg;
                var name = m.FromUserName;

                var u = contacts.FirstOrDefault(o => o.UserName == m.FromUserName);
                if (u == null) { outLog("收到无法识别的消息：" + m.Content); wx.LoadContact(null, false); return; }

                var cot = m.Content;// Tools.RemoveHtml(m.Content);
                Wc.Contact ur = null;

                if (m.FromUserName[1] == '@' && cot[0] == '@')
                {
                    var ct = cot.Substring(0, cot.IndexOf(":"));// cot.Split(':');
                    ur = u?.MemberList.FirstOrDefault(o => o.UserName == ct);
                    cot = cot.Replace(ct + ":", "");
                }

                if (u.MemberCount < Rbt.cfg.full_ct && (msg.MsgType == 10000 || ur == null) && (cot.Contains("加入了群聊") || cot.Contains("移出了群聊") || cot.Contains("修改群名为")))
                    wx.LoadContact(new List<object>(){
                        new {
                            EncryChatRoomId = u.EncryChatRoomId,
                            UserName = u.UserName
                        }
                    }, true);

                if (msg.MsgType == 3 && !string.IsNullOrEmpty(Rbt.cfg.audit_txt) && u.NickName.StartsWith(Rbt.cfg.gpname))
                {
                    lock (msg_qu)
                    {
                        msg_qu.Enqueue(new Msg()
                        {
                            content = Rbt.cfg.audit_txt.Replace("[发送人]", ur == null ? "" : ur.NickName),
                            tp = 1,
                            username = m.FromUserName
                        });
                    }
                }

                if (m.FromUserName[1] == '@' && u.NickName.StartsWith(Rbt.cfg.gpname) && msg.MsgType == 10000 && (cot.Contains("加入了群聊") || cot.Contains("加入群聊")))
                {
                    lock (cts)
                    {
                        var ct = cts.FirstOrDefault(o => o.gname == m.FromUserName);
                        if (ct == null) { ct = new Ct() { gname = m.FromUserName, nicks = 1, lst = DateTime.Now }; cts.Add(ct); }
                        ct.nicks++;
                    }
                }

                object mg = null;

                if (ur != null)
                {
                    var img = wx.GetHeadImage(new List<string>() { u.UserName, ur.UserName });
                    mg = new
                    {
                        body = cot,
                        u = new { name = ur.NickName, img = img != null && img.ContainsKey(ur.UserName) ? img[ur.UserName] : "", id = ur.UserName },
                        r = new { name = u.NickName, img = img != null && img.ContainsKey(u.UserName) ? img[u.UserName] : "", id = u.UserName }
                    };
                }

                if (mg == null)
                {
                    var img = wx.GetHeadImage(new List<string>() { u.UserName });
                    mg = new
                    {
                        body = cot,
                        u = new { name = u.NickName, img = img != null && img.ContainsKey(u.UserName) ? img[u.UserName] : "", id = u.UserName }
                    };
                }

                main.SetMsg(mg);

            }).Start(m);
        }
        static void Wx_Scaned(string hdimg)
        {
            headimg = hdimg;
            lg.SetHeadimg(hdimg);
        }
        static void Wx_LoadQr(string qrcode)
        {
            lg.SetQrcode(qrcode);
        }

        class Msg
        {
            public string username { get; set; }
            public string content { get; set; }
            public int tp { get; set; }
        }

        class Ct
        {
            public string gname { get; set; }
            public int nicks { get; set; }
            public DateTime lst { get; set; }
        }
    }
}
