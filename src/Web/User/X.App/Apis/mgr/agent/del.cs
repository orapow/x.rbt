using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.agent
{
    public class del : xmg
    {
        public int id { get; set; }

        protected override void Validate()
        {
            base.Validate();
            Validator.CheckRange("id", id, 0, null);
        }

        protected override Web.Com.XResp Execute()
        {
            var ag = DB.x_agent.SingleOrDefault(o => o.agent_id == id);
            if (ag == null) throw new XExcep("x0005");

            DB.x_reserve.DeleteAllOnSubmit(ag.x_reserve);//删除预约
            var rents = DB.x_rent.Where(o => ag.x_coop.Select(t => (int?)t.coop_id).Contains(o.coop_id)).ToList();//删除出租记录
            var yues = DB.x_reserve.Where(o => rents.Select(r => (int?)r.rent_id).Contains(o.rent_id)).ToList();
            DB.x_reserve.DeleteAllOnSubmit(yues);
            DB.x_rent.DeleteAllOnSubmit(rents);
            var cols = DB.x_collect.Where(o => ag.x_coop.Select(t => (int?)t.coop_id).Contains(o.coop_id));//删除收藏
            DB.x_coop.DeleteAllOnSubmit(ag.x_coop);//删除房源
            DB.x_track.DeleteAllOnSubmit(ag.x_track);//删除跟踪记录

            DB.x_agent.DeleteOnSubmit(ag);

            SubmitDBChanges();

            return new XResp();
        }
    }
}
