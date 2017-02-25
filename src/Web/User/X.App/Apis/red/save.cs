using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Core.Utility;
using X.Data;
using X.Web;
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
            if (amount > cu.balance) throw new XExcep("0x0017");
            if (amount * 100 < count) throw new XExcep("0x0018");

            var m = new x_red() { ctime = DateTime.Now, user_id = cu.user_id };

            m.status = 1;
            m.mode = type;
            m.geted = 0;
            m.amount = amount;
            m.count = count;
            m.upcash = upc;
            m.remark = remark;
            m.qrcount = 0;
            m.adcount = 0;

            DB.x_red.InsertOnSubmit(m);

            amount *= 100;
            int am = (int)amount / count;
            for (var i = 0; i < count; i++)
            {
                var g = new x_red_get() { status = 1, ramount = 0 };
                if (type == 1) g.amount = am;
                else
                {
                    g.amount = Tools.GetRandNext(1, (int)amount / (count - i) * 2);
                    if (g.amount < 1) g.amount = 1;
                }
                amount -= g.amount.Value;
                g.ramount = 0;
                g.myramount = 0;
                m.x_red_get.Add(g);
            }

            var dt = new x_balan_detail()
            {
                amount = -m.amount,
                user_id = cu.user_id,
                ctime = DateTime.Now,
                remark = "发红包，金额：" + m.amount + "，个数：" + m.count
            };
            DB.x_balan_detail.InsertOnSubmit(dt);

            cu.balance -= m.amount.Value;

            SubmitDBChanges();

            return new XResp();
        }
    }
}
