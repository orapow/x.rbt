using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using X.Core.Utility;

namespace X.Lpw.App
{
    public class Rbt
    {
        public static Config cfg = null;
        public static User user = null;

        public static void LoadConfig()
        {
            try
            {
                var text = File.ReadAllText(Application.StartupPath + "\\cfg.x");
                if (!string.IsNullOrEmpty(text)) text = Secret.Rc4.Decrypt(text);
                cfg = Serialize.FromJson<Config>(text);
            }
            catch { }
            if (cfg == null) cfg = new Config();
        }

        public static void LoadUser(string uin)
        {
            try
            {
                var text = File.ReadAllText(Application.StartupPath + "\\" + uin + "_cfg.x");
                if (!string.IsNullOrEmpty(text)) text = Secret.Rc4.Decrypt(text);
                user = Serialize.FromJson<User>(text);
            }
            catch { }
            if (user == null) user = new User() { Uin = uin };
        }

        public static void SaveUser()
        {
            if (user == null || string.IsNullOrEmpty(user.Uin)) return;
            lock (user)
            {
                var text = Secret.Rc4.Encrypt(Serialize.ToJson(user));
                File.WriteAllText(Application.StartupPath + "\\" + user.Uin + "_cfg.x", text);
            }
        }

        public static void SaveConfig()
        {
            if (cfg == null) return;
            var text = Secret.Rc4.Encrypt(Serialize.ToJson(cfg));
            File.WriteAllText(Application.StartupPath + "\\cfg.x", text);
        }

        public class Config
        {
            public string GateWay { get; set; }
            public int City_Id { get; set; }
            public string CityName { get; set; }
            public User User { get; set; }
            public Config() { User = new User(); }
        }

        public class User
        {
            public string Uin { get; set; }
            public ReplyCfg Reply { get; set; }
            public List<SendRule> Send { get; set; }
            public CollectRule Collect { get; set; }
            public List<Wc.Contact> Contacts { get; set; }
            public bool IsDebug { get; set; }
            public User()
            {
                Reply = new ReplyCfg();
                Send = new List<SendRule>();
                Collect = new CollectRule();
                Contacts = new List<Wc.Contact>();
            }
        }

        public class CollectRule
        {
            public string Keys { get; set; }
            public List<string> Ids { get; set; }
            public CollectRule() { Ids = new List<string>(); }
        }

        public class ReplyCfg
        {
            /// <summary>
            /// 提交成功
            /// </summary>
            public string Succ { get; set; }
            /// <summary>
            /// 提交失败
            /// </summary>
            public string Fail { get; set; }
            public string Fail1 { get; set; }
            public string Warn_User { get; set; }
        }

        public class SendRule
        {
            /// <summary>
            /// 通讯录ID
            /// </summary>
            public long Contact_id { get; set; }
            /// <summary>
            /// 用户名
            /// </summary>
            public string UserName { get; set; }
            /// <summary>
            /// 昵称
            /// </summary>
            public string NickName { get; set; }
            /// <summary>
            /// 楼盘名
            /// </summary>
            public string BuildName { get; set; }

            public override string ToString()
            {
                return BuildName + " -> " + NickName;
            }
        }
    }
}
