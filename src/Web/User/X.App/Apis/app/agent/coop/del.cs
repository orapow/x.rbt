using System;
using System.Collections.Generic;
using System.Linq;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.app.agent.coop
{
    public class del : xag
    {
        public int cpid { get; set; }

        protected override void Validate()
        {
            base.Validate();
            Validator.CheckRange("cpid", cpid, 0, null);
        }

        protected override XResp Execute()
        {
            var coop = DB.x_coop.SingleOrDefault(o => o.coop_id == cpid);
            if (coop == null) throw new XExcep("x0005");

            if (coop.agent_id != cag.id) throw new XExcep("T房源不属于你");

            //var rets = DB.x_rent.Where(o => o.coop_id == cpid);
            //DB.x_rent.DeleteAllOnSubmit(rets.ToList());

            //var cols = DB.x_collect.Where(o => o.coop_id == cpid);
            //DB.x_collect.DeleteAllOnSubmit(cols.ToList());

            //DB.x_coop.DeleteOnSubmit(coop);

            var rents = DB.x_rent.Where(o => o.coop_id == cpid).ToList();//删除出租记录
            var yues = DB.x_reserve.Where(o => rents.Select(r => (int?)r.rent_id).Contains(o.rent_id)).ToList();

            DB.x_reserve.DeleteAllOnSubmit(yues);
            DB.x_rent.DeleteAllOnSubmit(rents);

            var cols = DB.x_collect.Where(o => o.coop_id == cpid);
            DB.x_collect.DeleteAllOnSubmit(cols.ToList());

            DB.x_coop.DeleteOnSubmit(coop);

            SubmitDBChanges();

            SubmitDBChanges();

            return new XResp();
        }
    }
}
