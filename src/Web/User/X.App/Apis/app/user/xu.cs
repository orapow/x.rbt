using System;
using System.Collections.Generic;
using System.Linq;
using X.Core.Cache;
using X.Data;
using X.Web;

namespace X.App.Apis.app.user
{
    public class xu : xap
    {
        protected x_user us = null;
        public string ukey { get; set; }

        protected override void Validate()
        {
            base.Validate();
            if (need_user) Validator.Require("ukey", ukey);
        }

        protected virtual bool need_user { get { return false; } }

        protected override void InitApi()
        {
            base.InitApi();
            if (!string.IsNullOrEmpty(ukey))
            {
                us = DB.x_user.SingleOrDefault(o => o.ukey == ukey);
                if (us == null && need_user) throw new XExcep("T用户登陆超时");
                if (us != null & lng > 0 && lat > 0)
                {
                    us.last_lng = lng;
                    us.last_lat = lat;
                    us.last_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    SubmitDBChanges();
                }
            }
        }
    }
}
