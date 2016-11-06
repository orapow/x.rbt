using System.Linq;
using X.Web;

namespace X.App.Views.mgr.cate
{
    public class edit : xmg
    {
        public int cid { get; set; }
        public int pid { get; set; }
        protected override string GetParmNames
        {
            get
            {
                return "cid-pid";
            }
        }
        protected override void InitDict()
        {
            base.InitDict();
            if (cid > 0)
            {
                var ent = DB.x_dict.FirstOrDefault(o => o.dict_id == cid);
                if (ent == null) throw new XExcep("0x0005");
                dict.Add("item", ent);
                dict.Add("up", ent.upval + "|" + (!string.IsNullOrEmpty(ent.upval) && ent.upval != "0" ? GetDictName("goods.cate", ent.upval) : "顶级分类"));
            }
            else if (pid > 0)
            {
                var ent = DB.x_dict.FirstOrDefault(o => o.dict_id == pid);
                if (ent != null) dict.Add("up", ent.value + "|" + ent.name);
            }
        }
    }
}
