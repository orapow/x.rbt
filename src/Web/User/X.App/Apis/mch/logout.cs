using System;
using System.Collections.Generic;
using System.Linq;
using X.Core.Cache;
using X.Core.Utility;
using X.Web.Com;

namespace X.App.Apis.mch
{
    public class logout : xmg
    {
        protected override string powercode
        {
            get
            {
                return string.Empty;
            }
        }
        protected override XResp Execute()
        {
            var k = GetReqParms("mch_ad");
            CacheHelper.Remove("mch." + k);
            return new XResp();
        }
    }
}
