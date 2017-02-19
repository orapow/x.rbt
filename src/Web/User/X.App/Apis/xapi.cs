using System.Linq;
using X.App.Com;
using X.Data;
using X.Web;
using X.Web.Apis;

namespace X.App.Apis
{
    public class xapi : Api
    {
        protected Config cfg = Config.LoadConfig();
        /// <summary>
        /// 
        /// </summary>
        protected x_user cu = null;

        protected virtual bool needus { get { return true; } }

        protected override void InitApi()
        {
            base.InitApi();

            var uk = GetReqParms("ukey");
            if (!string.IsNullOrEmpty(uk)) cu = DB.x_user.FirstOrDefault(o => o.ukey == uk);

            if (cu == null && needus) throw new XExcep("0x0006");
        }
    }
}
