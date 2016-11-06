using System.Linq;
using X.Web;

namespace X.App.Views.wx
{
    public class detail : _wx
    {
        public int coopid { get; set; }

        protected override void Validate()
        {
            base.Validate();
            Validator.CheckRange("coopid", coopid, -1, null);
        }

        protected override string GetParmNames
        {
            get
            {
                return "coopid";
            }
        }

        protected override void InitDict()
        {
            base.InitDict();

            var q = from c in DB.x_coop
                    where c.coop_id == coopid
                    select new
                    {
                        agname = c.x_agent.name,
                        agtel = c.x_agent.tel,
                        images = c.images.Split(','),
                        c.house,
                        c.room_name,
                        c.area,
                        cover = c.cover1,
                        c.pay_way_name,
                        c.lea_way,
                        c.lea_way_name,
                        c.lea_room_name,
                        c.price,
                        unit = "元/月",
                        ms = c.ms.Split(','),
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
                        c.x_coop_dt
                    };

            var cp = q.SingleOrDefault();
            if (cp == null) throw new XExcep("T房源不存在");

            //var sts = from dt in DB.x_dict
            //          where dt.code == "coop.dt" && dt.upval != "0"
            //          select new
            //          {
            //              id = dt.value,
            //              st_name = dt.name,
            //              line_name = GetDictName("coop.dt", dt.upval),
            //              dist = (dt.pointx == null || dt.pointy == null) ? null : DB.fnGetDistance((float)cp.longitude, (float)cp.latitude, (float)dt.pointx, (float)dt.pointy) / 1000
            //          };

            //sts = sts.Where(o => o.dist != null && o.dist <= 1.5).OrderBy(o => o.dist);

            dict.Add("stas", cp.x_coop_dt);

            dict.Add("cp", cp);

            if (cp.lea_way == 2)
            {
                var ros = from c in DB.x_rent
                          where c.x_coop.door_no == cp.door_no && c.x_coop.house == cp.house && c.coop_id != coopid && c.status == 1
                          select c.x_coop;

                var rooms = ros.ToList().Select(c => new
                {
                    cpid = c.coop_id,
                    title = c.house + " " + c.door_no_name + " " + c.lea_room_name,
                    price = c.price,
                    unit = "元/月"
                });

                dict.Add("rooms", rooms.ToList());
            }
        }
    }
}
