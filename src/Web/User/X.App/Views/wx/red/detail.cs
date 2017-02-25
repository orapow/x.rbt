using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;

namespace X.App.Views.wx.red
{
    public class detail : _red
    {
        protected override void InitDict()
        {
            base.InitDict();

            if (r.ad != null) dict.Add("ad", DB.x_ad.FirstOrDefault(o => o.ad_id == r.ad));

            var get = r.x_red_get.FirstOrDefault(o => o.get_op == cu.openid);

            dict.Add("get", get);

            dict.Add("am", (get.amount / 100M).Value.ToString("F2"));
            dict.Add("rt", (get.myramount / 100M).Value.ToString("F2"));
        }
    }
}
