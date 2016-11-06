using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.App.Com;
using X.Core.Plugin;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.app.user.yue
{
    public class make : xu
    {
        /// <summary>
        /// 出租记录id
        /// </summary>
        public int rentid { get; set; }
        /// <summary>
        /// 预约日期
        /// 1、今天
        /// 2、明天
        /// 3、后天
        /// 4、三天后
        /// </summary>
        public int ydate { get; set; }
        /// <summary>
        /// 预约时间
        /// </summary>
        public string ytime { get; set; }
        /// <summary>
        /// 预约备注
        /// </summary>
        public string remark { get; set; }

        protected override bool need_user
        {
            get
            {
                return true;
            }
        }

        protected override XResp Execute()
        {
            var re = DB.x_rent.FirstOrDefault(o => o.rent_id == rentid);
            if (re == null) throw new XExcep("T房源出租记录不存在");

            var ret = DB.x_reserve.FirstOrDefault(o => o.user_id == us.id && o.rent_id == rentid);

            if (ret == null) ret = new x_reserve()
            {
                user_id = us.id,
                time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                agent_id = re.x_coop.agent_id,
                coop_id = re.x_coop.coop_id,
                rent_id = re.rent_id
            };

            ret.status = 1;
            ret.remark = remark;
            ret.reserve_date = getDate();
            ret.reserve_time = ytime;

            if (ret.reserve_id == 0) DB.x_reserve.InsertOnSubmit(ret);
            SubmitDBChanges();

            var cot = us.name + " 预约 " + ret.reserve_date_name + " " + ret.reserve_time + " 带客户来看" + re.x_coop.house + "的" + re.x_coop.door_no_name + "号房";
            Loger.Info("Sms->" + ret.x_agent.tel + "->>" + cot);
            Sms.SendSms(cfg.sms_cfg, ret.x_agent.tel, cot);

            return new XResp();

        }

        string getDate()
        {
            var dt = DateTime.Now;
            return dt.AddDays(ydate - 1).ToString("yyyy-MM-dd");
        }
    }
}
