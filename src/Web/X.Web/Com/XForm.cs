using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using X.Core.Utility;

namespace X.Web.Com
{
    public class XForm
    {
        static Views.View view = null;
        static Assembly assb = Assembly.Load("X.Web");
        public XForm(Views.View v)
        {
            view = v;
        }

        #region 私有方法
        static ict loadct(string cn, string ps)
        {
            var ct = assb.CreateInstance("X.Web.Com.XForm+" + cn) as ict;
            if (ct == null) return null;

            var reg_p = new Regex("([\\d\\w]+)=\"([^\"]+)\"");
            var tp = ct.GetType();

            foreach (Match mp in reg_p.Matches(ps))
            {
                var pn = mp.Groups[1].Value.ToLower();
                var pv = mp.Groups[2].Value;
                var p = tp.GetProperty(pn);
                if (p == null) continue;
                switch (p.PropertyType.Name.ToLower())
                {
                    case "int64":
                    case "int32":
                    case "int16":
                    case "int":
                        var def = 0;
                        int.TryParse(pv, out def);
                        p.SetValue(ct, def, null);
                        break;
                    case "string":
                    default:
                        p.SetValue(ct, pv, null);
                        break;
                }
            }

            return ct;
        }
        static string loadcts(string body)
        {
            var ctreg = new Regex("<x:(\\w+)([^>]*)/>");
            return ctreg.Replace(body, o =>
            {
                var ct = loadct(o.Groups[1].Value, o.Groups[2].Value);
                ct.init("");
                return ct == null ? "" : ct.ToString();
            });
        }
        #endregion

        #region 公有方法
        public string Parse(string html)
        {
            var frgex = new Regex("<x:form([^>]*)>([\\S\\s\\n]*?)</x:form>");
            foreach (Match m in frgex.Matches(html))
            {
                var f = loadct("form", m.Groups[1].Value) as form;
                f.init(m.Groups[2].Value);
                html = html.Replace(m.Groups[0].Value, f.ToString());
            }
            return html;
        }
        #endregion

        interface ict
        {
            void init(string body);
            string valid();
        }

        abstract class b
        {
            protected StringBuilder sb_html = new StringBuilder();
            public string css { get; set; }
            public override string ToString() { return sb_html.ToString(); }
        }

        class ct : b
        {
            public string width { get; set; }
            public string title { get; set; }
            public string name { get; set; }
            public string tip { get; set; }
            public string def { get; set; }
        }

        class form : b, ict
        {
            public string api { get; set; }
            public string callback { get; set; }
            public string prepost { get; set; }
            /// <summary>
            /// 0、红框
            /// 1、弹窗+红框
            /// </summary>
            public int tip { get; set; }
            public void init(string body)
            {
                var html = ("<form class='x-form' {--}>");
                if (!string.IsNullOrEmpty(api)) html = html.Replace("{--}", "x-api='" + api + "' {--}");
                if (!string.IsNullOrEmpty(callback)) html = html.Replace("{--}", "x-callback='" + callback + "' {--}");
                if (!string.IsNullOrEmpty(prepost)) html = html.Replace("{--}", "x-prepost='" + prepost + "' {--}");
                if (tip > 0) html = html.Replace("{--}", "x-tip='" + tip + "' {--}");
                sb_html.Append(html.Replace("{--}", ""));

                var row = new Regex("<x:row([^>]*)>([\\S\\s\\n]*?)</x:row>");
                body = row.Replace(body, o =>
                {
                    var ct = loadct("row", o.Groups[1].Value) as row;
                    ct.init(o.Groups[2].Value);
                    return ct == null ? "" : ct.ToString();
                });

                body = loadcts(body);
                sb_html.Append(body);

                sb_html.Append("</form>");
            }

            public string valid()
            {
                throw new NotImplementedException();
            }
        }

