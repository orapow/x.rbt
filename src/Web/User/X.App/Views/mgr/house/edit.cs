using System;
using System.Collections.Generic;
using System.Linq;
using X.Web;

namespace X.App.Views.mgr.house
{
    public class edit : xmg
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
            if (id > 0)
            {
                var house = DB.x_houses.SingleOrDefault(o => o.houses_id == id);
                if (house == null) throw new XExcep("0x0005");
                dict.Add("item", house);
            }
        }
    }
}
