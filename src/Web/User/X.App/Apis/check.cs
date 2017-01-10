﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using X.Core.Cache;
using X.Core.Utility;
using X.Web;
using X.Web.Com;

namespace X.App.Apis
{
    public class check : xapi
    {
        protected override bool needus
        {
            get
            {
                return false;
            }
        }

        public string akey { get; set; }

        protected override XResp Execute()
        {
            if (cu != null) return new XResp();

            cu = DB.x_user.FirstOrDefault(o => o.akey == akey);
            if (cu == null) throw new XExcep("0x0006");

            if (string.IsNullOrEmpty(cu.ukey))
            {
                cu.ukey = Secret.MD5(Guid.NewGuid().ToString());
                SubmitDBChanges();
            }

            return new XResp() { msg = cu.ukey };
        }

    }
}
