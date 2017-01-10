using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using wxBot;
using System.Drawing;
using System.Collections.Generic;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            log4net.Config.XmlConfigurator.Configure();

            log4net.ILog _log = log4net.LogManager.GetLogger("App");

            BotManager bot = new BotManager();

            bot.GetUUID();

            //Console.WriteLine(bot.UUID);

            Image qrCode = bot.GetQRCode();
            qrCode.Save("qr.png");

            System.Diagnostics.Process.Start("qr.png");

            bot.GetUserAvatar();
            System.Diagnostics.Process.Start("avatar.png");


            bot.WaitForLogin();

            bot.Login();

            bot.wxInit();

            bot.wxStatusNotify();

            bot.GetContact();

            bot.TargetGroup = "这是测试";
            IList<Group> group = new List<Group>();
            foreach (var item in bot.GroupList)
            {
                Console.WriteLine(item.NickName);

                if (item.NickName == bot.TargetGroup)
                {
                    group.Add(item);
                    Console.WriteLine(item.UserName);
                }
            }
            bot.GroupList = group;
            Console.WriteLine();

            bot.BatchGetContact();

            //IList<Contact> userList = bot.UserList;
            //foreach (var item in userList)
            //{
            //    Console.WriteLine(item.NickName + item.Uin.ToString());
            //}


            //发送图片
            string webwx_data_ticket = String.Empty;
            System.Net.CookieCollection cc = bot._cookie.GetCookies(new Uri("http://qq.com"));
            foreach (System.Net.Cookie item in cc)
            {
                if (item.Name == "webwx_data_ticket")
                {
                    webwx_data_ticket = item.Value;
                    break;
                }
            }
            string aa = bot.wxUpload(webwx_data_ticket);
            Console.WriteLine(aa);



            //SyncCheck();

            //WebwxSync();

            while (true)
            {
                string val = bot.SyncCheck();
                if (val != "window.synccheck={retcode:\"0\",selector:\"0\"}")
                {
                    bot.wxSync();
                }
            }
        }
    }
}
