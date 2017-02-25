using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;

namespace X.App.Views.wx.red
{
    public class _red : _wx
    {
        [ParmsAttr(name = "红包id", min = 1)]
        public int rid { get; set; }

        protected x_red r = null;
        x_ad ad = null;

        protected override string GetParmNames
        {
            get
            {
                return "rid";
            }
        }

        protected override int needus
        {
            get
            {
                return 2;
            }
        }

        protected override x_wxmp Mp
        {
            get
            {
                return ad.x_wxmp;
            }
        }

        protected override void getUser(string key)
        {
            base.getUser(Mp.appid);
        }

        protected override void InitView()
        {
            GetPageParms();
            r = DB.x_red.FirstOrDefault(o => o.red_id == rid);
            if (r == null) throw new XExcep("0x0015");

            if (r.ad != null) ad = DB.x_ad.FirstOrDefault(o => o.ad_id == r.ad);
            if (ad == null) throw new XExcep("0x0031");

            base.InitView();
        }

        protected override void InitDict()
        {
            base.InitDict();
            dict.Add("bao", r);
            dict.Add("ad", ad);
            dict.Add("mp", new
            {
                name = ad.x_wxmp.nick_name,
                img = ad.x_wxmp.head_img
            });
        }
    }
}
