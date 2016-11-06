using System;
using System.Collections.Generic;
using System.Linq;
using X.Web;

namespace X.App.Views.mgr.agent
{
    public class detail : xmg
    {
        public int agid { get; set; }
        protected override string GetParmNames
        {
            get
            {
                return "agid-page";
            }
        }
        protected override void InitDict()
        {
            base.InitDict();
            if (agid > 0)
            {
                var ag = DB.x_agent.SingleOrDefault(o => o.agent_id == agid);
                if (ag == null) throw new XExcep("0x0005");
                dict.Add("item", ag);


                dict.Add("s1", DB.x_coop.Count(o => o.status == 1 && o.agent_id == agid));
                dict.Add("s2", DB.x_coop.Count(o => o.status == 2 && o.agent_id == agid));
                dict.Add("s3", DB.x_coop.Count(o => o.status == 3 && o.agent_id == agid));

                dict.Add("s4", DB.x_reserve.Count(o => o.agent_id == agid && o.status == 1));
            }
        }
    }
}
