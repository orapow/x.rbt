using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.cate
{
    public class del : xmg
    {
        [ParmsAttr(min = 1)]
        public int id { get; set; }

        protected override XResp Execute()
        {
            var ent = DB.x_dict.FirstOrDefault(o => o.dict_id == id);
            if (ent == null) throw new XExcep("T分类不存在");

            var childs = DB.x_dict.Where(o => o.upval.StartsWith(ent.value));
            foreach (var e in childs.ToList()) e.upval = "";

            var goods = DB.x_goods.Where(o => o.cate_id.StartsWith(ent.value));
            foreach (var g in goods.ToList()) g.cate_id = "";

            var fields = DB.x_goods_field.Where(o => o.cate_id == ent.value);
            foreach (var f in fields.ToList()) f.cate_id = "";

            DB.x_dict.DeleteOnSubmit(ent);

            SubmitDBChanges();

            return new XResp();
        }
    }
}
