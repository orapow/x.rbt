using System;
using System.Linq;
using X.Core.Cache;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.wx.red
{
    public class get : _red
    {
        public int upid { get; set; }

        protected override int needus
        {
            get
            {
                return 2;
            }
        }

        protected override XResp Execute()
        {
            if (r.status == 2) throw new XExcep("0x0021");
            if (r.status == 3) throw new XExcep("0x0022");

            if (r.x_red_get.Count(o => o.get_op == cu.openid) > 0) throw new XExcep("0x0023");

            var gt = r.x_red_get.Where(o => o.status == 1).OrderBy(o => Guid.NewGuid()).FirstOrDefault();
            if (gt == null) throw new XExcep("0x0021");

            var lk = CacheHelper.Get<string>("red.get:" + cu.user_id);
            if (!string.IsNullOrEmpty(lk)) return new XResp();

            CacheHelper.Save("red.get:" + cu.user_id, "1");

            gt.status = 2;
            gt.upid = 0;
            gt.get_op = cu.openid;
            gt.get_nk = cu.nickname;
            gt.get_img = cu.headimg;
            gt.ctime = DateTime.Now;

            r.geted++;
            if (r.geted == r.count || r.x_red_get.Count(o => o.status == 1) == 0)
            {
                r.status = 2;
                r.geted = r.count;
                r.ftime = DateTime.Now;
                r.freason = "已经抢完";
            }

            var am = gt.amount.Value;// / 100.0);

            if (upid > 0)
            {
                gt.upid = upid;
                gt.ramount = (int)(am * r.upcash / 100.0);// / (decimal)100.0;//上级提拥10%
                var get = r.x_red_get.FirstOrDefault(o => o.red_get_id == upid);
                if (get != null) get.myramount += gt.ramount;
            }

            SubmitDBChanges();

            CacheHelper.Remove("red.get:" + cu.user_id);

            return new XResp();
        }
    }
}
