using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Core.Cache;
using X.Core.Utility;
using X.Web;

namespace X.App.Views.wx.user
{
    public class cash : _wx
    {
        protected override int needus
        {
            get
            {
                return 1;
            }
        }

        public string gt { get; set; }

        protected override void getUser(string key)
        {
            base.getUser(cfg.wx_appid);
        }

        protected override string GetParmNames
        {
            get
            {
                return "gt";
            }
        }

        protected override void InitDict()
        {
            base.InitDict();

            var id = CacheHelper.Get<string>("cash-" + gt).Split('-');
            if (id.Length != 2) throw new XExcep("0x0028");

            var getid = int.Parse(id[1]);
            var get = DB.x_red_get.FirstOrDefault(o => o.red_get_id == getid);
            dict.Add("get", get);
            dict.Add("am", (get.amount.Value + get.myramount.Value) / 100M);

        }
    }
}
