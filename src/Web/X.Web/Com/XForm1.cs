using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Core.Utility;

namespace X.Web.Com
{
    public class XForm1
    {
        private XFace face = null;
        /// <summary>
        /// Initializes a new instance of the XForm class.
        /// </summary>
        public XForm1(XFace fa)
        {
            face = fa;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="name"></param>
        /// <param name="css"></param>
        /// <param name="title"></param>
        /// <param name="tip"></param>
        /// <param name="def"></param>
        /// <param name="check">
        /// no|t:type-reg-msg|len:min,max|range:min,max|re:name,type|
        /// </param>
        /// <returns></returns>
        public string GetInput(int t, string name, string css, string title, string tip, string def, string check)
        {
            var txt_sb = new StringBuilder();
            txt_sb.Append("<div class=\"li\" id=\"li_" + name + "\">");
            var c = new Check(check);
            if (!string.IsNullOrEmpty(title)) txt_sb.AppendFormat("<label class=\"lbe\" for=\"id_{0}\">{1}：</label>", name, (c.notempty ? "<span class='red'>*</span>" : "") + title);
            txt_sb.AppendFormat("<input type=\"{0}\"  id=\"id_{1}\" name=\"{1}\" placeholder=\"{2}\" class=\"{3}\" value=\"{4}\" data-check='{5}' />",
                        t == 1 ? "text" : "password",
                        name,
                        string.IsNullOrEmpty(tip) ? title : tip,
                        string.IsNullOrEmpty(css) ? "input-xlarge" : css,
                        def,
                        c.ToString());

            txt_sb.Append("</div>");
            return txt_sb.ToString();
        }
        public string GetInput(string name, string title, string check, string def)
        {
            return GetInput(1, name, "", title, "", def, check);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="css"></param>
        /// <param name="title"></param>
        /// <param name="tip"></param>
        /// <param name="def"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        public string GetMuti(string name, string css, string title, string tip, string def, string check)
        {
            var txt_sb = new StringBuilder();
            txt_sb.Append("<div class=\"li\" id=\"li_" + name + "\">");
            var c = new Check(check);
            if (!string.IsNullOrEmpty(title)) txt_sb.AppendFormat("<label class=\"lbe\" for=\"id_{0}\">{1}：</label>", name, (c.notempty ? "<span class='red'>*</span>" : "") + title);
            txt_sb.AppendFormat("<textarea id=\"id_{0}\" name=\"{0}\" placeholder=\"{1}\" class=\"{2}\" data-check='{4}'>{3}</textarea>",
                        name,
                        string.IsNullOrEmpty(tip) ? title : tip,
                        string.IsNullOrEmpty(css) ? "input-xlarge" : css,
                        def,
                        c.ToString());

            txt_sb.Append("</div>");
            return txt_sb.ToString();
        }
        public string GetMuti(string name, string title, string def, string check)
        {
            return GetMuti(name, "", title, "", def, check);
        }


        public string GetPicker(string name, string title, string defid, string deftxt, string src)
        {
            return GetPicker(name, "", title, "", defid, deftxt, "", src, "");
        }
        public string GetPicker(string name, string title, string defid, string deftxt, string src, string chk, string upid)
        {
            return GetPicker(name, "", title, "", defid, deftxt, chk, src, upid);
        }
        public string GetPicker(string name, string title, string defid, string deftxt, string src, string upid)
        {
            return GetPicker(name, "", title, "", defid, deftxt, "", src, upid);
        }
        public string GetPicker(string name, string css, string title, string tip, string defid, string deftxt, string check, string src, string upid)
        {
            var txt_sb = new StringBuilder();
            if (!string.IsNullOrEmpty(title)) txt_sb.Append("<div class=\"li\" id=\"li_" + name + "\">");
            var c = new Check(check);
            if (string.IsNullOrEmpty(deftxt) && !string.IsNullOrEmpty(defid))
            {
                defid = defid.Trim();
                var ps = src.Split(':');
                if (ps[0] == "dict")
                {
                    deftxt = face.GetDictName(ps[1], defid);
                }
            }
            if (!string.IsNullOrEmpty(title)) txt_sb.AppendFormat("<label class=\"lbe\" for=\"id_{0}\">{1}：</label>", name, (c.notempty ? "<span class='red'>*</span>" : "") + title);
            txt_sb.AppendFormat("<span class=\"btn\" data-src=\"{0}\" data-check='{1}' name=\"{2}\" id=\"id_{2}\" placeholder=\"{3}\" data-val=\"{4}\" parms=\"{6}\"><span class=\"text\" txt=\"{5}\">{3}：{5}</span></span>",
                        src,
                        c.ToString(),
                        name,
                        string.IsNullOrEmpty(tip) ? title : tip,
                        defid,
                        deftxt,
                        string.IsNullOrEmpty(upid) ? "" : "-" + upid);
            if (!string.IsNullOrEmpty(title)) txt_sb.Append("</div>");
            return txt_sb.ToString();
        }

        public string GetSwitch(string name, string title, string def)
        {
            var txt_sb = new StringBuilder();
            txt_sb.Append("<div class=\"li\" id=\"li_" + name + "\">");
            if (!string.IsNullOrEmpty(title)) txt_sb.AppendFormat("<label class=\"lbe\" for=\"id_{0}\">{1}：</label>", name, title);
            txt_sb.AppendFormat("<span class=\"switch {0}\" data-check=\"\" name=\"{1}\"><i class=\"off\">否</i><i class=\"on\">是</i></span></div>", (def == "1" || def.ToLower() == "true" ? "selected" : ""), name);
            return txt_sb.ToString();
        }

        /// <summary>
        /// 获取多选
        /// </summary>
        /// <param name="title">显示标题</param>
        /// <param name="names">字段名列表</param>
        /// <param name="textes">显示值列表</param>
        /// <param name="defs">默认值列表</param>
        /// <returns></returns>
        public string GetCheckBoxes(string title, string names, string textes, string defs, string chk)
        {
            var txt_sb = new StringBuilder();
            var c = new Check(chk);
            txt_sb.Append("<div class=\"li\" id=\"li_" + names.Replace(",", "_") + "\">");
            if (!string.IsNullOrEmpty(title)) txt_sb.AppendFormat("<label class=\"lbe\">{0}：</label>", (c.notempty ? "<span class='red'>*</span>" : "") + title);
            var cs = textes.Split(',');
            var ns = names.Split(',');
            var ds = defs.Split(',');

            for (var i = 0; i < cs.Length; i++)
            {
                txt_sb.AppendFormat("<span class=\"btn checkbox {0}\" data-check='{3}' name=\"{1}\">{2}</span>", (i < ds.Length && (ds[i].Trim() == "1" || ds[i].ToLower() == "true") ? "btn-primary" : ""), ns[i], cs[i], c.ToString());
                //txt_sb.AppendFormat("<span class=\"btn checkbox {0}\" data-check='{3}' name=\"{1}\">{2}</span>", (defs.Contains(ns[i]) == true ? "btn-primary" : ""), ns[i], cs[i], new Check(chk).ToString());
            }
            txt_sb.Append("</div>");
            return txt_sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="names"></param>
        /// <param name="chks"></param>
        /// <param name="defs"></param>
        /// <param name="src">
        /// 来源:参数
        /// 来源：dict 字典 loc 本地字符
        /// 参数：字典代码  字符串（文字-值|文字-值）
        /// </param>
        /// <returns></returns>
        public string GetRadioes(string title, string name, string def, string src, string chk)
        {
            var txt_sb = new StringBuilder();
            txt_sb.Append("<div class=\"li\" id=\"li_" + name + "\">");
            var c = new Check(chk);
            if (!string.IsNullOrEmpty(title)) txt_sb.AppendFormat("<label class=\"lbe rdo\" name=\"{1}\" data-check='{2}' >{0}：</label>", (c.notempty ? "<span class='red'>*</span>" : "") + title, name, c.ToString());
            var s = src.Split(':');
            var ps = new Dictionary<string, string>();
            if (s[0] == "dict")
            {
                var list = face.GetDictList(s[1], "");
                foreach (var d in list)
                {
                    ps.Add(d.name, d.value);
                }
            }
            else if (s[0] == "loc")
            {
                var st = s[1].Split('|');
                var i = 1;
                foreach (var p in st)
                {
                    var _p = p.Split('-');
                    if (_p.Length == 1) ps.Add(_p[0], (i++) + "");
                    else if (_p.Length == 2) ps.Add(_p[0], _p[1]);
                }
            }
            foreach (var p in ps.Keys)
            {
                txt_sb.AppendFormat("<span class=\"btn radio {0}\" name=\"{1}\" data-val='{2}'>{3}</span>", (ps[p] == def ? "btn-primary" : ""), name, ps[p], p);
            }
            txt_sb.Append("</div>");
            return txt_sb.ToString();
        }

        /// <summary>
        /// 上传器
        /// </summary>
        /// <param name="name"></param>
        /// <param name="title"></param>
        /// <param name="uptype"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public string GetUploader(string name, string title, string uptype, string defimg, int max, string chk)
        {
            var txt_sb = new StringBuilder();
            var c = new Check(chk);
            txt_sb.Append("<div class=\"li\" id=\"li_" + name + "\">");
            txt_sb.Append("<label class=\"lbe\" for=\"id_" + name + "\">" + (c.notempty ? "<span class='red'>*</span>" : "") + title + "：</label><span class=\"upload imgs\" style=\"display: inline-block; vertical-align: middle;\"></span><span class=\"btn\" style=\"padding: 0; height: 40px; line-height: 40px; width: 50px;\" onclick=\"id_" + name + "_file.click();\" id=\"id_" + name + "_add\"><i class=\"icon-plus\" style=\"margin: 0; font-size: 14px;\"></i></span>");
            txt_sb.Append("<input type=\"hidden\" name=\"" + name + "\" id=\"txt_" + name + "\" value=\"\" data-check=\"no\" />");
            txt_sb.Append("<input type=\"file\" style=\"display: none\" id=\"id_" + name + "_file\" onchange=\"x.com.doupload(this, 'img', function (d) { " + name + "_uploaded(d.msg); });\" />");
            txt_sb.Append("<script type=\"text/javascript\">");
            txt_sb.Append("function " + name + "_uploaded(url) {");
            txt_sb.Append("var img = \"<img onerror='this.src=\\\"/img/no.jpg'\\\" src=\\\"\" + url + \"\\\" tip=\\\"\" + url + \"\\\" title='单击设为封面，双击删除' onclick=\\\"$('img.cover').removeClass('cover');$(this).addClass('cover'); if(typeof(upload_set_cover)!='undefined') upload_set_cover($(this).attr('tip'));\\\" ondblclick=\\\"txt_" + name + ".value=txt_" + name + ".value.replace(','+ $(this).attr('tip') + ',',',');$(this).remove();id_" + name + "_add.style.display='';if(typeof(upload_delimg)!='undefined') upload_delimg($(this).attr('tip'));\\\" />\";");
            txt_sb.Append("$(\"#li_" + name + " .imgs\").append(img);");
            txt_sb.Append("if (txt_" + name + ".value) txt_" + name + ".value = txt_" + name + ".value + url + \",\";");
            txt_sb.Append("else txt_" + name + ".value = \",\" + url + \",\";");
            txt_sb.Append("if ($('#li_" + name + " .imgs img').size() >= " + max + ") $('#id_" + name + "_add').hide(); id_" + name + "_file.outerHTML = id_" + name + "_file.outerHTML;");
            txt_sb.Append("}");
            if (!string.IsNullOrEmpty(defimg)) txt_sb.Append("var " + name + "_ps='" + defimg + "'.split(','); for(var p in " + name + "_ps){if(!" + name + "_ps[p])continue;" + name + "_uploaded(" + name + "_ps[p])}");
            txt_sb.Append("</script>");
            txt_sb.Append("</div>");
            return txt_sb.ToString();
        }
        public string GetUploader(string name, string title, string uptype, string defimg)
        {
            return GetUploader(name, title, uptype, defimg, 1, "no");
        }

        /// <summary>
        /// 
        /// </summary>
        public class Check
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="chkstr">
            /// no|t:type-reg-msg|len:min,max|range:min,max|re:name,type|
            /// </param>
            public Check(string chkstr)
            {
                var cs = chkstr.Split('|');
                foreach (var c in cs)
                {
                    var p = c.Split(':');
                    if (p[0] == "no") notempty = true;
                    if (p[0] == "len") len = p[1];
                    if (p[0] == "re") re = p[1];
                    if (p[0] == "range") range = p[1];
                    if (p[0] == "t")
                    {
                        var rt = p[1].Split('-');
                        if (rt[0] == "cst") type = "{\"reg\":\"" + rt[1] + "\",\"msg\":\"" + rt[2] + "\"}";
                        else type = rt[0];
                    }
                }
            }
            /// <summary>
            /// 为空验证
            /// </summary>
            public bool notempty { get; set; }
            /// <summary>
            /// 长度（min,max）
            /// 可只传一个值，min,max都值都可以为空
            /// </summary>
            public string len { get; set; }
            /// <summary>
            /// 范围（min,max）
            /// 可只传一个值，min,max都值都可以为空
            /// </summary>
            public string range { get; set; }
            /// <summary>
            /// 比较（name,type）
            /// name:目标控件名
            /// type:-1:小于|0:等于|1:大于
            /// </summary>
            public string re { get; set; }
            public string type { get; set; }
            public override string ToString()
            {
                return Serialize.ToJson(this, true);
            }
        }
    }
}
