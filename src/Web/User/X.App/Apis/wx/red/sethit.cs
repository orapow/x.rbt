using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.wx.red
{
    public class sethit : xapi
    {
        [ParmsAttr(name = "红包id", min = 1)]
        public int rid { get; set; }

        [ParmsAttr(name = "点击类型", min = 1)]
        public int tp { get; set; }

        protected override int needus
        {
            get
            {
                return 2;
            }
        }

        protected override XResp Execute()
        {
            var r = DB.x_red.FirstOrDefault(o => o.red_id == rid);
            if (r == null) throw new XExcep("0x0015");

            var ht = new x_ad_hit()
            {
                opid = cu.openid,
                ad_id = r.ad,
                ctime = DateTime.Now,
                red_id = rid,
                tp = tp
            };
            DB.x_ad_hit.InsertOnSubmit(ht);
            SubmitDBChanges();

            r.adcount = r.x_ad_hit.Count(o => o.tp == 1);
            r.qrcount = r.x_ad_hit.Count(o => o.tp == 2);

            SubmitDBChanges();

            return new XResp();

        }
    }
}
