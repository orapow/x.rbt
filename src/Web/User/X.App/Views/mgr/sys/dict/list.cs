using System;
using System.Collections.Generic;
using System.Linq;
using X.Data;
using X.Web.Com;

namespace X.App.Views.mgr.sys.dict
{
    public class list : xmg
    {
        public string code { get; set; }
        public string upval { get; set; }
        public string val { get; set; }
        protected override string GetParmNames
        {
            get
            {
                return "code-upval-val";
            }
        }
        protected override void InitDict()
        {
            base.InitDict();

            dict.Add("dict_list", DB.x_dict.Where(o => o.upval == null || o.upval == "").ToList());

            if (!string.IsNullOrEmpty(code))
            {
                dict.Add("list", GetDictList(code, "0"));
                if (!string.IsNullOrEmpty(upval))
                {
                    dict.Add("sub_list", GetDictList(code, upval));
                    if (!string.IsNullOrEmpty(val)) dict.Add("item", DB.x_dict.FirstOrDefault(o => o.code == code && o.value == val));
                    else dict.Add("item", DB.x_dict.FirstOrDefault(o => o.code == code && o.value == upval));
                }
            }
        }
    }
}
