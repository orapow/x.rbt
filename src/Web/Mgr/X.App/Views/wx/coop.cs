using System.Linq;

namespace X.App.Views.wx
{
    public class coop : _wx
    {
        /// <summary>
        /// 1、生活圈 2、地铁 3、附近（默认）
        /// </summary>
        public int ar { get; set; }
        //区块类型
        // 1:生活圈 t1:大区   t2:小区
        // 2:地铁   t1:地铁线 t2:地铁站
        // 3:附近
        public int t1 { get; set; }
        public int t2 { get; set; }
        public decimal lng { get; set; }
        public decimal lat { get; set; }
        /// <summary>
        /// 价格：
        /// min,max
        /// </summary>
        public string pr { get; set; }//价格
        /// <summary>
        /// 排序
        /// 0、距离 1、上架时间（取消） 2、价格顺序 3、价格倒序
        /// </summary>
        public int px { get; set; }
        /// <summary>
        /// 租凭方式
        /// 1、整租 2、合租 3、单间 4、床位
        /// </summary>
        public int lea { get; set; }
        /// <summary>
        /// 居室
        /// 1-5 1-5居
        /// 6、5居以上 
        /// 10、主卧
        /// 11、次卧
        /// 12、明隔
        /// 13、暗隔
        /// 14、阳隔
        /// 15、主卧独卫
        /// 99、大开间
        /// </summary>
        public int room { get; set; }

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
                return "ar-t1-t2-pr-px-lea-room-key-page";
            }
        }

        protected override void InitDict()
        {
            base.InitDict();
            if (page == 0) page = 1;

            var q = from c in DB.x_rent
                    where c.status == 1
                    select new
                    {
                        house = c.x_coop.house,
                        c.x_coop.subwaystation,
                        c.x_coop.subwayline,
                        c.x_coop.region,
                        c.x_coop.area,
                        c.x_coop.type,
                        c.x_coop.type_name,
                        cover = c.x_coop.cover1,
                        c.x_coop.lea_way,
                        c.x_coop.door_no_name,
                        c.x_coop.toward_name,
                        c.x_coop.latitude,
                        c.x_coop.longitude,
                        c.x_coop.businessarea,
                        c.x_coop.room,
                        c.x_coop.onfloor,
                        c.x_coop.room_name,
                        c.x_coop.lea_room,
                        c.x_coop.lea_room_name,
                        c.x_coop.lea_way_name,
                        dist = lng > 0 && lat > 0 ? ((decimal)DB.fnGetDistance((float)lng, (float)lat, (float)c.x_coop.longitude, (float)c.x_coop.latitude) / 1000) : -1,
                        c.x_coop.price,
                        c.x_coop.more,
                        agtel = c.x_coop.x_agent.tel,
                        agid = c.x_coop.agent_id,
                        c.bk_amount,
                        c.coop_id,
                        c.x_coop.time,
                        c.z_time,
                        c.z_time_name,
                        c.id,
                        fx_name = c.x_coop.fx_name,
                        c.x_coop.x_coop_dt
                    };

            q = q.Where(o => !new int?[] { 30, 31, 32 }.Contains(o.agid));

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

            //lea:租凭方式
            if (lea > 0) q = q.Where(o => o.lea_way == lea);

            //room:居室
            if (room > 0)
            {
                if (room == 4) q = q.Where(o => o.room > 3 && o.room < 10 && o.lea_way == 1);
                else if (room == 99) q = q.Where(o => o.room == room);
                else if (room < 10) q = q.Where(o => o.room == room);
                else q = q.Where(o => o.lea_room == room - 9);
            }

            //pr:价格
            if (!string.IsNullOrEmpty(pr))
            {
                var p = pr.Split(',');
                if (p.Length == 2 && !string.IsNullOrEmpty(p[0]) && !string.IsNullOrEmpty(p[1]))
                {
                    if (!string.IsNullOrEmpty(p[0]) && !string.IsNullOrEmpty(p[1])) q = q.Where(o => o.price >= decimal.Parse(p[0]) && o.price <= decimal.Parse(p[1]));
                    else if (string.IsNullOrEmpty(p[0])) q = q.Where(o => o.price <= decimal.Parse(p[1]));
                    else q = q.Where(o => o.price >= decimal.Parse(p[0]));
                }
            }

            //if (ar == 0) ar = 3;
            switch (ar)
            {
                case 1:
                    if (t2 > 0) q = q.Where(o => o.businessarea == t2);
                    else if (t1 > 0) q = q.Where(o => o.region == t1);
                    break;
                case 2:
                    if (t2 > 0) q = q.Where(o => o.x_coop_dt.Select(d => d.name_id).Contains(t2));
                    else if (t1 > 0) q = q.Where(o => o.x_coop_dt.Select(d => d.line_id).Contains(t1));
                    break;
                case 3:
                    if (lng > 0 && lat > 0) q = q.Where(o => o.dist < (t1 == 0 ? 5 : t1));
                    break;
                default:
                    break;
            }

            if (px == 2) q = q.OrderBy(o => o.price);
            else if (px == 3) q = q.OrderByDescending(o => o.price);
            else q = q.OrderBy(o => o.dist < 2000 ? 0 : o.dist).ThenByDescending(o => o.z_time);

            var coop_list = q.Skip((page - 1) * 10)
                .Take(10)
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
                        o.dist,
                        o.room_name,
                        o.toward_name,
                        o.more,
                        o.lea_way_name,
                        o.lea_room_name,
                        up_time = o.z_time_name,
                        o.door_no_name,
                        o.agtel
                    }
                ).ToList();

            dict.Add("list", coop_list);
            dict.Add("count", q.Count());
        }
    }
}
