using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web.Com;

namespace X.App.Apis.contact
{
    public class del : xapi
    {
        [ParmsAttr(name = "微信号", min = 1)]
        public long uin { get; set; }
        protected override XResp Execute()
        {
            var cs = cu.x_contact.Where(o => o.uin == uin);
            DB.x_contact.DeleteAllOnSubmit(cs);

            var lg = cu.x_logon.Where(o => o.uin == uin);
            DB.x_logon.DeleteAllOnSubmit(lg);

            SubmitDBChanges();
            return new XResp();
        }
    }
}
