using System;
using System.Collections.Generic;
using System.Linq;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.user
{
    /// <summary>
    /// 设置用户等级
    /// </summary>
    public class setlv : xmg
    {
        public int id { get; set; }
        public int lv { get; set; }

        protected override void Validate()
        {
            base.Validate();
            Validator.CheckRange("id", id, 0, null);
            Validator.CheckRange("lv", lv, 0, null);
        }

        protected override XResp Execute()
        {
            var u = DB.x_user.FirstOrDefault(o => o.user_id == id);
            if (u == null) throw new XExcep("T用户不存在");

            u.level = lv;

            SubmitDBChanges();

            return new XResp();
        }

    }
}
