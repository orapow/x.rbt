using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web.Com;

namespace X.App.Apis.app.user
{
    public class update : xu
    {
        /// <summary>
        /// 头像
        /// </summary>
        public string img { get; set; }
        /// <summary>
        /// 区域
        /// </summary>
        public int range { get; set; }
        /// <summary>
        /// 商圈
        /// </summary>
        public int region { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 密码
        /// 不为空时视为修改密码
        /// </summary>
        public string pwd { get; set; }

        protected override bool need_user
        {
            get
            {
                return true;
            }
        }

        protected override Web.Com.XResp Execute()
        {

            if (!string.IsNullOrEmpty(img)) us.image = img;
            if (range > 0) us.range = range;
            if (region > 0) us.region = region;
            if (!string.IsNullOrEmpty(name)) us.name = name;

            if (!string.IsNullOrEmpty(pwd)) us.pwd = pwd;

            SubmitDBChanges();

            var r = new us()
            {
                id = us.id,
                image = us.image,
                ukey = us.ukey,
                name = us.name,
                tel = us.tel,
                range = us.range,
                region = us.region,
                pwd = us.pwd,
                auth_status = us.auth_status
            };

            return r;
        }
    }
}
