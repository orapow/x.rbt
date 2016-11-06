using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Core.Cache;
using X.Core.Utility;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.sys.dict
{
    public class save : xmg
    {
        public int id { get; set; }

        public string code { get; set; }//代号
        public string name { get; set; }//名称
        public string value { get; set; }//值
        public string img { get; set; }
        public string upval { get; set; }//上级值
        public string jp { get; set; }//简拼
        public string point { get; set; }

        protected override void Validate()
        {
            base.Validate();

            //Validator.Require("code", code);
            //Validator.Require("name", name);
            //Validator.Require("value", value);
            //Validator.Require("jp", jp);
        }

        protected override Web.Com.XResp Execute()
        {
            var q = from d in DB.x_dict
                    where (d.name == name || d.value == value) && d.code == code
                    select d;
            if (id > 0) q = q.Where(o => o.dict_id != id);
            if (!string.IsNullOrEmpty(upval)) q = q.Where(o => o.upval == upval);

            if (q.Count() > 0) throw new XExcep("0x0007");

            x_dict dt = null;
            if (id > 0) dt = DB.x_dict.FirstOrDefault(o => o.dict_id == id);
            if (dt == null) dt = new x_dict() { code = code };

            dt.name = name;
            dt.value = value;
            dt.upval = string.IsNullOrEmpty(upval) ? "0" : upval;
            dt.jp = jp;
            dt.img = img;

            if (id == 0) DB.x_dict.InsertOnSubmit(dt);

            SubmitDBChanges();

            CacheHelper.Remove("dict." + code);

            return new XResp();
        }
    }
}