        class row : b, ict
        {
            public string title { get; set; }
            public void init(string body)
            {
                sb_html.Append("<div class=\"row " + css + "\">");
                if (!string.IsNullOrEmpty(title)) sb_html.Append("<label class='lbe'>" + title + "：</label>");
                sb_html.Append("<div class='rt'>" + loadcts(body) + "</div>");
                sb_html.Append("</div>");
            }

            public string valid()
            {
                throw new NotImplementedException();
            }
        }

        class text : ct, ict
        {
            /// <summary>
            /// 验证字符串
            /// </summary>
            public string chk { get; set; }
            /// <summary>
            /// 类型
            /// 1、输入框
            /// 2、密码框
            /// 3、多行框
            /// </summary>
            public int tp { get; set; }
            /// <summary>
            /// 大小
            /// 1、mini
            /// 2、small
            /// 3、medium
            /// 4、large
            /// 5、xlarge
            /// 6、xxlarge
            /// </summary>
            public int size { get; set; }

            static string[] sizes = "mini,small,medium,large,xlarge,xxlarge".Split(',');
            public void init(string body)
            {
                if (tp <= 0 || tp > 3) tp = 1;
                if (size <= 0 || size > 6) size = tp == 3 ? 4 : 2;

                var c = new check(chk);
                sb_html.Append("<div class='li text " + name + " " + css + "' id='li_" + name + "' >");

                if (string.IsNullOrEmpty(title))
                {
                    sb_html.Append(getinput(c));
                }
                else if (title.Contains(" "))
                {
                    var ti = title.Split(' ');
                    if (!string.IsNullOrEmpty(ti[0]) && !string.IsNullOrEmpty(ti[1]))
                    {
                        sb_html.Append("<div class='input-prepend input-append'>");
                        sb_html.Append("<span class='add-on'>" + ti[0] + "</span>");
                        sb_html.Append(getinput(c));
                        sb_html.Append("<span class='add-on'>" + ti[1] + "</span>");
                    }
                    else if (!string.IsNullOrEmpty(ti[0]))
                    {
                        sb_html.Append("<div class='input-prepend'>");
                        sb_html.Append("<span class='add-on'>" + ti[0] + "</span>");
                        sb_html.Append(getinput(c));
                    }
                    else
                    {
                        sb_html.Append("<div class='input-append'>");
                        sb_html.Append(getinput(c));
                        sb_html.Append("<span class='add-on'>" + ti[1] + "</span>");
                    }
                    sb_html.Append("</div>");
                }
                else
                {
                    sb_html.AppendFormat("<label class='lbe' for='id_{0}'>" + (c.no ? "<span class='star'>*</span>" : "") + "{1}</label>", name, title + "：");
                    sb_html.Append(getinput(c));
                }

                sb_html.Append("</div>");
            }

            string getinput(check c)
            {
                if (tp == 1)
                    return "<input type='text' id='id_" + name + "' name='" + name + "' x-check='" + c.ToString() + "' title='" + title + "' placeholder='" + (string.IsNullOrEmpty(tip) ? title : tip) + "' class='input-" + sizes[size - 1] + "' value='" + def + "' />";
                else if (tp == 2)
                    return "<input type='password' id='id_" + name + "' name='" + name + "' title='" + title + "' x-check='" + c.ToString() + "' placeholder='" + (string.IsNullOrEmpty(tip) ? title : tip) + "' class='input-" + sizes[size - 1] + "' value='" + def + "' />";
                else if (tp == 3)
                    return string.Format("<textarea id='id_{0}' name='{0}' placeholder='{1}' x-check='{2}' class='input-{3}' title='" + title + "' >{4}</textarea>",
                             name,
                             string.IsNullOrEmpty(tip) ? title : tip,
                             c.ToString(),
                             sizes[size - 1],
                             def);
                return "";
            }

            public string valid()
            {
                throw new NotImplementedException();
            }
        }

