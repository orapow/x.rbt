using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web.Com;

namespace X.App.Apis.mgr.admin
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

            var q = from ad in DB.x_admin
                    orderby ad.time descending
                    select new
                    {
                        ad.id,
                        ad.uid,
                        ad.name,
                        ad.tel,
                        ad.status,
                        ad.status_name,
                        ad.pwd
                    };

            if (!string.IsNullOrEmpty(key))
            {
                q = q.Where(o => o.tel.Contains(key) || o.name.Contains(key) || o.uid.Contains(key));
            }

            r.items = q.Skip((page - 1) * limit).Take(limit);
            r.count = q.Count();
            return r;
        }

    }
}
