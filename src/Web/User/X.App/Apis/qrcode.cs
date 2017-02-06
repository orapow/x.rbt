using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using X.Core.Cache;
using X.Core.Utility;
using X.Web;
using X.Web.Com;

namespace X.App.Apis
{
    public class qrcode : xapi
    {
        [ParmsAttr(name = "url", req = true)]
        public string url { get; set; }

        protected override XResp Execute()
        {
            var data = Tools.GetHttpFile(url);
            if (data == null) throw new XExcep("文件获取失败");
            return new XResp() { msg = Convert.ToBase64String(data) };
        }
    }
}
