using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web.Com;

namespace X.App.Apis.mgr.track
{
    /// <summary>
    /// 服务器日志列表
    /// </summary>
    public class list : xmg
    {
        public int agid { get; set; }
        public int page { get; set; }
        public int limit { get; set; }
        public string key { get; set; }

        protected override Web.Com.XResp Execute()
        {
            var r = new Resp_List();
            r.page = page;

            var q = from track in DB.x_track
                    join ag in DB.x_agent on track.agent_id equals ag.agent_id into ag_join
                    from agt in ag_join.DefaultIfEmpty()

                    join ad in DB.x_admin on track.user_id equals ad.admin_id into ad_join
                    from adm in ad_join.DefaultIfEmpty()
                    where track.agent_id == agid
                    orderby track.time descending
                    select new
                    {
                        track.id,
                        track.agent_id,//二房东
                        agent_name = agt.name,//二房东名称
                        track.user_id,//管理员
                        user_name = adm.name,//管理员名称
                        track.content,//跟踪内容
                        track.time,//跟踪时间
                    };

            if (!string.IsNullOrEmpty(key))
            {
                q = q.Where(o => o.agent_id == int.Parse(key));
            }

            r.items = q.Skip((page - 1) * limit).Take(limit);
            r.count = q.Count();
            return r;
        }

    }
}
