using System;
using System.Collections.Generic;
using System.Linq;
using X.Web;

namespace X.App.Views.mgr.agent
{
    public class log_edit : xmg
    {
        public int id { get; set; }
        protected override string GetParmNames
        {
            get
            {
                return "id-agent_id";
            }
        }
        protected override void InitDict()
        {
            base.InitDict();
            if (id > 0)
            {
                var track = DB.x_track.SingleOrDefault(o => o.track_id == id);
                if (track == null) throw new XExcep("0x0005");
                dict.Add("item", track);
            }
        }
    }
}
