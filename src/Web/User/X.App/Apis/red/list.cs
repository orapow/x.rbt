using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web.Com;

namespace X.App.Apis.red
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
            r.items = cu.x_red
                .Skip((page - 1) * limit)
                .Take(limit)
                .OrderByDescending(o => o.ctime)
                .Select(o => new
                {
                    id = o.red_id,
                    tp = o.mode == 1 ? "普通红包" : "手气红包",
                    o.amount,
                    o.count,
                    o.geted,
                    st = o.status == 1 ? "正常" : o.status == 2 ? "完成" : "停止"
                }).ToList();
            r.count = cu.x_red.Count();
            r.page = page;
            return r;
        }

    }
}
