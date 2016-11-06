using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Core.Cache;
using X.Data;

namespace X.App.Apis.wx.bw
{
    public class _bw : xapi
    {
        protected string opid { get; set; }
        protected x_wx_user cu { get; set; }
        protected override void InitApi()
        {
            var cu_key = GetReqParms("cu_key");
            opid = CacheHelper.Get<string>(cu_key);
            cu = DB.x_wx_user.FirstOrDefault(o => o.openid == opid);
            base.InitApi();
        }
        protected override void Validate()
        {
            base.Validate();
            Validator.Require("opid", opid);
        }
    }
}
