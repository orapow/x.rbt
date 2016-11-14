using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using X.App.Com;
using X.Data;
using X.Web;
using X.Web.Com;
using X.Web.Views;

namespace X.App.Views
{
    public class xview : View
    {
        /// <summary>
        /// 系统配置文件
        /// </summary>
        protected Config cfg = null;
        /// <summary>
        /// 
        /// </summary>
        protected x_user cu = null;

        protected override void InitView()
        {

            base.InitView();
            cfg = Config.LoadConfig();
            dict.Add("cfg", cfg);

            cu = DB.x_user.FirstOrDefault(o => o.user_id == 1);
            dict.Add("cu", cu);

        }
    }
}
