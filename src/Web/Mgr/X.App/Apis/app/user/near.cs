using System;
using System.Collections.Generic;
using System.Linq;
using X.Core.Plugin;
using X.Core.Utility;
using X.Data;
using X.Web.Com;

namespace X.App.Apis.app.user
{
    public class near : xu
    {
        public string key { get; set; }

        protected override XResp Execute()
        {
            var r = new nears();

            if (!string.IsNullOrEmpty(key)) loadsearch(r);
            else loadnear(r);

            if (us == null || us.level != 10) r.agentes = null;

            Loger.Info("near->" + Serialize.ToJson(r));

            return r;
        }

        void loadsearch(nears r)
        {
            r.houses = DB.x_houses
                .Where(o => o.name.Contains(key) || o.jianpin.Contains(key.ToLower()))
                .Select(h => new item(1) { name = h.name, value = h.id })
                .ToList();
            r.regions = DB.x_dict
                .Where(o => (o.name.Contains(key) || o.jp.Contains(key.ToLower())) && o.code == "coop.qy")
                .Select(h => new item(2) { name = h.name, value = int.Parse(h.value) })
                .ToList();
            r.stations = DB.x_dict
                .Where(o => (o.name.Contains(key) || o.jp.Contains(key.ToLower())) && o.code == "coop.dt")
                .AsEnumerable()
                .Distinct(new dict_comp())
                .Select(h => new item(3) { name = h.name, value = int.Parse(h.value) })
                .ToList();
            r.agentes = DB.x_agent
                .Where(o => o.name.Contains(key))
                .Select(h => new item(4) { name = h.name, value = h.id })
                .ToList();
        }

        void loadnear(nears r)
        {
            if (lng == 0 || lat == 0)
            {
                r.houses = DB.x_houses
                    .Where(o => (DB.x_coop
                        .GroupBy(g => g.house)
                        .OrderByDescending(g => g.Count())
                        .Take(4)
                        .Select(g => g.Key)
                        .Contains(o.name)))
                    .Select(h => new item(1) { name = h.name, value = h.id })
                    .ToList();
                r.regions = DB.x_dict
                    .Where(o => (DB.x_coop
                        .GroupBy(g => g.businessarea + "")
                        .OrderByDescending(g => g.Count())
                        .Take(4)
                        .Select(g => g.Key)
                        .Contains(o.value)) && o.code == "coop.qy")
                    .Select(h => new item(2) { name = h.name, value = int.Parse(h.value) })
                    .Take(4)
                    .ToList();
                r.stations = DB.x_dict
                    .Where(o => (DB.x_coop
                        .GroupBy(g => g.subwaystation + "")
                        .OrderByDescending(g => g.Count())
                        .Take(4)
                        .Select(g => g.Key)
                        .Contains(o.value)) && o.code == "coop.dt")
                    .AsEnumerable()
                    .Distinct(new dict_comp())
                    .Select(h => new item(3) { name = h.name, value = int.Parse(h.value) })
                    .Take(4)
                    .ToList();
                r.agentes = DB.x_agent
                    .Where(o => (DB.x_coop
                        .GroupBy(g => g.agent_id)
                        .OrderByDescending(g => g.Count())
                        .Take(4)
                        .Select(g => g.Key.Value)
                        .Contains(o.agent_id)))
                    .Select(h => new item(4) { name = h.name, value = h.id })
                    .ToList();
            }
            else
            {
                r.houses = DB.x_houses
                    .Select(h => new item(1) { name = h.name, value = h.id, dist = (h.longitude == null || h.latitude == null) ? null : DB.fnGetDistance((float)lng, (float)lat, (float)h.longitude, (float)h.latitude) })
                    .Where(o => o.dist != null)
                    .OrderBy(o => o.dist)
                    .Take(4)
                    .ToList();
                r.regions = DB.x_dict
                    .Where(o => o.code == "coop.qy" && o.upval != "0")
                    .Select(h => new item(2)
                    {
                        name = h.name,
                        value = int.Parse(h.value),
                        dist = (
                            DB.x_coop
                            .Where(o => o.businessarea == Convert.ToInt16(h.value) && o.longitude > 0 && o.latitude > 0)
                            .Select(s => DB.fnGetDistance((float)lng, (float)lat, (float)s.longitude, (float)s.latitude))
                            .FirstOrDefault()
                            )
                    })
                    .Where(o => o.dist != null)
                    .OrderBy(o => o.dist)
                    .Take(4)
                    .ToList();
                r.stations = DB.x_dict
                    .Where(o => o.code == "coop.dt" && o.upval != "0")
                    .AsEnumerable()
                    .Distinct(new dict_comp())
                    .Select(h => new item(3) { name = h.name, value = int.Parse(h.value), dist = (h.pointx == null || h.pointy == null) ? null : DB.fnGetDistance((float)lng, (float)lat, (float)h.pointx, (float)h.pointy) })
                    .Where(o => o.dist != null)
                    .OrderBy(o => o.dist)
                    .Take(4)
                    .ToList();
                r.agentes = DB.x_agent
                    .Select(h => new item(4) { name = h.name, value = h.id, dist = (h.pointx == null || h.pointy == null) ? null : DB.fnGetDistance((float)lng, (float)lat, (float)h.pointx, (float)h.pointy) })
                     .Where(o => o.dist != null)
                    .OrderBy(o => o.dist)
                    .Take(4)
                    .ToList();
            }
        }

        public class dict_comp : IEqualityComparer<x_dict>
        {

            #region IEqualityComparer<DataRow> 成员

            public bool Equals(x_dict x, x_dict y)
            {
                return x.name.CompareTo(y.name) == 0; //这个是根据自己的需求写比较字段的
            }

            public int GetHashCode(x_dict obj)
            {
                return obj.ToString().GetHashCode();
            }

            #endregion
        }

        public class nears : XResp
        {
            public List<item> houses { get; set; }
            public List<item> agentes { get; set; }
            public List<item> stations { get; set; }
            public List<item> regions { get; set; }
        }

        public class item
        {
            /// <summary>
            /// 类型
            /// 1、楼盘
            /// 2、商圈
            /// 3、地铁
            /// 4、房东
            /// </summary>
            public int tp { get; set; }
            public int value { get; set; }
            public double? dist { get; set; }
            public string name { get; set; }
            public item(int t) { this.tp = t; }
        }
    }
}
