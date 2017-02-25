using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.App.Com;
using X.Web.Com;

namespace X.App.Apis.wx
{
    public class newtk : xapi
    {
        protected override XResp Execute()
        {
            Wx.GetToken(cfg.wx_appid, cfg.wx_scr, true);
            return new XResp();
        }
    }
}
