using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using X.Core.Cache;
using X.Core.Plugin;
using X.Core.Utility;

namespace X.App.Com
{
    public class Wx
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="appid"></param>
        /// <param name="scope">
        /// snsapi_base
        /// snsapi_userinfo
        /// </param>
        /// <returns></returns>
        public static string GetWxLoginUrl(string url, string appid, string scope)
        {
            return "https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + appid + "&redirect_uri=" + url + "&response_type=code&scope=" + scope + "&state=" + Tools.GetRandRom(6, 3) + "#wechat_redirect";
        }

        public static string GetJsTicket(string tk)
        {
            return GetJsTicket(tk, false);
        }
        public static string GetJsTicket(string tk, bool isnew)
        {
            var tick = CacheHelper.Get<string>("wx.js_ticket");
            if (string.IsNullOrEmpty(tick) || isnew)
            {
                var json = Tools.GetHttpData("https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token=" + tk + "&type=jsapi");
                var tkn = Serialize.FromJson<tick>(json);
                tick = tkn.ticket;
                CacheHelper.Save("wx.js_ticket", tick, tkn.expires_in - 500);
            }
            return tick;
        }

        public static string GetToken(string appid, string sec, bool isnew)
        {
            var tk = CacheHelper.Get<string>("wx.access_token");
            if (string.IsNullOrEmpty(tk) || isnew)
            {
                var json = Tools.GetHttpData("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + appid + "&secret=" + sec);
                Debug.WriteLine("getToken->" + json);
                var tke = Serialize.FromJson<token>(json);
                tk = tke.access_token;
                CacheHelper.Save("wx.access_token", tk, tke.expires - 500);
            }
            return tk;
        }
        public static string GetToken(string appid, string sec)
        {
            return GetToken(appid, sec, false);
        }

        static string RefreshToken(string appid)
        {
            var re_tk = CacheHelper.Get<string>("wx.refresh_token");
            if (string.IsNullOrEmpty(re_tk)) return "";
            var json = Tools.GetHttpData("https://api.weixin.qq.com/sns/oauth2/refresh_token?appid=" + appid + "&grant_type=refresh_token&refresh_token=" + re_tk);
            var rtk = Serialize.FromJson<web_token>(json);
            if (!string.IsNullOrEmpty(rtk.errcode)) Loger.Error(json);
            re_tk = rtk.refresh_token;
            CacheHelper.Save("wx.refresh_token", rtk.refresh_token);
            return rtk.access_token;
        }

        public static string ToSign(Dictionary<string, string> ps, bool backxml, string mch_key)
        {
            var sign_str = new StringBuilder();
            var xml_data = new StringBuilder();
            xml_data.Append("<xml>");
            foreach (var d in ps.OrderBy(o => o.Key))
            {
                sign_str.Append(d.Key + "=" + d.Value + "&");
                xml_data.Append("<" + d.Key + ">" + d.Value + "</" + d.Key + ">");
            }
            sign_str.Append("key=" + mch_key);
            var sign = Secret.MD5(sign_str.ToString().TrimEnd('&'), 0).ToUpper();
            xml_data.Append("<sign>" + sign + "</sign>");
            xml_data.Append("</xml>");
            return backxml ? xml_data.ToString() : sign;
        }

        public static web_token GetWebToken(string appid, string sec, string code)
        {
            var json = Tools.GetHttpData("https://api.weixin.qq.com/sns/oauth2/access_token?appid=" + appid + "&secret=" + sec + "&code=" + code + "&grant_type=authorization_code");
            var token = Serialize.FromJson<web_token>(json);
            if (!string.IsNullOrEmpty(token.errcode)) { Loger.Error(json); }
            return token;
        }

        public class Open
        {
            private static string ticket_file = HttpContext.Current.Server.MapPath("/dat/wxmp.x");
            /// <summary>
            /// 微信推送Key
            /// </summary>
            private static string verify_ticket
            {
                get
                {
                    var ticket = CacheHelper.Get<string>("wx.verify_ticket");
                    if (string.IsNullOrEmpty(ticket)) ticket = File.ReadAllText(ticket_file);
                    CacheHelper.Save("wx.verify_ticket", ticket, 5 * 60 * 1000);//有效期 10 分钟 此处存5分钟
                    return ticket;
                }
            }

            public static string appid = "wx55bfd5ad6998a826";
            private static string appsecret = "4b98cc985a194ede629f1599fe847134";

            /// <summary>
            /// 第三方令牌
            /// 需要做缓存
            /// </summary>
            /// <returns></returns>
            static string component_access_token()
            {
                var acc_token = CacheHelper.Get<string>("wx.component_access_token");
                if (!string.IsNullOrEmpty(acc_token)) return acc_token;

                var dict = new Dictionary<string, string>();
                dict.Add("component_appid", appid);
                dict.Add("component_appsecret", appsecret);
                dict.Add("component_verify_ticket", verify_ticket);

                var url = "https://api.weixin.qq.com/cgi-bin/component/api_component_token";

                var json = Tools.PostHttpData(url, Serialize.ToJson(dict));
                var token = Serialize.FromJson<component_token>(json);

                if (token == null || string.IsNullOrEmpty(token.component_access_token)) throw new WxExcep(json);
                CacheHelper.Save("wx.component_access_token", token.component_access_token, 7200);
                return token.component_access_token;
            }
            /// <summary>
            /// 获取预授权码
            /// </summary>
            /// <returns></returns>
            static string api_create_preauthcode()
            {
                var tk = component_access_token();
                if (string.IsNullOrEmpty(tk)) return "";

                var dict = new Dictionary<string, string>();
                dict.Add("component_appid", appid);

                var url = "https://api.weixin.qq.com/cgi-bin/component/api_create_preauthcode?component_access_token=" + tk;

                var json = Tools.PostHttpData(url, Serialize.ToJson(dict));
                var pre = Serialize.FromJson<create_preauthcode>(json);

                if (pre == null || string.IsNullOrEmpty(pre.pre_auth_code)) throw new WxExcep(json);

                return pre.pre_auth_code;

            }

            /// <summary>
            /// 设置验证Key
            /// </summary>
            /// <param name="ticket"></param>
            public static void SetVerify_Ticket(string verify_ticket)
            {
                File.WriteAllText(ticket_file, verify_ticket);
                CacheHelper.Save("wx.verify_ticket", verify_ticket, 5 * 60 * 1000);//有效期 10 分钟 此处存5分钟
            }

            /// <summary>
            /// 获取Mp用户授权地址
            /// </summary>
            /// <param name="url"></param>
            /// <param name="mpid"></param>
            /// <param name="scope"></param>
            /// <returns></returns>
            public static string GetWxLoginUrl(string url, string mpid, string scope)
            {
                return "https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + mpid + "&redirect_uri=" + url + "&response_type=code&scope=" + scope + "&state=" + Tools.GetRandRom(6, 3) + "&component_appid=" + appid + "#wechat_redirect";
            }

            /// <summary>
            /// 获MpToken
            /// </summary>
            /// <param name="mpid"></param>
            /// <param name="code"></param>
            /// <returns></returns>
            public static web_token GetMpToken(string mpid, string code)
            {
                var api = "https://api.weixin.qq.com/sns/oauth2/component/access_token?appid=" + mpid + "&code=" + code + "&grant_type=authorization_code&component_appid=" + appid + "&component_access_token=" + component_access_token();
                var json = Tools.GetHttpData(api);
                var wtk = Serialize.FromJson<web_token>(json);
                return Serialize.FromJson<web_token>(json);
            }

            /// <summary>
            /// 获取授权地址
            /// </summary>
            /// <param name="tourl"></param>
            /// <returns></returns>
            public static string Get_AuthUrl(string tourl)
            {
                return "https://mp.weixin.qq.com/cgi-bin/componentloginpage?component_appid=" + appid + "&pre_auth_code=" + api_create_preauthcode() + "&redirect_uri=" + tourl;
            }

            /// <summary>
            /// 接收消息
            /// </summary>
            /// <param name="tk_xml">
            /// 消息 xml
            /// </param>
            /// <returns></returns>
            public static Push Revice(string tk_xml, string sign, string nonce, string timestamp)
            {
                try
                {
                    var xml = Crypt.DecryptMsg(appid, sign, timestamp, nonce, tk_xml);
                    return new Push(xml);
                }
                catch (WxExcep wex)
                {
                    Loger.Error(wex);
                }
                return null;
            }

            #region RegionName
            /// <summary>
            /// 获取公众号授权信息
            /// </summary>
            /// <param name="auth_code">
            /// 查询授权码
            /// </param>
            /// <returns></returns>
            public static MpAuth_Info Get_AuthInfo(string auth_code)
            {
                var dict = new Dictionary<string, string>();
                dict.Add("component_appid", appid);
                dict.Add("authorization_code", auth_code);

                var json = Tools.PostHttpData("https://api.weixin.qq.com/cgi-bin/component/api_query_auth?component_access_token=" + component_access_token(), Serialize.ToJson(dict));
                var info = Serialize.FromJson<MpAuth_Info>(json);
                if (info == null || !string.IsNullOrEmpty(info.errmsg)) throw new WxExcep(json);
                return info;
            }

            /// <summary>
            /// 获取公众号信息
            /// </summary>
            /// <param name="auth_code"></param>
            public static MpInfo Get_MpInfo(string auth_appid)
            {
                var dict = new Dictionary<string, string>();
                dict.Add("component_appid", appid);
                dict.Add("authorizer_appid", auth_appid);

                var json = Tools.PostHttpData("https://api.weixin.qq.com/cgi-bin/component/api_get_authorizer_info?component_access_token=" + component_access_token(), Serialize.ToJson(dict));
                var info = Serialize.FromJson<MpInfo>(json);
                if (info == null || !string.IsNullOrEmpty(info.errmsg)) throw new WxExcep(json);
                return info;
            }

            /// <summary>
            /// 获取、刷新公众号授权令牌
            /// </summary>
            /// <param name="auth_appid"></param>
            /// <param name="re_token"></param>
            /// <returns></returns>
            public static MpAuth_Token Get_MpAuth_Token(string auth_appid, string re_token)
            {
                var dict = new Dictionary<string, string>();
                dict.Add("component_appid", appid);
                dict.Add("authorizer_appid", auth_appid);
                dict.Add("authorizer_refresh_token", re_token);

                var json = Tools.PostHttpData("https://api.weixin.qq.com/cgi-bin/component/api_authorizer_token?component_access_token=" + component_access_token(), Serialize.ToJson(dict));
                var tk = Serialize.FromJson<MpAuth_Token>(json);

                if (tk == null || !string.IsNullOrEmpty(tk.errmsg)) throw new WxExcep(json);
                return tk;
            }
            #endregion
            #region

            class component_token : mbase
            {
                public string component_access_token { get; set; }
                public int expires_in { get; set; }
            }
            class create_preauthcode : mbase
            {
                public string pre_auth_code { get; set; }
                public int expires_in { get; set; }
            }
            #endregion

            public class Push
            {
                public string AppId { get { return GetValue("AppId"); } }
                public string CreateTime { get { return GetValue("CreateTime"); } }
                public string InfoType { get { return GetValue("InfoType"); } }

                private Dictionary<string, string> ps = new Dictionary<string, string>();

                /// <summary>
                /// Initializes a new instance of the Push class.
                /// </summary>
                public Push(string xml)
                {
                    var doc = new XmlDocument();
                    doc.LoadXml(xml);
                    var root = doc.FirstChild;
                    foreach (XmlNode n in root.ChildNodes)
                    {
                        var v = "";
                        if (n.NodeType == XmlNodeType.CDATA) v = n.FirstChild.InnerText;
                        else v = n.InnerText;
                        ps.Add(n.Name, v);
                    }
                }

                public string GetValue(string name)
                {
                    if (ps.ContainsKey(name)) return ps[name];
                    return "";
                }
            }

            [XmlType("xml")]
            public class Un_Auth
            {
                public string AppId { get; set; }
                public string CreateTime { get; set; }
                public string InfoType { get; set; }
                public string AuthorizerAppid { get; set; }
            }

            /// <summary>
            /// 权限对象
            /// </summary>
            public class Func_Category
            {
                /// <summary>
                /// 1、消息与菜单权限集
                /// 2、用户管理权限集
                /// 3、帐号管理权限集
                /// 4、网页授权权限集
                /// 5、微信小店权限集
                /// 6、多客服权限集
                /// 7、业务通知权限集
                /// 8、微信卡券权限集
                /// 9、微信扫一扫权限集
                /// 10、微信连WIFI权限集
                /// 11、素材管理权限集
                /// 12、摇一摇周边权限集
                /// 13、微信门店权限集
                /// </summary>
                public int id { get; set; }
            }

            /// <summary>
            /// 公众号授权令牌
            /// </summary>
            public class MpAuth_Token : mbase
            {
                public string authorizer_access_token { get; set; }
                public string authorizer_refresh_token { get; set; }
                public int expires_in { get; set; }
            }

            /// <summary>
            /// 公众号信息
            /// </summary>
            public class MpInfo : mbase
            {
                public Authorizer authorizer_info { get; set; }
                public Authorization authorization_info { get; set; }
                public class Authorizer
                {
                    public string nick_name { get; set; }
                    public string head_img { get; set; }
                    public string user_name { get; set; }
                    public string alias { get; set; }
                    public string qrcode_url { get; set; }
                    public Func_Category service_type_info { get; set; }
                    public Func_Category verify_type_info { get; set; }
                    public Business business_info { get; set; }
                }

                public class Business
                {
                    /// <summary>
                    /// 是否开通微信门店功能
                    /// 0、代表未开通
                    /// 1、代表已开通
                    /// </summary>
                    public int open_store { get; set; }
                    /// <summary>
                    /// 是否开通微信扫商品功能
                    /// 0、代表未开通
                    /// 1、代表已开通
                    /// </summary>
                    public int open_scan { get; set; }
                    /// <summary>
                    /// 是否开通微信支付功能
                    /// 0、代表未开通
                    /// 1、代表已开通
                    /// </summary>
                    public int open_pay { get; set; }
                    /// <summary>
                    /// 是否开通微信卡券功能
                    /// 0、代表未开通
                    /// 1、代表已开通
                    /// </summary>
                    public int open_card { get; set; }
                    /// <summary>
                    /// 是否开通微信摇一摇功能
                    /// 0、代表未开通
                    /// 1、代表已开通
                    /// </summary>
                    public int open_shake { get; set; }
                }
            }

            /// <summary>
            /// 授权信息对象
            /// </summary>
            public class Authorization
            {
                public string appid { get; set; }
                /// <summary>
                /// 授权appid
                /// </summary>
                public string authorizer_appid { get; set; }
                /// <summary>
                /// 授权令牌
                /// </summary>
                public string authorizer_access_token { get; set; }
                /// <summary>
                /// 授权刷新令牌
                /// </summary>
                public string authorizer_refresh_token { get; set; }
                /// <summary>
                /// 过期时间 2 小时
                /// </summary>
                public int expires_in { get; set; }
                /// <summary>
                /// 权限集
                /// </summary>
                public List<Func_Category> func_info { get; set; }
            }

            /// <summary>
            /// 公众号授权信息
            /// </summary>
            public class MpAuth_Info : mbase
            {
                /// <summary>
                /// 授权信息
                /// </summary>
                public Authorization authorization_info { get; set; }
            }
        }

        public class Media
        {
            //public static string DownImage(string tk, string mmid)
            //{
            //    return Tools.DownImage("https://api.weixin.qq.com/cgi-bin/media/get?access_token=" + tk + "&media_id=" + mmid);
            //}
        }

        /// <summary>
        /// 用户相关
        /// </summary>
        public class User
        {
            /// <summary>
            /// 获取单个用户信息
            /// </summary>
            /// <param name="opid"></param>
            /// <param name="tk"></param>
            /// <param name="isweb"></param>
            /// <returns></returns>
            public static uinfo GetUserInfo(string opid, string tk, bool isweb)
            {
                var json = "";
                if (isweb) json = Tools.GetHttpData("https://api.weixin.qq.com/sns/userinfo?access_token=" + tk + "&openid=" + opid + "&lang=zh_CN");
                else json = Tools.GetHttpData("https://api.weixin.qq.com/cgi-bin/user/info?access_token=" + tk + "&openid=" + opid + "&lang=zh_CN");
                var ui = Serialize.FromJson<uinfo>(json);
                if (!string.IsNullOrEmpty(ui.errcode)) Loger.Error(json);
                return ui;
            }

            /// <summary>
            /// 获取单个用户信息
            /// </summary>
            /// <param name="opid"></param>
            /// <param name="tk"></param>
            /// <returns></returns>
            public static uinfo GetUserInfo(string opid, string tk)
            {
                return GetUserInfo(opid, tk, false);
            }
            /// <summary>
            /// 批量或取用户信息
            /// </summary>
            /// <param name="pus"></param>
            /// <param name="tk"></param>
            /// <returns></returns>
            public static users GetMutiUserInfo(List<object> pus, string tk)
            {
                var url = "https://api.weixin.qq.com/cgi-bin/user/info/batchget?access_token=" + tk;
                var json = Tools.PostHttpData(url, Serialize.ToJson(new { user_list = pus }));
                Loger.Info("getmutiuserinfo->" + Serialize.ToJson(new { user_list = pus }));
                var us = Serialize.FromJson<users>(json);
                if (!string.IsNullOrEmpty(us.errcode)) return null;
                return us;
            }

            public class users : mbase
            {
                public List<uinfo> user_info_list { get; set; }
            }

            public class uinfo : mbase
            {
                /// <summary>
                /// 用户是否订阅该公众号标识，值为0时，代表此用户没有关注该公众号，拉取不到其余信息。
                /// </summary>
                public int subscribe { get; set; }
                /// <summary>
                /// 用户的标识，对当前公众号唯一
                /// </summary>
                public string openid { get; set; }
                /// <summary>
                /// 用户的昵称
                /// </summary>
                public string nickname { get; set; }
                /// <summary>
                /// 用户的性别，值为1时是男性，值为2时是女性，值为0时是未知
                /// </summary>
                public int sex { get; set; }
                /// <summary>
                /// 用户所在城市
                /// </summary>
                public string city { get; set; }
                /// <summary>
                /// 用户所在国家
                /// </summary>
                public string country { get; set; }
                /// <summary>
                /// 用户所在省份
                /// </summary>
                public string province { get; set; }
                /// <summary>
                /// 用户的语言，简体中文为zh_CN
                /// </summary>
                public string language { get; set; }
                /// <summary>
                /// 用户头像，最后一个数值代表正方形头像大小（有0、46、64、96、132数值可选，0代表640*640正方形头像），用户没有头像时该项为空。若用户更换头像，原有头像URL将失效。
                /// </summary>
                public string headimgurl { get; set; }
                /// <summary>
                /// 用户关注时间，为时间戳。如果用户曾多次关注，则取最后关注时间
                /// </summary>
                public string subscribe_time { get; set; }
                /// <summary>
                /// 只有在用户将公众号绑定到微信开放平台帐号后，才会出现该字段。详见：获取用户个人信息（UnionID机制）
                /// </summary>
                public string unionid { get; set; }
                /// <summary>
                /// 公众号运营者对粉丝的备注，公众号运营者可在微信公众平台用户管理界面对粉丝添加备注
                /// </summary>
                public string remark { get; set; }
                /// <summary>
                /// 用户所在的分组ID
                /// </summary>
                public int groupid { get; set; }

            }
        }

        /// <summary>
        /// 支付相关
        /// </summary>
        public class Pay
        {
            public static Oxml MdOrder(string body, string order_no, string total, string notify_url, string openid, string appid, string mch_id, string mch_key, bool isapp)
            {
                var url = "https://api.mch.weixin.qq.com/pay/unifiedorder";

                var ps = new Dictionary<string, string>();
                ps.Add("appid", appid);
                ps.Add("mch_id", mch_id);
                ps.Add("nonce_str", Tools.GetRandRom(24, 3));
                ps.Add("body", body);
                ps.Add("out_trade_no", order_no);
                ps.Add("total_fee", total);
                ps.Add("spbill_create_ip", Tools.GetClientIP());
                ps.Add("notify_url", notify_url);
                ps.Add("trade_type", isapp ? "APP" : "JSAPI");//"NATIVE");
                ps.Add("openid", openid);
                var topostxml = ToSign(ps, true, mch_key);

                var odxml = Tools.PostHttpData(url, topostxml);
                odxml = odxml.Replace("xml", "Oxml");
                return Serialize.FormXml<Oxml>(odxml);
            }

            /// <summary>
            /// 验证回调参数
            /// </summary>
            /// <param name="nt"></param>
            /// <returns></returns>
            public static bool ValidNotify(Ntxml nt, string mch_id, string appid, string mch_key)
            {
                if (nt == null || string.IsNullOrEmpty(nt.sign)) return false;
                if (nt.mch_id != mch_id || nt.appid != appid) return false;
                var ps = new Dictionary<string, string>();
                ps.Add("appid", nt.appid);
                ps.Add("bank_type", nt.bank_type);
                ps.Add("cash_fee", nt.cash_fee);
                ps.Add("fee_type", nt.fee_type);
                ps.Add("is_subscribe", nt.is_subscribe);
                ps.Add("mch_id", nt.mch_id);
                ps.Add("nonce_str", nt.nonce_str);
                ps.Add("openid", nt.openid);
                ps.Add("out_trade_no", nt.out_trade_no);
                ps.Add("result_code", nt.result_code);
                ps.Add("return_code", nt.return_code);
                ps.Add("time_end", nt.time_end);
                ps.Add("total_fee", nt.total_fee);
                ps.Add("trade_type", nt.trade_type);
                ps.Add("transaction_id", nt.transaction_id);

                return (nt.sign.ToLower() == ToSign(ps, false, mch_key).ToLower());
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="appid"></param>
            /// <param name="mchid"></param>
            /// <param name="openid"></param>
            /// <param name="no"></param>
            /// <param name="amount">
            /// 付款金额（元）
            /// </param>
            /// <returns></returns>
            public static payrsp PayToOpenid(string appid, string mchid, string openid, string no, decimal amount, string cert_path, string signkey)
            {
                var dict = new Dictionary<string, string>();
                dict.Add("mch_appid", appid);
                dict.Add("mchid", mchid);
                dict.Add("nonce_str", Tools.GetRandRom(32, 3));
                dict.Add("partner_trade_no", no);
                dict.Add("openid", openid);
                dict.Add("check_name", "NO_CHECK");
                dict.Add("amount", amount * 100 + "");
                dict.Add("desc", "提现红包：" + no + "，提现金额：" + amount + "元");
                dict.Add("spbill_create_ip", "120.26.215.240");
                var to_md5 = "";
                var xml_data = "<xml>";
                foreach (var d in dict.OrderBy(o => o.Key))
                {
                    if (!string.IsNullOrEmpty(d.Value)) to_md5 += d.Key + "=" + d.Value + "&";
                    xml_data += "<" + d.Key + ">" + d.Value + "</" + d.Key + ">\n";
                }
                xml_data += "<sign>" + Secret.MD5(to_md5 + "key=" + signkey, 0).ToUpper() + "</sign>\n";
                xml_data += "</xml>";
                var xml = Tools.PostHttpData("https://api.mch.weixin.qq.com/mmpaymkttransfers/promotion/transfers", xml_data, "POST", cert_path + "apiclient_cert.p12", mchid);
                return Serialize.FormXml<payrsp>(xml);
            }

            /// <summary>
            /// 下单XML
            /// </summary>
            public class Oxml : xml
            {
                /// <summary>
                /// 调用接口提交的公众账号ID
                /// </summary>
                public string appid { get; set; }
                /// <summary>
                /// 调用接口提交的商户号
                /// </summary>
                public string mch_id { get; set; }
                /// <summary>
                /// 调用接口提交的终端设备号，
                /// </summary>
                public string device_info { get; set; }
                /// <summary>
                /// 微信返回的随机字符串
                /// </summary>
                public string nonce_str { get; set; }
                /// <summary>
                /// 微信返回的签名，详见签名算法
                /// </summary>
                public string sign { get; set; }
                /// <summary>
                /// SUCCESS	SUCCESS/FAIL
                /// </summary>
                public string result_code { get; set; }
                /// <summary>
                /// 详细参见第6节错误列表
                /// </summary>
                public string err_code { get; set; }
                /// <summary>
                /// 错误返回的信息描述
                /// </summary>
                public string err_code_des { get; set; }
                /// <summary>
                /// 调用接口提交的交易类型，取值如下：JSAPI，NATIVE，APP，详细说明见参数规定
                /// </summary>
                public string trade_type { get; set; }
                /// <summary>
                /// 微信生成的预支付回话标识，用于后续接口调用中使用，该值有效期为2小时
                /// </summary>
                public string prepay_id { get; set; }
                /// <summary>
                /// trade_type为NATIVE是有返回，可将该参数值生成二维码展示出来进行扫码支付
                /// </summary>
                public string code_url { get; set; }
            }

            /// <summary>
            /// 退款XML
            /// </summary>
            public class Ruxml : xml
            {
                /// <summary>
                /// SUCCESS退款申请接收成功，结果通过退款查询接口查询
                /// FAIL 提交业务失败
                /// </summary>
                public string result_code { get; set; }
                /// <summary>
                /// 错误代码
                /// </summary>
                public string err_code { get; set; }
                /// <summary>
                /// 结果信息描述
                /// </summary>
                public string err_code_des { get; set; }
                /// <summary>
                /// 微信分配的公众账号ID
                /// </summary>
                public string appid { get; set; }
                /// <summary>
                /// 微信支付分配的商户号
                /// </summary>
                public string mch_id { get; set; }
                /// <summary>
                /// 微信支付分配的终端设备号，与下单一致
                /// </summary>
                public string device_info { get; set; }
                /// <summary>
                /// 随机字符串，不长于32位
                /// </summary>
                public string nonce_str { get; set; }
                /// <summary>
                /// 签名，详见签名算法
                /// </summary>
                public string sign { get; set; }
                /// <summary>
                /// 微信订单号
                /// </summary>
                public string transaction_id { get; set; }
                /// <summary>
                /// 商户系统内部的订单号
                /// </summary>
                public string out_trade_no { get; set; }
                /// <summary>
                /// 商户退款单号
                /// </summary>
                public string out_refund_no { get; set; }
                /// <summary>
                /// 微信退款单号
                /// </summary>
                public string refund_id { get; set; }
                /// <summary>
                /// ORIGINAL—原路退款
                /// BALANCE—退回到余额
                /// </summary>
                public string refund_channel { get; set; }
                /// <summary>
                /// 退款总金额,单位为分,可以做部分退款
                /// </summary>
                public string refund_fee { get; set; }
                /// <summary>
                /// 订单总金额，单位为分，只能为整数，详见支付金额
                /// </summary>
                public string total_fee { get; set; }
                /// <summary>
                /// CNY	订单金额货币类型，符合ISO 4217标准的三位字母代码，默认人民币：CNY，其他值列表详见货币类型
                /// </summary>
                public string fee_type { get; set; }
                /// <summary>
                /// 现金支付金额，单位为分，只能为整数，详见支付金额
                /// </summary>
                public string cash_fee { get; set; }
                /// <summary>
                /// 现金退款金额，单位为分，只能为整数，详见支付金额
                /// </summary>
                public string cash_refund_fee { get; set; }
                /// <summary>
                /// 代金券或立减优惠退款金额=订单金额-现金退款金额，注意：立减优惠金额不会退回
                /// </summary>
                public string coupon_refund_fee { get; set; }
                /// <summary>
                /// 代金券或立减优惠使用数量
                /// </summary>
                public string coupon_refund_count { get; set; }
                /// <summary>
                /// SUCCESS退款申请接收成功，结果通过退款查询接口查询
                /// FAIL 提交业务失败
                /// </summary>
                public string coupon_refund_id { get; set; }
            }

            /// <summary>
            /// 闭关订单XML
            /// </summary>
            public class Clxml : xml
            {
                /// <summary>
                /// 微信分配的公众账号ID
                /// </summary>
                public string appid { get; set; }
                /// <summary>
                /// 微信支付分配的商户号
                /// </summary>
                public string mch_id { get; set; }
                /// <summary>
                /// 随机字符串，不长于32位
                /// </summary>
                public string nonce_str { get; set; }
                /// <summary>
                /// 签名，验证签名算
                /// </summary>
                public string sign { get; set; }
                /// <summary>
                /// 详细参见第6节错误列表
                /// </summary>
                public string err_code { get; set; }
                /// <summary>
                /// 结果信息描述
                /// </summary>
                public string err_code_des { get; set; }
            }

            /// <summary>
            /// 支付回调XML
            /// </summary>
            public class Ntxml : xml
            {
                public string appid { get; set; }
                public string bank_type { get; set; }
                public string cash_fee { get; set; }
                public string fee_type { get; set; }
                public string is_subscribe { get; set; }
                public string mch_id { get; set; }
                public string nonce_str { get; set; }
                public string openid { get; set; }
                public string out_trade_no { get; set; }
                public string result_code { get; set; }
                public string sign { get; set; }
                public string time_end { get; set; }
                public string total_fee { get; set; }
                public string trade_type { get; set; }
                public string transaction_id { get; set; }
            }

            [XmlType("xml")]
            public class payrsp
            {
                /// <summary>
                /// 返回状态码
                /// SUCCESS/FAIL
                /// 此字段是通信标识，非交易标识，交易是否成功需要查看result_code来判断
                /// </summary>
                public string return_code { get; set; }
                /// <summary>
                /// 返回信息
                /// </summary>
                public string return_msg { get; set; }
                /// <summary>
                /// 微信分配的公众账号ID（企业号corpid即为此appId）
                /// </summary>
                public string mch_appid { get; set; }
                /// <summary>
                /// 微信支付分配的商户号
                /// </summary>
                public string mchid { get; set; }
                /// <summary>
                /// 微信支付分配的终端设备号
                /// </summary>
                public string device_info { get; set; }
                /// <summary>
                /// 随机字符串，不长于32位
                /// </summary>
                public string nonce_str { get; set; }
                /// <summary>
                /// SUCCESS/FAIL
                /// </summary>
                public string result_code { get; set; }
                /// <summary>
                /// 错误码信息
                /// </summary>
                public string err_code { get; set; }
                /// <summary>
                /// 结果信息描述
                /// </summary>
                public string err_code_des { get; set; }
                /// <summary>
                /// 商户订单号
                /// 我们使用返现卡卡号
                /// </summary>
                public string partner_trade_no { get; set; }
                /// <summary>
                /// 微信订单号
                /// </summary>
                public string payment_no { get; set; }
                /// <summary>
                /// 微信支付成功时间
                /// </summary>
                public string payment_time { get; set; }
            }
        }

        public class Account
        {
            public static string GetQrcode(string appid, string tk, string scene_id)
            {
                var api = "https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token=" + tk;
                var rsp = Tools.PostHttpData(api, "{\"expire_seconds\": 60, \"action_name\": \"QR_SCENE\", \"action_info\": {\"scene\": {\"scene_id\": \"" + scene_id + "\"} } }");
                var d = Serialize.JsonToDict(rsp);
                return "https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=" + d["ticket"];
            }
        }

        /// <summary>
        /// 消息
        /// </summary>
        public class Msg
        {
            public static MsgObj Get(string appid, string tk_xml, string sign, string nonce, string timestamp)
            {
                try
                {
                    var xml = tk_xml;
                    if (xml.IndexOf("<Encrypt>") > 0) xml = Crypt.DecryptMsg(appid, sign, timestamp, nonce, xml);
                    var obj = new MsgObj(xml);
                    return obj;
                }
                catch (WxExcep wex)
                {
                    Loger.Error(wex);
                    return null;
                }
            }

            public class MsgObj
            {
                public string FromUserName { get { return GetString("FromUserName"); } }
                public string ToUserName { get { return GetString("ToUserName"); } }
                public int CreateTime { get { return GetInt("CreateTime"); } }
                public string MsgType { get { return GetString("MsgType"); } }

                private Dictionary<string, string> dict = new Dictionary<string, string>();

                public string GetString(string name)
                {
                    if (dict.ContainsKey(name)) return dict[name];
                    return "";
                }

                public int GetInt(string name)
                {
                    if (dict.ContainsKey(name)) return dict[name] == null ? 0 : int.Parse(dict[name]);
                    return 0;
                }

                /// <summary>
                /// Initializes a new instance of the MsgObj class.
                /// </summary>
                public MsgObj(string xml)
                {
                    if (string.IsNullOrEmpty(xml)) return;
                    var doc = new XmlDocument();
                    doc.LoadXml(xml);
                    var root = doc.FirstChild;
                    foreach (XmlNode n in root.ChildNodes)
                    {
                        var v = "";
                        if (n.NodeType == XmlNodeType.CDATA) v = n.FirstChild.InnerText;
                        else v = n.InnerText;
                        dict.Add(n.Name, v);
                    }
                }

                public string ToXml(string appid)
                {
                    var sb_str = new StringBuilder();
                    sb_str.Append("<xml>");
                    foreach (var k in dict.Keys)
                    {
                        if ("CreateTime|Latitude|Longitude|Precision".IndexOf(k) < 0) sb_str.Append("<" + k + "><![CDATA[" + dict[k] + "]]></" + k + ">");
                        else sb_str.Append("<" + k + ">" + dict[k] + "</" + k + ">");
                    }
                    sb_str.Append("</xml>");
                    return Crypt.EncryptMsg(appid, sb_str.ToString(), Tools.GetGreenTime(""), Tools.GetRandRom(9, 3));
                }

                public void AddValue(string name, string value)
                {
                    dict.Add(name, value);
                }

            }
        }

        public class WxExcep : Exception
        {
            /// <summary>
            /// Initializes a new instance of the WxExcep class.
            /// </summary>
            public WxExcep(string msg)
                : base(msg)
            {
                if (msg.IndexOf("{") == 0) error = X.Core.Utility.Serialize.FromJson<Err>(msg);
                else error = new Err(msg);
            }

            public Err error { get; set; }

            public class Err
            {
                public string errcode { get; set; }
                public string errmsg { get; set; }
                /// <summary>
                /// Initializes a new instance of the Err class.
                /// </summary>
                public Err(string msg)
                {
                    errmsg = msg;
                }
            }
        }

        class AES
        {
            /// <summary>
            /// 解密方法
            /// </summary>
            /// <param name="Input">密文</param>
            /// <param name="EncodingAESKey"></param>
            /// <returns></returns>
            public static string Decrypt(String Input, string EncodingAESKey, ref string appid)
            {
                byte[] Key;
                Key = Convert.FromBase64String(EncodingAESKey + "=");
                byte[] Iv = new byte[16];
                Array.Copy(Key, Iv, 16);
                byte[] btmpMsg = AES_decrypt(Input, Iv, Key);

                int len = BitConverter.ToInt32(btmpMsg, 16);
                len = IPAddress.NetworkToHostOrder(len);


                byte[] bMsg = new byte[len];
                byte[] bAppid = new byte[btmpMsg.Length - 20 - len];
                Array.Copy(btmpMsg, 20, bMsg, 0, len);
                Array.Copy(btmpMsg, 20 + len, bAppid, 0, btmpMsg.Length - 20 - len);
                string oriMsg = Encoding.UTF8.GetString(bMsg);
                appid = Encoding.UTF8.GetString(bAppid);

                return oriMsg;
            }
            /// <summary>
            /// 加密方法
            /// </summary>
            /// <param name="Input"></param>
            /// <param name="EncodingAESKey"></param>
            /// <param name="appid"></param>
            /// <returns></returns>
            public static String Encrypt(String Input, string EncodingAESKey, string appid)
            {
                byte[] Key;
                Key = Convert.FromBase64String(EncodingAESKey + "=");
                byte[] Iv = new byte[16];
                Array.Copy(Key, Iv, 16);
                string Randcode = CreateRandCode(16);
                byte[] bRand = Encoding.UTF8.GetBytes(Randcode);
                byte[] bAppid = Encoding.UTF8.GetBytes(appid);
                byte[] btmpMsg = Encoding.UTF8.GetBytes(Input);
                byte[] bMsgLen = BitConverter.GetBytes(HostToNetworkOrder(btmpMsg.Length));
                byte[] bMsg = new byte[bRand.Length + bMsgLen.Length + bAppid.Length + btmpMsg.Length];

                Array.Copy(bRand, bMsg, bRand.Length);
                Array.Copy(bMsgLen, 0, bMsg, bRand.Length, bMsgLen.Length);
                Array.Copy(btmpMsg, 0, bMsg, bRand.Length + bMsgLen.Length, btmpMsg.Length);
                Array.Copy(bAppid, 0, bMsg, bRand.Length + bMsgLen.Length + btmpMsg.Length, bAppid.Length);

                return AES_encrypt(bMsg, Iv, Key);

            }

            #region 私有方法
            static UInt32 HostToNetworkOrder(UInt32 inval)
            {
                UInt32 outval = 0;
                for (int i = 0; i < 4; i++)
                    outval = (outval << 8) + ((inval >> (i * 8)) & 255);
                return outval;
            }
            static Int32 HostToNetworkOrder(Int32 inval)
            {
                Int32 outval = 0;
                for (int i = 0; i < 4; i++)
                    outval = (outval << 8) + ((inval >> (i * 8)) & 255);
                return outval;
            }
            static string CreateRandCode(int codeLen)
            {
                string codeSerial = "2,3,4,5,6,7,a,c,d,e,f,h,i,j,k,m,n,p,r,s,t,A,C,D,E,F,G,H,J,K,M,N,P,Q,R,S,U,V,W,X,Y,Z";
                if (codeLen == 0)
                {
                    codeLen = 16;
                }
                string[] arr = codeSerial.Split(',');
                string code = "";
                int randValue = -1;
                Random rand = new Random(unchecked((int)DateTime.Now.Ticks));
                for (int i = 0; i < codeLen; i++)
                {
                    randValue = rand.Next(0, arr.Length - 1);
                    code += arr[randValue];
                }
                return code;
            }
            static String AES_encrypt(String Input, byte[] Iv, byte[] Key)
            {
                var aes = new RijndaelManaged();
                //秘钥的大小，以位为单位
                aes.KeySize = 256;
                //支持的块大小
                aes.BlockSize = 128;
                //填充模式
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;
                aes.Key = Key;
                aes.IV = Iv;
                var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
                byte[] xBuff = null;

                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                    {
                        byte[] xXml = Encoding.UTF8.GetBytes(Input);
                        cs.Write(xXml, 0, xXml.Length);
                    }
                    xBuff = ms.ToArray();
                }
                String Output = Convert.ToBase64String(xBuff);
                return Output;
            }
            static String AES_encrypt(byte[] Input, byte[] Iv, byte[] Key)
            {
                var aes = new RijndaelManaged();
                //秘钥的大小，以位为单位
                aes.KeySize = 256;
                //支持的块大小
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.None;
                aes.Mode = CipherMode.CBC;
                aes.Key = Key;
                aes.IV = Iv;
                var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
                byte[] xBuff = null;

                #region 自己进行PKCS7补位，用系统自己带的不行
                byte[] msg = new byte[Input.Length + 32 - Input.Length % 32];
                Array.Copy(Input, msg, Input.Length);
                byte[] pad = KCS7Encoder(Input.Length);
                Array.Copy(pad, 0, msg, Input.Length, pad.Length);
                #endregion

                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                    {
                        cs.Write(msg, 0, msg.Length);
                    }
                    xBuff = ms.ToArray();
                }

                String Output = Convert.ToBase64String(xBuff);
                return Output;
            }
            static byte[] KCS7Encoder(int text_length)
            {
                int block_size = 32;
                // 计算需要填充的位数
                int amount_to_pad = block_size - (text_length % block_size);
                if (amount_to_pad == 0)
                {
                    amount_to_pad = block_size;
                }
                // 获得补位所用的字符
                char pad_chr = chr(amount_to_pad);
                string tmp = "";
                for (int index = 0; index < amount_to_pad; index++)
                {
                    tmp += pad_chr;
                }
                return Encoding.UTF8.GetBytes(tmp);
            }
            static char chr(int a)
            {

                byte target = (byte)(a & 0xFF);
                return (char)target;
            }
            static byte[] AES_decrypt(String Input, byte[] Iv, byte[] Key)
            {
                RijndaelManaged aes = new RijndaelManaged();
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.None;
                aes.Key = Key;
                aes.IV = Iv;
                var decrypt = aes.CreateDecryptor(aes.Key, aes.IV);
                byte[] xBuff = null;
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
                    {
                        byte[] xXml = Convert.FromBase64String(Input);
                        byte[] msg = new byte[xXml.Length + 32 - xXml.Length % 32];
                        Array.Copy(xXml, msg, xXml.Length);
                        cs.Write(xXml, 0, xXml.Length);
                    }
                    xBuff = decode2(ms.ToArray());
                }
                return xBuff;
            }
            static byte[] decode2(byte[] decrypted)
            {
                int pad = (int)decrypted[decrypted.Length - 1];
                if (pad < 1 || pad > 32)
                {
                    pad = 0;
                }
                byte[] res = new byte[decrypted.Length - pad];
                Array.Copy(decrypted, 0, res, 0, decrypted.Length - pad);
                return res;
            }
            #endregion
        }
        class Crypt
        {
            static string otoken = "lioDqc3SIgmEo3lo3D8nO8hLYVa0BcDMZd55KidrUG4NGEWDyDSpC9naTVZuIoWP";
            static string msgencrypt = "BdR8DXlQXulijO5crf68us3CjUUcalRKkDOo62DZ0HV";

            /// <summary>
            /// 验证签名
            /// </summary>
            /// <param name="sTimeStamp"></param>
            /// <param name="sNonce"></param>
            /// <param name="sMsgBody"></param>
            /// <param name="sSigture"></param>
            /// <returns></returns>
            static void VerifySignature(string sTimeStamp, string sNonce, string sMsgBody, string sSigture)
            {
                var hash = GenarateSinature(sTimeStamp, sNonce, sMsgBody);
                if (hash != sSigture) throw new WxExcep("签名验证错误。");
            }
            /// <summary>
            /// 生成签名
            /// </summary>
            /// <param name="timestamp"></param>
            /// <param name="nonce"></param>
            /// <param name="msgencrypt"></param>
            /// <param name="msgsignature"></param>
            /// <returns></returns>
            static string GenarateSinature(string timestamp, string nonce, string msgencrypt)
            {
                var ps = new List<string>();
                ps.Add(otoken);
                ps.Add(timestamp);
                ps.Add(nonce);
                ps.Add(msgencrypt);

                ps.Sort((x, y) => { return x.CompareTo(y); });

                string raw = "";
                for (int i = 0; i < ps.Count; ++i) { raw += ps[i]; }

                return Secret.SHA1(raw).ToLower();
            }

            /// <summary>
            /// 检验消息的真实性，并且获取解密后的明文
            /// </summary>
            /// <param name="sMsgSignature">签名串，对应URL参数的msg_signature</param>
            /// <param name="sTimeStamp">时间戳，对应URL参数的timestamp</param>
            /// <param name="sNonce">随机串，对应URL参数的nonce</param>
            /// <param name="sPostData">密文，对应POST请求的数据</param>
            /// <param name="sMsg">解密后的原文，当return返回0时有效</param>
            /// <returns>成功0，失败返回对应的错误码</returns>
            public static string DecryptMsg(string appid, string sMsgSignature, string sTimeStamp, string sNonce, string sPostData)
            {
                if (msgencrypt.Length != 43) throw new WxExcep("AESKey不正确。");

                XmlDocument doc = new XmlDocument();
                XmlNode root;
                string sEncryptMsg;
                try
                {
                    doc.LoadXml(sPostData);
                    root = doc.FirstChild;
                    sEncryptMsg = root["Encrypt"].InnerText;
                }
                catch (Exception)
                {
                    throw new WxExcep("XML解析失败");
                }

                VerifySignature(sTimeStamp, sNonce, sEncryptMsg, sMsgSignature);

                string cpid = "";
                var sMsg = "";
                try
                {
                    sMsg = AES.Decrypt(sEncryptMsg, msgencrypt, ref cpid);
                }
                catch (FormatException)
                {
                    throw new WxExcep("BASE64解密异常");
                }
                catch (Exception)
                {
                    throw new WxExcep("AES 解密失败");
                }

                if (cpid != appid) throw new WxExcep("APPID 校验错误");

                return sMsg;
            }

            /// <summary>
            /// 将企业号回复用户的消息加密打包
            /// </summary>
            /// <param name="sReplyMsg">企业号待回复用户的消息，xml格式的字符串</param>
            /// <param name="sTimeStamp">时间戳，可以自己生成，也可以用URL参数的timestamp</param>
            /// <param name="sNonce">随机串，可以自己生成，也可以用URL参数的nonce</param>
            /// <param name="sEncryptMsg">加密后的可以直接回复用户的密文，包括msg_signature, timestamp, nonce, encrypt的xml格式的字符串</param>
            /// <returns>
            /// 成功0，失败返回对应的错误码
            /// </returns>
            public static string EncryptMsg(string appid, string sReplyMsg, string sTimeStamp, string sNonce)
            {
                if (msgencrypt.Length != 43) throw new WxExcep("AESKey不正确。");

                string raw = "";
                try
                {
                    raw = AES.Encrypt(sReplyMsg, msgencrypt, appid);
                }
                catch (Exception)
                {
                    throw new WxExcep("AES 加密失败");
                }

                string MsgSigature = GenarateSinature(sTimeStamp, sNonce, raw);

                var sEncryptMsg = "";
                string EncryptLabelHead = "<Encrypt><![CDATA[";
                string EncryptLabelTail = "]]></Encrypt>";
                string MsgSigLabelHead = "<MsgSignature><![CDATA[";
                string MsgSigLabelTail = "]]></MsgSignature>";
                string TimeStampLabelHead = "<TimeStamp><![CDATA[";
                string TimeStampLabelTail = "]]></TimeStamp>";
                string NonceLabelHead = "<Nonce><![CDATA[";
                string NonceLabelTail = "]]></Nonce>";

                sEncryptMsg = sEncryptMsg + "<xml>" + EncryptLabelHead + raw + EncryptLabelTail;
                sEncryptMsg = sEncryptMsg + MsgSigLabelHead + MsgSigature + MsgSigLabelTail;
                sEncryptMsg = sEncryptMsg + TimeStampLabelHead + sTimeStamp + TimeStampLabelTail;
                sEncryptMsg = sEncryptMsg + NonceLabelHead + sNonce + NonceLabelTail;
                sEncryptMsg += "</xml>";

                return sEncryptMsg;

            }
        }
        /// <summary>
        /// 微信Xml
        /// </summary>
        public class xml
        {
            /// <summary>
            /// SUCCESS/FAIL
            /// 此字段是通信标识，非交易标识，交易是否成功需要查看result_code来判断
            /// </summary>
            public string return_code { get; set; }
            /// <summary>
            /// 返回信息，如非空，为错误原因
            /// 签名失败
            /// 参数格式校验错误
            /// </summary>
            public string return_msg { get; set; }
        }
        public class mbase
        {
            public string errcode { get; set; }
            public string errmsg { get; set; }
        }
        public class web_token : mbase
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
            public string refresh_token { get; set; }
            public string openid { get; set; }
            public string scope { get; set; }
        }

        class tick : mbase
        {
            public string ticket { get; set; }
            public int expires_in { get; set; }
        }
        class token : mbase
        {
            public string access_token { get; set; }
            public int expires { get; set; }
        }

    }
}
