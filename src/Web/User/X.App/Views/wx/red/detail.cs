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

            var get = r.x_red_get.FirstOrDefault(o => o.get_op == cu.openid);

            dict.Add("get", get);

            if (get != null)
            {
                dict.Add("am", (get.amount / 100M).Value.ToString("F2"));
                dict.Add("rt", (get.myramount / 100M).Value.ToString("F2"));
            }
        }
    }
}
