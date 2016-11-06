using System;
using System.Collections.Generic;
using System.Linq;
using X.App.Com;
using X.Core.Utility;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.wx.bw
{
    public class order : _bw
    {
        public int mo { get; set; }

        protected override void Validate()
        {
            base.Validate();
            Validator.CheckRange("mo", mo, 1, null);
        }

        protected override XResp Execute()
        {
            var od = new x_order()
            {
                ctime = DateTime.Now,
                user_id = cu.user_id,
                no = Secret.MD5(Guid.NewGuid().ToString(), 0),
                status = 1
            };

            var et = cu.etime > DateTime.Now ? (DateTime)cu.etime : DateTime.Now;
            od.etime = et.AddMonths(mo);
            od.amount = mo * 10;//(decimal)0.01;

            od.desc = "业主房源服务续费到" + od.etime.Value.ToString("yyyy-MM-dd HH:mm:ss");

            DB.x_order.InsertOnSubmit(od);
            SubmitDBChanges();

            var co = Wx.Pay.MdOrder(od.desc, od.no, ((int)(od.amount * 100)).ToString(), "http://" + cfg.domain + "/wx/bw/notify-" + od.no + ".html", opid, cfg.wx_appid, cfg.wx_mch_id, cfg.wx_paykey);

            if (co.return_code == "FAIL") throw new XExcep(co.return_msg);
            if (co.result_code == "FAIL") throw new XExcep(co.err_code + "," + co.err_code_des);
            if (string.IsNullOrEmpty(co.prepay_id)) throw new XExcep("T预付款号为空");

            od.wx_no = co.prepay_id;
            SubmitDBChanges();

            var ps = new Dictionary<string, string>();
            ps.Add("appId", cfg.wx_appid);
            ps.Add("timeStamp", Tools.GetGreenTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            ps.Add("nonceStr", Tools.GetRandRom(24, 3));
            ps.Add("package", "prepay_id=" + od.wx_no);
            ps.Add("signType", "MD5");

            var r = new od()
            {
                ns = ps["nonceStr"],
                ts = ps["timeStamp"],
                pkg = ps["package"],
                sign = Wx.ToSign(ps, false, cfg.wx_paykey)
            };

            return r;
        }

        public class od : XResp
        {
            public string ts { get; set; }
            public string ns { get; set; }
            public string pkg { get; set; }
            public string sign { get; set; }
        }
    }
}
