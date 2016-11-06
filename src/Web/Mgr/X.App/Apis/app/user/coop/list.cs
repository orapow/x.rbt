using System;
using System.Collections.Generic;
using System.Linq;
using X.Web.Com;

namespace X.App.Apis.app.user.coop
{
    public class list : xu
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
        /// <summary>
        /// 距离
        /// ar=3 时有效
        /// </summary>
        public int dist { get; set; }
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

        /// <summary>
        /// 房屋类型
        /// </summary>
        public int type { get; set; }

        /// <summary>
        /// 二房东
        /// </summary>
        public int agid { get; set; }

        public int page { get; set; }
        public int limit { get; set; }

        /// <summary>
        /// 搜索关键字
        /// 楼盘、商圈、地铁站、居室
        /// </summary>
        public string key { get; set; }

        protected override XResp Execute()
        {
            var r = new Resp_List();

            var q = from c in DB.x_coop
                    where c.status == 1
                    select new
                    {
                        house = c.house,
                        c.subwaystation,
                        c.subwayline,
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
                        dist = lng > 0 && lat > 0 ? ((decimal)DB.fnGetDistance((float)lng, (float)lat, (float)c.longitude, (float)c.latitude) / 1000) : -1,
                        c.price,
                        c.more,
                        agname = (us != null && us.level == 10) ? c.x_agent.name : "",
                        agtel = c.x_agent.tel,
                        agid = c.agent_id,
                        c.coop_id,
                        c.time,
                        c.up_time,
                        c.up_time_name,
                        c.id,
                        zz = c.x_agent.c_zz,
                        hz = c.x_agent.c_hz
                    };

            if (us == null || !new int?[] { 7595, 7596, 7603 }.Contains(us.user_id)) q = q.Where(o => !new int?[] { 30, 31, 32 }.Contains(o.agid));

            //key:关键字
            if (!string.IsNullOrEmpty(key))
            {
                var dt = DB.x_dict.FirstOrDefault(o => o.code == "coop.dt" && o.name == key && o.upval != "0");
                if (dt != null)
                {
                    q = q.Where(o =>
                        o.house.Contains(key) ||
                        o.agname.Equals(key) ||
                        (DB.x_dict.Where(d => d.code == "code.qy" && d.name == key).Select(s => s.value)).Contains(o.businessarea + "") ||
                        DB.fnGetDistance((float)o.longitude, (float)o.latitude, (float)dt.pointx, (float)dt.pointy) < 1.5 * 1000
                        );
                }
                else
                {
                    q = q.Where(o =>
                            o.house.Contains(key) ||
                            o.agname.Equals(key) ||
                            (DB.x_dict.Where(d => d.code == "code.qy" && d.name == key).Select(s => s.value)).Contains(o.businessarea + ""));
                }
            }

            //lea:租凭方式
            if (lea > 0) q = q.Where(o => o.lea_way == lea);

            if (agid > 0) q = q.Where(o => o.agid == agid);

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
                var p = pr.Split('-');
                if (p.Length == 2)
                {
                    if (!string.IsNullOrEmpty(p[0]) && !string.IsNullOrEmpty(p[1])) q = q.Where(o => o.price >= decimal.Parse(p[0]) && o.price <= decimal.Parse(p[1]));
                    else if (string.IsNullOrEmpty(p[0])) q = q.Where(o => o.price <= decimal.Parse(p[1]));
                    else q = q.Where(o => o.price >= decimal.Parse(p[0]));
                }
            }

            if (type > 0) q = q.Where(o => o.type == type);

            switch (ar)
            {
                case 1:
                    if (t2 > 0) q = q.Where(o => o.businessarea == t2);
                    else if (t1 > 0) q = q.Where(o => o.region == t1);
                    break;
                case 2:
                    if (t2 > 0)
                    {
                        var dt = DB.x_dict.FirstOrDefault(o => o.code == "coop.dt" && o.value == t2 + "");
                        if (dt != null && dt.pointx > 0 && dt.pointy > 0) q = q.Where(o => DB.fnGetDistance((float)o.longitude, (float)o.latitude, (float)dt.pointx, (float)dt.pointy) < 2000);
                    }
                    break;
                case 3:
                    if (lng > 0 && lat > 0) q = q.Where(o => o.dist < (dist == 0 ? 5 : dist / 1000));
                    break;
                default:
                    break;
            }

            if (px == 2) q = q.OrderBy(o => o.price);
            else if (px == 3) q = q.OrderByDescending(o => o.price);
            else if (px == 4) q = q.OrderBy(o => o.dist);
            else q = q.OrderBy(o => o.dist >= 0 && o.dist <= 2 ? 0 : o.dist).ThenByDescending(o => o.up_time);

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
                        o.dist,
                        o.room_name,
                        o.toward_name,
                        o.more,
                        o.lea_way_name,
                        o.lea_room_name,
                        up_time = o.up_time_name,
                        o.agname,
                        o.door_no_name,
                        o.agtel,
                        o.hz,
                        o.zz
                    }
                ).ToList();
            r.count = q.Count();

            return r;
        }
    }
}
