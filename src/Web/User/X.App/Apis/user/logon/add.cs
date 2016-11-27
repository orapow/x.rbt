using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web.Com;

namespace X.App.Apis.user.logon
{
    public class add : xapi
    {
        protected override XResp Execute()
        {
            var lg = new x_logon()
            {
                status = 1,
                user_id = cu.user_id
            };
            DB.x_logon.InsertOnSubmit(lg);
            DB.SubmitChanges();
            return new XResp() { msg = lg.logon_id + "" };
        }
    }
}
