using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using X.Core.Cache;
using X.Core.Utility;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis
{
    public class login : xapi
    {
        [ParmsAttr(name = "code", req = true)]
        public string code { get; set; }

        protected override int needus
        {
            get
            {
                return 0;
            }
        }

        protected override XResp Execute()
        {
            string cu_key = "";
            var dt = DateTime.Now;

            do
            {
                cu_key = CacheHelper.Get<string>("login." + code);
                Thread.Sleep(100);
                if ((DateTime.Now - dt).TotalSeconds >= 60 * 5) break;
            } while (string.IsNullOrEmpty(cu_key));

            CacheHelper.Remove("login." + code);
            if (string.IsNullOrEmpty(cu_key) || cu_key == "###") throw new XExcep("0x0006");

            cu = CacheHelper.Get<x_user>(cu_key);
            if (cu == null) throw new XExcep("0x0006");

            Context.Response.SetCookie(new System.Web.HttpCookie("ukey-" + cfg.wx_appid, cu_key));

            return new lback() { headimg = cu.headimg, nickname = cu.nickname };
        }

        class lback : XResp
        {
            public string headimg { get; set; }
            public string nickname { get; set; }
        }
    }
}
