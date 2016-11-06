using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.app.user.yue
{
    public class get : xu
    {
        public int yue_id { get; set; }

        protected override bool need_user
        {
            get
            {
                return true;
            }
        }

        protected override XResp Execute()
        {
            var c = DB.x_reserve.Where(o => o.reserve_id == yue_id && o.user_id == us.id);
            if (c.Count() == 0) throw new XExcep("T预约记录不存在");

            var r = new Resp_List();
            r.items = c.Select(o => new
            {
                o.reserve_id,
                reserve_date = getDate(o.reserve_date),
                o.reserve_date_name,
                o.reserve_time,
                o.remark
            }).FirstOrDefault();
            return r;

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
