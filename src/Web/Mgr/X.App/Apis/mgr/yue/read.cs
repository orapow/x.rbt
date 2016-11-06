using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.yue
{
    /// <summary>
    /// 设为已读
    /// </summary>
    public class read : xmg
    {
        public int id { get; set; }

        protected override void Validate()
        {
            base.Validate();
            Validator.CheckRange("id", id, 0, null);
        }

        protected override XResp Execute()
        {
            var u = DB.x_reserve.FirstOrDefault(o => o.reserve_id == id);
            if (u == null) throw new XExcep("T预约记录不存在");

            u.status = 2;
            SubmitDBChanges();

            return new XResp();
        }

    }
}
