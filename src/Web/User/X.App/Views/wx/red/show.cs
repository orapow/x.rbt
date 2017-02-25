using System.Linq;
using X.Data;
using X.Web;

namespace X.App.Views.wx.red
{
    public class show : _red
    {
        public int getid { get; set; }

        protected override string GetParmNames
        {
            get
            {
                return "rid-getid";
            }
        }

        protected override void InitView()
        {
            base.InitView();
            if (r.x_red_get.Count(o => o.get_op == cu.openid) > 0) Context.Response.Redirect("/wx/red/detail-" + rid + ".html");
        }

        protected override void InitDict()
        {
            base.InitDict();
            dict.Add("from", r.x_red_get.FirstOrDefault(o => o.red_get_id == getid));
        }

    }
}
