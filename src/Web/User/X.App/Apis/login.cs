using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using X.Core.Cache;
using X.Core.Utility;
using X.Web;
using X.Web.Com;

namespace X.App.Apis
{
    public class login : xapi
    {
        [ParmsAttr(name = "code", req = true)]
        public string code { get; set; }

        protected override bool needus
        {
            get
            {
                return false;
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
                if ((DateTime.Now - dt).TotalSeconds >= 20) break;
            } while (string.IsNullOrEmpty(cu_key));

            if (string.IsNullOrEmpty(cu_key) || cu_key == "###") throw new XExcep("0x0006");

            cu = DB.x_user.FirstOrDefault(o => o.ukey == cu_key);

            CacheHelper.Save("cu." + cu_key, cu, 60 * 20);
            CacheHelper.Remove("login." + code);

            return new lback() { ukey = cu.ukey, headimg = cu.headimg, nickname = cu.nickname };
        }

        class lback : XResp
        {
            public string ukey { get; set; }
            public string headimg { get; set; }
            public string nickname { get; set; }
        }
    }
}
