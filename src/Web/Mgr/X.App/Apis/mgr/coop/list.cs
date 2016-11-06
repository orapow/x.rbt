using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web.Com;

namespace X.App.Apis.mgr.coop
{
    public class list : xmg
    {
        public int page { get; set; }
        public int limit { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int st { get; set; }
        public int agid { get; set; }
        public string key { get; set; }

        protected override Web.Com.XResp Execute()
        {
            var r = new Resp_List();
            r.page = page;

            var q = from c in DB.x_coop
                    orderby c.time descending
                    select new
                    {
                        c.id,
                        c.agent_id,
                        c.x_agent.name,
                        c.x_agent.contract,
                        c.x_agent.tel,
                        c.house,//楼盘
                        c.room_name,//室
                        c.door_no_name,
                        c.intime_name,
                        up_time = c.up_time.Substring(0, 10),
                        d_time = c.x_rent.OrderByDescending(o => o.rent_id).FirstOrDefault().d_time.Substring(0, 10),
                        c.down_time,
                        c.floor,
                        c.onfloor,
                        c.floor_name,
                        etime = c.intime,
                        c.lea_room_name,
                        more = c.more.Substring(0, 20),
                        cover = c.cover1,//封面
                        c.area,
                        c.pay_way,//付款方式
                        c.pay_way_name,
                        c.lea_way,//出租方式（整租|合租）
                        c.lea_way_name,
                        c.price,//价格
                        c.unit,//单位
                        c.status,//状态
                        c.status_name,
                        c.businessarea_name,
                        c.region_name,
                        fx_name = (c.lea_way == 1 && c.x_agent.c_zz > 0) ? "成交返现" + c.x_agent.c_zz + "元" : (c.lea_way == 1 && c.x_agent.c_zz > 0) ? "成交返现" + c.x_agent.c_hz + "元" : ""
                    };

            if (agid > 0) q = q.Where(o => o.agent_id == agid);
            if (st > 0) q = q.Where(o => o.status == st);
            if (!string.IsNullOrEmpty(key)) q = q.Where(o => o.house.Contains(key) || o.name.Contains(key) || o.contract.Contains(key));

            if (st == 2) q = q.OrderBy(o => o.etime);

            r.items = q.Skip((page - 1) * limit).Take(limit);
            r.count = q.Count();
            return r;
        }
    }
}