        class pick : ct, ict
        {
            public string callback { get; set; }
            /// <summary>
            /// 选择个数
            /// </summary>
            public string count { get; set; }
            /// <summary>
            /// 1、不为空
            /// </summary>
            public int no { get; set; }
            /// <summary>
            /// 联动：目标控件
            /// </summary>
            public string to { get; set; }
            /// <summary>
            /// 显示模式
            /// 
            /// </summary>
            public int mode { get; set; }
            /// <summary>
            /// 值范围
            /// src 为 date时有效
            /// </summary>
            public string range { get; set; }
            /// <summary>
            /// 来源
            /// code:字典 代号:上级:按字母显示
            /// url:网址
            /// loc:指定选项
            /// color:颜色选择
            /// date:日期时间选择
            /// </summary>
            public string src { get; set; }

            public void init(string body)
            {
                sb_html.Append("<div class='li pick " + name + " " + css + "' id='li_" + name + "'" + (count != null ? " x-count='" + count + "'" : "") + (callback != null ? " x-callback='" + callback + "'" : "") + " >");

                var ti = (title ?? "").Split(' ');

                if (!string.IsNullOrEmpty(def)) def = def.Trim();

                if (!string.IsNullOrEmpty(ti[0])) sb_html.AppendFormat("<label class='lbe' for='id_{0}'>" + (no == 1 ? "<span class='star'>*</span>" : "") + "{1}</label>", name, ti[0] + "：");

                var data = src.Split(':');
                if (data[0] == "dict" && !string.IsNullOrEmpty(def)) def = def + "|" + view.GetDictName(data[1], def);

                if (mode == 2 && (data[0] == "dict" || data[0] == "loc"))
                {
                    sb_html.Append("<div class='btn-group' title='" + title + "' x-check='" + (no == 1 ? "{\"no\":true}" : "") + "' x-val='" + def + "' name='" + name + "'>");
                    if (data[0] == "dict")
                    {
                        var d = view.GetDictList(data[1], "0");
                        foreach (var i in d) sb_html.Append("<span class='btn " + (def == i.value || (def != null && def.Contains("[" + i.value + "]")) ? "btn-primary" : "") + "' name='" + name + "' x-val='" + i.value + "'>" + i.name + "</span>");
                    }
                    else
                    {
                        var st = data[1].Split('|'); var i = 1;
                        foreach (var p in st)
                        {
                            var _p = p.Split('-');
                            var _v = _p.Length == 1 ? i++ + "" : _p[1];
                            sb_html.Append("<span class='btn " + (def == _v || (def != null && def.Contains("[" + _v + "]")) ? "btn-primary" : "") + "' name='" + name + "' x-val='" + _v + "'>" + _p[0] + "</span>");
                        }
                    }
                    sb_html.Append("</div>");
                }
                else if (!string.IsNullOrEmpty(def))//data[0] == "url" &&
                {
                    var d = def.Split('|');
                    sb_html.Append("<span class='btn' x-to='" + to + "' x-src='" + src + "' name='" + name + "' x-val='" + d[0] + "' title='" + title + "' x-check='" + (no == 1 ? "{\"no\":true}" : "") + "' title='" + (string.IsNullOrEmpty(tip) ? "请选择 " + title : tip) + "' onclick='x.pick.show(this)'><span class='text' txt=''>" + (string.IsNullOrEmpty(def) ? "请选择" : d.Length == 2 ? d[1] : d[0]) + "</span><i class='icon-caret-down'></i></span>");
                }
                else
                {
                    sb_html.Append("<span class='btn' x-to='" + to + "' x-src='" + src + "' name='" + name + "' x-val='' title='" + title + "' x-check='" + (no == 1 ? "{\"no\":true}" : "") + "' title='" + (string.IsNullOrEmpty(tip) ? "请选择 " + title : tip) + "' onclick='x.pick.show(this)'><span class='text' txt=''>请选择</span><i class='icon-caret-down'></i></span>");
                }
                //else
                //{
                //    sb_html.Append("<span class='btn' x-src='" + src + "' name='" + name + "' x-val='" + def + "' title='" + title + "' x-check='" + (no == 1 ? "{\"no\":true}" : "") + "' title='" + (string.IsNullOrEmpty(tip) ? "请选择 " + title : tip) + "' onclick='x.pick.show(this)'><span class='text' txt=''>请选择</span><i class='icon-caret-down'></i></span>");
                //}

                sb_html.Append("</div>");
            }

