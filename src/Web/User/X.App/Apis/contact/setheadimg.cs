using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Core.Utility;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.contact
{
    public class setheadimg : xapi
    {
        [ParmsAttr(name = "头像数据", req = true)]
        public string imgs { get; set; }
        [ParmsAttr(name = "微信号", min = 1)]
        public long uin { get; set; }

        protected override XResp Execute()
        {
            var us = Serialize.FromJson<Dictionary<string, string>>(Context.Server.HtmlDecode(imgs));
            if (us == null) throw new XExcep("T头像数据为空");

            var cs = cu.x_contact.Where(o => us.Keys.Contains(o.username));
            foreach (var c in cs)
            {
                if (us.Keys.Contains(c.username)) c.headimg = us[c.username];
            }

            SubmitDBChanges();

            return new XResp();
        }
    }
}
