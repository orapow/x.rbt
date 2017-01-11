using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web.Com;

namespace X.App.Apis.wx
{
    public class login : xapi
    {
        [ParmsAttr(name = "uin", req = true)]
        public long uin { get; set; }
        [ParmsAttr(name = "nickname", req = true)]
        public string nickname { get; set; }
        [ParmsAttr(name = "headimg", req = true)]
        public string headimg { get; set; }

        protected override XResp Execute()
        {
            var lg = cu.x_logon.FirstOrDefault(o => o.uin == uin);
            if (lg == null) lg = new Data.x_logon() { uin = uin, user_id = cu.user_id };
            lg.lastime = DateTime.Now;
            lg.nickname = nickname;
            lg.headimg = headimg;

            if (lg.logon_id == 0) DB.x_logon.InsertOnSubmit(lg);

            SubmitDBChanges();
            return new XResp();
        }
    }
}
