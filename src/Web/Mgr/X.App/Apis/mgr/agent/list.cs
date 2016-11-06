using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web.Com;

namespace X.App.Apis.mgr.agent
{
    public class list : xmg
    {
        public int page { get; set; }
        public int limit { get; set; }
        public string key { get; set; }

        protected override Web.Com.XResp Execute()
        {
            var r = new Resp_List();
            r.page = page;

            var q = from ag in DB.x_agent
                    select new
                    {
                        ag.id,
                        ag.name,
                        ag.tel,
                        zu = ag.x_coop.Count(o => o.status == 1),
                        ag.addr,
                        ag.logo,
                        ag.contract,
                        cu1 = ag.x_coop.Count(o => o.status == 1),
                        cu2 = ag.x_coop.Count(o => o.status == 2),
                        cu3 = ag.x_coop.Count(o => o.status == 3),
                        statusname = GetDictName("agent.st", ag.status),
                        yc = ag.x_reserve.Count(o => o.status == 1)
                    };

            q = q.OrderByDescending(o => o.yc);

            if (!string.IsNullOrEmpty(key))
            {
                q = q.Where(o => o.tel.Contains(key) || o.name.Contains(key) || o.contract.Contains(key) || o.addr.Contains(key));
            }

            r.items = q.Skip((page - 1) * limit).Take(limit);
            r.count = q.Count();
            return r;
        }

    }
}
