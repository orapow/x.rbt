using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Core.Cache;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.sys.dict
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
            var dt = DB.x_dict.SingleOrDefault(o => o.dict_id == id);
            if (dt == null) throw new XExcep("x0005");

            CacheHelper.Remove("dict." + dt.code);
            DB.x_dict.DeleteOnSubmit(dt);

            SubmitDBChanges();

            return new XResp();
        }
    }
}
