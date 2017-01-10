using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Core.Utility;
using X.Data;
using X.Web.Com;

namespace X.App.Apis.msg
{
    public class save : xapi
    {
        public int id { get; set; }
        [ParmsAttr(name = "消息类型", min = 1)]
        public int type { get; set; }
        public string text { get; set; }
        public string img { get; set; }
        [ParmsAttr(name = "发送方式", min = 1)]
        public int way { get; set; }
        [ParmsAttr(name = "群发对象", req = true)]
        public string uids { get; set; }
        public int ucount { get; set; }
        public string tdate { get; set; }
        public int tspan { get; set; }

        protected override XResp Execute()
        {
            x_msg m = cu.x_msg.FirstOrDefault(o => o.msg_id == id);
            if (m == null) m = new x_msg() { ctime = DateTime.Now, user_id = cu.user_id };

            m.log = "";
            m.type = type;

            if (type == 1) m.content = text;
            else if (type == 2) m.content = img;

            if (way == 1) m.next_time = DateTime.Now.AddMinutes(1);
            else if (way == 2)
            {
                m.next_time = DateTime.Parse(tdate);
                m.tcfg = long.Parse(Tools.GetGreenTime(tdate));
            }
            else if (way == 3)
            {
                m.next_time = DateTime.Now.AddMinutes(tspan);
                m.tcfg = tspan;
            }

            m.uids = uids;
            m.ucount = ucount;
            m.way = way;
            m.status = 1;

            if (m.msg_id == 0) DB.x_msg.InsertOnSubmit(m);

            SubmitDBChanges();

            return new XResp();
        }

    }
}
