using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Core.Utility;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.admin
{
    public class save : xmg
    {
        public int id { get; set; }

        public string name { get; set; }//用户名称
        public string tel { get; set; }//联系电话
        public string uid { get; set; }//帐号
        public string pwd { get; set; }//密码

        public int status { get; set; }//权限(管理员:1,客服:2)

        protected override void Validate()
        {
            base.Validate();
            Validator.CheckRange("status", status, 0, null);

            Validator.Require("name", name);
            Validator.Require("uid", uid);
            Validator.Require("pwd", pwd);
            Validator.Require("tel", tel);

        }

        protected override Web.Com.XResp Execute()
        {
            x_admin ad = new x_admin();
            if (id > 0)
            {
                ad = DB.x_admin.SingleOrDefault(o => o.admin_id == id);
                if (ad == null) throw new XExcep("0x0005");
            }
            else
            {
                //判断用户是否已经存在(根据用户名或账户)
                if (!string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(uid))
                {
                    ad = DB.x_admin.SingleOrDefault(o => o.name == name || o.uid == uid);
                    if (ad != null) throw new XExcep("0x0007"); else ad = new x_admin();
                }
            }
            ad.uid = uid;
            if (!string.IsNullOrEmpty(pwd)) ad.pwd = Secret.MD5(pwd);
            ad.name = name;
            ad.tel = tel;
            ad.time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            if (ad.id == 0) DB.x_admin.InsertOnSubmit(ad);

            SubmitDBChanges();

            return new XResp();
        }
    }
}
