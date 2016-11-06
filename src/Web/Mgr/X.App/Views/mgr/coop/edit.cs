using System;
using System.Collections.Generic;
using System.Linq;
using X.Web;

namespace X.App.Views.mgr.coop
{
    public class edit : xmg
    {
        public int id { get; set; }
        public int st { get; set; }
        protected override string GetParmNames
        {
            get
            {
                return "id-agid-st";
            }
        }
        protected override void InitDict()
        {
            base.InitDict();
            if (id > 0)
            {
                var coop = DB.x_coop.SingleOrDefault(o => o.coop_id == id);
                if (coop == null) throw new XExcep("0x0005");
                dict.Add("item", coop);

                dict.Add("door_no", (coop.door_no + "  ").Split(' '));
            }
            dict["st"] = st - 1;
        }
    }
}
