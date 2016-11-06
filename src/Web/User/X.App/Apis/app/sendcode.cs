using System;
using System.Collections.Generic;
using System.Linq;
using X.App.Com;
using X.Core.Cache;
using X.Core.Utility;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.app
{
    public class sendcode : xap
    {
        public string tel { get; set; }
        /// <summary>
        /// 1、经纪人
        /// 2、二房东
        /// </summary>
        public int tp { get; set; }
        protected override void Validate()
        {
            base.Validate();
            Validator.Require("tel", tel);
        }

        protected override XResp Execute()
        {
            var v = CacheHelper.Get<string>("send." + tel);
            if (!string.IsNullOrEmpty(v)) throw new XExcep("T发送过于频繁");

            var c = 0;
            if (tp == 2) c = DB.x_agent.Count(o => o.uid == tel);
            else c = DB.x_user.Count(o => o.tel == tel);
            if (c > 0) throw new XExcep("0x0017");

            var code = Tools.GetRandRom(6);

            var str = Sms.SendSms(cfg.sms_cfg, tel, "您的验证码为：" + code);

            if (str != string.Empty) throw new XExcep("0x0007", str);

            CacheHelper.Save("code." + tel, code, 20 * 60);
            CacheHelper.Save("send." + tel, code, 50);

            return new XResp() { msg = code };
        }
    }
}
