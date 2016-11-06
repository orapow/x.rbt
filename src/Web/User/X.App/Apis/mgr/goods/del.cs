using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.goods
{
    public class del : xmg
    {
        [ParmsAttr(min = 1)]
        public int id { get; set; }

        protected override XResp Execute()
        {
            var ent = DB.x_goods.FirstOrDefault(o => o.goods_id == id);
            if (ent == null) throw new XExcep("T商品不存在");

            /*----------------
            *1已录入
            *2已上架
            *3已下架
            *4已删除
            ----------------*/
            ent.status = 4;

            SubmitDBChanges();

            return new XResp();
        }
    }
}
