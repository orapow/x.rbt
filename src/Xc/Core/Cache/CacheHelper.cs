using System;
using System.Collections.Generic;
using System.Linq;
using X.Core.Plugin;

namespace X.Core.Cache
{
    public class CacheHelper
    {
        private static int ctype = 1;
        private static BaseCache cache = null;

        /// <summary>
        /// 缓存类型
        /// </summary>
        /// <param name="type">
        /// 1、Memcached
        /// 2、WebCache
        /// </param>
        public static void Init(int type)
        {
            ctype = type;
            switch (type)
            {
                case 1:
                default:
                    cache = new MemCache();
                    break;
                case 2:
                    cache = new WebCache();
                    break;
            }
        }

        public static void Save(string key, object value)
        {
            Save(key, value, 0);
        }

        public static void Save(string key, object value, int sec)
        {
            cache.Save(key, value, sec);
        }

        public static T Get<T>(string key)
        {
            return cache.Get<T>(key);
        }

        public static void Remove(string key)
        {
            cache.Remove(key);
        }

        public static void Clear()
        {
            cache.Clear();
        }
    }
}
