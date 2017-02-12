using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Linq;
using System.Web;
using X.Data;
using X.Web.Cache;

namespace X.Web
{
    public abstract class XFace
    {
        /// <summary>
        /// 当前网站标识
        /// </summary>
        protected int site_id = 0;
        /// <summary>
        /// 当前语言标识
        /// </summary>
        protected int lang_id = 0;
        /// <summary>
        /// 数据库对象
        /// </summary>
        protected RbtDBDataContext DB = null;
        /// <summary>
        /// 会话缓存
        /// </summary>
        protected SessionCache Session = null;
        /// <summary>
        /// 上下文缓存
        /// </summary>
        protected ContextCache XCache = null;
        /// <summary>
        /// Http上下文
        /// </summary>
        protected HttpContext Context = null;
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(HttpContext c)
        {
            DB = new RbtDBDataContext() { DeferredLoadingEnabled = true };
            Session = new SessionCache();
            XCache = new ContextCache();
            Context = c;
        }
        /// <summary>
        /// 参数验证
        /// </summary>
        protected virtual void Validate() { }
        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="parms"></param>
        protected void SetParms(NameValueCollection parms)
        {
            var postes = parms;
            var type = GetType();
            foreach (string k in postes.Keys)
            {
                var p = type.GetProperty(k);
                if (p == null) continue;
                var attr = p.GetCustomAttributes(false);
                object v = null;
                switch (p.PropertyType.Name.ToLower())
                {
                    case "string":
                        v = Context.Server.HtmlEncode(postes[k]);
                        Checker.check(attr, v.ToString());
                        break;
                    case "int":
                    case "int32":
                        var iv = 0;
                        int.TryParse(postes[k], out iv);
                        Checker.check(attr, iv);
                        v = iv;
                        break;
                    case "int64":
                        long lv = 0;
                        long.TryParse(postes[k], out lv);
                        Checker.check(attr, lv);
                        v = lv;
                        break;
                    case "decimal":
                        var dev = (decimal)0.0;
                        decimal.TryParse(postes[k], out dev);
                        Checker.check(attr, dev);
                        v = dev;
                        break;
                    case "datetime":
                        DateTime dtv = DateTime.MinValue;
                        DateTime.TryParse(postes[k], out dtv);
                        Checker.check(attr, dtv);
                        v = dtv;
                        break;
                    default:
                        continue;
                }
                if (v != null) p.SetValue(this, v, null);
            }
        }

