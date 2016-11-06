using System;
using System.Collections.Generic;
using System.Linq;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.app.agent.coop
{
    public class up : xag
    {
        public int cpid { get; set; }
        public decimal price { get; set; }
        protected override void Validate()
        {
            base.Validate();
            Validator.CheckRange("cpid", cpid, -1, null);
            Validator.CheckRange("price", price, -1, null);
        }

        protected override XResp Execute()
        {
            var coop = DB.x_coop.SingleOrDefault(o => o.coop_id == cpid);
            if (coop == null) throw new XExcep("x0005");

            if (coop.agent_id != cag.id) throw new XExcep("T房源不属于你");
            if (coop.status == 1) throw new XExcep("T房源已经上架");

            var rt = new x_rent()
            {
                coop_id = cpid,
                z_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                status = 1,
                z_price = price
            };

            DB.x_rent.InsertOnSubmit(rt);
            coop.price = price;
            coop.up_time = rt.z_time;
            coop.intime = "";
            coop.status = 1;

            SubmitDBChanges();

            return new XResp();
        }
    }
}
