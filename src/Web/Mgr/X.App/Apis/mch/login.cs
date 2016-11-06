using System;
using System.Collections.Generic;
using System.Linq;
using X.Core.Cache;
using X.Core.Plugin;
using X.Core.Utility;
using X.Web;
using X.Web.Apis;
using X.Web.Com;

namespace X.App.Apis.mch
{
    public class login : Api
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string uid { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string pwd { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        public string code { get; set; }
        protected override void Validate()
        {
            base.Validate();
            //Validator.Require("uid", uid);
            //Validator.Require("pwd", pwd);
            //Validator.Require("code", code);
        }
        protected override XResp Execute()
        {
            var c = CacheHelper.Get<string>("code." + uid);
            CacheHelper.Remove("code." + uid);
            if (c == null || c != code) throw new XExcep("T验证码不正确");
            var ad = DB.x_mch.SingleOrDefault(o => o.account == uid);

            if (ad == null || ad.pwd != Secret.MD5(pwd)) throw new XExcep("用户名或密码不正确");
            var ukey = Guid.NewGuid().ToString();

            CacheHelper.Save("mch." + ukey, ad, 60 * 20);

            SubmitDBChanges();
            return new XResp() { msg = ukey };
        }
    }
}
