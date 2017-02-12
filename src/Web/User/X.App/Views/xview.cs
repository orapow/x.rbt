using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
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
        protected x_user cu = null;

        protected virtual bool needus { get { return true; } }

        protected virtual string menu_id { get { return ""; } }

        protected override void InitView()
        {
            base.InitView();
            cfg = Config.LoadConfig();

            var uk = GetReqParms("ukey");
            if (!string.IsNullOrEmpty(uk)) cu = DB.x_user.FirstOrDefault(o => o.ukey == uk);

            if (cu == null && needus) throw new XExcep("0x0006");

            if (!string.IsNullOrEmpty(menu_id)) dict.Add("m" + menu_id, "selected");

        }
        protected override void InitDict()
        {
            base.InitDict();
            if (cu != null) dict.Add("cu", cu);
            dict.Add("cfg", cfg);
        }
    }
}
