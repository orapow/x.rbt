using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.user
{
    /// <summary>
    /// 用户管理列表
    /// </summary>
    public class list : xmg
    {
        public int range { get; set; }//区域
        public int region { get; set; }//商圈
        public int page { get; set; }
        public int limit { get; set; }
        public string key { get; set; }

        protected override Web.Com.XResp Execute()
        {
            var r = new Resp_List();
            r.page = page;

            var q = from u in DB.x_user
                    orderby u.reg_time descending
                    select new
                    {
                        u.name,
                        u.id,
                        u.tel,
                        u.region,
                        u.region_name,
                        u.range,
                        u.range_name,
                        u.integral,
                        u.level,
                        u.balance,
                        u.reg_time,
                        u.auth_status,
                        u.auth_status_name
                    };

            if (!string.IsNullOrEmpty(key)) q = q.Where(o => o.name.Contains(key) || o.tel.Contains(key));
            if (range > 0) q = q.Where(o => o.range == range);
            if (region > 0) q = q.Where(o => o.region == region);

            r.items = q.Skip((page - 1) * limit).Take(limit).ToList();
            r.count = q.Count();

            return r;
        }

    }
}
