using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web;

namespace X.App.Views.wx.user
{
    public class reds : _wx
    {
        protected override int needus
        {
            get
            {
                return 1;
            }
        }
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
                    where r.get_op == opid
                    select new
                    {
                        id = r.red_id,
                        am = r.amount / 100.0M,
                        dt = r.ctime,
                        ram = (r.myramount / 100M).Value.ToString("F2"), //DB.x_red_get.Where(o => o.red_id == r.red_id && o.upid == opid).Sum(o => o.ramount / 100.0),
                        name = r.get_op,//c.name,
                        hd = r.get_img
                    };

            dict.Add("list", q.Skip((p - 1) * 10).Take(10).ToList());

        }
    }
}
