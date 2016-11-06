using System;
using System.Collections.Generic;
using System.Web;
using X.App.Com;
using X.Core.Cache;
using X.Core.Utility;

namespace X.App.Views.wx
{
    public class _wx : xview
    {
        /// <summary>
        /// 0、无
        /// 1、openid
        /// 2、userinfo
        /// </summary>
        public virtual int needwx { get { return 1; } }
        protected string opid { get; set; }
        protected Wx.web_token tk = null;
        protected override void InitView()
        {
            base.InitView();

            if (needwx == 1)
            {
                var code = GetReqParms("code");
                var cu_key = GetReqParms("cu_key");
                if (string.IsNullOrEmpty(cu_key)) cu_key = Secret.MD5(Guid.NewGuid().ToString());
                opid = CacheHelper.Get<string>(cu_key);
                if (!string.IsNullOrEmpty(code))
                {
                    tk = Wx.GetWebToken(cfg.wx_appid, cfg.wx_scr, code);
                    if (!string.IsNullOrEmpty(tk.errcode)) ToWxUrl("snsapi_base");
                    opid = tk.openid;
                    Context.Response.Cookies.Add(new HttpCookie("cu_key", cu_key));
                }
                else if (string.IsNullOrEmpty(opid))
                {
                    ToWxUrl("snsapi_base");
                }
                CacheHelper.Save(cu_key, opid, 60 * 20);
            }
        }

        protected void ToWxUrl(string scope)
        {
            var url = Context.Request.RawUrl.Split('?')[0];
            Context.Response.Redirect("https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + cfg.wx_appid + "&redirect_uri=" + Context.Server.UrlEncode("http://" + cfg.domain + url) + "&response_type=code&scope=" + scope + "&state=" + Tools.GetRandRom(6, 3) + "#wechat_redirect");
            Context.Response.End();
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
