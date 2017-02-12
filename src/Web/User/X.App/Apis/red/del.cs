using System.Linq;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.red
{
    public class del : xapi
    {
        [ParmsAttr(name = "群发ID", min = 1)]
        public int id { get; set; }
        protected override XResp Execute()
        {
            var et = cu.x_red.FirstOrDefault(o => o.red_id == id);
            if (et == null) throw new XExcep("T红包不存在");

            if (et.geted > 0) throw new XExcep("T当前红包已经有人领取，不能删除，只能关闭");

            DB.x_red_get.DeleteAllOnSubmit(et.x_red_get);
            DB.x_red.DeleteOnSubmit(et);
            SubmitDBChanges();

            return new XResp();
        }
    }
}
