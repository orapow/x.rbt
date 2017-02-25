using System;
using System.Linq;
using X.App.Com;
using X.Core.Cache;
using X.Core.Utility;

namespace X.App.Views.wx.red
{
    public class qrcode : _red
    {
        protected override void InitDict()
        {
            base.InitDict();

            var get = r.x_red_get.FirstOrDefault(o => o.get_op == cu.openid);
            if (get != null) dict.Add("am", (get.amount.Value + get.myramount.Value) / 100M);

            dict.Add("get", get);

            var code = Tools.GetRandNext(1, int.MaxValue);
            CacheHelper.Save("cash-" + code, "gt-" + get.red_get_id);

            var ad = DB.x_ad.FirstOrDefault(o => o.ad_id == r.ad);
            if (ad != null)
            {
                var url = Wx.Account.GetQrcode(Mp.appid, Mp.access_token, code + "");
                var data = Tools.GetHttpFile(url);
                if (data != null && data.Length > 0) dict.Add("img", Convert.ToBase64String(data));
            }
        }
    }
}
