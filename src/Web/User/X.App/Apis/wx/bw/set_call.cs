using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.wx.bw
{
    public class set_call : _bw
    {
        public int cpid { get; set; }
        protected override void Validate()
        {
            base.Validate();
            Validator.CheckRange("cpid", cpid, 1, null);
        }
        protected override XResp Execute()
        {
            var tl = new x_tel_log()
            {
                ctime = DateTime.Now,
                ocoop_id = cpid,
                user_id = cu.user_id
            };

            DB.x_tel_log.InsertOnSubmit(tl);
            SubmitDBChanges();

            return new XResp();
        }
    }
}
