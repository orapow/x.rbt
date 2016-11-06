using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web.Com;

namespace X.App.Apis.app.agent.user
{
    public class near : xag
    {
        public int page { get; set; }
        public int limit { get; set; }
        public int dist { get; set; }

        protected override void Validate()
        {
            base.Validate();
            Validator.CheckRange("lng", lng, 0, null);
            Validator.CheckRange("lat", lat, 0, null);
        }

        protected override Web.Com.XResp Execute()
        {
            if (dist == 0) dist = 5;
            if (page == 0) page = 1;
            if (limit == 0) limit = 1;

            var q = from u in DB.x_user
                    select new
                    {
                        u.image,
                        u.name,
                        u.tel,
                        dist = DB.fnGetDistance((float?)lng, (float?)lat, (float?)u.last_lng, (float?)u.last_lat) / 1000
                    };

            q = q.Where(o => o.dist <= dist);
            q = q.OrderBy(o => o.dist);

            var r = new Resp_List();
            r.page = 1;
            r.count = q.Count();
            r.items = q.ToList();

            return r;
        }
    }
}
