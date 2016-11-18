using System;
using System.Collections.Generic;
using System.Linq;
using X.Core.Cache;

namespace X.Data
{
    [Serializable]
    partial class x_group
    {
    }
    partial class x_dict
    {
        public long id { get { return dict_id; } }

        /// <summary>
        /// 获取字典文字
        /// </summary>
        /// <param name="code"></param>
        /// <param name="value">
        /// 多个值用 , 隔开
        /// </param>
        /// <returns></returns>
        public static string GetDictName(string code, object value, string split, RbtDBDataContext db)
        {
            if (value == null || string.IsNullOrEmpty(code)) return string.Empty;
            var list = GetDictList(code, "00", db);
            if (list == null || list.Count == 0) return string.Empty;

            var val = (value + "").Trim().Split(',');
            var dicts = list.FindAll(o =>
            {
                return val.Contains(o.value); //val.IndexOf(String.Format(",{0},", o.value)) >= 0;
            });
            if (dicts == null || dicts.Count == 0) return string.Empty;
            var ns = string.Empty;
            foreach (var d in dicts)
            {
                if (!string.IsNullOrEmpty(ns))
                {
                    ns += split;
                }
                ns += d.name;
            }
            return ns;
        }

        /// <summary>
        /// 获取字典列表
        /// </summary>
        /// <param name="code"></param>
        /// <param name="upval"></param>
        /// <returns></returns>
        public static List<x_dict> GetDictList(string code, string upval, RbtDBDataContext db)
        {
            var key = "dict." + code;
            var list = CacheHelper.Get<List<x_dict>>(key);
            if (list == null || list.Count == 0)
            {
                var q = from c in db.x_dict
                        where c.code == code
                        select c;
                list = q.ToList();
                if (list == null || list.Count == 0) return null;
                CacheHelper.Save(key, list);
            }
            if (upval == "00") return list;
            if (string.IsNullOrEmpty(upval) || upval == "0") return list.FindAll(o => { return o.upval == "0"; });
            else
            {
                var u = list.FirstOrDefault(o => o.value == upval.Split('-').Last());
                return list.FindAll(o => { return o.upval == (u.upval == "0" ? u.value : u.upval + "-" + u.value); });
            }
        }

    }

    public class com
    {
        public static string Get_Time(string t)
        {
            if (string.IsNullOrEmpty(t)) return "";
            DateTime st;
            var b = DateTime.TryParse(t, out st);
            if (!b) return "";
            var dt = DateTime.Now;
            var sp = dt - st;
            if (sp.TotalDays > 30) return "大于30天";
            else if (sp.TotalDays > 2) return (int)sp.Days + "天前";
            else if (sp.TotalDays == 2) return "前天";
            else if (sp.TotalDays == 1) return "昨天";
            else if (sp.TotalHours > 3) return "今天";
            else if (sp.TotalHours > 0) return (int)sp.Hours + "小时前";
            else if (sp.TotalMinutes > 0) return (int)sp.Minutes + "分钟前";
            else return "刚刚";
        }
    }
}