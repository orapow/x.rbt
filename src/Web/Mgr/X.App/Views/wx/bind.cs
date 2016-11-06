using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.App.Com;
using X.Core.Utility;

namespace X.App.Views.wx
{
    public class bind : wx
    {
        public override int needwx
        {
            get
            {
                return 2;
            }
        }
        protected override string GetParmNames
        {
            get
            {
                return "backurl";
            }
        }
        protected override void InitDict()
        {
            base.InitDict();
            var url = dict.GetString("backurl");
            if (!string.IsNullOrEmpty(url)) dict["backurl"] = Secret.FormBase64(url);
        }
    }
}
