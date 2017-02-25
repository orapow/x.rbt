using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.App.Com;
using X.Core.Cache;
using X.Core.Utility;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.wx.user
{
    public class getcash : xapi
    {
        [ParmsAttr(name = "getid", req = true)]
        public string gt { get; set; }

        protected override int needus
        {
            get
            {
                return 1;
            }
        }

        protected override XResp Execute()
        {
            var id = CacheHelper.Get<string>("cash-" + gt).Split('-');
            if (id.Length != 2) throw new XExcep("0x0028");

            var getid = int.Parse(id[1]);
            var get = DB.x_red_get.FirstOrDefault(o => o.red_get_id == getid);
            if (get == null) throw new XExcep("0x0015");

            if (get.cashed == true) throw new XExcep("0x0029");

            var od = Wx.Pay.PayToOpenid(cfg.wx_appid, cfg.wx_mch_id, cu.openid, getid.ToString(), (get.amount.Value + get.myramount.Value) / 100M, cfg.wx_certpath, cfg.wx_paykey);
            get.remark = Serialize.ToJson(od);
            get.cashtime = DateTime.Now;
            if (od.result_code == "SUCCESS") { get.cashed = true; }
            SubmitDBChanges();

            if (od.result_code == "SUCCESS") return new XResp();
            else if (od.err_code == "NOTENOUGH") return new XResp() { issucc = false, msg = "提现人数过多，请10分钟后再试" };
            else throw new XExcep("0x0026");

        }
    }
}
