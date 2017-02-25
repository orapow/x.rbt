using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using X.Core.Plugin;
using X.Web.Com;

namespace X.Web.Views
{
    public abstract class View : XFace
    {
        /// <summary>
        /// 静态化时长 分钟
        /// 0 为不静态化
        /// -1 为永久静态化
        /// </summary>
        protected virtual int html_time
        {
            get { return 0; }
        }

        ///// <summary>
        ///// 错误信息显示视图名称
        ///// </summary>
        //protected virtual string err_viewname
        //{
        //    get { return "com.err"; }
        //}

        /// <summary>
        /// 获取页面参数
        /// </summary>
        protected void GetPageParms()
        {
            if (string.IsNullOrEmpty(GetParmNames)) return;

            var ns = GetParmNames.Split('-');
            var vs = GetReqParms("p").Split('-');

            var ps = new NameValueCollection();
            for (var i = 0; i < ns.Length; i++)
            {
                if (i >= vs.Length) { ps.Add(ns[i], ""); continue; }
                if (dict.ContainsKey(ns[i])) dict[ns[i]] = Context.Server.UrlDecode(vs[i]);
                else dict.Add(ns[i], Context.Server.UrlDecode(vs[i]));
                ps.Add(ns[i], vs[i]);
            }

            SetParms(ps);
        }

        /// <summary>
        /// 模板引擎数据字典
        /// </summary>
        public TplDict dict = new TplDict();

        protected virtual void InitDict()
        {
            dict.Add("T", this);
        }

        protected virtual string GetParmNames
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 初始化视图
        /// </summary>
        protected virtual void InitView()
        {
            GetPageParms();
        }

        public virtual string GetTplFile()
        {
            return GetType().FullName.Replace("X.App.Views.", string.Empty).Replace(".", "/");
        }

        /// <summary>
        /// 执行结果
        /// </summary>
        /// <returns></returns>
        public override byte[] GetResponse()
        {
            var qs = Context.Request.QueryString;
            var v = qs["v"];

            var ht = "/html/" + v;
            ht += ((!string.IsNullOrEmpty(qs["p"]) ? "-" + qs["p"] : "") + ".html").ToLower();

            var file = Context.Server.MapPath(ht); //Context.Request.RawUrl;

            if (html_time > 0 || html_time == -1)
            {
                var fi = new FileInfo(file);
                if (fi.Exists && (DateTime.Now - fi.LastWriteTime).TotalMinutes < html_time) return File.ReadAllBytes(file);
            }

            var html = "";
            try
            {
                InitView();
                InitDict();
                html = Tpl.Instance.Merge(GetTplFile() + ".html", dict);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException is XExcep)
                {
                    throw (XExcep)ex.InnerException;
                }
                throw ex;
            }

            dict.Clear();

            var xf = new XForm(this);
            html = xf.Parse(html);

            #region 压缩页面
            //html = Regex.Replace(html, "(/\\*([^*]|[\r\n]|(\\*+([^*/]|[\r\n])))*\\*+/)|([^:]//.*)", "");
            //html = Regex.Replace(html, "\\s{2,}", " ");//(>)?\\s+< //去掉空格
            #endregion

            var data = Encoding.UTF8.GetBytes(html);

            if (html_time > 0 || html_time == -1)
            {
                try
                {
                    Directory.CreateDirectory(file.Substring(0, file.LastIndexOf('\\')));
                    File.WriteAllBytes(file, data);
                }
                catch { }
            }

            return data;

        }

    }
}
