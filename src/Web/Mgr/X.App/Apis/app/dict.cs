using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web.Com;

namespace X.App.Apis.app
{
    public class dict : xap
    {
        /// <summary>
        /// 代号
        /// </summary>
        public string code { get; set; }

        protected override void Validate()
        {
            base.Validate();
            Validator.Require("code", code);
        }

        protected override XResp Execute()
        {
            var r = new Resp_List();

            var dict_list = x_com.GetDictList(code, "00");
            var top = dict_list.Where(o => o.upval == "0");
            var list = new List<item>();

            foreach (var d in top)
            {
                var dt = new item(d.name, d.value);
                var sub = dict_list.Where(o => o.upval == d.value);
                foreach (var t in sub) dt.items.Add(new item(t.name, t.value));
                list.Add(dt);
            }
            r.items = list;

            return r;
        }

        public class item
        {
            public string name { get; set; }
            public string value { get; set; }
            public List<item> items { get; set; }
            /// <summary>
            /// Initializes a new instance of the dict class.
            /// </summary>
            /// <param name="name"></param>
            /// <param name="value"></param>
            public item(string name, string value)
            {
                this.name = name;
                this.value = value;
                items = new List<item>();
            }
        }
    }
}
