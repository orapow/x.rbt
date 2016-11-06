using System;
using System.Collections.Generic;
using System.Linq;
using X.Core.Utility;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.app.user
{
    public class login : xu
    {
        public string tel { get; set; }
        public string pwd { get; set; }

        protected override void Validate()
        {
            base.Validate();
            Validator.Require("tel", tel);
            Validator.Require("pwd", pwd);
        }

        protected override XResp Execute()
        {
            var u = DB.x_user.FirstOrDefault(o => o.tel == tel);
            if (u == null) throw new XExcep("T手机号和密码不匹配");
            if (u.pwd != pwd) throw new XExcep("T手机号和密码不匹配");

            u.last_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            u.last_lng = lng;
            u.last_lat = lat;
            u.ukey = Tools.GetRandRom(32, 3);

            var r = new us()
            {
                id = u.id,
                image = u.image,
                ukey = u.ukey,
                name = u.name,
                tel = tel,
                level = u.level,
                range = u.range,
                region = u.region,
                pwd = u.pwd,
                auth_status = u.auth_status
            };

            SubmitDBChanges();

            return r;
        }
    }
    public class us : XResp
    {
        public string name { get; set; }
        public string pwd { get; set; }
        public int id { get; set; }
        /// <summary>
        /// 认证状态：
        /// 1、待认证
        /// 2、已认证
        /// 3、不通过
        /// </summary>
        public int? auth_status { get; set; }
        public int? level { get; set; }
        public int? range { get; set; }
        public string range_name { get { return x_com.GetDictName("coop.qy", range); } }
        public int? region { get; set; }
        public string region_name { get { return x_com.GetDictName("coop.qy", region); } }
        public string image { get; set; }
        public string tel { get; set; }
        public string ukey { get; set; }
    }
}
