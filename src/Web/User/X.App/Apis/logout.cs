using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Core.Cache;
using X.Core.Utility;
using X.Web;
using X.Web.Com;

namespace X.App.Apis
{
    public class logout : xapi
    {
        protected override XResp Execute()
        {
            cu.ukey = "";
            SubmitDBChanges();

            return new XResp();

        }
    }
}
