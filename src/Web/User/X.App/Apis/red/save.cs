using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Core.Utility;
using X.Data;
using X.Web.Com;

namespace X.App.Apis.red
{
    public class save : xapi
    {
        [ParmsAttr(name = "红包类型", min = 1)]
        public int type { get; set; }
        public string remark { get; set; }
        public decimal amount { get; set; }
        public int count { get; set; }
        public int upc { get; set; }

        protected override XResp Execute()
        {
            var m = new x_red() { ctime = DateTime.Now, user_id = cu.user_id };

            m.status = 1;
            m.mode = type;
            m.geted = 0;
            m.amount = amount;
            m.count = count;
            m.upcash = upc;
            m.remark = remark;

            DB.x_red.InsertOnSubmit(m);

            amount *= 100;
            int am = (int)amount / count;
            for (var i = 0; i < count; i++)
            {
                var g = new x_red_get() { status = 1, owner = 0, upid = 0, ramount = 0 };
                if (type == 1)
                {
                    if (i == count - 1) g.amount = (int)(amount);
                    else g.amount = am;
                }
                else
                {
                    if (i == count - 1) g.amount = (int)(amount);
                    else
                    {
                        var md = Tools.GetRandNext(1, am * 2);
                        g.amount = md;
                    }
                }
                amount -= g.amount.Value;
                m.x_red_get.Add(g);
            }

            SubmitDBChanges();

            return new XResp();
        }
    }
}
