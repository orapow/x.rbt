using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using X.App.Com;
using X.Core.Cache;
using X.Core.Utility;

namespace X.App.Views.wx
{
    public class login : _wx
    {
        public string key { get; set; }
        protected override string GetParmNames
        {
            get
            {
                return "key";
            }
        }

        protected override void InitDict()
        {
            base.InitDict();
            dict.Add("issucc", !string.IsNullOrEmpty(key));
            dict["cu"] = cu;
            CacheHelper.Save("login." + key, cu.ukey);
        }
    }
}
