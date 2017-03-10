using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using X.Core.Utility;
using X.Wx.App;

namespace X.Wx
{
    class Program
    {
        static Wc wx = null;
        static bool stop = false;
        static bool ctdone = false;

        static Queue<Msg> msg_qu = null;

        static List<Wc.Contact> contacts = null;

        static void Main(string[] args)
        {
            startWx();

            msgHandle();

            batchSend();
        }

        /// <summary>
        /// 运行微信
        /// </summary>
        private static void startWx()
        {
            new Thread(o =>
            {
                wx = new Wc("");
                wx.Logout += Wx_Logout;
                wx.OutLog += Wx_OutLog;
                wx.NewMsg += Wx_NewMsg;
                wx.ContactLoaded += Wx_ContactLoaded; ;
                wx.Run();
            }).Start();
        }

        #region 微信事件
        private static void Wx_ContactLoaded(Wc.Contact ct, bool isdone)
        {
            if (ct == null || ct.MemberList == null) return;

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
                if (!ctdone) outLog("通讯录同步完成");
                else outLog("通讯录已经更新");
                ctdone = true;
            }
        }

        private static void Wx_NewMsg(Wc.Msg m)
        {
            new Thread(p =>
            {
                if (contacts == null || m == null) return;
                var msg = p as Wc.Msg;
                var name = m.FromUserName;

                var u = contacts.FirstOrDefault(o => o.UserName == m.FromUserName);
                if (u == null && (msg.MsgType == 1 || msg.MsgType == 10000)) { outLog("收到无法识别的消息：" + m.Content); wx.LoadContact(null, false); return; }

                var cot = Tools.RemoveHtml(m.Content);
                Wc.Contact ur = null;

                if (m.FromUserName[1] == '@' && cot[0] == '@')
                {
                    var ct = cot.Substring(0, cot.IndexOf(":"));// cot.Split(':');
                    ur = u?.MemberList.FirstOrDefault(o => o.UserName == ct);
                    cot = cot.Replace(ct + ":", "");
                }

                if ((msg.MsgType == 10000 || ur == null) && (cot.Contains("加入了群聊") || cot.Contains("移出了群聊") || cot.Contains("修改群名为")))
                    wx.LoadContact(new List<object>(){
                        new {
                            EncryChatRoomId = u.EncryChatRoomId,
                            UserName = u.UserName
                        }
                    }, true);



                //object mg = null;

                //if (msg.MsgType != 1) return;//  main.OutLog(cot);

                ////消息采集
                //if (Rbt.user.Collect.Ids.Contains(u.NickName) && new Regex("(\r\n)").Matches(cot).Count >= 4)
                //{
                //    var c = false;
                //    foreach (var k in Rbt.user.Collect.Keys.Split(' ')) if (cot.Contains(k)) { c = true; break; }
                //    if (c)
                //    {
                //        var rsp = Sdk.Submit(cot, wx.user.Uin + "_" + u.NickName.Replace("_", "-") + (ur == null ? "" : "_" + ur.NickName.Replace("_", "-")));
                //        if (rsp.RCode != "200")
                //            lock (msg_qu)
                //            {
                //                if (!string.IsNullOrEmpty(Rbt.user.Reply.Identify_Fail))
                //                {
                //                    msg_qu.Enqueue(new Msg()
                //                    {
                //                        content = Rbt.user.Reply.Identify_Fail
                //                            .Replace("[发送人]", ur == null ? u.NickName : ur.NickName).Replace("[错误信息]", rsp.RMessage),
                //                        username = u.UserName
                //                    });
                //                }
                //                if (Rbt.user.Reply.SendTpl_OnFail && !string.IsNullOrEmpty(Rbt.user.Reply.Msg_Tpl))
                //                {
                //                    msg_qu.Enqueue(new Msg()
                //                    {
                //                        content = Rbt.user.Reply.Msg_Tpl,
                //                        username = u.UserName
                //                    });
                //                }
                //            }
                //        else
                //        {
                //            if (!string.IsNullOrEmpty(Rbt.user.Reply.Identify_Succ))
                //            {
                //                lock (msg_qu)
                //                    msg_qu.Enqueue(new Msg()
                //                    {
                //                        content = Rbt.user.Reply.Identify_Succ
                //                            .Replace("[城市]", Rbt.cfg.CityName)
                //                            .Replace("[楼盘]", rsp.Data.build_name)
                //                            .Replace("[发送人]", ur == null ? u.NickName : ur.NickName)
                //                            .Replace("[经纪人]", rsp.Data.agent_name)
                //                            .Replace("[经纪人电话]", rsp.Data.agent_mobile)
                //                            .Replace("[客户姓名]", rsp.Data.customer_name)
                //                            .Replace("[客户电话]", rsp.Data.customer_mobile),
                //                        username = u.UserName
                //                    });
                //            }
                //        }
                //    }
                //}

                //if (ur != null)
                //{
                //    var img = wx.GetHeadImage(new List<string>() { u.UserName, ur.UserName });
                //    mg = new
                //    {
                //        body = cot,
                //        u = new { name = ur.NickName, img = img != null && img.ContainsKey(ur.UserName) ? img[ur.UserName] : "", id = ur.UserName },
                //        r = new { name = u.NickName, img = img != null && img.ContainsKey(u.UserName) ? img[u.UserName] : "", id = u.UserName }
                //    };
                //}

                //if (mg == null)
                //{
                //    var img = wx.GetHeadImage(new List<string>() { u.UserName });
                //    mg = new
                //    {
                //        body = cot,
                //        u = new { name = u.NickName, img = img != null && img.ContainsKey(u.UserName) ? img[u.UserName] : "", id = u.UserName }
                //    };
                //}

                //main.SetMsg(mg);

            }).Start(m);
        }

