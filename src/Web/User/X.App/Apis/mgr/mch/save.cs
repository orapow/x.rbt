using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Core.Utility;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.mch
{
    public class save : xmg
    {
        public int id { get; set; }

        public string name { get; set; }//名称
        public string uid { get; set; }//帐号
        public string tel { get; set; }//电话座机
        public string pwd { get; set; }//密码
        public string logo { get; set; }//Logo
        public string cp_name { get; set; }//公司名称
        public string cp_addr { get; set; }//办公地址
        public string cp_man { get; set; }//法人
        public string cp_tel { get; set; }//法人电话
        public string bk_ac_name { get; set; }//银行户名
        public string bk_account { get; set; }//银行帐号
        public string bk_name { get; set; }//银行名称
        public decimal rate { get; set; }//收佣比率
        public string remark { get; set; }//商家说明

        protected override void Validate()
        {
            base.Validate();
            //Validator.Require("name", name);
            //Validator.Require("uid", uid);
            //Validator.Require("tel", tel);
            //Validator.Min("rate", rate, 0);
            //Validator.Max("rate", rate, 100);
        }

        protected override Web.Com.XResp Execute()
        {
            x_mch ad = new x_mch();
            if (id > 0)
            {
                ad = DB.x_mch.SingleOrDefault(o => o.mch_id == id);
                if (ad == null) throw new XExcep("0x0005");
            }

            ad.mch_id = id;
            if (!string.IsNullOrEmpty(pwd)) ad.pwd = Secret.MD5(pwd);
            ad.name = name;
            ad.account = uid;
            ad.tel = tel;
            ad.logo = logo;
            ad.cp_name = cp_name;
            ad.cp_addr = cp_addr;
            ad.cp_man = cp_man;
            ad.cp_tel = cp_tel;
            ad.bank_ac_name = bk_ac_name;
            ad.bank_account = bk_account;
            ad.bank_name = bk_name;
            ad.rate = rate;
            ad.remark = remark;

            if (ad.mch_id == 0)
            {
                ad.ctime = DateTime.Now;
                DB.x_mch.InsertOnSubmit(ad);
            }

            ad.mtime = DateTime.Now;

            SubmitDBChanges();

            return new XResp();
        }
    }
}
