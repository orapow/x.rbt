using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Core.Utility;
using X.Data;
using X.Web.Com;

namespace X.App.Apis.msg
{
    public class load : xapi
    {
        [ParmsAttr(name = "微信号uin", req = true)]
        public long uin { get; set; }

        protected override XResp Execute()
        {
            var ms = cu.x_msg.Where(o => o.next_time <= DateTime.Now && o.status == 1);
            var r = new Resp_List();
            var list = new List<object>();

            var cts = cu.x_contact.Where(o => o.uin == uin);

            foreach (var m in ms)
            {
                List<string> us = null;

                if (string.IsNullOrEmpty(m.uids) || m.uids == "[]") us = cts.Select(o => o.username).ToList();
                else us = cts.Where(o => Serialize.FromJson<List<long>>(System.Web.HttpUtility.HtmlDecode(m.uids)).Contains(o.contact_id)).Select(o => o.username).ToList();

                list.Add(new
                {
                    content = m.content,
                    type = m.type,
                    touser = us
                });

                m.last_time = m.next_time;
                if (m.way != 3) m.status = 2;
                else m.next_time = DateTime.Now.AddMinutes(m.tcfg.Value);
            }
            SubmitDBChanges();
            r.items = list;
            return r;
        }
    }
}
