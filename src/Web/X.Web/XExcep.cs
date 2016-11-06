using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using X.Core.Cache;
using X.Core.Utility;

namespace X.Web
{
    public class XExcep : Exception
    {
        private static Dictionary<string, string> codes
        {
            get
            {
                var errores = CacheHelper.Get<Dictionary<string, string>>("x.codes");
                if (errores == null || errores.Count == 0)
                {
                    errores = new Dictionary<string, string>();
                    var urls = Tools.ReadFileLines(HttpContext.Current.Server.MapPath("/dat/codes.x"));
                    foreach (var u in urls)
                    {
                        var url = u.Split(',');
                        errores.Add(url[0], url[1]);
                    }
                    CacheHelper.Save("x.codes", errores);
                }
                return errores;
            }
        }

        public string ErrCode { get; set; }
        public string ErrMsg { get; set; }

        public XExcep(string code, string msg)
        {
            this.ErrCode = code;
            if (codes != null)
            {
                var c = codes.Count(o => o.Key == code);
                if (c > 0)
                {
                    var err = codes.First(o => o.Key == code);
                    this.ErrMsg = err.Value + " " + (string.IsNullOrEmpty(msg) ? string.Empty : msg);
                }
                else
                {
                    this.ErrMsg = String.Format("未知错误代码({0})", code);
                }
            }
        }

        public XExcep(string code)
            : this(code, string.Empty)
        {
        }
    }
}
