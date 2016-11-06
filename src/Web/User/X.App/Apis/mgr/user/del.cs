using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.user
{
    /// <summary>
    /// 删除用户
    /// </summary>
    public class del : xmg
    {
        public int id { get; set; }

        protected override void Validate()
        {
            base.Validate();
            Validator.CheckRange("id", id, 0, null);
        }

        protected override XResp Execute()
        {
            var u = DB.x_user.FirstOrDefault(o => o.user_id == id);
            if (u == null) throw new XExcep("T用户不存在");

            var cols = DB.x_collect.Where(o => o.user_id == id);
            DB.x_collect.DeleteAllOnSubmit(cols.ToList());

            var revs = DB.x_reserve.Where(o => o.user_id == id);
            DB.x_reserve.DeleteAllOnSubmit(revs.ToList());

            DB.x_user.DeleteOnSubmit(u);

            SubmitDBChanges();

            return new XResp();
        }

    }
}
