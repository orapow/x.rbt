using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Core.Utility;
using X.Data;
using X.Web.Com;

namespace X.App.Apis.reply
{
    public class load : xapi
    {
        protected override XResp Execute()
        {
            var r = new Resp_List();

            var list = new List<object>();
            foreach (var m in cu.x_reply)
            {
                List<x_contact> us = null;
                if (string.IsNullOrEmpty(m.uids) || m.uids == "[]") us = cu.x_contact.Where(o => o.user_id == m.user_id).ToList();
                else us = cu.x_contact.Where(o => Serialize.FromJson<List<long>>(System.Web.HttpUtility.HtmlDecode(m.uids)).Contains(o.contact_id)).ToList();

                if (us == null) continue;

                list.Add(new
                {
                    m.type,
                    m.content,
                    keys = m.keys?.Split(' '),
                    m.match,
                    m.tp,
                    users = us.Select(o => o.username).ToArray()
                });

            }
            r.items = list;
            return r;
        }
    }
}
