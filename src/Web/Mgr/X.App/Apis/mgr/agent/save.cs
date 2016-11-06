using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.agent
{
    public class save : xmg
    {
        public int id { get; set; }

        public string name { get; set; }//二房东名称

        public int zzback { get; set; }//整租返现
        public int hzback { get; set; }//合租返现

        public int status { get; set; }//合作状态

        public string uid { get; set; }//二房东名称
        public string pwd { get; set; }//密码

        public string contract { get; set; }//联系人
        public string tel { get; set; }//联系电话
        public string intro { get; set; }//公司简介
        public string addr { get; set; }//公司地址

        public string logo { get; set; }//公司Logo
        public string point { get; set; }//公司经度,公司纬度
        public string remark { get; set; }

        public decimal hz { get; set; }
        public decimal zz { get; set; }

        protected override void Validate()
        {
            base.Validate();

            Validator.Require("name", name);
            Validator.Require("pwd", pwd);
            Validator.Require("contract", contract);
            Validator.Require("tel", tel);

        }

        protected override Web.Com.XResp Execute()
        {
            x_agent ag = new x_agent();
            if (id > 0)
            {
                ag = DB.x_agent.SingleOrDefault(o => o.agent_id == id);
                if (ag == null) throw new XExcep("0x0005");
            }
            else
            {
                //判断用户是否已经存在(根据用户名或账户)
                if (!string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(uid))
                {
                    ag = DB.x_agent.SingleOrDefault(o => o.name == name || o.uid == uid);
                    if (ag != null) throw new XExcep("0x0007"); else ag = new x_agent();
                }
            }

            ag.addr = addr;
            ag.admin = ad.id;
            ag.contract = contract;
            ag.intro = intro;
            ag.logo = logo.Trim(',');
            ag.name = name;
            ag.pwd = pwd;
            ag.remark = remark;
            ag.status = status;
            ag.tel = tel;
            ag.c_hz = hz;
            ag.c_zz = zz;
            ag.time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            ag.uid = uid;

            if (!string.IsNullOrEmpty(point))
            {
                var pt = point.Split(',');
                if (pt.Length == 2)
                {
                    ag.pointx = decimal.Parse(pt[0]);
                    ag.pointy = decimal.Parse(pt[1]);
                }
            }

            if (ag.id == 0) DB.x_agent.InsertOnSubmit(ag);

            SubmitDBChanges();

            return new XResp();
        }
    }
}
