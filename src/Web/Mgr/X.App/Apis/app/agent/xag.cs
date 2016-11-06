using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;

namespace X.App.Apis.app.agent
{
    public class xag : xap
    {
        protected x_agent cag = null;
        public string ukey { get; set; }

        protected virtual bool need_user { get { return true; } }

        protected override void InitApi()
        {
            base.InitApi();
            if (!string.IsNullOrEmpty(ukey))
            {
                cag = DB.x_agent.SingleOrDefault(o => o.ukey == ukey);
                if (cag == null && need_user) throw new XExcep("T用户登陆超时");
            }
        }

    }
}
