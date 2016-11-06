using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Core.Utility;
using X.Data;
using X.Web.Com;

namespace X.App.Apis.app.agent
{
    public class update : xag
    {
        public string name { get; set; }
        public string pwd { get; set; }
        public string contract { get; set; }
        public string tel { get; set; }
        public string intro { get; set; }
        public string addr { get; set; }
        public string logo { get; set; }
        public decimal c_zz { get; set; }
        public decimal c_hz { get; set; }

        protected override bool need_user
        {
            get
            {
                return true;
            }
        }

        protected override XResp Execute()
        {
            if (!string.IsNullOrEmpty(name)) cag.name = name;
            if (!string.IsNullOrEmpty(contract)) cag.contract = contract;
            if (!string.IsNullOrEmpty(tel)) cag.tel = tel;
            if (!string.IsNullOrEmpty(intro)) cag.intro = intro;
            if (!string.IsNullOrEmpty(addr)) cag.addr = addr;
            if (!string.IsNullOrEmpty(logo)) cag.logo = logo;
            if (!string.IsNullOrEmpty(pwd)) cag.pwd = pwd;
            cag.c_zz = c_zz;
            cag.c_hz = c_hz;

            SubmitDBChanges();

            var r = new cag
            {
                ukey = cag.ukey,
                contract = cag.contract,
                logo = cag.logo,
                name = cag.name,
                pwd = Secret.MD5(cag.pwd),
                tel = cag.tel,
                uid = cag.uid,
                addr = cag.addr,
                intro = cag.intro,
                hz = (decimal)cag.c_hz,
                zz = (decimal)cag.c_zz
            };

            return r;
        }
    }
}
