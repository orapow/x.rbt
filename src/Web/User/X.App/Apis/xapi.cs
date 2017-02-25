using System.Linq;
using X.App.Com;
using X.Core.Cache;
using X.Data;
using X.Web;
using X.Web.Apis;

namespace X.App.Apis
{
    public class xapi : Api
    {
        /// <summary>
        /// 
        /// </summary>
        protected Config cfg = Config.LoadConfig();
        /// <summary>
        /// 
        /// </summary>
        protected x_user cu = null;

        protected virtual int needus { get { return 3; } }

        protected virtual string get_appid()
        {
            return cfg.wx_appid;
        }

        protected override void InitApi()
        {
            base.InitApi();
            var uk = GetReqParms("ukey-" + get_appid());
            if (!string.IsNullOrEmpty(uk)) cu = needus == 3 ? DB.x_user.FirstOrDefault(o => o.ukey == uk) : CacheHelper.Get<x_user>(uk); //;
            if (cu == null && needus > 0) throw new XExcep("0x0006");
        }
    }
}
