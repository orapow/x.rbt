using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.brand
{
    public class del : xmg
    {
        [ParmsAttr(min = 1)]
        public int id { get; set; }

        protected override XResp Execute()
        {
            var ent = DB.x_brand.FirstOrDefault(o => o.brand_id == id);
            if (ent == null) throw new XExcep("T品牌不存在");

            DB.x_brand.DeleteOnSubmit(ent);

            SubmitDBChanges();

            return new XResp();
        }
    }
}
