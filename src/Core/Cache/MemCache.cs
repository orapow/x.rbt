using Enyim.Caching;
using Enyim.Caching.Memcached;
using System;
using System.Collections.Generic;
using System.Linq;

namespace X.Core.Cache
{
    public class MemCache : BaseCache
    {
        private readonly static MemcachedClient Instance = new MemcachedClient("enyim.com/memcached");

        public override void Save(string key, object value, int seconds)
        {
            if (seconds > 0)
            {
                var date = DateTime.Now;
                date.AddSeconds(seconds);
                Instance.Store(StoreMode.Set, key, value, date);
            }
            else
            {
                Instance.Store(Enyim.Caching.Memcached.StoreMode.Set, key, value);
            }
        }

        public override T Get<T>(string key)
        {
            return Instance.Get<T>(key);
        }

        public override void Remove(string key)
        {
            Instance.Remove(key);
        }

        public override void Clear()
        {
            Instance.FlushAll();
        }

    }
}
