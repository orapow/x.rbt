using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;
using X.Core.Cache;
using X.Core.Utility;

namespace X.Core.Plugin
{
    public class UrlWrite : IHttpModule, IReadOnlySessionState, IRequiresSessionState
    {
        private HttpApplication application = null;

        /// <summary>
        /// 重写规则
        /// </summary>
        Dictionary<string, string> url_rules
        {
            get
            {
                var rules = CacheHelper.Get<Dictionary<string, string>>("x.url_rules");
                if (rules == null || rules.Count == 0)
                {
                    rules = new Dictionary<string, string>();
                    var urls = Tools.ReadFileLines(application.Server.MapPath("/dat/urls.x"));
                    foreach (var u in urls)
                    {
                        var str = u.Replace(",", "<!=_=>").Replace("->", ",");
                        var url = str.Split(',');
                        rules.Add(url[0].Replace("<!=_=>", ","), url[1].Replace("<!=_=>", ","));
                    }
                    CacheHelper.Save("x.url_rules", rules);
                }
                return rules;
            }
        }

        public void Dispose()
        {
            if (application != null) application.Dispose();
        }

        /// <summary> 
        /// 实现接口的Init方法 
        /// </summary> 
        /// <param name="context"></param> 
        void IHttpModule.Init(HttpApplication app)
        {
            application = app;
            app.BeginRequest += ReUrl_BeginRequest;
        }

        /// <summary>
        /// 处理HTTP请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReUrl_BeginRequest(object sender, EventArgs e)
        {
            HttpContext context = ((HttpApplication)sender).Context;
            if (url_rules == null) return;

            string url = context.Request.FilePath;
            if (File.Exists(context.Server.MapPath(url))) { return; }

            string newurl = "";
            foreach (var r in url_rules.Keys)
            {
                Regex reg = new Regex(r);
                Match m = reg.Match(url);
                if (m.Success)
                {
                    newurl = GetNewUrl(m, url_rules[r], context.Request.QueryString.ToString());
                    break;
                }
            }
            if (!string.IsNullOrEmpty(newurl))
            {
                if (File.Exists(context.Server.MapPath(newurl.Split('?')[0])))
                    context.RewritePath(newurl);
            }
        }

        protected virtual string GetNewUrl(Match m, string url, string query)
        {
            for (var i = 1; i < m.Groups.Count; i++)
            {
                url = url.Replace("{" + (i - 1) + "}", m.Groups[i].Value);
            }
            if (!string.IsNullOrEmpty(query)) url += (url.IndexOf("?") >= 0 ? "&" : "?") + query;
            return url;
        }
    }
}
