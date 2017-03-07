using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using X.Core.Utility;

namespace X.GetFans.App
{
    public class Rbt
    {
        public static Config cfg = null;
        static string key = "TpuRuwJvgE3B93K5c8w91FIJktAcHu5btKZBycbfmaryS0uYn2GrhO2CP7r5eSWH";

        public static void LoadConfig()
        {
            try
            {
                var text = File.ReadAllText(Application.StartupPath + "\\cfg.x");
                if (!string.IsNullOrEmpty(text)) text = Secret.Rc4.Decrypt(text, key);
                cfg = Serialize.FromJson<Config>(text);
            }
            catch { }
            if (cfg == null) cfg = new Config();
        }

        public static void SaveConfig()
        {
            if (cfg == null) return;
            var text = Secret.Rc4.Encrypt(Serialize.ToJson(cfg), key);
            File.WriteAllText(Application.StartupPath + "\\cfg.x", text);
        }

        public class Config
        {
            public string audit_txt { get; set; }
            public string sh_txt { get; set; }
            public string sh_pic { get; set; }
            public string in_txt { get; set; }
            public int newct { get; set; }
            public int tosec { get; set; }
            public string gpname { get; set; }
            public bool isdebug { get; set; }
            public string full_txt { get; set; }
            public int full_ct { get; set; }
        }
    }
}
