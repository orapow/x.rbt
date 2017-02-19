using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.App.Com;
using X.Core.Utility;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.wx.user
{
    public class getcash : xapi
    {
        [ParmsAttr(name = "金额", min = 2)]
        public decimal amount { get; set; }
        protected override XResp Execute()
        {
            if (amount > cu.balance || cu.balance < 2) throw new XExcep("0x0024");

            if (cu.x_balan_get.Count(o => o.ctime.Value.Date == DateTime.Now.Date) > 0) throw new XExcep("0x0025");

            var g = new x_balan_get() { user_id = cu.user_id, ctime = DateTime.Now, amount = amount, remark = "提现" + amount + "元", balance = cu.balance };
            DB.x_balan_get.InsertOnSubmit(g);
            SubmitDBChanges();

            g = DB.x_balan_get.FirstOrDefault(o => o.balan_get_id == g.balan_get_id);

            var od = Wx.Pay.PayToOpenid(cfg.wx_appid, cfg.wx_mch_id, cu.openid, g.balan_get_id + "", amount, cfg.wx_certpath, cfg.wx_paykey);
            g.result = Serialize.ToJson(od);
            if (od.result_code == "SUCCESS") { cu.balance -= amount; g.balance = cu.balance; }
            SubmitDBChanges();

            if (od.result_code == "SUCCESS") return new XResp();
            else if (od.err_code == "NOTENOUGH") return new XResp() { issucc = false, msg = "提现人数过多，请10分钟后再试" };
            else throw new XExcep("0x0026");

        }
    }
}
