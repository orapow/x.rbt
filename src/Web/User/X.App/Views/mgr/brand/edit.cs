using System.Linq;
using X.Web;

namespace X.App.Views.mgr.brand
{
    public class edit : xmg
    {
        public int id { get; set; }
        protected override string GetParmNames
        {
            get
            {
                return "id";
            }
        }
        protected override void InitDict()
        {
            base.InitDict();
            if (id > 0)
            {
                var ent = DB.x_brand.FirstOrDefault(o => o.brand_id == id);
                if (ent == null) throw new XExcep("0x0005");
                dict.Add("item", ent);
                dict.Add("cate", ent.cate + "|" + GetDictName("goods.cate", ent.cate));
            }
        }
    }
}
