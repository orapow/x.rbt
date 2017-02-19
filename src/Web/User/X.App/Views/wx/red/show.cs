using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;

namespace X.App.Views.wx.red
{
    public class show : _wx
    {
        [ParmsAttr(name = "红包id", min = 1)]
        public int rid { get; set; }
        public int uid { get; set; }

        protected override string GetParmNames
        {
            get
            {
                return "rid-uid";
            }
        }

        protected override void InitDict()
        {
            base.InitDict();

            var r = DB.x_red.FirstOrDefault(o => o.red_id == rid);
            if (r == null) throw new XExcep("红包不存在");

            dict.Add("bao", r);

            if (r.ad != null) dict.Add("ad", DB.x_ad.FirstOrDefault(o => o.ad_id == r.ad));

            var gd = r.x_red_get.FirstOrDefault(o => o.owner == cu.user_id);
            if (gd != null)
            {
                dict.Add("am", (gd.amount.Value / 100.0).ToString("F2"));
                if (gd.upid > 0) dict.Add("from", DB.x_user.FirstOrDefault(o => o.user_id == gd.upid));
                dict.Add("rt", r.x_red_get.Where(o => o.upid == cu.user_id).Sum(o => o.ramount) / 100.0);
            }
            else
            {
                dict.Add("get", "1");
                if (uid > 0) dict.Add("from", DB.x_user.FirstOrDefault(o => o.user_id == uid));
            }

            dict.Add("u", DB.x_user.FirstOrDefault(o => o.user_id == r.user_id));

        }
    }
}
