using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;

namespace X.App.Views.app.red
{
    public class detail : xview
    {
        public int id { get; set; }

        x_red r = null;
        protected override string GetParmNames
        {
            get
            {
                return "id";
            }
        }

        protected override void InitDict()
        {
            base.InitDict();
            r = cu.x_red.FirstOrDefault(o => o.red_id == id);
            dict.Add("r", r);

            dict.Add("count", r.count);
            dict.Add("min", r.x_red_get.Min(o => o.amount) / 100.0);
            dict.Add("max", r.x_red_get.Max(o => o.amount) / 100.0);
            dict.Add("amount", r.amount);
            dict.Add("st", r.ctime.Value.ToString("yy-MM-dd HH:mm"));
            if (r.ftime != null)
            {
                dict.Add("et", r.ftime.Value.ToString("yy-MM-dd HH:mm"));
                var sp = (r.ftime - r.ctime).Value;
                dict.Add("ut", sp.Days + "天" + sp.Hours + "小时" + sp.Minutes + "分钟");
            }
            dict.Add("gc", r.geted);
            dict.Add("cmax", r.x_red_get.Where(o => o.status == 2).Max(o => o.amount) / 100.0);
            dict.Add("cmin", r.x_red_get.Where(o => o.status == 2).Min(o => o.amount) / 100.0);
            dict.Add("rmax", r.x_red_get.Where(o => o.status == 2).GroupBy(o => o.get_op).Select(o => new { uid = o.Key, sum = o.Sum(c => c.ramount / 100.0M) }).OrderByDescending(o => o.sum).FirstOrDefault());
            dict.Add("dp", 0);
            dict.Add("pmax", r.x_red_get.Where(o => o.status == 2).GroupBy(o => o.get_op).Select(o => new { uid = o.Key, ct = o.Count() }).OrderByDescending(o => o.ct).FirstOrDefault());
            dict.Add("dmax", 0);
            dict.Add("adc", r.adcount ?? 0);
            dict.Add("qdc", r.qrcount ?? 0);

        }

        public string GetHtml(long upid)
        {
            var q = from g in r.x_red_get
                    where g.red_get_id == upid && g.status == 2
                    select new
                    {
                        id = g.red_get_id,
                        am = (g.amount.Value / (decimal)100).ToString("F2"),
                        nk = g.get_nk,
                        hd = g.get_img,
                        ram = (g.myramount / 100M).Value.ToString("F2"), //r.x_red_get.Where(o => o. == g.get_op && o.status == 2).Sum(o => o.ramount / 100.0M).Value.ToString("F2"),
                        dt = g.ctime.Value.ToString("MM月dd日 HH:mm"),
                        ow = g.get_op,
                        ht1 = g.x_red.x_ad_hit.Count(o => o.red_id == g.red_id && o.tp == 1),
                        ht2 = g.x_red.x_ad_hit.Count(o => o.red_id == g.red_id && o.tp == 2)
                    };

            var list = q.ToList();

            if (list.Count == 0) return "";
            var sb = new StringBuilder();
            sb.Append("<ul>");
            foreach (var r in list.OrderByDescending(o => o.dt))
            {
                sb.Append("<li data-id='" + r.ow + "' data-ht1='" + r.ht1 + "' data-ht2='" + r.ht2 + "'>");
                sb.Append("<a href=\"javascript:; \" class=\"item\"><img src=\"" + r.hd + "\" title=\"" + r.nk + " " + r.dt + " 返：" + r.ram + " 广告：" + (r.ht1 + r.ht2) + "(" + r.ht1 + "," + r.ht2 + ")\" width='40' />" + r.am + "元</a>");
                sb.Append(GetHtml(r.id));
                sb.Append("</li>");
            }
            sb.Append("</ul><span class='clr'></span>");
            return sb.ToString();
        }
    }
}
