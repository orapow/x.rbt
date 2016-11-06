using System;
using System.Collections.Generic;
using System.Linq;
using X.Web;

namespace X.App.Views
{
    public class detail : xview
    {
        public int id { get; set; }
        protected override string GetParmNames
        {
            get
            {
                return "id";
            }
        }
        protected override int html_time
        {
            get
            {
                return 60 * 2;
            }
        }
        protected override void Validate()
        {
            base.Validate();
            Validator.CheckRange("id", id, 1, null);
        }

        protected override void InitDict()
        {
            base.InitDict();
            var coop = DB.x_coop.SingleOrDefault(o => o.coop_id == id);
            if (coop == null) throw new XExcep("T房源不存在");
            dict.Add("c", coop);

            //var q = from d in DB.x_dict
            //        where d.code == "coop.dt" && d.upval != "0" && DB.fnGetDistance((float?)d.pointx, (float?)d.pointy, (float?)coop.longitude, (float?)coop.latitude) < 1.5 * 1000
            //        select new
            //        {
            //            dist = (decimal)DB.fnGetDistance((float?)d.pointx, (float?)d.pointy, (float?)coop.longitude, (float?)coop.latitude) / 1000,
            //            line_name = GetDictName("coop.dt", d.upval),
            //            st_name = d.name
            //        };

            dict.Add("dts", coop.x_coop_dt.OrderBy(o => o.dist));

            var qi = from c in DB.x_coop
                     where c.coop_id != coop.id
                     select new
                     {
                         dist = (decimal)DB.fnGetDistance((float?)c.longitude, (float?)c.latitude, (float?)coop.longitude, (float?)coop.latitude),
                         c.house,
                         c.agent_id,
                         id = c.coop_id,
                         c.room_name,
                         c.businessarea_name,
                         c.lea_room_name,
                         c.door_no_name,
                         c.area,
                         cover = c.cover1,
                         c.lea_way_name,
                         c.price
                     };

            qi = qi.Where(o => o.dist < 5000 || o.price > coop.price - 200 || o.price < coop.price + 200 || o.house == coop.house);
            qi = qi.OrderBy(o => o.dist).ThenBy(o => o.price);

            dict.Add("istc", qi.Where(o => !new int?[] { 30, 31, 32 }.Contains(o.agent_id)).Take(4).ToList());

            if (!string.IsNullOrEmpty(coop.images)) dict.Add("imgs", coop.images.Split(','));

        }
    }
}
