using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace X.App.Views.user.logon
{
    public class list : xview
    {
        protected override string menu_id
        {
            get
            {
                return "15";
            }
        }

        protected override void InitDict()
        {
            base.InitDict();
            dict.Add("lgs", cu.x_logon.Select(o => new { o.nickname, o.headimg, o.status, id = o.logon_id }));
        }

    }
}
