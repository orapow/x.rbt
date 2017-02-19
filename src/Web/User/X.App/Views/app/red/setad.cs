using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web;

namespace X.App.Views.app.red
{
    public class setad : xview
    {
        public int id { get; set; }
        protected override string GetParmNames
        {
            get
            {
                return "id";
            }
        }
        protected override void InitDict()
        {
            base.InitDict();
            var r = cu.x_red.FirstOrDefault(o => o.red_id == id);

            if (r == null) throw new XExcep("0x0015");

            dict.Add("ad", r.ad);

            dict.Add("ads", cu.x_ad.Where(o => o.status != 3).Select(o => new
            {
                id = o.ad_id,
                o.style,
                o.txt,
                o.name,
                o.img,
                o.link
            }).ToList());

        }
    }
}
