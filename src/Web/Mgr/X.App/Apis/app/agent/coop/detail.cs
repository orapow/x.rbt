using System;
using System.Collections.Generic;
using System.Linq;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.app.agent.coop
{
    public class detail : xag
    {
        public int coopid { get; set; }

        protected override void Validate()
        {
            base.Validate();
            Validator.CheckRange("coopid", coopid, 0, null);
        }

        protected override XResp Execute()
        {
            var q = from c in DB.x_coop
                    where c.coop_id == coopid
                    select new
                    {
                        c.agent_id,
                        c.images,
                        c.house,
                        c.room_name,
                        c.area,
                        c.pay_way_name,
                        c.lea_way,
                        c.lea_way_name,
                        c.lea_room_name,
                        c.price,
                        unit = "元/月",
                        c.ms,
                        c.cfgs,
                        c.fys,
                        c.type_name,
                        c.toward_name,
                        floor_name = c.onfloor + "层",
                        c.decorate_name,
                        c.build_age,
                        more = c.more,
                        c.intime_name,
                        c.door_no,
                        c.door_no_name,
                        c.longitude,
                        c.latitude,
                        status_name = c.status_name1,
                        dist = lng > 0 && lat > 0 ? ((decimal)DB.fnGetDistance((float)lng, (float)lat, (float)c.longitude, (float)c.latitude) / 1000) : -1
                    };

            if (q.Count() == 0) throw new XExcep("x0005");

            var coop = q.FirstOrDefault();

            if (coop.agent_id != cag.id) throw new XExcep("T房源不属于你");

            var r = new Resp_List();
            r.items = coop;

            return r;
        }
    }
}
