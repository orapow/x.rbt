using System.Linq;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.msg
{
    public class del : xapi
    {
        [ParmsAttr(name = "群发ID", min = 1)]
        public int id { get; set; }
        protected override XResp Execute()
        {
            var et = cu.x_msg.FirstOrDefault(o => o.msg_id == id);
            if (et == null) throw new XExcep("0x0014");

            DB.x_msg.DeleteOnSubmit(et);
            SubmitDBChanges();

            return new XResp();
        }
    }
}
