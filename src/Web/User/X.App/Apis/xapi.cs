using System.Linq;
using X.App.Com;
using X.Data;
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

        protected override void InitApi()
        {
            base.InitApi();
            cu = DB.x_user.FirstOrDefault(o => o.user_id == 1);
        }
    }
}
