using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using X.App.Com;
using X.Core.Cache;
using X.Core.Utility;

namespace X.App.Views.wx
{
    public class _wx : xview
    {
        protected string opid { get; set; }
        protected Wx.web_token tk = null;

        protected override bool needus
        {
            get
            {
                return false;
            }
        }

        protected override void InitView()
        {

            base.InitView();
            if (cu == null)
            {
                var code = GetReqParms("code");
                if (string.IsNullOrEmpty(code)) ToWxUrl("snsapi_base");
                else
                {
                    tk = Wx.GetWebToken(cfg.wx_appid, cfg.wx_scr, code);
                    if (string.IsNullOrEmpty(tk.errcode)) opid = tk.openid;
                }

                if (string.IsNullOrEmpty(opid)) ToWxUrl("snsapi_base");

                cu = DB.x_user.FirstOrDefault(o => o.openid == opid);
                if (cu == null) { cu = new Data.x_user() { ctime = DateTime.Now, openid = opid, wxcount = 2, akey = Secret.SHA256(Guid.NewGuid().ToString()) }; }

                cu.ukey = Secret.MD5(Guid.NewGuid().ToString());
                cu.last_time = DateTime.Now;

                if (tk.scope == "snsapi_base" && (cu.user_id == 0 || cu.last_time < DateTime.Now.AddDays(-7) || cu.nickname == null)) ToWxUrl("snsapi_userinfo");//新用户或超过7天的用户重新拉取用户信息

                Wx.User.uinfo u = null;
                if (tk.scope == "snsapi_userinfo") u = Wx.User.GetUserInfo(opid, tk.access_token, true);

                if (u != null)
                {
                    cu.city = u.city;
                    cu.country = u.country;
                    cu.headimg = u.headimgurl;
                    cu.nickname = u.nickname;
                    cu.name = u.nickname;
                    cu.province = u.province;
                    cu.subscribe = u.subscribe;
                    cu.subscribe_time = u.subscribe_time;
                    cu.sex = u.sex;
                }
                if (cu.user_id == 0) DB.x_user.InsertOnSubmit(cu);
                SubmitDBChanges();
            }
        }

        protected void ToWxUrl(string scope)
        {
            var url = Context.Request.RawUrl.Split('?')[0];
            Context.Response.Redirect(Wx.GetWxLoginUrl(Context.Server.UrlEncode("http://" + cfg.domain + url), cfg.wx_appid, scope));
        }

        protected override void InitDict()
        {
            base.InitDict();
            var ts = Tools.GetGreenTime("");
            var no = Tools.GetRandRom(6);

            dict.Add("wx_appid", cfg.wx_appid);
            dict.Add("wx_ts", ts);
            dict.Add("wx_no", no);

            var tk = Wx.GetToken(cfg.wx_appid, cfg.wx_scr);
            var tick = Wx.GetJsTicket(tk);
            if (string.IsNullOrEmpty(tk)) tick = Wx.GetJsTicket(Wx.GetToken(cfg.wx_appid, cfg.wx_scr, true), true);

            var dt = new List<string>();
            dt.Add("noncestr=" + no);
            dt.Add("jsapi_ticket=" + tick);
            dt.Add("timestamp=" + ts);
            dt.Add("url=http://" + cfg.domain + Context.Request.RawUrl);
            dt.Sort();

            var sign = Secret.SHA1(string.Join("&", dt));

            dict.Add("wx_sign", sign.ToLower());
        }
    }
}
