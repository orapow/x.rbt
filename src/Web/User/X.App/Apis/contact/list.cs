using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web.Com;

namespace X.App.Apis.contact
{
    public class list : xapi
    {
        public int page { get; set; }
        public int limit { get; set; }
        [ParmsAttr(name = "uin", min = 1)]
        public long uin { get; set; }
        public int gid { get; set; }
        protected override XResp Execute()
        {
            var r = new Resp_List();

            var q = from l in cu.x_contact.Where(o => o.uin == uin && o.group_id == gid)
                    select l;

            r.items = q.OrderByDescending(o => o.flag).ThenBy(o => o.nickname).Select(o => new
            {
                id = o.contact_id,
                headimg = o.headimg,
                o.flag,
                o.nickname,
                count = o.membercount > 0 ? o.membercount + "人" : "",
                o.username,
                o.remarkname
            }).Skip((page - 1) * limit).Take(limit).ToList();

            r.count = q.Count();
            r.page = page;
            return r;
        }
    }
}
