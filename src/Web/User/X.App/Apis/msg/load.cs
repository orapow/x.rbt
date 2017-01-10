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
        public string uins { get; set; }

        protected override XResp Execute()
        {
            var ms = cu.x_msg.Where(o => o.next_time <= DateTime.Now && o.status == 1);
            var r = new Resp_List();
            var list = new List<object>();
            foreach (var m in ms)
            {
                List<x_contact> us = null;

                if (string.IsNullOrEmpty(m.uids)) us = cu.x_contact.Where(o => o.user_id == m.user_id).ToList();
                else us = cu.x_contact.Where(o => Serialize.FromJson<List<long>>(System.Web.HttpUtility.HtmlDecode(m.uids)).Contains(o.contact_id)).ToList();

                foreach (var u in uins.Split(','))
                {
                    list.Add(new
                    {
                        body = m.content,
                        type = m.type,
                        uin = u,
                        touser = us.Where(o => o.uin + "" == u).Select(o => o.username).ToList()
                    });
                }

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
