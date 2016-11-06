using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.App.Views;

namespace X.App.Views.mgr.agent
{
    public class log_list : xmg
    {
        public int id { get; set; }
        protected override string GetParmNames
        {
            get
            {
                return "id";
            }
        }
    }
}
