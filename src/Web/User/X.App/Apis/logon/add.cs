using System.Linq;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.logon
{
    public class add : xapi
    {
        protected override XResp Execute()
        {
            if (cu.x_logon.Count() >= cu.wxcount) throw new XExcep("T最多只能添加" + cu.wxcount + "个登陆器");
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
