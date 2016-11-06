using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.app.user.yue
{
    public class list : xu
    {
        public int page { get; set; }
        public int limit { get; set; }

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
                    orderby c.time descending
                    where c.user_id == us.id
                    select new
                    {
                        yid = c.id,
                        c.x_rent.x_coop.area,
                        c.x_rent.x_coop.house,
                        cover = c.x_rent.x_coop.cover1,
                        c.x_rent.x_coop.coop_id,
                        c.x_rent.x_coop.id,
                        c.x_rent.x_coop.type_name,
                        c.x_rent.x_coop.more,
                        c.x_rent.x_coop.door_no_name,
                        floor_name = c.x_rent.x_coop.onfloor + "层",
                        price = c.x_rent.x_coop.price,
                        unit = "元/月",
                        c.x_rent.x_coop.room_name,
                        c.x_rent.x_coop.lea_way_name,
                        c.x_rent.x_coop.lea_room_name,
                        up_time = c.x_rent.z_time_name,
                        c.x_rent.x_coop.toward_name,
                        c.reserve_time,
                        status_name = c.x_rent.x_coop.status_name1,
                        dist = lng > 0 && lat > 0 ? ((decimal)DB.fnGetDistance((float)lng, (float)lat, (float)c.x_rent.x_coop.longitude, (float)c.x_rent.x_coop.latitude) / 1000) : -1,
                        c.reserve_date_name,
                        agname = (us != null && us.level == 10) ? c.x_agent.name : "",
                        agtel = c.x_agent.tel,
                        zz = c.x_agent.c_zz,
                        hz = c.x_agent.c_hz
                    };

            var r = new Resp_List();
            r.page = page;
            r.count = q.Count();
            r.items = q.Skip((page - 1) * limit).Take(limit).ToList();
            return r;
        }
    }
}
