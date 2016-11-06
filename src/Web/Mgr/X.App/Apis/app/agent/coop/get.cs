using System;
using System.Collections.Generic;
using System.Linq;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.app.agent.coop
{
    public class get : xag
    {
        public int coopid { get; set; }

        protected override void Validate()
        {
            base.Validate();
            Validator.CheckRange("coopid", coopid, -1, null);
        }

        protected override XResp Execute()
        {
            var r = new Resp_List();
            var q = from c in DB.x_coop
                    where c.coop_id == coopid
                    select c;

            var cp = q.SingleOrDefault();
            if (cp == null) throw new XExcep("T房源不存在");

            r.items = cp;
            cp.x_agent = null;
            cp.x_collect = null;
            cp.x_rent = null;

            return r;
        }
    }
}
