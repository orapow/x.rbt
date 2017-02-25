using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Core.Utility;
using X.Web;

namespace X.App.Views.app.cd
{
    public class edit : xview
    {
        public int id { get; set; }
        protected override string GetParmNames
        {
            get { return "id"; }
        }
        protected override void InitDict()
        {
            base.InitDict();
            if (id > 0)
            {
                var item = cu.x_ad.FirstOrDefault(o => o.ad_id == id);
                if (item == null) throw new XExcep("0x0009");
                dict.Add("item", item);
            }

            var mps = "";
            foreach (var m in cu.x_wxmp.Where(o => o.verify_type_info == 0 && o.service_type_info == 2)) mps += m.nick_name + "-" + m.wxmp_id + "|";
            dict.Add("mps", mps.TrimEnd('|'));
        }
    }
}
