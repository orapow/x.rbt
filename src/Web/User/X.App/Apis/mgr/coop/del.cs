using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.coop
{
    public class del : xmg
    {
        public int cpid { get; set; }

        protected override void Validate()
        {
            base.Validate();
            Validator.CheckRange("id", cpid, 0, null);
        }

        protected override Web.Com.XResp Execute()
        {
            var coop = DB.x_coop.SingleOrDefault(o => o.coop_id == cpid);
            if (coop == null) throw new XExcep("x0005");

            var rents = DB.x_rent.Where(o => o.coop_id == cpid).ToList();//删除出租记录
            var yues = DB.x_reserve.Where(o => rents.Select(r => (int?)r.rent_id).Contains(o.rent_id)).ToList();

            DB.x_reserve.DeleteAllOnSubmit(yues);
            DB.x_rent.DeleteAllOnSubmit(rents);

            var cols = DB.x_collect.Where(o => o.coop_id == cpid);
            DB.x_collect.DeleteAllOnSubmit(cols.ToList());

            DB.x_coop.DeleteOnSubmit(coop);

            SubmitDBChanges();

            return new XResp();
        }
    }
}
