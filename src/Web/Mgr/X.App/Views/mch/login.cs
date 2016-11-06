using System;
using System.Collections.Generic;
using System.Linq;
using X.Core.Utility;
using X.Web.Views;

namespace X.App.Views.mch
{
    public class login : xview
    {
        protected override string GetParmNames
        {
            get { return "bu"; }
        }
        protected override void InitDict()
        {
            base.InitDict();
            var burl = dict.GetString("bu");
            if (!string.IsNullOrEmpty(burl)) burl = Secret.FormBase64(burl);
            else burl = "/mgr/index.html";
            dict.Add("backurl", burl);
        }
    }
}
