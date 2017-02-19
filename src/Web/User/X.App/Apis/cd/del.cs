using System.Linq;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.cd
{
    public class del : xapi
    {
        [ParmsAttr(name = "群发ID", min = 1)]
        public int id { get; set; }
        protected override XResp Execute()
        {
            var et = cu.x_ad.FirstOrDefault(o => o.ad_id == id);
            if (et == null) throw new XExcep("0x0009");
            if (et.status == 2) throw new XExcep("0x0010");

            DB.x_ad.DeleteOnSubmit(et);
            SubmitDBChanges();

            return new XResp();
        }
    }
}
