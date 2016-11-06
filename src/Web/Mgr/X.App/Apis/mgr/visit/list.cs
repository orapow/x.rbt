using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.visit
{
    public class list : xmg
    {
        public int page { get; set; }
        public int limit { get; set; }
        public string key { get; set; }

        protected override XResp Execute()
        {
            var q = from v in DB.x_visit
                    orderby v.ctime descending
                    select new
                    {
                        v.ctime,
                        v.ctime_name,
                        v.remark,
                        v.type_name,
                        v.utype_name,
                        v.name,
                        v.tel,
                        uname = v.x_user.name,
                        utel = v.x_user.tel,
                        cid = v.coop_id
                    };

            if (!string.IsNullOrEmpty(key)) q = q.Where(o => o.name.Contains(key) || o.uname.Contains(key));

            var r = new Resp_List();
            r.page = page;
            r.count = q.Count();
            r.items = q.Skip((page - 1) * limit).Take(limit).ToList();
            return r;
        }
    }
}
