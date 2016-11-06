using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.goods
{
    public class status : xmg
    {
        [ParmsAttr(min = 1)]
        public int id { get; set; }
        public int type { get; set; }

        protected override XResp Execute()
        {
            var ent = DB.x_goods.FirstOrDefault(o => o.goods_id == id);
            if (ent == null) throw new XExcep("T商品不存在");

            if (type == 2)
            {
                ent.refunded = !ent.refunded;
            }
            if (type == 1)
            {
                ent.sended = !ent.sended;
            }

            SubmitDBChanges();

            return new XResp();
        }
    }
}
