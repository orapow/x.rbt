using System;
using System.Collections.Generic;
using System.Linq;
using X.Core.Utility;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.app.agent
{
    public class login : xag
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string uid { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string pwd { get; set; }

        protected override bool need_user
        {
            get
            {
                return false;
            }
        }

        protected override void Validate()
        {
            base.Validate();
            Validator.Require("uid", uid);
            Validator.Require("pwd", pwd);
        }

        protected override XResp Execute()
        {
            var ag = DB.x_agent.SingleOrDefault(o => o.uid == uid);
            if (ag == null || (Secret.MD5(ag.pwd) != Secret.MD5(pwd) && Secret.MD5(ag.pwd) != pwd)) throw new XExcep("用户名或密码不正确");

            ag.ukey = Tools.GetRandRom(32, 3);
            SubmitDBChanges();

            var r = new cag
            {
                ukey = ag.ukey,
                contract = ag.contract,
                logo = ag.logo,
                name = ag.name,
                pwd = Secret.MD5(ag.pwd),
                tel = ag.tel,
                uid = ag.uid,
                addr = ag.addr,
                intro = ag.intro,
                status = ag.status ?? 0,
                status_name = ag.status_name,
                zz = (decimal)ag.c_zz,
                hz = (decimal)ag.c_hz
            };

            return r;
        }
    }
    public class cag : XResp
    {
        public string ukey { get; set; }
        public string uid { get; set; }
        public string pwd { get; set; }
        public string name { get; set; }
        public string logo { get; set; }
        public string contract { get; set; }
        public string tel { get; set; }
        public string intro { get; set; }
        public string addr { get; set; }
        public string status_name { get; set; }
        public int status { get; set; }
        public decimal zz { get; set; }
        public decimal hz { get; set; }
    }
}
