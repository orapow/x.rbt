using System;
using System.IO;
using System.Linq;
using System.Text;
using X.App.Com;
using X.Core.Plugin;
using X.Core.Utility;

namespace X.App.Views.wx.bw
{
    public class notify : _wx
    {
        public string no { get; set; }
        protected override string GetParmNames
        {
            get
            {
                return "no";
            }
        }
        protected override void Validate()
        {
            base.Validate();
            Validator.Require("no", dict.GetString("no"));
        }
        public override byte[] GetResponse()
        {
            GetPageParms();
            cfg = Config.LoadConfig();
            var ntxml = string.Empty;
            using (var sr = new StreamReader(Context.Request.InputStream)) ntxml = sr.ReadToEnd();

            if (string.IsNullOrEmpty(ntxml)) return null;
            ntxml = ntxml.Replace("xml", "Ntxml");
            var nt = Serialize.FormXml<Wx.Pay.Ntxml>(ntxml);

            if (!Wx.Pay.ValidNotify(nt, cfg.wx_mch_id, cfg.wx_appid, cfg.wx_paykey))
            {
                Loger.Info("回调验证失败，地址：" + Context.Request.RawUrl + "，参数：" + ntxml);
                return null;
            }

            var no = dict.GetString("no");
            if (string.IsNullOrEmpty(no)) return null;
            if (nt.out_trade_no != no)
            {
                Loger.Info("订单号不正确，地址：" + Context.Request.RawUrl + "，提交单号：" + nt.out_trade_no);
                return null;
            }

            var order = DB.x_order.SingleOrDefault(o => o.no == dict.GetString("no"));

            var okxml = @"<xml>
                            <return_code><![CDATA[SUCCESS]]></return_code>
                            <return_msg><![CDATA[OK]]></return_msg>
                        </xml>";

            if (order == null)
            {
                Loger.Info("订单不存在，订单号：" + no);
                return Encoding.UTF8.GetBytes(okxml);
            }

            if (order.status != 1)
            {
                Loger.Info("订单确认失败，订单号：" + dict.GetString("no"));
                return Encoding.UTF8.GetBytes(okxml);
            }
            else
            {
                var u = DB.x_wx_user.SingleOrDefault(o => o.user_id == order.user_id);
                order.status = 2;
                u.etime = order.etime;
            }

            SubmitDBChanges();

            return Encoding.UTF8.GetBytes(okxml);
        }
    }
}
