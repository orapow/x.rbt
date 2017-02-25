using System;
using X.Core.Cache;
using X.Core.Utility;

namespace X.App.Views.wx
{
    public class login : _wx
    {
        public string key { get; set; }
        protected override int needus
        {
            get
            {
                return 3;
            }
        }
        protected override string GetParmNames
        {
            get
            {
                return "key";
            }
        }

        protected override void getUser(string k)
        {
            base.getUser(cfg.wx_appid);
            cu.ukey = Secret.MD5(Guid.NewGuid().ToString());
            SubmitDBChanges();
            CacheHelper.Save(cu.ukey, cu);
        }

        protected override void InitDict()
        {
            base.InitDict();
            dict.Add("issucc", !string.IsNullOrEmpty(key));
            CacheHelper.Save("login." + key, cu.ukey);
            dict["cu"] = cu;
        }
    }
}
