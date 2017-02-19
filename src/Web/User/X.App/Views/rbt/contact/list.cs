using System;
using System.Linq;
using System.Threading;

namespace X.App.Views.rbt.contact
{
    public class list : xview
    {
        protected override string mu_name
        {
            get
            {
                return "rbt_contact";
            }
        }
        protected override void InitDict()
        {
            base.InitDict();
            dict.Add("lgs", cu.x_logon.Where(o => o.uin > 0).Select(o => new { o.nickname, o.uin, o.headimg, id = o.logon_id, count = DB.x_contact.Count(c => c.uin == o.uin && c.group_id == 0) }));
        }
    }
}
