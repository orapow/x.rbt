using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;

namespace X.App.Views
{
    public class list : xview
    {
        protected override int html_time
        {
            get
            {
                return 0;
            }
        }
        /// <summary>
        /// 方式：
        /// 1、区域（默认）
        /// 2、地铁
        /// </summary>
        public int ar { get; set; }
        public int t1 { get; set; }
        public int t2 { get; set; }
        /// <summary>
        /// 价格：
        /// min,max
        /// </summary>
        public string pr { get; set; }
        /// <summary>
        /// 居室
        /// </summary>
        public int room { get; set; }
        /// <summary>
        /// 出租方式
        /// 1、整租
        /// 2、合租
        /// </summary>
        public int lea { get; set; }
        /// <summary>
        /// 出租居室（lea为2时有效）
        /// </summary>
        public int lroom { get; set; }
        /// <summary>
        /// 关键字
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 排序
        /// 1、时间（默认）
        /// 2、价格倒序
        /// 3、价格顺序
        /// </summary>
        public int od { get; set; }
        /// <summary>
        /// 页码（默认1）
        /// </summary>
        public int page { get; set; }
        int count = 0;

        protected override string GetParmNames
        {
            get
            {
                return "ar-t1-t2-pr-room-lea-lroom-key-od-page";
            }
        }

        protected override void Validate()
        {
            base.Validate();
            if (ar <= 0 || ar > 2) ar = 1;
            if (od <= 0 || od > 3) od = 1;
            if (page <= 0) page = 1;
            dict["ar"] = ar;
        }

