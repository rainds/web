using FrameWork.Core.Cache;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FrameWork.CacheService
{
    /// <summary>
    /// 上下文缓存
    /// </summary>
    public class ContextCache : ICache
    {
        private readonly IDictionary<string, object> objects;

        public ContextCache()
        {
            this.objects = new Dictionary<string, object>();
        }

        public object Get(string key, string regionName = null)
        {
            var fullkey = regionName + "_" + key;
            object obj;
            this.objects.TryGetValue(fullkey, out obj);

            return obj;
        }

        public T Get<T>(string key, string regionName = null)
        {
            var obj = this.Get(key, regionName);
            return (obj == null) ? default(T) : (T)obj;
        }

        public bool Set(string key, object value, double minute = 10d, string regionName = null)
        {
            this.objects[regionName + "_" + key] = value;
            return true;
        }

        public bool Remove(string key, string regionName = null)
        {
            return this.objects.Remove(regionName + "_" + key);
        }

        public void ClearRegion(string regionName)
        {
            if (string.IsNullOrWhiteSpace(regionName)) return;

            foreach (var key in this.objects.Keys.ToArray()
                .Where(key => key.StartsWith(regionName)))
            {
                this.objects.Remove(key);
            }
        }

        public T Get<T>(string key, Func<T> func, double minute = 10d, string regionName = null)
        {
            var value = this.Get(key, regionName);
            if (value == null)
            {
                value = func();
                Set(key, value, minute, regionName);
            }

            return (T)value;
        }
    }
}