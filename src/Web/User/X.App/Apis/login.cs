using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Core.Cache;
using X.Core.Utility;
using X.Web;
using X.Web.Com;

namespace X.App.Apis
{
    public class login : xapi
    {
        [ParmsAttr(name = "用户帐号", req = true)]
        public string uin { get; set; }
        [ParmsAttr(name = "登陆密码", req = true)]
        public string pwd { get; set; }
        [ParmsAttr(name = "验证码", req = true)]
        public string code { get; set; }

        protected override XResp Execute()
        {

            var c = CacheHelper.Get<string>("code." + uin);
            CacheHelper.Remove("code." + uin);
            if (c == null || c != code) throw new XExcep("T验证码不正确");
            var cu = DB.x_user.FirstOrDefault(o => o.uin == uin);

            if (cu == null || cu.pwd != pwd) throw new XExcep("用户名或密码不正确");
            var ukey = Secret.MD5(Guid.NewGuid().ToString());

            cu.ukey = ukey;
            SubmitDBChanges();

            CacheHelper.Save("cu." + cu.ukey, cu, 60 * 20);

            return new XResp() { msg = cu.ukey };

        }
    }
}
