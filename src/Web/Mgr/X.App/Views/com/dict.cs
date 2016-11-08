using System;
using System.Collections.Generic;
using System.Linq;
using Rbt.Data;
using X.Web.Views;

namespace X.App.Views.com
{
    public class dict : xview
    {
        private string code = string.Empty;
        protected override void Validate()
        {
            base.Validate();
            code = dict.GetString("code");
            //Validator.Require("code", code);
        }
        protected override void InitDict()
        {
            base.InitDict();
            if (dict.GetInt("bylet") == 1)
            {
                dict.Add("list", "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToList());
            }
            else
            {
                //var upval = dict.GetString("upval");
                //if (string.IsNullOrEmpty(upval)) upval = "0";
                dict.Add("dict", GetDictList(code, dict.GetString("upval")));
            }
        }

        protected override string GetParmNames
        {
            get
            {
                ///代号-上级值-按字母排
                return "code-upval-bylet";
            }
        }

        public List<x_dict> GetByLetter(char l)
        {
            var list = GetDictList(dict.GetString("code"), "00");
            return list.FindAll(d =>
            {
                return d.jp.ToUpper()[0] == l && d.upval == dict.GetString("upval");
            });
        }
    }
}
