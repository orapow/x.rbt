using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web.Com;

namespace X.App.Apis.cd
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
            r.items = cu.x_ad
                .Skip((page - 1) * limit)
                .Take(limit)
                .OrderByDescending(o => o.ctime)
                .Select(o => new
                {
                    id = o.ad_id,
                    o.name,
                    style = o.style == 1 ? "名片" : o.style == 2 ? "图文" : "通栏",
                    o.txt,
                    o.img,
                    o.link,
                    st = o.status == 1 ? "待使用" : o.status == 2 ? "使用中" : "信用"
                }).ToList();
            r.count = cu.x_red.Count();
            r.page = page;
            return r;
        }

    }
}
