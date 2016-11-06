using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using X.Core.Plugin;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.app
{
    public class bug : xapi
    {
        protected override XResp Execute()
        {
            var c = Context;
            if (c.Request.Files.Count == 0) throw new XExcep("T没有提交文件");// return new XResp() { msg = "没有提交文件" };
            var f = c.Request.Files[0];
            using (var sr = new StreamReader(f.InputStream))
            {
                var log = sr.ReadToEnd();
                Loger.Error(log);
            }
            return new XResp();
        }
    }
}
