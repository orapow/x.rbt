using System;
using System.Collections.Generic;
using System.Linq;
using X.Web.Com;

namespace X.App.Apis.app.agent.coop
{
    public class list : xag
    {
        public int st { get; set; }
        public string key { get; set; }
        public int page { get; set; }
        public int limit { get; set; }

        protected override XResp Execute()
        {
            var r = new back();

            var q = from c in DB.x_coop
                    where c.agent_id == cag.agent_id
                    orderby c.up_time descending
                    select new
                    {
                        house = c.house,
                        c.region,
                        c.area,
                        c.type,
                        c.type_name,
                        cover = c.cover1,
                        c.lea_way,
                        c.door_no_name,
                        c.toward_name,
                        c.latitude,
                        c.longitude,
                        c.businessarea,
                        c.room,
                        c.onfloor,
                        c.room_name,
                        c.lea_room,
                        c.lea_room_name,
                        c.lea_way_name,
                        c.price,
                        more = c.more,
                        c.status,
                        c.coop_id,
                        c.time,
                        c.up_time,
                        c.up_time_name,
                        c.id
                    };

            //key:关键字
            if (!string.IsNullOrEmpty(key))
            {
                var dt = DB.x_dict.FirstOrDefault(o => o.code == "coop.dt" && o.name == key && o.upval != "0");
                if (dt != null)
                {
                    q = q.Where(o =>
                        o.house.Contains(key) ||
                        (DB.x_dict.Where(d => d.code == "code.qy" && d.name == key).Select(s => s.value)).Contains(o.businessarea + "") ||
                        DB.fnGetDistance((float)o.longitude, (float)o.latitude, (float)dt.pointx, (float)dt.pointy) < 1.5 * 1000
                        );
                }
                else
                {
                    q = q.Where(o =>
                            o.house.Contains(key) ||
                            (DB.x_dict.Where(d => d.code == "code.qy" && d.name == key).Select(s => s.value)).Contains(o.businessarea + ""));
                }
            }

            if (st > 0) q = q.Where(o => o.status == st);

            if (page > 0)
            {
                r.page = page;
                r.items = q
                    .Skip((page - 1) * limit)
                    .Take(limit)
                    .Select(o =>
                        new
                        {
                            o.house,
                            o.cover,
                            o.area,
                            o.coop_id,
                            o.id,
                            o.type_name,
                            floor_name = o.onfloor + "层",
                            price = o.price,
                            unit = "元/月",
                            o.room_name,
                            o.toward_name,
                            o.more,
                            o.lea_way_name,
                            o.lea_room_name,
                            up_time = o.up_time_name,
                            o.door_no_name,
                            o.status
                        }
                    ).ToList();
            }
            r.count = q.Count();
            r.st1 = q.Count(o => o.status == 1);
            r.st2 = q.Count(o => o.status == 2);
            r.st3 = q.Count(o => o.status == 3);

            return r;
        }

        public class back : Resp_List
        {
            public int st1 { get; set; }
            public int st2 { get; set; }
            public int st3 { get; set; }
        }
    }
}
