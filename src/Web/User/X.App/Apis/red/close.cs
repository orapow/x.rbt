using System;
using System.Linq;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.red
{
    public class close : xapi
    {
        [ParmsAttr(name = "红包id", min = 1)]
        public int id { get; set; }
        protected override XResp Execute()
        {
            var et = cu.x_red.FirstOrDefault(o => o.red_id == id);
            if (et == null) throw new XExcep("0x0015");

            et.status = 3;
            et.freason = "用户关闭红包";
            et.ftime = DateTime.Now;

            SubmitDBChanges();

            foreach (var g in et.x_red_get.Where(o => o.status == 1))
            {
                g.status = 3;
                cu.balance += g.amount.Value / 100;
            }

            SubmitDBChanges();

            return new XResp();
        }
    }
}
