using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows.Forms;
using X.Bot.App;
using X.Core.Plugin;

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

            Loger.Init();

            System.Net.ServicePointManager.DefaultConnectionLimit = 512;

            var key = ConfigurationManager.AppSettings["app-key"];//key = "GCEOrLSsBxmsKQTFV63UNRSG8wwhFkXbTujBbfuqPO4AFjljiYEOTZ8w7JtiDz8q";
            
            if (string.IsNullOrEmpty(key))
            {
                var ak = new Akey();
                Application.Run(ak);
                if (string.IsNullOrEmpty(ak.key)) return;

                var cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (key == null) cfg.AppSettings.Settings.Add("app-key", ak.key);
                else cfg.AppSettings.Settings["app-key"].Value = ak.key;
                cfg.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

                key = ak.key;
            }

            if (!Sdk.Init(key)) return;

            Application.Run(new Main());

        }
    }
}
