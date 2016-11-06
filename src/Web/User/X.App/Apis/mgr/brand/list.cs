using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web.Com;

namespace X.App.Apis.mgr.brand
{
    public class list : xmg
    {
        public int page { get; set; }
        public int limit { get; set; }
        public string key { get; set; }
        public string cate{ get; set; }

        protected override Web.Com.XResp Execute()
        {
            var r = new Resp_List();
            r.page = page;

            var q = from ad in DB.x_brand
                    select new {name=ad.name,ad.brand_id,ad.img,ad.remark,cate=GetDictName("goods.cate",ad.cate)};

            if (!string.IsNullOrEmpty(key)) q = q.Where(o => o.name.Contains(key) || o.remark.Contains(key));
            if (!string.IsNullOrEmpty(cate)) q = q.Where(o => o.cate==(cate));

            r.items = q.Skip((page - 1) * limit).Take(limit);
            r.count = q.Count();
            return r;
        }

    }
}
