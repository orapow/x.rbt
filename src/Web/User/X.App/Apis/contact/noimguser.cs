using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web.Com;

namespace X.App.Apis.contact
{
    public class noimguser : xapi
    {
        [ParmsAttr(name = "微信号", min = 1)]
        public long uin { get; set; }
        protected override XResp Execute()
        {
            var r = new Resp_List();
            var us = cu.x_contact.Where(o => o.uin == uin && o.headimg == null).OrderBy(o => Guid.NewGuid()).Take(50);
            if (us.Count() == 0) us = cu.x_contact.Where(o => o.uin == uin && o.headimg == "1").OrderBy(o => Guid.NewGuid()).Take(50);
            r.items = us.Select(o => o.username).ToList();
            return r;
        }
    }
}
