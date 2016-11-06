using System;
using System.Collections.Generic;
using System.Linq;
using X.Core.Cache;
using X.Core.Utility;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.app.user
{
    public class reg : xu
    {
        public string tel { get; set; }
        public string img { get; set; }
        public string pwd { get; set; }
        public string code { get; set; }

        protected override void Validate()
        {
            base.Validate();
            Validator.Require("tel", tel);
            Validator.Require("pwd", pwd);
            Validator.Require("code", code);
        }

        protected override XResp Execute()
        {
            var vcode = CacheHelper.Get<string>("code." + tel);
            if (string.IsNullOrEmpty(code)) throw new XExcep("T验证码已经过期");
            if (vcode != code) throw new XExcep("T验证码不正确");

            if (DB.x_user.Count(o => o.tel == tel) > 0) throw new XExcep("T用户已经存在");

            var u = new x_user
            {
                tel = tel,
                pwd = pwd,
                image = img,
                level = 0,
                reg_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                ukey = Tools.GetRandRom(32, 3)
            };

            DB.x_user.InsertOnSubmit(u);

            SubmitDBChanges();

            return new XResp() { msg = u.ukey };
        }
    }
}
