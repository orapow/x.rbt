using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.logon
{
    public class del : xapi
    {
        [ParmsAttr(name = "登陆器ID", min = 1)]
        public int id { get; set; }
        protected override XResp Execute()
        {
            var lg = cu.x_logon.FirstOrDefault(o => o.logon_id == id);
            if (lg == null) throw new XExcep("T登陆器不存在");
            if (lg.status != 1) throw new XExcep("T登陆器正在运行中，请先在微信端退出！");

            DB.x_logon.DeleteOnSubmit(lg);
            SubmitDBChanges();

            return new XResp();
        }
    }
}
