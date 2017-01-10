using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using X.Bot.App;

namespace X.Bot
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var lg = new Login();
            Application.Run(lg);
            if (!string.IsNullOrEmpty(lg.nickname)) Application.Run(new Main(lg.nickname, lg.headimg, lg.ukey));
            BotSdk.LogonOut();
        }
    }
}
