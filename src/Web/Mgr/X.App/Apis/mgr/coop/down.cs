using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.coop
{
    public class down : xmg
    {
        public int cpid { get; set; }

        protected override void Validate()
        {
            base.Validate();
            Validator.CheckRange("cpid", cpid, -1, null);
        }

        protected override Web.Com.XResp Execute()
        {
            var rent = DB.x_rent.Where(o => o.x_coop.coop_id == cpid).OrderByDescending(o => o.rent_id).FirstOrDefault();//.SingleOrDefault(o => o.coop_id == cpid && (o.status == 1 || o.status == 2));
            if (rent == null) throw new XExcep("x0005");

            if (rent.status == 3) throw new XExcep("T房源已经下架，不要重复操作！");

            rent.x_coop.status = 3;
            rent.x_coop.intime = "";
            rent.e_time = "";
            rent.down_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            rent.x_coop.down_time = rent.down_time;
            rent.status = 3;

            SubmitDBChanges();

            return new XResp();
        }
    }
}
