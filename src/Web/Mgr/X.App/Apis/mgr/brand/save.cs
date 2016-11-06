using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.brand
{
    public class save : xmg
    {
        public int id { get; set; }
        [ParmsAttr(req = true, name = "分类名称")]
        public string cate { get; set; }
        public string name { get; set; }
        public string img { get; set; }
        public string remark { get; set; }

        protected override XResp Execute()
        {
            x_brand ent = null;
            if (id > 0) ent = DB.x_brand.FirstOrDefault(o => o.brand_id == id);
            if (ent == null) ent = new x_brand();

            //var up = DB.x_dict.FirstOrDefault(o => o.code == "goods.cate" && o.value == upv);
            ent.cate = cate;
            ent.name = name;
            ent.img = img;
            ent.remark = remark;

            if (id == 0) DB.x_brand.InsertOnSubmit(ent);
            SubmitDBChanges();

            return new XResp();
        }
    }
}
