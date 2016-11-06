using System;
using System.Collections.Generic;
using System.Linq;
using X.Web.Com;

namespace X.App.Apis.app.agent.coop
{
    public class list : xag
    {
        public int page { get; set; }
        public int limit { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int st { get; set; }
        public string key { get; set; }

        protected override XResp Execute()
        {
            var r = new back();
            r.page = page;

            var q = from coop in DB.x_coop
                    where coop.agent_id == cag.id
                    orderby coop.time descending
                    select new
                    {
                        coop.id,
                        coop.agent_id,
                        coop.x_agent.name,
                        coop.x_agent.contract,
                        coop.x_agent.tel,
                        coop.house,//楼盘
                        coop.room_name,//室
                        coop.door_no_name,
                        coop.intime_name,
                        coop.floor,
                        coop.onfloor,
                        coop.floor_name,
                        etime = coop.intime,
                        coop.lea_room_name,
                        more = coop.more.Substring(0, 20),
                        coop.cover,//封面
                        coop.area,
                        coop.pay_way,//付款方式
                        coop.pay_way_name,
                        coop.lea_way,//出租方式（整租|合租）
                        coop.lea_way_name,
                        coop.price,//价格
                        coop.unit,//单位
                        coop.status,//状态
                        coop.status_name,
                        coop.businessarea_name,
                        coop.region_name
                    };

            if (st > 0) q = q.Where(o => o.status == st);
            if (!string.IsNullOrEmpty(key)) q = q.Where(o => o.house.Contains(key));

            if (st == 2) q = q.OrderBy(o => o.etime);

            r.items = q.Skip((page - 1) * limit).Take(limit);
            r.count = q.Count();
            r.st1 = q.Count(o => o.status == 1);
            r.st2 = q.Count(o => o.status == 2);
            r.st3 = q.Count(o => o.status == 3);

            return r;
        }

        public class back : Resp_List
        {
            public int st1 { get; set; }
            public int st2 { get; set; }
            public int st3 { get; set; }
        }
    }
}
