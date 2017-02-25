using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web.Com;

namespace X.App.Apis.mp
{
    public class list : xapi
    {
        [ParmsAttr(name = "页码", def = 1)]
        public int page { get; set; }
        [ParmsAttr(name = "条数", def = 30)]
        public int limit { get; set; }

        protected override XResp Execute()
        {
            var r = new Resp_List();
            r.items = cu.x_wxmp
                .Skip((page - 1) * limit)
                .Take(limit)
                .OrderByDescending(o => o.ctime)
                .Select(o => new
                {
                    id = o.wxmp_id,
                    name = o.nick_name,
                    img = o.head_img,
                }).ToList();
            r.count = cu.x_wxmp.Count();
            r.page = page;
            return r;
        }

    }
}