        protected override void InitDict()
        {
            base.InitDict();

            dict.Add("rlist", GetDictList("coop.lea_room", "0"));
            var pr_name = "";
            var r_name = "";

            var sels = new List<item>() { };

            var q = from c in DB.x_coop
                    where c.status == 1
                    select c;

            if (ar == 1)
            {
                if (t2 > 0) q = q.Where(o => o.businessarea == t2);
                else if (t1 > 0) q = q.Where(o => o.region == t1);

                if (t1 > 0 && t2 > 0) sels.Add(new item() { name = GetDictName("coop.qy", t1) + "-" + GetDictName("coop.qy", t2), url = "list-" + ar + "-" + t1 + "--" + pr + "-" + room + "-" + lea + "-" + lroom + "-" + key + "-" + od + ".html" });
                else if (t1 > 0) sels.Add(new item() { name = GetDictName("coop.qy", t1), url = "list-" + ar + "---" + pr + "-" + room + "-" + lea + "-" + lroom + "-" + key + "-" + od + ".html" });

            }
            else if (ar == 2)
            {
                if (t2 > 0) 
                {
                    q = q.Where(o => o.x_coop_dt.Select(d => d.name_id).Contains(t2));
                    //var dt = DB.x_dict.FirstOrDefault(o => o.code == "coop.dt" && o.value == t2 + "");
                    //if (dt != null && dt.pointx > 0 && dt.pointy > 0) q = q.Where(o => DB.fnGetDistance((float)o.longitude, (float)o.latitude, (float)dt.pointx, (float)dt.pointy) < 2000);
                    sels.Add(new item() { name = "地铁：" + GetDictName("coop.dt", t1) + "-" + GetDictName("coop.dt", t2), url = "list-" + ar + "-" + t1 + "--" + pr + "-" + room + "-" + lea + "-" + lroom + "-" + key + "-" + od + ".html" });
                }
                else if (t1 > 0)
                {
                    q = q.Where(o => o.x_coop_dt.Select(d => d.line_id).Contains(t1));
                    sels.Add(new item() { name = "地铁：" + GetDictName("coop.dt", t1), url = "list-" + ar + "---" + pr + "-" + room + "-" + lea + "-" + lroom + "-" + key + "-" + od + ".html" });
                }
            }


            //pr:价格
            if (!string.IsNullOrEmpty(pr))
            {
                var p = pr.Split(',');
                if (p.Length == 2)
                {
                    if (!string.IsNullOrEmpty(p[0]) && !string.IsNullOrEmpty(p[1])) q = q.Where(o => o.price >= decimal.Parse(p[0]) && o.price <= decimal.Parse(p[1]));
                    else if (string.IsNullOrEmpty(p[0])) q = q.Where(o => o.price <= decimal.Parse(p[1]));
                    else q = q.Where(o => o.price >= decimal.Parse(p[0]));
                }
                if (string.IsNullOrEmpty(p[0])) { pr_name = p[1] + "元以下"; sels.Add(new item() { name = pr_name, url = "list-" + ar + "-" + t1 + "-" + t2 + "--" + room + "-" + lea + "-" + lroom + "-" + key + "-" + od + ".html" }); }
                else if (string.IsNullOrEmpty(p[1])) { pr_name = p[1] + "元以上"; sels.Add(new item() { name = pr_name, url = "list-" + ar + "-" + t1 + "-" + t2 + "--" + room + "-" + lea + "-" + lroom + "-" + key + "-" + od + ".html" }); }
                else { pr_name = pr.Replace(",", "-") + "元"; sels.Add(new item() { name = pr_name, url = "list-" + ar + "-" + t1 + "-" + t2 + "--" + room + "-" + lea + "-" + lroom + "-" + key + "-" + od + ".html" }); }
            }

            //room:居室
            if (room > 0)
            {
                var n = "";
                if (room == 4) { q = q.Where(o => o.room > 3 && o.room < 10 && o.lea_way == 1); n = "4室及以上"; }
                else if (room == 99) { q = q.Where(o => o.room == room); n = "大开间"; }
                else if (room < 10) { q = q.Where(o => o.room == room); n = room + "室"; }
                r_name = n;
                sels.Add(new item() { name = n, url = "list-" + ar + "-" + t1 + "-" + t2 + "-" + pr + "--" + lea + "-" + lroom + "-" + key + "-" + od + ".html" });
            }

            //lea:租凭方式
            if (lea > 0)
            {
                q = q.Where(o => o.lea_way == lea);

                if (lroom > 0)
                {
                    q = q.Where(o => o.lea_room == lroom);
                    sels.Add(new item() { name = "合租-" + GetDictName("coop.lea_room", lroom), url = "list-" + ar + "-" + t1 + "-" + t2 + "-" + pr + "-" + room + "-" + lea + "--" + key + "-" + od + ".html" });
                }
                else
                {
                    sels.Add(new item() { name = lea == 1 ? "整租" : "合租", url = "list-" + ar + "-" + t1 + "-" + t2 + "-" + pr + "-" + room + "--" + lroom + "-" + key + "-" + od + ".html" });
                }
                //wayname = lea == 1 ? "整租" : "合租";
            }


            //key:关键字
            if (!string.IsNullOrEmpty(key))
            {
                var dt = DB.x_dict.FirstOrDefault(o => o.code == "coop.dt" && o.name == key && o.upval != "0");
                if (dt != null)
                {
                    q = q.Where(o =>
                        o.house.Contains(key) ||
                        (DB.x_dict.Where(d => d.code == "code.qy" && d.name == key).Select(s => s.value)).Contains(o.businessarea + "") || DB.fnGetDistance((float)o.longitude, (float)o.latitude, (float)dt.pointx, (float)dt.pointy) < 1.5 * 1000);
                }
                else
                {
                    q = q.Where(o =>
                            o.house.Contains(key) ||
                            (DB.x_dict.Where(d => d.code == "code.qy" && d.name == key).Select(s => s.value)).Contains(o.businessarea + ""));
                }
                sels.Add(new item() { name = key, url = "list-" + ar + "-" + t1 + "-" + t2 + "-" + pr + "-" + room + "-" + lea + "-" + lroom + "--" + od + ".html" });
            }

            if (ar == 1)
            {
                dict.Add("list", GetDictList("coop.qy", "0"));
                if (t1 > 0) dict.Add("slist", GetDictList("coop.qy", t1 + ""));
                dict.Add("title", (t2 > 0 ? GetDictName("coop.qy", t2) + "附近租房" : "") + (string.IsNullOrEmpty(pr) ? "" : " " + pr_name) + (room > 0 ? " " + r_name : "") + (lea > 0 ? " " + (lea == 1 ? "整租" : "合租") : "") + (t1 > 0 ? " " + GetDictName("coop.qy", t1) + "房屋出租" : ""));
                dict.Add("keys", (t1 > 0 ? GetDictName("coop.qy", t1) + "租房" : "") + (t2 > 0 ? " " + GetDictName("coop.qy", t2) + "附近租房" : "") + (string.IsNullOrEmpty(key) ? "" : " " + key + "房屋出租"));
                dict.Add("desc", "找房网提供100%真实" + (t1 > 0 ? GetDictName("coop.qy", t1) + "租房信息" : "") + (t2 > 0 ? " " + GetDictName("coop.qy", t2) + "附近" + ((lea > 0 ? " " + (lea == 1 ? "整租" : "合租") : "")) + "房源 ，每日实时更新" : ""));// + "" + " 区域关键词 +租房信息 + 商圈关键词 + 附近 +方式 + 房源 ，每日实时更新");
            }
            else
            {
                dict.Add("list", GetDictList("coop.dt", "0"));
                if (t1 > 0) dict.Add("slist", GetDictList("coop.dt", t1 + ""));
                dict.Add("title", (t1 > 0 ? GetDictName("coop.dt", t1) + "沿线租房" : "") + (t2 > 0 ? " " + GetDictName("coop.dt", t2) + "附近房屋出租" : ""));//+"地铁线路关键词+沿线租房 + 地铁站关键词 + 附近 + 房屋出租");
                dict.Add("keys", (t1 > 0 ? GetDictName("coop.dt", t1) + "沿线租房" : "") + (t2 > 0 ? " " + GetDictName("coop.dt", t2) + "附近房屋出租" : ""));
                dict.Add("desc", "找房网提供100%真实" + (t1 > 0 ? " " + GetDictName("coop.dt", t1) + "沿线租房" : "") + (t2 > 0 ? " " + GetDictName("coop.dt", t2) + "附近房屋出租" : "") + " 每日实时更新");//+"每日实时更新");
            }

            if (t1 == 0 && t2 == 0)
            {
                dict["title"] = "北京租房";
                dict["keys"] = "北京 租房 整租 合租 主卧 次卧";
                dict["desc"] = "北京租房信息";
            }

            dict.Add("sels", sels);

            q = q.Where(o => !new int?[] { 30, 31, 32 }.Contains(o.agent_id));

            if (od == 2) q = q.OrderByDescending(o => o.price);
            else if (od == 3) q = q.OrderBy(o => o.price);
            else q = q.OrderByDescending(o => o.up_time);

            var q1 = q.Skip((page - 1) * 30)
                .Take(30)
                .Select(o => new
                {
                    o.house,
                    cover = o.cover1,
                    o.door_no_name,
                    o.lea_way_name,
                    o.lea_room_name,
                    o.region_name,
                    o.businessarea_name,
                    o.room_name,
                    o.floor_name,
                    o.more,
                    o.area,
                    dt = o.x_coop_dt.OrderBy(d => d.dist).FirstOrDefault(), //"",// getdt((float?)o.longitude, (float?)o.latitude),
                    o.price,
                    ycount = DB.x_reserve.Count(c => c.coop_id == o.coop_id) + DB.x_visit.Count(v => v.coop_id == o.coop_id),
                    o.id,
                    o.fx_name
                });

            count = q.Count();
            dict.Add("clist", q1.ToList());

            dict["key"] = Context.Server.UrlEncode(Context.Server.UrlEncode(key));

            //if (page == 100)
            //{
            //    foreach (var cp in DB.x_coop.Where(o => o.longitude > 0 && o.latitude > 0).ToList())
            //    {
            //        var dts = DB.x_dict.Where(d => d.code == "coop.dt" && d.pointx != null && d.pointy != null && DB.fnGetDistance((float?)d.pointx, (float?)d.pointy, (float?)cp.longitude, (float?)cp.latitude) < 1.5 * 1000);
            //        foreach (var d in dts.ToList())
            //        {
            //            if (DB.x_coop_dt.Count(o => o.line_id == Convert.ToInt16(d.upval) && o.coop_id == cp.coop_id && o.name_id == Convert.ToInt16(d.value)) > 0) continue;
            //            var cdt = new x_coop_dt()
            //            {
            //                coop_id = cp.id,
            //                dist = (decimal?)DB.fnGetDistance((float?)d.pointx, (float?)d.pointy, (float?)cp.longitude, (float?)cp.latitude) / 1000,
            //                line = GetDictName("coop.dt", d.upval),
            //                line_id = int.Parse(d.upval),
            //                name = d.name,
            //                name_id = int.Parse(d.value)
            //            };
            //            DB.x_coop_dt.InsertOnSubmit(cdt);
            //            SubmitDBChanges();
            //        }
            //    }
            //}

        }
        //string getdt(float? lng, float? lat)
        //{
        //    if (lng == null || lat == null || lng == 0 || lat == 0) return "";

