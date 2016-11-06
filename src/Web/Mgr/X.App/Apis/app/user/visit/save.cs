using System;
using System.Collections.Generic;
using System.Linq;
using X.App.Com;
using X.Core.Plugin;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.app.user.visit
{
    public class save : xu
    {
        /// <summary>
        /// 合作id
        /// </summary>
        public int cid { get; set; }
        /// <summary>
        /// 类型
        /// 1、咨询
        /// 2、带看
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 用户类型
        /// 1、独立经纪人
        /// 2、普通经纪人
        /// 3、C端用户
        /// </summary>
        public int utype { get; set; }
        /// <summary>
        /// 图类型
        /// </summary>
        public string pt { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        public string img { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string tel { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }

        protected override void Validate()
        {
            base.Validate();
            Validator.CheckRange("cid", cid, 1, null);
            if (type == 0) type = 1;
            //if (type == 2)
            //{
            //    Validator.Require("pt", pt);
            //    Validator.Require("img", img);
            //}
        }

        protected override bool need_user
        {
            get
            {
                return true;
            }
        }

        protected override XResp Execute()
        {
            if (us.level != 10) throw new XExcep("T用户权限不够");
            var c = DB.x_coop.SingleOrDefault(o => o.coop_id == cid);
            if (c == null) throw new XExcep("T房源不存在");

            var v = new x_visit()
            {
                coop_id = cid,
                ctime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                dist = (decimal)(DB.fnGetDistance((float?)lng, (float?)lat, (float?)c.longitude, (float?)c.latitude) / 1000),
                img = img,
                name = name,
                tel = tel,
                utype = utype,
                lat = lat,
                lng = lng,
                pt = pt,
                remark = remark,
                type = type,
                user_id = us.user_id
            };

            DB.x_visit.InsertOnSubmit(v);

            SubmitDBChanges();

            return new XResp();

        }
    }
}
