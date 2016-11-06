using System;
using System.Collections.Generic;
using System.Linq;

namespace X.Core.Cache
{
    public abstract class BaseCache
    {
        public abstract void Save(string key, object value, int seconds);

        public abstract T Get<T>(string key);

        public abstract void Remove(string key);

        public abstract void Clear();
    }
}