            public string valid()
            {
                throw new NotImplementedException();
            }
        }

        class upload : ct, ict
        {
            /// <summary>
            /// 上传类型
            /// 图片 img(jpg,png,gif)
            /// 文件 file(rar,zip,xls,csv,doc,pdf)
            /// 数据 x(xls,csv,txt)
            /// 媒体 media(flv,mp4,wav,mp3,swf)
            /// 文档 paper(swf) flash文档
            /// </summary>
            public string tp { get; set; }
            /// <summary>
            /// 1、不能为空
            /// </summary>
            public int no { get; set; }
            /// <summary>
            /// 上传个数
            /// </summary>
            public int count { get; set; }

            public void init(string body)
            {
                sb_html.Append("<div class='li upload " + name + " ' id='li_" + name + "' x-tp='" + tp + "' x-count='" + (count == 0 ? 1 : count) + "' x-name='" + name + "'>");
                if (!string.IsNullOrEmpty(title)) sb_html.Append("<label class='lbe' for='id_" + name + "'>" + title + "：</label>");
                sb_html.Append("<span class='btn xbtn'><i class='icon-upload-alt'></i>上传文件</span><input type='hidden' name='" + name + "' id='id_" + name + "' title='" + title + "' x-check='" + (no == 1 ? "{\"no\":true}" : "") + "' value='" + def + "' /><div class='" + (tp == "img" ? "imgs" : "files") + "'></div>");
                sb_html.Append("</div>");
            }

            public string valid()
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 字符串
        /// no|t:type,reg,msg|len:min,max|cp:min,max|re:name,type
        /// </summary>
        class check
        {
            /// <summary>
            /// 字符是否为空
            /// 数字是否为0
            /// </summary>
            public bool no { get; set; }
            /// <summary>
            /// 格式：type-reg-msg
            /// reg 自定义正则
            /// msg 正定义错误消息
            /// type 验证类型
            ///   mail 邮件
            ///   num 整数
            ///   ch 汉字
            ///   mp 手机号
            ///   tel 电话号码
            ///   nd 小数或整数
            ///   d 日期
            ///   dt 日期时间
            ///   url 网址
            ///   char 字符（字母、数字、汉字）
            ///   c 颜色值 #000000
            ///   cust 自定义
            /// </summary>
            public string tp { get; set; }
            /// <summary>
            /// 格式：min-max
            /// 可只传一个值，但“-”不能少
            /// 验证时包含当前值
            /// </summary>
            public string len { get; set; }
            /// <summary>
            /// 范围
            /// 格式：min-max
            /// 可只传一个值
            /// 验证时包含当前值
            /// </summary>
            public string cp { get; set; }
            /// <summary>
            /// 比较
            /// 格式：name-type
            /// name 目标控件名
            /// type 比较类型 -1 小于 0 等于 1 大于
            /// </summary>
            public string re { get; set; }

            public check(string chkstr)
            {
                if (string.IsNullOrEmpty(chkstr)) return;
                var cs = chkstr.Split('|');
                foreach (var c in cs)
                {
                    var p = c.Split(':');
                    if (p[0] == "no") no = true;
                    if (p[0] == "len") len = p[1];
                    if (p[0] == "re") re = p[1];
                    if (p[0] == "cp") cp = p[1];
                    if (p[0] == "t")
                    {
                        var rt = p[1].Split('-');
                        if (rt[0] == "cst") tp = "{\"reg\":\"" + rt[1] + "\",\"msg\":\"" + rt[2] + "\"}";
                        else tp = rt[0];
                    }
                }
            }

            public override string ToString()
            {
                return Serialize.ToJson(this, true);
            }
        }
    }

}
