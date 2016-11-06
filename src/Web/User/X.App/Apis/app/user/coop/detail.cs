using System;
using System.Collections.Generic;
using System.Linq;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.app.user.coop
{
    public class detail : xu
    {
        public int coopid { get; set; }

        protected override void Validate()
        {
            base.Validate();
            Validator.CheckRange("coopid", coopid, -1, null);
        }

        protected override XResp Execute()
        {
            var r = new Resp_List();
            var q = from c in DB.x_coop
                    where c.coop_id == coopid
                    select new
                    {
                        agname = c.x_agent.name,
                        agtel = c.x_agent.tel,
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
                        c.more,
                        c.intime_name,
                        c.door_no,
                        c.door_no_name,
                        c.longitude,
                        c.latitude,
                        status_name = c.status_name1,
                        dist = lng > 0 && lat > 0 ? ((decimal)DB.fnGetDistance((float)lng, (float)lat, (float)c.longitude, (float)c.latitude) / 1000) : -1,
                        rentid = getrid(),
                        iscollect = getiscollect(),
                        yue_id = getyueid(),
                        zz = c.x_agent.c_zz,
                        hz = c.x_agent.c_hz
                    };

            var cp = q.SingleOrDefault();
            if (cp == null) throw new XExcep("T房源不存在");

            r.items = cp;

            return r;
        }

        x_rent ret = null;

        bool getiscollect()
        {
            return (us == null) ? false : DB.x_collect.Count(o => o.coop_id == coopid && o.user_id == us.id) > 0;
        }

        int getyueid()
        {
            if (us == null) return 0;
            var rid = getrid();// if (ret == null) ret = DB.x_rent.Where(o => o.coop_id == coopid).OrderByDescending(o => o.rent_id).FirstOrDefault();
            if (rid == 0) return 0;
            var y = ret.x_reserve.FirstOrDefault(o => o.user_id == us.id && o.rent_id == rid);
            if (y == null) return 0;
            return y.id;
        }

        int getrid()
        {
            if (ret == null) ret = DB.x_rent.Where(o => o.coop_id == coopid).OrderByDescending(o => o.rent_id).FirstOrDefault();
            if (ret == null) return 0;
            return ret.id;
        }
    }
}
