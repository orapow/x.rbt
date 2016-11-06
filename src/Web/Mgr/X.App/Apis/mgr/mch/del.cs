using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.mch
{
    public class del : xmg
    {
        public int id { get; set; }

        protected override void Validate()
        {
            base.Validate();
            //Validator.CheckRange("id", id, 0, null);
        }

        protected override Web.Com.XResp Execute()
        {
            var ag = DB.x_mch.SingleOrDefault(o => o.mch_id == id);
            if (ag == null) throw new XExcep("x0005");

            DB.x_mch.DeleteOnSubmit(ag);

            SubmitDBChanges();

            return new XResp();
        }
    }
}
