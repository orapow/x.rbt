using System.Linq;
using System.Web;
using X.App.Com;
using X.Core.Cache;
using X.Data;
using X.Web;
using X.Web.Views;

namespace X.App.Views
{
    public class xview : View
    {
        /// <summary>
        /// 系统配置文件
        /// </summary>
        protected Config cfg = null;
        protected x_user cu = null;

        protected virtual int needus { get { return 3; } }
        protected virtual string mu_name { get { return ""; } }

        private string k = "";

        protected virtual void getUser(string key)
        {
            k = string.IsNullOrEmpty(key) ? cfg.wx_appid : key;
            var uk = GetReqParms("ukey-" + k);
            if (!string.IsNullOrEmpty(uk)) cu = needus == 3 ? DB.x_user.FirstOrDefault(o => o.ukey == uk) : CacheHelper.Get<x_user>(uk); //;
        }

        protected override void InitView()
        {
            base.InitView();
            cfg = Config.LoadConfig();

            if (cu == null) getUser("");

            if (cu == null && needus > 0) throw new XExcep("0x0006");
            if (cu != null)
            {
                Context.Response.SetCookie(new HttpCookie("ukey-" + k, cu.ukey));
                CacheHelper.Save(cu.ukey, cu);
            }

            if (!string.IsNullOrEmpty(mu_name)) dict.Add("m_" + mu_name, "selected");
        }
        protected override void InitDict()
        {
            base.InitDict();
            if (cu != null) dict.Add("cu", cu);
            dict.Add("cfg", cfg);
        }
    }
}
