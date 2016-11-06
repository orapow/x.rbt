using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web.Com;

namespace X.App.Apis.mgr.mch
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

            var q = from ad in DB.x_mch
                    orderby ad.mch_id descending
                    select new
                    {
                        ad.mch_id,
                        ad.name,
                        ad.account,
                        ad.tel,
                        ad.pwd,
                        ad.logo,
                        ad.cp_name,
                        ad.cp_addr,
                        ad.cp_man,
                        ad.cp_tel,
                        ad.bank_ac_name,
                        ad.bank_account,
                        ad.bank_name,
                        ad.rate,
                        ad.remark
                    };

            if (!string.IsNullOrEmpty(key))
            {
                q = q.Where(o => o.name.Contains(key) || o.account.Contains(key));
            }

            r.items = q.Skip((page - 1) * limit).Take(limit);
            r.count = q.Count();
            return r;
        }

    }
}
