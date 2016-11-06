using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web;

namespace X.App.Views.mgr.mch
{
    public class edit : xmg
    {
        public int mch_id { get; set; }
        protected override string GetParmNames
        {
            get
            {
                return "mch_id";
            }
        }
        protected override void InitDict()
        {
            base.InitDict();
            if (mch_id > 0)
            {
                var ag = DB.x_mch.SingleOrDefault(o => o.mch_id == mch_id);
                if (ag == null) throw new XExcep("0x0005");
                dict.Add("item", ag);
            }
        }
    }
}
