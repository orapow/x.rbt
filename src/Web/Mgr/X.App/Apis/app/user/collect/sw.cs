using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.app.user.collect
{
    public class sw : xu
    {
        public int coopid { get; set; }

        protected override bool need_user
        {
            get
            {
                return true;
            }
        }

        protected override XResp Execute()
        {
            var c = DB.x_collect.FirstOrDefault(o => o.coop_id == coopid && o.user_id == us.id);
            var r = new XResp() { msg = "" };
            if (c == null)
            {
                c = new x_collect()
                {
                    coop_id = coopid,
                    time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    user_id = us.id
                };
                DB.x_collect.InsertOnSubmit(c);
                r.msg = c.id + "";
            }
            else
            {
                DB.x_collect.DeleteOnSubmit(c);
            }

            SubmitDBChanges();

            return r;

        }
    }
}
