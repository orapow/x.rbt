using System;
using System.Collections.Generic;
using System.Linq;
using X.App.Com;
using X.Core;
using X.Core.Cache;
using X.Core.Plugin;
using X.Web.Com;

namespace X.App
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            var cfg = Config.LoadConfig();
            Tpl.Configuration(Server.MapPath("~/tpl/"));
            Loger.Init();
            CacheHelper.Init(cfg.cache);
            //Wx.Mp.Init(new Wx.Mp.Wxcfg() { appid = "wxb8288a0ed0f2cc7f", appsecret = "1a55599a01fe52767029848fb3b44061" });
            //WxHelper.Init(cfg.wxcfg);
            //SmsHelper.Init(cfg.smscfg);
        }
    }
}
