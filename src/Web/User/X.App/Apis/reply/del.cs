using System.Linq;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.reply
{
    public class del : xapi
    {
        [ParmsAttr(name = "群发ID", min = 1)]
        public int id { get; set; }
        protected override XResp Execute()
        {
            var et = cu.x_reply.FirstOrDefault(o => o.reply_id == id);
            if (et == null) throw new XExcep("0x0020");

            DB.x_reply.DeleteOnSubmit(et);
            SubmitDBChanges();

            return new XResp();
        }
    }
}
