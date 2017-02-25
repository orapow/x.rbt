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
    public class cancel : xapi
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
            CacheHelper.Save("login." + code, "###");
            return new XResp();
        }

    }
}