        private static void Wx_OutLog(string log)
        {
            throw new NotImplementedException();
        }

        private static void Wx_Logout()
        {
            throw new NotImplementedException();
        }
        #endregion

        /// <summary>
        /// 消息处理
        /// </summary>
        static void msgHandle()
        {
            new Thread(o =>
            {
                while (!stop)
                {
                    if (msg_qu.Count() == 0) { Thread.Sleep(1500); continue; }
                    Msg m = null;
                    lock (msg_qu) m = msg_qu.Dequeue();
                    if (m == null) { Thread.Sleep(1500); continue; }
                    var r = wx.Send(m.username, 1, m.content);
                    //if (r && m.status > 0) Sdk.SetStatus(m.msg_id, m.status);
                    Thread.Sleep(Tools.GetRandNext(100, 1000));
                }
            }).Start();
        }

        static void batchSend()
        {
            new Thread(o =>
            {
                Thread.Sleep(20 * 1000);
                while (!stop)
                {
                    Thread.Sleep(10 * 1000);
                    //var rsp = Sdk.LoadMsg(uin);
                    if (stop) break;
                    //if (rsp.Data == null || rsp.Data.Count() == 0) { outLog("转发@" + st + "->无内容"); continue; }

                    //outLog("转发@" + st + "->开始发送，" + rsp.Data.Count() + "条");

                    //foreach (var m in rsp.Data)
                    //{
                    //    var r = Rbt.user.Send.FirstOrDefault(rc => rc.BuildName == m.build_name);
                    //    if (r == null)
                    //    {
                    //        outLog("缺少楼盘：" + m.build_name + "的转发规则，请添加");
                    //        continue;
                    //    }

                    //    var ps = m.regist_user_id.Split('_');

                    //    var ct = Rbt.user.Contacts.FirstOrDefault(c => c.NickName == r.NickName);
                    //    if (ct == null)
                    //    {
                    //        outLog("楼盘：" + m.build_name + "的转发规则失效，找不到转发目标群，请更改");
                    //        continue;
                    //    }

                    //    var txt = @"" + m.build_name + "<br/>公司：" + m.company_name + "<br/>客户姓名：" + m.customer_name + "<br/>客户电话：" + m.customer_mobile + "<br/>经纪姓名：" + m.agent_name + "<br/>经纪电话：" + m.agent_mobile + "<br/>看房时间：" + m.look_date_str;

                    //    lock (msg_qu) msg_qu.Enqueue(new Msg()
                    //    {
                    //        msg_id = m.regist_id,
                    //        content = txt,
                    //        username = ct.UserName,
                    //        status = 2
                    //    });

                    //    Thread.Sleep(5 * 1000);
                    //}

                    //outLog("群发@" + st + "->结束");
                }
            }).Start();
        }

        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="msg"></param>
        static void outLog(string msg)
        {

        }

        class Msg
        {
            public int msg_id { get; set; }
            public string username { get; set; }
            public string content { get; set; }
            public int status { get; set; }
        }

    }
}
