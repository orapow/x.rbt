using System;
using System.Linq;
using X.Data;
using X.Web.Com;

namespace X.App.Apis.cd
{
    public class save : xapi
    {
        [ParmsAttr(name = "样式", min = 1)]
        public int style { get; set; }
        public int id { get; set; }
        public string txt { get; set; }
        public string img { get; set; }
        [ParmsAttr(name = "公众号", min = 1)]
        public int mp { get; set; }
        public string qrcode { get; set; }
        public string link { get; set; }
        [ParmsAttr(name = "名称", req = true)]
        public string name { get; set; }

        protected override XResp Execute()
        {
            x_ad m = null;
            if (id > 0) m = cu.x_ad.FirstOrDefault(o => o.ad_id == id);
            if (m == null) m = new x_ad() { ctime = DateTime.Now, user_id = cu.user_id, status = 1 };

            m.style = style;
            m.txt = txt;
            m.link = link;
            m.img = img;
            m.name = name;
            m.wxmp_id = mp;

            if (m.ad_id == 0) DB.x_ad.InsertOnSubmit(m);
            SubmitDBChanges();

            return new XResp();
        }
    }
}
