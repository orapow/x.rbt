using System;
using System.Collections.Generic;
using System.Linq;
using X.Web.Com;

namespace X.App.Apis.app.agent.house
{
    public class list : xag
    {
        public int top { get; set; }
        public string key { get; set; }

        protected override XResp Execute()
        {
            var hq = from h in DB.x_houses
                     where h.state == 2
                     select new
                     {
                         h.jianpin,
                         h.quanpin,
                         h.address,
                         h.region,
                         h.build_age,
                         h.region_name,
                         h.businessarea,
                         h.businessarea_name,
                         h.name,
                         h.id,
                         h.longitude,
                         h.latitude
                     };

            if (!string.IsNullOrEmpty(key)) hq = hq.Where(o => o.name.Contains(key) || o.jianpin.StartsWith(key) || o.quanpin.StartsWith(key));

            if (top == 0) { top = 10; }

            var newhq = hq.Select(h => new
            {
                h.region,
                h.region_name,
                h.address,
                h.build_age,
                h.businessarea,
                h.businessarea_name,
                h.name,
                h.id,
                point = h.longitude > 0 && h.latitude > 0 ? h.longitude + "," + h.latitude : ""
            });

            var r = new Resp_List();
            r.page = 1;
            r.count = newhq.Count();
            r.items = newhq.Take(top).ToList();
            return r;
        }
    }
}
