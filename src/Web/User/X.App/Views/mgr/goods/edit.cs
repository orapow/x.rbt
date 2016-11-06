using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web;

namespace X.App.Views.mgr.goods
{
    public class edit : xmg
    {
        public int id { get; set; }
        /// <summary>
        /// 1、商品
        /// 2、团购
        /// 3、积分
        /// </summary>
        [ParmsAttr(req = true, name = "商品类型")]
        public int tp { get; set; }
        public string action { get; set; }
        protected override string GetParmNames
        {
            //传参数
            get
            {
                return "tp-id-action";
            }
        }
        protected override void InitDict()
        {
            base.InitDict();
            if (id > 0)
            {
                var ent = DB.x_goods.SingleOrDefault(o => o.goods_id == id);
                if (ent == null) throw new XExcep("0x0005");
                dict.Add("item", ent);
                dict.Add("red", ent.refunded == true ? 1 : 2);
                dict.Add("rnd", ent.sended == true ? 1 : 2);
                dict.Add("cate", ent.cate_id + "|" + GetDictName("goods.cate", ent.cate_id));
            }
            else
            {
                dict.Add("red", 1);
                dict.Add("rnd", 1);
            }
        }
    }
}
