using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.houses
{
    /// <summary>
    /// 审核楼盘
    /// </summary>
    public class check : xmg
    {
        public int id { get; set; }
        public int state { get; set; }
        protected override XResp Execute()
        {
            if (id <= 0) throw new XExcep("0x0005");
            x_houses houses = new x_houses();
            houses = DB.x_houses.SingleOrDefault(o => o.houses_id == id);
            //(初始:1,通过:2,不通过:3)
            houses.state = state;
            SubmitDBChanges();

            return new XResp();
        }
    }
}
