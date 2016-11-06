using System.Linq;
using X.Web.Com;

namespace X.App.Apis.mgr.sms
{
    public class list : xmg
    {
        public int page { get; set; }
        public int limit { get; set; }
        public string key { get; set; }

        protected override XResp Execute()
        {
            var r = new Resp_List();
            r.page = page;

            var q = from sm in DB.x_sms
                    orderby sm.ctime descending
                    select new
                    {
                        sm.content,
                        sm.ctime,
                        sm.result,
                        sm.tel
                    };

            if (!string.IsNullOrEmpty(key)) q = q.Where(o => o.tel.Contains(key));

            r.items = q.Skip((page - 1) * limit).Take(limit);
            r.count = q.Count();
            return r;
        }

    }
}
