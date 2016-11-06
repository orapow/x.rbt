using System;
using System.Collections.Generic;
using System.Linq;
using X.Core.Utility;
using X.Data;
using X.Web.Views;

namespace X.App.Views.com
{
    public class view : xview
    {
        /// <summary>
        /// 代号
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 搜索
        /// </summary>
        public string key { get; set; }
        protected override void Validate()
        {
            base.Validate();
            //Validator.Require("code", code);
        }
        List<Item> list = null;
        protected override void InitDict()
        {
            base.InitDict();
            list = new List<Item>();
            switch (code)
            {
                default:
                    break;
            }
            dict.Add("dict", list);
        }

        protected override string GetParmNames
        {
            get { return "code-key-"; }
        }

        public override string GetTplFile()
        {
            return "com/dict";
        }
        public class Item
        {
            public string name { get; set; }
            public int value { get; set; }
            public string data { get; set; }
            /// <summary>
            /// Initializes a new instance of the Item class.
            /// </summary>
            /// <param name="name"></param>
            /// <param name="value"></param>
            public Item(string name, int value, object data)
            {
                this.name = name;
                this.value = value;
                this.data = Serialize.ToJson(data);
            }
        }
    }
}
