using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web;

namespace X.App.Views.wx.user
{
    public class reds : _wx
    {
        public int p { get; set; }
        protected override string GetParmNames
        {
            get
            {
                return "p";
            }
        }
        protected override void InitDict()
        {
            base.InitDict();

            var q = from r in DB.x_red_get
                    join c in DB.x_user on r.x_red.user_id equals c.user_id
                    where r.owner == cu.user_id
                    select new
                    {
                        id = r.red_id,
                        am = r.amount / 100.0M,
                        dt = r.ctime,//.Value.ToString("MM月dd日 HH:mm"),
                        ram = DB.x_red_get.Where(o => o.red_id == r.red_id && o.upid == c.user_id).Sum(o => o.ramount / 100.0),
                        c.name,
                        hd = c.headimg
                    };

            dict.Add("list", q.Skip((p - 1) * 10).Take(10).ToList());

        }
    }
}
