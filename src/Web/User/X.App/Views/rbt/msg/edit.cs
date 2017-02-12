using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Core.Utility;
using X.Web;

namespace X.App.Views.rbt.msg
{
    public class edit : xview
    {
        public int id { get; set; }
        protected override string GetParmNames
        {
            get { return "id"; }
        }
        protected override void InitDict()
        {
            base.InitDict();
            dict.Add("lgs", cu.x_logon.Where(o => o.uin > 0).Select(o => new { o.nickname, o.uin, o.headimg, id = o.logon_id }));
            if (id > 0)
            {
                var item = cu.x_msg.Select(o => new
                {
                    id = o.msg_id,
                    img = o.type == 1 ? "" : o.content,
                    text = o.type == 1 ? o.content : "",
                    tdate = o.way == 2 ? Tools.FromGreenTime(o.tcfg + "") : "",
                    tspan = o.way == 3 ? o.tcfg + "" : "",
                    o.type,
                    uids = Context.Server.HtmlDecode(o.uids),
                    o.way
                }).FirstOrDefault(o => o.id == id);
                if (item == null) throw new XExcep("T群发记录不存在");
                dict.Add("item", item);
            }
        }
    }
}
