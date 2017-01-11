using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Core.Utility;
using X.Data;
using X.Web.Com;

namespace X.App.Apis.reply
{
    public class save : xapi
    {
        public int id { get; set; }
        [ParmsAttr(name = "回复类型", min = 1)]
        public int tp { get; set; }
        [ParmsAttr(name = "内容类型", min = 1)]
        public int type { get; set; }
        public string text { get; set; }
        public string img { get; set; }
        [ParmsAttr(name = "匹配方式")]
        public int match { get; set; }
        public string keys { get; set; }
        [ParmsAttr(name = "适用对象", req = true)]
        public string uids { get; set; }

        protected override XResp Execute()
        {
            var m = cu.x_reply.FirstOrDefault(o => o.reply_id == id);
            if (m == null) m = new x_reply() { ctime = DateTime.Now, user_id = cu.user_id, tp = tp };

            if (tp == 1) m.uids = "";
            else m.uids = uids;

            m.type = type;
            if (type == 1) m.content = text;
            else if (type == 2) m.content = img;

            if (tp == 4)
            {
                m.match = match;
                m.keys = keys;
            }

            if (m.reply_id == 0) DB.x_reply.InsertOnSubmit(m);

            SubmitDBChanges();

            return new XResp();
        }

    }
}
