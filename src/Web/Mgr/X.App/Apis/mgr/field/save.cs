using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.field
{
    public class save : xmg
    {
        public int id { get; set; }
        [ParmsAttr(req = true, name = "分类名称")]
        public string name { get; set; }
        public string upv { get; set; }
        public string img { get; set; }
        public string jp { get; set; }

        protected override XResp Execute()
        {
            x_dict ent = null;
            if (id > 0) ent = DB.x_dict.FirstOrDefault(o => o.dict_id == id);
            if (ent == null) ent = new x_dict() { code = "goods.cate" };

            //var up = DB.x_dict.FirstOrDefault(o => o.code == "goods.cate" && o.value == upv);
            ent.name = name;
            ent.img = img;
            ent.jp = jp;
            ent.upval = upv;

            if (id == 0) DB.x_dict.InsertOnSubmit(ent);
            SubmitDBChanges();

            if (string.IsNullOrEmpty(upv) || upv == "0") ent.value = ent.dict_id + "";
            else ent.value = upv + "-" + ent.dict_id;

            SubmitDBChanges();

            return new XResp();
        }
    }
}
