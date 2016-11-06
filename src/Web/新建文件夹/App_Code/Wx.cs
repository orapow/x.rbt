using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using X.Core.Cache;
using X.Core.Plugin;
using X.Core.Utility;

namespace Rbt.Web
{
    public class Wx
    {
        public static string appid = "wxb8288a0ed0f2cc7f";
        public static string appsecret = "1a55599a01fe52767029848fb3b44061";

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

        public static string GetToken(bool isnew)
        {
            var tk = CacheHelper.Get<string>("wx.access_token");
            if (string.IsNullOrEmpty(tk) || isnew)
            {
                var json = Tools.GetHttpData("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + appid + "&secret=" + appsecret);
                Debug.WriteLine("getToken->" + json);
                var tke = Serialize.FromJson<token>(json);
                tk = tke.access_token;
                CacheHelper.Save("wx.access_token", tk, tke.expires - 500);
            }
            return tk;
        }
        public static string GetToken(string appid, string sec)
        {
            return GetToken(false);
        }

        static string RefreshToken()
        {
            var re_tk = CacheHelper.Get<string>("wx.refresh_token");
            if (string.IsNullOrEmpty(re_tk)) return "";
            var json = Tools.GetHttpData("https://api.weixin.qq.com/sns/oauth2/refresh_token?appid=" + appid + "&grant_type=refresh_token&refresh_token=" + re_tk);
            var rtk = Serialize.FromJson<web_token>(json);
            //if (!string.IsNullOrEmpty(rtk.errcode)) Loger.Error(json);
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

        public static web_token GetWebToken(string code)
        {
            var json = Tools.GetHttpData("https://api.weixin.qq.com/sns/oauth2/access_token?appid=" + appid + "&secret=" + appsecret + "&code=" + code + "&grant_type=authorization_code");
            var token = Serialize.FromJson<web_token>(json);
            //if (!string.IsNullOrEmpty(token.errcode)) { Loger.Error(json); }
            return token;
        }

        public class Media
        {
            public static string DownImage(string tk, string mmid)
            {
                return Tools.DownImage("https://api.weixin.qq.com/cgi-bin/media/get?access_token=" + tk + "&media_id=" + mmid);
            }
        }

        /// <summary>
        /// 用户相关
        /// </summary>
        public class User
        {
            public static uinfo GetUserInfo(string opid, string tk, bool isweb)
            {
                var json = "";
                if (isweb) json = Tools.GetHttpData("https://api.weixin.qq.com/sns/userinfo?access_token=" + tk + "&openid=" + opid + "&lang=zh_CN");
                else json = Tools.GetHttpData("https://api.weixin.qq.com/cgi-bin/user/info?access_token=" + tk + "&openid=" + opid + "&lang=zh_CN");
                var ui = Serialize.FromJson<uinfo>(json);
                //if (!string.IsNullOrEmpty(ui.errcode)) Loger.Error(json);
                return ui;
            }

            public static uinfo GetUserInfo(string opid, string tk)
            {
                return GetUserInfo(opid, tk, false);
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
            public static Oxml MdOrder(string body, string order_no, string total, string notify_url, string openid, string appid, string mch_id, string mch_key)
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
                ps.Add("trade_type", "JSAPI");//"NATIVE");
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
                if (nt == null || string.IsNullOrEmpty(nt.sign))
                {
                    return false;
                }
                if (nt.mch_id != mch_id || nt.appid != appid)
                {
                    return false;
                }
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
            public static payrsp PayToOpenid(string appid, string mchid, string openid, string no, int amount, string cert_path, string signkey)
            {
                var dict = new Dictionary<string, string>();
                dict.Add("mch_appid", appid);
                dict.Add("mchid", mchid);
                dict.Add("nonce_str", Tools.GetRandRom(32, 3));
                dict.Add("partner_trade_no", no + "");
                dict.Add("openid", openid);
                dict.Add("check_name", "NO_CHECK");
                dict.Add("amount", amount * 100 + "");
                dict.Add("desc", "返现卡号：" + no + "，返现金额：" + amount + "元");
                dict.Add("spbill_create_ip", Tools.GetClientIP());
                var to_md5 = "";
                var xml_data = "<xml>";
                foreach (var d in dict.OrderBy(o => o.Key))
                {
                    if (!string.IsNullOrEmpty(d.Value)) to_md5 += d.Key + "=" + d.Value + "&";
                    xml_data += "<" + d.Key + ">" + d.Value + "</" + d.Key + ">";
                }
                xml_data += "<sign>" + Secret.MD5(to_md5 + "key=" + signkey, 0).ToUpper() + "</sign>";
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
                    //Loger.Error(wex);
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

        public class Crypt
        {
            static string otoken = "3kzjAeIAW4VfdUrQlBuAFdvY7";
            static string msgencrypt = "oNHXy0UeqbiaQ7GU8UFGiSOgz87fXvzgntCU7c4N1KQ";

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
                Debug.WriteLine("VerifySignature->" + sTimeStamp);
                Debug.WriteLine("VerifySignature->" + sNonce);
                Debug.WriteLine("VerifySignature->" + sMsgBody);
                Debug.WriteLine("VerifySignature:" + hash + "<->" + sSigture);
                if (hash != sSigture)
                {
                    throw new WxExcep("签名验证错误。");
                }
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
        public class mbase
        {
            public string errcode { get; set; }
            public string errmsg { get; set; }
        }
    }
}
