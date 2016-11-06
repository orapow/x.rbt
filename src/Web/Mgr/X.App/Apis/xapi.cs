using System;
using System.Collections.Generic;
using System.Linq;
using X.App.Com;
using X.Core.Cache;
using X.Data;
using X.Web;
using X.Web.Apis;

namespace X.App.Apis
{
    public class xapi : Api
    {
        protected Config cfg = Config.LoadConfig();
    }
}
