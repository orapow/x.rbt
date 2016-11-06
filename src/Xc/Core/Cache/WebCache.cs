using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using X.Core.Plugin;

namespace X.Core.Cache
{
    public class WebCache : BaseCache
    {
        private System.Web.Caching.Cache Instance = null;

        /// <summary>
        /// Initializes a new instance of the MemCache class.
        /// </summary>
        public WebCache()
        {
            if (HttpContext.Current != null) Instance = HttpContext.Current.Cache;
        }

        public override void Save(string key, object value, int seconds)
        {
            if (value == null) return;
            if (Instance == null) return;
            if (seconds > 0) Instance.Insert(key, value, null, DateTime.Now.AddSeconds(seconds), System.Web.Caching.Cache.NoSlidingExpiration);
            else Instance.Insert(key, value);
            //Loger.Info("cache:" + key + "->" + Get<object>(key));
        }

        public override T Get<T>(string key)
        {
            if (Instance == null) return default(T);
            return (T)(Instance.Get(key) ?? default(T));
        }

        public override void Remove(string key)
        {
            if (Instance != null && !string.IsNullOrEmpty(key)) Instance.Remove(key);
        }

        public override void Clear() { }

    }
}
