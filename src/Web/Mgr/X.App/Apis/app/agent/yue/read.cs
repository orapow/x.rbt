using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.app.agent.yue
{
    public class read : xag
    {
        public int yid { get; set; }

        protected override bool need_user
        {
            get
            {
                return true;
            }
        }

        protected override XResp Execute()
        {
            var c = DB.x_reserve.SingleOrDefault(o => o.reserve_id == yid && o.agent_id == cag.agent_id);
            if (c == null) throw new XExcep("T预约记录不存在");

            if (c.status == 1)
            {
                c.status = 2;
                SubmitDBChanges();
            }

            return new XResp();

        }

        string getDate(string dt)
        {
            var date = DateTime.Parse(dt + " 23:59:59");
            var now = DateTime.Now;
            var sp = date - now;
            if (sp.Hours > 0 && sp.Hours < 24 && sp.Days <= 2) return (sp.Days + 1) + "";
            else return dt;
        }
    }
}
