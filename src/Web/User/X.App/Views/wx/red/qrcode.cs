using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;

namespace X.App.Views.wx.red
{
    public class qrcode : _wx
    {
        public int rid { get; set; }
        protected override string GetParmNames
        {
            get
            {
                return "rid";
            }
        }
        protected override void InitDict()
        {
            base.InitDict();
            var red = DB.x_red.FirstOrDefault(o => o.red_id == rid);
            if (red != null)
            {
                var ad = DB.x_ad.FirstOrDefault(o => o.ad_id == red.ad);
                if (ad != null) dict.Add("img", ad.qrcode);
            }
        }
    }
}