        //    var q = from d in DB.x_dict
        //            where d.code == "coop.dt" && d.upval != "0" && DB.fnGetDistance((float?)d.pointx, (float?)d.pointy, lng, lat) < 1.5 * 1000
        //            orderby DB.fnGetDistance((float?)d.pointx, (float?)d.pointy, lng, lat) ascending
        //            select new
        //            {
        //                dist = (decimal)DB.fnGetDistance((float?)d.pointx, (float?)d.pointy, lng, lat) / 1000,
        //                line_name = GetDictName("coop.dt", d.upval),
        //                st_name = d.name
        //            };

        //    var dt = q.FirstOrDefault();
        //    return dt == null ? "" : "距离 地铁" + dt.line_name + " " + dt.st_name + " " + dt.dist.ToString("F2") + "km<br/>";

        //    //var sb_dt = new StringBuilder();
        //    //foreach (var d in q.OrderBy(o => o.dist).Take(1).ToList()) sb_dt.Append("距离 地铁" + d.line_name + " " + d.st_name + " " + d.dist.ToString("F2") + "km<br/>");
        //    //return sb_dt.ToString();
        //}

        public string getpage()
        {
            var sb_page = new StringBuilder();
            if (page > 1) sb_page.Append("<a href='" + "list-" + ar + "-" + t1 + "-" + t2 + "-" + pr + "-" + room + "-" + lea + "-" + lroom + "-" + dict["key"] + "-" + od + "-" + (page - 1) + ".html" + "'>&lt;&lt;上一页</a>");
            var c = (int)Math.Ceiling((decimal)count / 30);
            var max = page + 3;
            var min = page - 3;
            if (min <= 1) { min = 1; max = 7; }
            if (max >= c) { max = c; if (c >= 7) min = c - 6; else min = 1; }

            for (var i = min; i <= max; i++)
            {
                sb_page.Append("<a href='" + "list-" + ar + "-" + t1 + "-" + t2 + "-" + pr + "-" + room + "-" + lea + "-" + lroom + "-" + dict["key"] + "-" + od + "-" + i + ".html" + "' class='" + (page == i ? "on" : "") + "'>" + i + "</a>");
            }

            if (page < max) sb_page.Append("<a href='" + "list-" + ar + "-" + t1 + "-" + t2 + "-" + pr + "-" + room + "-" + lea + "-" + lroom + "-" + dict["key"] + "-" + od + "-" + (page + 1) + ".html" + "'>下一页&gt;&gt;</a>");
            return sb_page.ToString();
        }

        public class item
        {
            public int at { get; set; }
            public string name { get; set; }
            public string url { get; set; }
        }
    }
}
