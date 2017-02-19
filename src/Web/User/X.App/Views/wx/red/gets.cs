using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web;

namespace X.App.Views.wx.red
{
    public class gets : _wx
    {
        public int rid { get; set; }
        public int p { get; set; }
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

            var rd = DB.x_red.FirstOrDefault(o => o.red_id == rid);
            if (rd == null) throw new XExcep("0x0015");

            var q = from g in rd.x_red_get
                    join c in DB.x_user on g.owner equals c.user_id
                    where g.owner > 0
                    orderby g.ctime descending
                    select new
                    {
                        uid = g.owner,
                        am = (g.amount.Value / (decimal)100).ToString("F2"),
                        nk = c.nickname,
                        hd = c.headimg,
                        dt = g.ctime.Value.ToString("MM月dd日 HH:mm"),
                        ram = g.upid == cu.user_id && g.ramount > 0 ? "返现：" + (g.ramount.Value / (decimal)100).ToString("F2") : ""
                    };

            dict.Add("us", q.Skip((p - 1) * 10).Take(10).ToList());

        }
    }
}
