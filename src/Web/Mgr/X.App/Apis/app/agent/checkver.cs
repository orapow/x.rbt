using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using X.Core.Utility;
using X.Web.Com;

namespace X.App.Apis.app.agent
{
    public class checkver : xag
    {
        public int ver { get; set; }
        protected override void Validate()
        {
            base.Validate();
            Validator.CheckRange("vno", ver, 0, null);
        }

        protected override bool need_user
        {
            get
            {
                return false;
            }
        }

        protected override XResp Execute()
        {
            var r = new version();
            var json = File.ReadAllText(Context.Server.MapPath("/app/ag_ver.x"));
            if (string.IsNullOrEmpty(json)) return r;

            var v = Serialize.FromJson<version>(json);
            if (v == null) return r;

            if (v.no <= ver) return r;

            v.hasnew = true;

            return v;
        }

        public class version : XResp
        {
            public bool hasnew { get; set; }
            public string name { get; set; }
            public string ranges { get; set; }
            public int no { get; set; }
            public string date { get; set; }
            public string url { get; set; }
            public string desc { get; set; }
        }
    }
}
