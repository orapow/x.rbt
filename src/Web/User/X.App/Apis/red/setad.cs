using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Core.Utility;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.red
{
    public class setad : xapi
    {
        public int id { get; set; }
        public int ad { get; set; }

        protected override XResp Execute()
        {
            var et = cu.x_red.FirstOrDefault(o => o.red_id == id);
            if (et == null) throw new XExcep("0x0015");

            var d = cu.x_ad.FirstOrDefault(o => o.ad_id == ad);
            if (d == null) throw new XExcep("0x0009");
            if (d.status == 3) throw new XExcep("0x0019");

            d.status = 2;
            et.ad = ad;

            SubmitDBChanges();

            return new XResp();
        }
    }
}
