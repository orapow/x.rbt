using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using X.Core.Cache;

namespace X.Web.Cache
{
    public class SessionCache : BaseCache
    {
        private System.Web.SessionState.HttpSessionState Instance = null;

        /// <summary>
        /// Initializes a new instance of the MemCache class.
        /// </summary>
        public SessionCache()
        {
            if (HttpContext.Current != null) Instance = HttpContext.Current.Session;
        }

        public override void Save(string key, object value, int sec)
        {
            if (value == null || Instance == null) return;
            Instance[key] = value;
        }

        public override T Get<T>(string key)
        {
            if (Instance == null) return default(T);
            return (T)Instance[key];
        }

        public override void Remove(string key)
        {
            if (Instance != null) Instance.Remove(key);
        }

        public override void Clear()
        {
            if (Instance != null) Instance.RemoveAll();
        }

        public void Abandon()
        {
            if (Instance != null) Instance.Abandon();
        }
    }
}
