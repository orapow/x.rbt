using System;
using System.Linq;
using X.App.Com;
using X.Core.Utility;

namespace X.App.Views.wx
{
    public class auth : xview
    {
        protected override int needus
        {
            get
            {
                return 0;
            }
        }
        protected override void InitDict()
        {
            base.InitDict();

            var auth_code = GetReqParms("auth_code");
            if (string.IsNullOrEmpty(auth_code)) Context.Response.Redirect(Wx.Open.Get_AuthUrl(Context.Server.UrlDecode("http://" + cfg.domain + Context.Request.RawUrl)));

            var auth = Wx.Open.Get_AuthInfo(auth_code);

            var dbmp = DB.x_wxmp.SingleOrDefault(o => o.appid == auth.authorization_info.authorizer_appid);
            if (dbmp == null)
            {
                var mp = Wx.Open.Get_MpInfo(auth.authorization_info.authorizer_appid);
                dbmp = new Data.x_wxmp()
                {
                    access_refresh_token = auth.authorization_info.authorizer_refresh_token,
                    access_token = auth.authorization_info.authorizer_access_token,
                    appid = auth.authorization_info.authorizer_appid,
                    expires_in = auth.authorization_info.expires_in,
                    service_type_info = mp.authorizer_info.service_type_info.id,
                    verify_type_info = mp.authorizer_info.verify_type_info.id,
                    alias = mp.authorizer_info.alias,
                    head_img = mp.authorizer_info.head_img,
                    nick_name = mp.authorizer_info.nick_name,
                    user_name = mp.authorizer_info.user_name,
                    token_time = DateTime.Now,
                    open_card = mp.authorizer_info.business_info.open_card,
                    open_pay = mp.authorizer_info.business_info.open_pay,
                    open_scan = mp.authorizer_info.business_info.open_scan,
                    open_shake = mp.authorizer_info.business_info.open_shake,
                    open_store = mp.authorizer_info.business_info.open_store,
                    qrcode_url = mp.authorizer_info.qrcode_url,
                    user_id = cu.user_id,
                    ctime = DateTime.Now
                };
                if (mp.authorization_info.func_info != null)
                {
                    var func = ",";
                    foreach (var t in mp.authorization_info.func_info) func += t.id + ",";
                    dbmp.func_info = func;
                }
                DB.x_wxmp.InsertOnSubmit(dbmp);
            }
            else
            {
                dbmp.access_refresh_token = auth.authorization_info.authorizer_refresh_token;
                dbmp.access_token = auth.authorization_info.authorizer_access_token;
                dbmp.appid = auth.authorization_info.authorizer_appid;
                dbmp.expires_in = auth.authorization_info.expires_in;
            }
            SubmitDBChanges();
            Context.Response.Redirect("/app/mp/list.html");
        }
    }
}
