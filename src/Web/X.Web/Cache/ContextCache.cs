using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using X.Core.Cache;

namespace X.Web.Cache
{
    public class ContextCache : BaseCache
    {
        private HttpContext Instance = null;

        /// <summary>
        /// 
        /// </summary>
        public ContextCache()
        {
            if (HttpContext.Current != null) Instance = HttpContext.Current;
        }

        public override void Save(string key, object value, int sec)
        {
            if (value == null) return;
            if (Instance == null) return;
            Instance.Items[key] = value;
        }

        public override T Get<T>(string key)
        {
            if (Instance == null) return default(T);
            return (T)Instance.Items[key];
        }

        public override void Remove(string key)
        {
            if (Instance != null) Instance.Items.Remove(key);
        }

        public override void Clear()
        {
        }
    }
}
