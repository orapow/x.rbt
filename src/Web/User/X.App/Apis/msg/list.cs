using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web.Com;

namespace X.App.Apis.msg
{
    public class list : xapi
    {
        public int page { get; set; }
        public int limit { get; set; }
        public int tp { get; set; }

        protected override XResp Execute()
        {
            if (page == 0) page = 1;
            if (limit == 0) limit = 30;

            var q = from m in cu.x_msg select m;

            var r = new Resp_List();
            r.page = page;
            r.count = cu.x_msg.Count();
            r.items = cu.x_msg
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(o => new
                {
                    id = o.msg_id,
                    o.type,
                    o.content,
                    o.ucount,
                    next = o.next_time.Value.ToString("yyyy-MM-dd HH:mm"),
                    wayname = getway(o)
                });
            return r;
        }
        string getway(x_msg m)
        {
            var sp = "即时发送|定时消息|间隔消息".Split('|');
            if (m.way != null && m.way > 0 && m.way <= sp.Length) return sp[m.way.Value - 1];
            else return "未知";
        }
    }
}
