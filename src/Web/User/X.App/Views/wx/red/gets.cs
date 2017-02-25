using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web;

namespace X.App.Views.wx.red
{
    public class gets : _red
    {
        public int p { get; set; }

        protected override int needus
        {
            get
            {
                return 2;
            }
        }

        protected override string GetParmNames
        {
            get
            {
                return "rid-p";
            }
        }

        protected override void InitDict()
        {
            base.InitDict();

            var get = r.x_red_get.FirstOrDefault(o => o.get_op == cu.openid);

            var q = from g in r.x_red_get
                    where g.status == 2
                    orderby g.ctime descending
                    select new
                    {
                        uid = g.get_op,
                        am = (g.amount.Value / (decimal)100).ToString("F2"),
                        nk = g.get_nk,
                        hd = g.get_img,
                        dt = g.ctime.Value.ToString("MM月dd日 HH:mm"),
                        ram = (get != null && g.upid == get.red_get_id && g.ramount > 0) ? "返现：" + (g.ramount.Value / (decimal)100).ToString("F2") : ""
                    };

            dict.Add("us", q.Skip((p - 1) * 10).Take(10).ToList());

        }
    }
}
