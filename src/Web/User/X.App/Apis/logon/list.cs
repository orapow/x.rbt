using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web.Com;

namespace X.App.Apis.logon
{
    public class list : xapi
    {
        public int page { get; set; }
        public int limit { get; set; }

        protected override XResp Execute()
        {
            if (page == 0) page = 1;
            if (limit == 0) limit = 30;
            var r = new Resp_List();
            r.page = page;
            r.count = cu.x_logon.Count();
            r.items = cu.x_logon.Skip((page - 1) * limit).Take(limit).Select(o => new { id = o.logon_id, o.headimg, nickname = o.status == 6 ? o.nickname : "请登陆", o.status }).ToList();
            return r;
        }
    }
}
