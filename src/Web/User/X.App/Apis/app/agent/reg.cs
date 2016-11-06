using System;
using System.Collections.Generic;
using System.Linq;
using X.Core.Cache;
using X.Core.Utility;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.app.agent
{
    public class reg : xag
    {
        public string uid { get; set; }
        public string pwd { get; set; }
        public string code { get; set; }

        protected override void Validate()
        {
            base.Validate();
            Validator.Require("uid", uid);
            Validator.Require("pwd", pwd);
            Validator.Require("code", code);
        }

        protected override XResp Execute()
        {
            var vcode = CacheHelper.Get<string>("code." + uid);
            if (string.IsNullOrEmpty(code)) throw new XExcep("T验证码已经过期");
            if (vcode != code) throw new XExcep("T验证码不正确");

            if (DB.x_agent.Count(o => o.uid == uid) > 0) throw new XExcep("T用户已经存在");

            var ag = new x_agent
            {
                tel = uid,
                uid = uid,
                pwd = pwd,
                time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                ukey = Tools.GetRandRom(32, 3)
            };

            DB.x_agent.InsertOnSubmit(ag);

            SubmitDBChanges();

            return new XResp() { msg = ag.ukey };
        }
    }
}
