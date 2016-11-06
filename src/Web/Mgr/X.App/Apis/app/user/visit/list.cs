using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.app.user.visit
{
    public class list : xu
    {
        /// <summary>
        /// 合作id
        /// </summary>
        public int cid { get; set; }
        public int page { get; set; }
        public int limit { get; set; }

        protected override void Validate()
        {
            base.Validate();
            Validator.CheckRange("cid", cid, 1, null);
        }

        protected override XResp Execute()
        {
            var q = from v in DB.x_visit
                    where v.coop_id == cid
                    orderby v.ctime descending
                    select new
                    {
                        v.ctime,
                        v.ctime_name,
                        v.remark,
                        v.type,
                        v.type_name,
                        v.utype,
                        v.utype_name,
                        v.name,
                        v.tel,
                        uname = v.x_user.name,
                        utel = v.x_user.tel,
                        uimage = v.x_user.image
                    };

            var r = new Resp_List();
            r.page = page;
            r.count = q.Count();
            r.items = q.Skip((page - 1) * limit).Take(limit).ToList();
            return r;
        }
    }
}
