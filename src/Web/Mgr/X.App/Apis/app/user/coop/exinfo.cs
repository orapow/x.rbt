using System;
using System.Collections.Generic;
using System.Linq;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.app.user.coop
{
    public class exinfo : xu
    {
        public int coopid { get; set; }

        protected override void Validate()
        {
            base.Validate();
            Validator.CheckRange("coopid", coopid, -1, null);
        }

        protected override XResp Execute()
        {
            var r = new item();
            var cp = DB.x_coop.SingleOrDefault(o => o.coop_id == coopid);
            var sts = from dt in DB.x_dict
                      where dt.code == "coop.dt" && dt.upval != "0"
                      select new
                      {
                          id = dt.value,
                          st_name = dt.name,
                          line_name = GetDictName("coop.dt", dt.upval),
                          dist = (dt.pointx == null || dt.pointy == null) ? null : DB.fnGetDistance((float)cp.longitude, (float)cp.latitude, (float)dt.pointx, (float)dt.pointy)
                      };

            sts = sts.Where(o => o.dist != null && o.dist <= 1500).OrderBy(o => o.dist);

            r.stations = sts
                .ToList()
                .Select(d => new
                {
                    st_id = d.id,
                    desc = "距离 地铁" + d.line_name + " " + d.st_name + " " + ((decimal)d.dist / 1000).ToString("F2") + "km"
                });


            if (cp.lea_way != 2 || string.IsNullOrEmpty(cp.door_no)) return r;

            var ros = from c in DB.x_rent
                      where c.x_coop.door_no == cp.door_no && c.x_coop.house == cp.house && c.coop_id != coopid && c.status == 1
                      select c.x_coop;

            r.rooms = ros.ToList().Select(c => new
                    {
                        cpid = c.coop_id,
                        title = c.house + " " + c.door_no_name + " " + c.lea_room_name,
                        price = c.price,
                        unit = "元/月"
                    });

            return r;
        }

        public class item : XResp
        {
            public object stations { get; set; }
            public object rooms { get; set; }
        }
    }
}
