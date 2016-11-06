using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web.Com;

namespace X.App.Apis.mgr.goods
{
    public class list : xmg
    {
        /// <summary>
        /// 1、商品
        /// 2、团购
        /// 3、积分
        /// </summary>
        [ParmsAttr(req = true, name = "商品类型")]
        public int tp { get; set; }
        public int page { get; set; }
        public int limit { get; set; }
        public string key { get; set; }

        protected override Web.Com.XResp Execute()
        {
            var r = new Resp_List();
            r.page = page;

            var q = from ad in DB.x_goods
                    where ad.status!=4 && ad.type == tp
                    orderby ad.goods_id descending
                    select new
                    {
                        id = ad.goods_id,
                        cate = GetDictName("goods.cate", ad.cate_id),
                        ad.name,
                        ad.remark,
                        ad.cover,
                        ad.sort,
                        ad.stock,
                        ad.old_price,
                        ad.new_price,
                        time = ad.mtime,
                        tk = ad.refunded == true ? 1 : 0,
                        ps = ad.sended == true ? 1 : 0
                    };

            if (!string.IsNullOrEmpty(key)) q = q.Where(o => o.name.Contains(key) || o.remark.Contains(key));

            r.items = q.Skip((page - 1) * limit).Take(limit);
            r.count = q.Count();
            return r;
        }

    }
}
