using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.track
{
    public class save_log : xmg
    {
        public int id { get; set; }
        public int agent_id { get; set; }//二房东
        public string content { get; set; }//跟踪内容

        protected override void Validate()
        {
            base.Validate();
            Validator.CheckRange("agent_id", agent_id, 0, null);
            Validator.Require("name", content);
        }

        protected override Web.Com.XResp Execute()
        {
            x_track track = new x_track();
            if (id > 0)
            {
                track = DB.x_track.SingleOrDefault(o => o.id == id);
                if (track == null) throw new XExcep("0x0005");
            }

            track.agent_id = agent_id;
            track.user_id = ad.admin_id;
            track.content = content;
     
            track.time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


            if (track.id == 0) DB.x_track.InsertOnSubmit(track);

            SubmitDBChanges();

            return new XResp();
        }
    }
}
