using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web.Com;

namespace X.App.Apis.mgr.coop
{
    public class toshare : xmg
    {
        public string key { get; set; }

        protected override Web.Com.XResp Execute()
        {
            var r = new Resp_List();
            r.page = 1;

            var q = from coop in DB.x_coop
                    where coop.status == 1
                    select new
                    {
                        agid = coop.agent_id,
                        coop.x_agent.name,
                        coop.x_agent.contract,
                        coop.x_agent.tel,
                        coop.longitude,
                        coop.latitude,
                        coop.house,//楼盘
                        coop.room_name,//室
                        coop.door_no,
                        coop.door_no_name1,
                        coop.lea_room_name,
                        more = coop.more.Substring(0, 20),
                        coop.area,
                        coop.status,
                        coop.lea_way,//出租方式（整租|合租）
                        coop.lea_way_name,
                        coop.price,//价格
                        coop.businessarea_name
                    };

            if (!string.IsNullOrEmpty(key))
            {
                var dt = DB.x_dict.FirstOrDefault(o => o.code == "coop.dt" && o.name == key && o.upval != "0");
                if (dt == null) q = q.Where(o => o.house.Contains(key) || o.name == key || o.contract == key);
                else q = q.Where(o => DB.fnGetDistance((float)o.longitude, (float)o.latitude, (float)dt.pointx, (float)dt.pointy) < 2 * 1000);
            }

            q = q.Where(o => !new int?[] { 30, 31, 32 }.Contains(o.agid));
            q = q.OrderBy(o => o.contract).ThenBy(o => o.name).ThenBy(o => o.house).ThenBy(o => o.door_no);

            r.items = q.Select(c => new
                {
                    c.name,
                    c.contract,
                    c.tel,
                    c.house,//楼盘
                    c.room_name,//室
                    c.status,
                    door_no_name = c.door_no_name1,
                    c.lea_way,
                    c.lea_room_name,
                    more = c.more.Substring(0, 20),
                    c.area,
                    c.lea_way_name,
                    c.price,//价格
                    c.businessarea_name
                }).ToList();

            r.count = q.Count();
            return r;
        }
    }
}