        /// <summary>
        /// 参数检查器
        /// </summary>
        class Checker
        {
            public static void check(object[] attr, string v)
            {
                if (attr.Length == 0) return;
                var pa = attr[0] as ParmsAttr;

                if (pa.def != null && string.IsNullOrEmpty(v)) v = pa.def.ToString();
                if (pa.req && string.IsNullOrEmpty(v)) throw new XExcep("0x0003", pa.name);

                string min = (string)pa.min;
                string max = (string)pa.max;
                if (!string.IsNullOrEmpty(min) && v.CompareTo(min) < 0) throw new XExcep("0x0004", String.Format("{0}的值要大于{1}", pa.name, min));
                if (!string.IsNullOrEmpty(max) && v.CompareTo(max) > 0) throw new XExcep("0x0004", String.Format("{0}的值要小于{1}", pa.name, max));

                if (string.IsNullOrEmpty(pa.len)) return;
                var ls = pa.len.Split(',');
                if (ls.Length == 1 && v.Length != int.Parse(ls[0])) throw new XExcep("0x0004", String.Format("{0}应为{1}个字符", pa.name, ls[0]));
                else if (ls.Length > 1)
                {
                    if (!string.IsNullOrEmpty(ls[0]) && v.Length < int.Parse(ls[0])) throw new XExcep("0x0004", String.Format("{0}至少{1}个字符", pa.name, ls[0]));
                    if (!string.IsNullOrEmpty(ls[1]) && v.Length > int.Parse(ls[1])) throw new XExcep("0x0004", String.Format("{0}最多{1}个字符", pa.name, ls[1]));
                }
            }
            public static void check(object[] attr, int v)
            {
                if (attr.Length == 0) return;
                var pa = attr[0] as ParmsAttr;

                if (pa.def != null && v == 0) v = (int)pa.def;
                if (pa.req && v == 0) throw new XExcep("0x0003", pa.name);

                int? min = null, max = null;
                min = Convert.ToInt32(pa.min);
                max = Convert.ToInt32(pa.max);

                if (pa.min != null && v < min) throw new XExcep("0x0004", String.Format("{0}的值要大于{1}", pa.name, min));
                if (pa.max != null && v > max) throw new XExcep("0x0004", String.Format("{0}的值要小于{1}", pa.name, max));
            }
            public static void check(object[] attr, decimal v)
            {
                if (attr.Length == 0) return;
                var pa = attr[0] as ParmsAttr;

                if (pa.def != null && v == 0) v = (decimal)pa.def;
                if (pa.req && v == 0) throw new XExcep("0x0003", pa.name);

                if (pa.min == null && pa.max == null) return;
                decimal min = Convert.ToDecimal(pa.min);
                decimal max = Convert.ToDecimal(pa.max);
                if (v < min && pa.min != null) throw new XExcep("0x0004", String.Format("{0}的值要大于{1}", pa.name, min));
                if (v > max && pa.max != null) throw new XExcep("0x0004", String.Format("{0}的值要小于{1}", pa.name, max));
            }
            public static void check(object[] attr, long v)
            {
                if (attr.Length == 0) return;
                var pa = attr[0] as ParmsAttr;

                if (pa.def != null && v == 0) v = (long)pa.def;
                if (pa.req && v == 0) throw new XExcep("0x0003", pa.name);

                if (pa.min == null && pa.max == null) return;
                long min = Convert.ToInt64(pa.min);
                long max = Convert.ToInt64(pa.max);
                if (v < min && pa.min != null) throw new XExcep("0x0004", String.Format("{0}的值要大于{1}", pa.name, min));
                if (v > max && pa.max != null) throw new XExcep("0x0004", String.Format("{0}的值要小于{1}", pa.name, max));
            }
            public static void check(object[] attr, DateTime v)
            {
                if (attr.Length == 0) return;
                var pa = attr[0] as ParmsAttr;

                if (pa.def != null && v == DateTime.MinValue) v = (DateTime)pa.def;
                if (pa.req && v == null) throw new XExcep("0x0003", pa.name);

                if (pa.min == null && pa.max == null) return;
                DateTime min = Convert.ToDateTime(pa.min);
                DateTime max = Convert.ToDateTime(pa.max);
                if (v < min && pa.min != null) throw new XExcep("0x0004", String.Format("{0}的值要大于{1}", pa.name, min));
                if (v > max && pa.max != null) throw new XExcep("0x0004", String.Format("{0}的值要小于{1}", pa.name, max));
            }
        }

        /// <summary>
        /// 提交数据库更改
        /// </summary>
        protected void SubmitDBChanges()
        {
            try
            {
                DB.SubmitChanges(ConflictMode.ContinueOnConflict);
                DB.SubmitChanges(ConflictMode.FailOnFirstConflict);
            }
            catch (ChangeConflictException)
            {
                foreach (ObjectChangeConflict occ in DB.ChangeConflicts)
                {
                    occ.Resolve(RefreshMode.KeepChanges);
                }
            }
        }
        /// <summary>
        /// 获取字典列表
        /// </summary>
        /// <param name="code"></param>
        /// <param name="upval"></param>
        /// <returns></returns>
        public List<x_dict> GetDictList(string code, string upval)
        {
            return x_dict.GetDictList(code, upval, DB);
        }
        /// <summary>
        /// 获取字典文字
        /// </summary>
        /// <param name="code"></param>
        /// <param name="value">
        /// 多个值用 , 隔开
        /// </param>
        /// <returns></returns>
        public string GetDictName(string code, object value, string split)
        {
            return x_dict.GetDictName(code, value, split, DB);
        }
        public string GetDictName(string code, object val)
        {
            return GetDictName(code, val + "", "、");
        }
        /// <summary>
        /// 获取格式化日期
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        protected string GetDateString(DateTime? dt, string format)
        {
            if (dt == null) return "";
            else return dt.Value.ToString(format);
        }

        /// <summary>
        /// 获取响应
        /// </summary>
        /// <returns></returns>
        public abstract byte[] GetResponse();

        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected string GetReqParms(string key)
        {
            try
            {
                return Context.Request.Params.Get(key);
            }
            catch { return ""; }
        }

        [AttributeUsage(AttributeTargets.Property)]
        protected class ParmsAttr : Attribute
        {
            public string name { get; set; }
            /// <summary>
            /// 字符串不许空
            /// 数字不为0
            /// </summary>
            public bool req { get; set; }
            /// <summary>
            /// 长度min,max
            /// </summary>
            public string len { get; set; }
            /// <summary>
            /// 最小值
            /// </summary>
            public object min { get; set; }
            /// <summary>
            /// 最大值
            /// </summary>
            public object max { get; set; }
            /// <summary>
            /// 默认值
            /// </summary>
            public object def { get; set; }
        }
    }
}
