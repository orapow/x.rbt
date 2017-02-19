using System;
using System.Linq;
using X.Core.Cache;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.wx.red
{
    public class get : xapi
    {
        [ParmsAttr(name = "红包ID", min = 1)]
        public int id { get; set; }
        public int uid { get; set; }

        protected override XResp Execute()
        {
            var r = DB.x_red.FirstOrDefault(o => o.red_id == id);
            if (r == null) throw new XExcep("0x0015");
            if (r.status == 2) throw new XExcep("0x0021");
            if (r.status == 3) throw new XExcep("0x0022");

            if (r.x_red_get.Count(o => o.owner == cu.user_id) > 0) throw new XExcep("0x0023");

            var gt = r.x_red_get.Where(o => o.owner == 0).OrderBy(o => Guid.NewGuid()).FirstOrDefault();
            if (gt == null) throw new XExcep("0x0021");

            var lk = CacheHelper.Get<string>("red.get:" + cu.user_id);
            if (!string.IsNullOrEmpty(lk)) return new XResp();

            CacheHelper.Save("red.get:" + cu.user_id, "1");

            gt.status = 2;
            gt.owner = cu.user_id;
            gt.ctime = DateTime.Now;

            r.geted++;
            if (r.geted == r.count || r.x_red_get.Count(o => o.owner == 0) == 0)
            {
                r.status = 2;
                r.geted = r.count;
                r.ftime = DateTime.Now;
                r.freason = "已经抢完";
            }

            var am = gt.amount.Value;// / 100.0);
            var uam = (int)(am * r.upcash / 100.0);// / (decimal)100.0;//上级提拥10%

            if (uid > 0 && uid != cu.user_id)
            {
                gt.upid = uid;
                var pu = DB.x_user.FirstOrDefault(o => o.user_id == uid);
                if (pu != null)
                {
                    gt.amount = am - uam;
                    gt.ramount = uam;
                    pu.balance += (decimal)(gt.ramount / 100.0);
                    cu.balance += (decimal)(gt.amount / 100.0);
                }
                else
                {
                    cu.balance += (decimal)(gt.amount / 100.0);
                }
            }
            else
            {
                cu.balance += (decimal)(gt.amount / 100.0);
            }

            var dt = new x_balan_detail()
            {
                amount = gt.amount / 100M,
                user_id = cu.user_id,
                ctime = DateTime.Now,
                remark = "领到" + r.x_user.name + "的红包，金额：" + gt.amount / 100.0M
            };
            DB.x_balan_detail.InsertOnSubmit(dt);

            if (gt.ramount > 0)
            {
                var pdt = new x_balan_detail()
                {
                    amount = gt.amount / 100.0M,
                    user_id = gt.upid,
                    ctime = DateTime.Now,
                    remark = "来自" + cu.nickname + "的红包返现，金额：" + gt.ramount / 100.0M
                };
                DB.x_balan_detail.InsertOnSubmit(pdt);
            }

            SubmitDBChanges();

            CacheHelper.Remove("red.get:" + cu.user_id);

            return new XResp();
        }
    }
}
