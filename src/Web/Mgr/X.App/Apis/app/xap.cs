using System;
using System.Collections.Generic;
using System.Linq;
using X.Core.Plugin;
using X.Core.Utility;
using X.Data;
using X.Web;

namespace X.App.Apis.app
{
    public class xap : xapi
    {
        /// <summary>
        /// 密钥
        /// </summary>
        private string scr = "wa5VVToZZ%C#A47Wzm3$ZgjiIpV^xnWwV#T6BFbPU#zH7EIx*8Pv0B1AmD00RD0g^ieoDJa7Cffn#dDpxUODhBzNG!hAaebrqCN";
        public string sign { get; set; }

        public decimal lng { get; set; }
        public decimal lat { get; set; }

        public string model { get; set; }

        protected override void Validate()
        {
            base.Validate();
            Validator.Require("sign", sign);
        }

        protected override void InitApi()
        {
            base.InitApi();
            checkSign();
        }

        protected void checkSign()
        {
            var dict = new Dictionary<string, string>();
            string sign = "";
            var formdata = Context.Request.Form;
            foreach (string k in formdata.Keys)
            {
                if (string.IsNullOrEmpty(formdata[k])) continue;
                if (k == "sign") { sign = formdata[k]; continue; }
                dict.Add(k, formdata[k]);
            }
            string str_tomd5 = "";
            foreach (var d in dict.OrderBy(o => o.Key)) str_tomd5 += d.Key + "=" + Secret.ToBase64(d.Value) + "&";
            if (sign != Secret.SHA256(str_tomd5 + scr).ToLower()) { Loger.Info("串：" + str_tomd5 + scr + "，密：" + Secret.SHA256(str_tomd5 + scr) + "，原：" + sign); throw new XExcep("T签名验证失败"); }
        }
    }
}
