using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.yue
{
    public class list : xmg
    {
        public int page { get; set; }
        public int limit { get; set; }
        public int agid { get; set; }

        protected override XResp Execute()
        {
            var q = from c in DB.x_reserve
                    orderby c.status, c.time descending
                    select new
                    {
                        yid = c.id,

                        c.x_rent.x_coop.coop_id,
                        c.x_rent.x_coop.house,
                        c.x_rent.x_coop.door_no_name,
                        c.x_rent.x_coop.lea_way_name,
                        c.x_rent.x_coop.lea_room_name,
                        c.x_rent.x_coop.region_name,
                        c.x_rent.x_coop.businessarea_name,
                        c.x_rent.x_coop.room_name,
                        c.x_rent.x_coop.area,
                        cover = c.x_rent.x_coop.cover1,
                        floor_name = c.x_rent.x_coop.onfloor + "层",
                        intime = c.x_rent.x_coop.intime,
                        c.x_rent.x_coop.more,
                        price = c.x_rent.x_coop.price,
                        unit = "元/月",

                        uname = c.x_user.name,
                        utel = c.x_user.tel,

                        c.reserve_date_name,
                        c.reserve_time,
                        c.status_name,
                        c.status,

                        agid = c.agent_id,
                        agname = c.x_agent.name,
                        agman = c.x_agent.contract,
                        agtel = c.x_agent.tel
                    };

            if (agid > 0) q = q.Where(o => o.agid == agid);

            var r = new Resp_List();
            r.page = page;
            r.count = q.Count();
            r.items = q.Skip((page - 1) * limit).Take(limit).ToList();
            return r;
        }
    }
}
