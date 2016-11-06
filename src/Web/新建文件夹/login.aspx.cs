using Rbt.Data;
using System;
using System.Web.UI;
using System.Linq;
using X.Core.Utility;

namespace Rbt.Web
{
    public partial class login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var db = new RbtDBDataContext();

            var code = Request.QueryString["code"];
            if (string.IsNullOrEmpty(code)) Response.Redirect("https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + Wx.appid + "&redirect_uri=" + Server.UrlEncode(Request.Url.AbsoluteUri.Split('?')[0]) + "&response_type=code&scope=snsapi_base&state=" + Tools.GetRandRom(6, 3) + "#wechat_redirect");

            var tk = Wx.GetWebToken(code);

            var lg = db.x_logon.FirstOrDefault(o => o.status == 3 && (o.openid == null || o.openid == ""));
            lg.openid = tk.openid;
            db.SubmitChanges();
            qrocde.Src = lg.qrcode;

        }
    }
}