using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using X.App.Com;
using X.Core.Utility;
using X.Data;
using X.Core.Cache;

namespace X.App.Views.wx
{
    public class _wx : xview
    {
        protected string opid { get; set; }
        protected Wx.web_token tk = null;

        /// <summary>
        /// 0、无
        /// 1、仅openid
        /// 2、用户信息
        /// 3、需存储用户信息
        /// </summary>
        protected override int needus
        {
            get
            {
                return 2;
            }
        }

        protected override void getUser(string key)
        {
            base.getUser(key);

            if (cu == null)
            {
                var code = GetReqParms("code");
                if (string.IsNullOrEmpty(code)) ToWxUrl("snsapi_base");
                else
                {
                    tk = Wx.Open.GetMpToken(Mp.appid, code); //Wx.GetWebToken("wx1f16fec4e8028bad", cfg.wx_scr, code);
                    if (string.IsNullOrEmpty(tk.errcode)) opid = tk.openid;
                }
                if (string.IsNullOrEmpty(opid)) ToWxUrl("snsapi_base");
                if (needus == 3) cu = DB.x_user.FirstOrDefault(o => o.openid == opid);
            }

            if (cu == null && tk != null && tk.scope == "snsapi_base" && needus > 1) ToWxUrl("snsapi_userinfo");

            if (cu == null)
            {
                cu = new x_user() { openid = opid, ukey = Secret.MD5(Guid.NewGuid().ToString()) };
                Wx.User.uinfo u = null;
                if (tk != null && tk.scope == "snsapi_userinfo") u = Wx.User.GetUserInfo(opid, tk.access_token, true);
                if (needus == 3)
                {
                    cu.wxcount = 2;
                    cu.akey = Secret.SHA256(Guid.NewGuid().ToString());
                    cu.city = u.city;
                    cu.country = u.country;
                    cu.headimg = u.headimgurl;
                    cu.nickname = u.nickname;
                    cu.name = u.nickname;
                    cu.province = u.province;
                    cu.subscribe = u.subscribe;
                    cu.subscribe_time = u.subscribe_time;
                    cu.sex = u.sex;
                    DB.x_user.InsertOnSubmit(cu);
                    SubmitDBChanges();
                }
                else if (needus == 2)
                {
                    cu.headimg = u.headimgurl;
                    cu.nickname = u.nickname;
                }
            }
        }

        protected virtual x_wxmp Mp
        {
            get { return DB.x_wxmp.FirstOrDefault(o => o.appid == cfg.wx_appid); }
        }

        //protected override void InitView()
        //{
        //    base.InitView();

        //    //var tick = Wx.GetJsTicket(Mp.access_token);
        //    //if (string.IsNullOrEmpty(tick))
        //    //{
        //    //    var tk = Wx.Open.Get_MpAuth_Token(Mp.appid, Mp.access_refresh_token);
        //    //    if (tk.errcode == "42002") ToWxUrl("snsapi_userinfo");
        //    //    Mp.access_token = tk.authorizer_access_token;
        //    //    Mp.access_refresh_token = tk.authorizer_refresh_token;
        //    //    tick = Wx.GetJsTicket(Mp.access_token);
        //    //}

        //    //{
        //    //    if (needus == 3) cu = new x_user() { ctime = DateTime.Now, openid = opid, wxcount = 2, akey = Secret.SHA256(Guid.NewGuid().ToString()) };
        //    //}

        //    //if (string.IsNullOrEmpty(cu.ukey)) cu.ukey = Secret.MD5(Guid.NewGuid().ToString());
        //    //cu.last_time = DateTime.Now;

        //    //if (&& (cu.user_id == 0 || cu.last_time < DateTime.Now.AddDays(-7) || cu.nickname == null) && Mp.appid != "wx1f16fec4e8028bad") ToWxUrl("snsapi_userinfo");//新用户或超过7天的用户重新拉取用户信息

        //    //Wx.User.uinfo u = null;
        //    //if (tk != null && tk.scope == "snsapi_userinfo") u = Wx.User.GetUserInfo(opid, tk.access_token, true);

        //    //if (u != null)
        //    //{
        //    //    cu.city = u.city;
        //    //    cu.country = u.country;
        //    //    cu.headimg = u.headimgurl;
        //    //    cu.nickname = u.nickname;
        //    //    cu.name = u.nickname;
        //    //    cu.province = u.province;
        //    //    cu.subscribe = u.subscribe;
        //    //    cu.subscribe_time = u.subscribe_time;
        //    //    cu.sex = u.sex;
        //    //}

        //    //var tick = Wx.GetJsTicket(Mp.access_token);
        //    //if (string.IsNullOrEmpty(tick))
        //    //{
        //    //    var tk = Wx.Open.Get_MpAuth_Token(Mp.appid, Mp.access_refresh_token);
        //    //    if (tk.errcode == "42002") ToWxUrl("snsapi_userinfo");
        //    //    Mp.access_token = tk.authorizer_access_token;
        //    //    Mp.access_refresh_token = tk.authorizer_refresh_token;
        //    //    tick = Wx.GetJsTicket(Mp.access_token);
        //    //}

        //    //if (cu.user_id == 0) DB.x_user.InsertOnSubmit(cu);
        //    //SubmitDBChanges();
        //    //Context.Response.SetCookie(new HttpCookie("ukey", cu.ukey));
        //}

        protected void ToWxUrl(string scope)
        {
            var url = Context.Request.RawUrl.Split('?')[0];
            var wxurl = Wx.Open.GetWxLoginUrl(Context.Server.UrlEncode("http://" + cfg.domain + url), Mp.appid, scope);
            Context.Response.Redirect(wxurl);
        }

        protected override void InitDict()
        {
            base.InitDict();
            var ts = Tools.GetGreenTime("");
            var no = Tools.GetRandRom(6);

            if (Mp.token_time.Value.AddSeconds(Mp.expires_in.Value) <= DateTime.Now)
            {
                var tk = Wx.Open.Get_MpAuth_Token(Mp.appid, Mp.access_refresh_token);
                Mp.access_token = tk.authorizer_access_token;
                Mp.access_refresh_token = tk.authorizer_refresh_token;
                Mp.expires_in = tk.expires_in;
                SubmitDBChanges();
            }

            var tick = Wx.GetJsTicket(Mp.access_token, true);

            dict.Add("wx_ts", ts);
            dict.Add("wx_no", no);
            dict.Add("wx_appid", Mp.appid);
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
