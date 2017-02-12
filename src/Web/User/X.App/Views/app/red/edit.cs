using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Core.Utility;
using X.Web;

namespace X.App.Views.app.red
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
                var item = cu.x_red.FirstOrDefault(o => o.red_id == id);
                if (item == null) throw new XExcep("T红包不存在");
                dict.Add("item", item);
            }
        }
    }
}
