using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web.Com;

namespace X.App.Apis.reply
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

            var q = from m in cu.x_reply select m;
            if (tp > 0) q = q.Where(o => o.tp == tp);

            var r = new Resp_List();
            r.page = page;
            r.count = q.Count();
            r.items = q
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(o => new
                {
                    id = o.reply_id,
                    o.type,
                    o.tp,
                    o.content,
                    o.keys,
                    match = getmatch(o.match)
                });
            return r;
        }
        string getmatch(int? mt)
        {
            var sp = "全字|开头|结尾|包含".Split('|');
            if (mt != null && mt > 0 && mt <= sp.Length) return sp[mt.Value - 1];
            else return "未知";
        }
    }
}
