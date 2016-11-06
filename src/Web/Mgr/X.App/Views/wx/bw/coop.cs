using System.Linq;

namespace X.App.Views.wx.bw
{
    public class coop : _us
    {
        public int tp { get; set; }

        public int page { get; set; }

        /// <summary>
        /// 搜索关键字
        /// 楼盘、商圈、地铁站、居室
        /// </summary>
        public string key { get; set; }

        protected override string GetParmNames
        {
            get
            {
                return "tp-key-page";
            }
        }

        protected override void InitDict()
        {
            base.InitDict();
            if (page == 0) page = 1;

            var q = from c in DB.x_ocoop
                    orderby c.ctime descending
                    select new
                    {
                        sort = c.city == cu.city ? 1 : 0,
                        c.city,
                        id = c.ocoop_id,
                        title = c.title1,
                        desc = c.desc1,
                        calltimes = c.x_tel_log.Count(o => o.user_id == cu.user_id),
                        c.price,
                        c.house,
                        c.region,
                        c.area,
                        c.lng,
                        c.lat,
                        c.hits
                    };

            //key:关键字
            if (!string.IsNullOrEmpty(key))
            {
                var dt = DB.x_dict.FirstOrDefault(o => o.code == "coop.dt" && o.name == key && o.upval != "0");
                if (dt != null)
                {
                    q = q.Where(o =>
                        o.house.Contains(key) ||
                        (DB.x_dict.Where(d => d.code == "code.qy" && d.name == key).Select(s => s.value)).Contains(o.region + "") ||
                        DB.fnGetDistance((float)o.lng, (float)o.lat, (float)dt.pointx, (float)dt.pointy) < 1.5 * 1000
                        );
                }
                else
                {
                    q = q.Where(o => o.house.Contains(key) || o.region.Contains(key));
                }
            }

            dict.Add("list", q.OrderByDescending(o => o.sort).Skip((page - 1) * 10).Take(10));
            dict.Add("count", q.Count());
        }
    }
}
