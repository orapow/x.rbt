using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;

namespace X.App.Apis.wx
{
    public class _red : xapi
    {
        public int rid { get; set; }

        protected x_red r = null;

        protected override string get_appid()
        {
            if (rid > 0)
            {
                r = DB.x_red.FirstOrDefault(o => o.red_id == rid);
                if (r.ad != null)
                {
                    var ad = DB.x_ad.FirstOrDefault(o => o.ad_id == r.ad);
                    if (ad == null) throw new XExcep("0x0031");
                    return ad.x_wxmp.appid;
                }
            }
            return base.get_appid();
        }
    }
}
