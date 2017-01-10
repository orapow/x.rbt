using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using X.Core.Utility;

namespace X.App.Com
{
    /// <summary>
    /// 常规配置
    /// </summary>
    public class Config
    {
        /// <summary>
        /// 域名
        /// </summary>
        public string domain { get; set; }
        /// <summary>
        /// 系统名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 百度key
        /// </summary>
        public string bdkey { get; set; }
        /// <summary>
        /// 缓存设置
        /// 1、Memcached
        /// 2、WebCached
        /// </summary>
        public int cache { get; set; }

        public string wx_appid { get; set; }
        public string wx_scr { get; set; }
        public string wx_mch_id { get; set; }
        /// <summary>
        /// 微信证书路径
        /// </summary>
        public string wx_certpath { get; set; }
        public string wx_paykey { get; set; }

        private static string file = HttpContext.Current.Server.MapPath("/dat/cfg.x");
        private static Config cfg = null;
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        public static Config LoadConfig()
        {
            if (cfg == null)
            {
                var json = Tools.ReadFile(file);
                if (string.IsNullOrEmpty(json)) return new Config();
                cfg = Serialize.FromJson<Config>(json);
            }
            return cfg;
        }
        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="cfg"></param>
        public static void SaveConfig(Config cfg)
        {
            Tools.SaveFile(HttpContext.Current.Server.MapPath("/dat/cfg.x"), Serialize.ToJson(cfg));
        }
    }
}
