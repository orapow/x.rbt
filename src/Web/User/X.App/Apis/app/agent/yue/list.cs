using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.app.agent.yue
{
    public class list : xag
    {
        public int page { get; set; }
        public int limit { get; set; }
        /// <summary>
        /// 1、未读
        /// 2、已读
        /// 3、过期
        /// </summary>
        public int st { get; set; }

        protected override bool need_user
        {
            get
            {
                return true;
            }
        }

        protected override XResp Execute()
        {
            var q = from c in DB.x_reserve
                    orderby c.status, c.time descending
                    where c.agent_id == cag.agent_id
                    select new
                    {
                        yid = c.id,
                        c.x_user.name,
                        c.x_user.tel,
                        c.x_user.image,
                        c.x_rent.coop_id,
                        c.x_rent.x_coop.house,
                        c.x_rent.x_coop.door_no_name,
                        c.x_rent.x_coop.lea_way_name,
                        c.x_rent.x_coop.lea_room_name,
                        c.x_rent.z_price,
                        unit = "元/月",
                        reserve_date = c.reserve_date_name,
                        c.reserve_time,
                        c.status,
                        c.status_name
                    };

            if (st > 0) q = q.Where(o => o.status == st);

            if (limit == 0 || page == 0) { limit = 20; page = 1; }

            var r = new Resp_List();
            r.page = page;
            r.count = q.Count();
            r.items = q.Skip((page - 1) * limit).Take(limit).ToList();
            return r;
        }
    }
}
