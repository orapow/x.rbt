using System;
using System.Collections.Generic;
using System.Linq;
using X.Web;

namespace X.App.Views.mgr.sys.dict
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
                var ag = DB.x_dict.SingleOrDefault(o => o.dict_id == id);
                if (ag == null) throw new XExcep("0x0005");
                dict.Add("item", ag);
            }
        }
    }
}
