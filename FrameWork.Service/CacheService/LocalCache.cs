using System;
using System.Linq;
using System.Runtime.Caching;
using FrameWork.Core.Cache;

namespace FrameWork.CacheService
{
    public class LocalCache : ICache
    {
        private readonly ObjectCache memoryCache;

        public LocalCache()
        {
            this.memoryCache = MemoryCache.Default;
        }

        public object Get(string key, string regionName = null)
        {
            return this.memoryCache.Get(regionName + "." + key);
        }

        public T Get<T>(string key, string regionName = null)
        {
            var obj = this.Get(key, regionName);
            return (obj == null) ? default(T) : (T)obj;
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

        public bool Set(string key, object value, double minute = 10d, string regionName = null)
        {
            if (minute <= 0) throw new ArgumentException("缓存过期时间必须大于0");
            if (value == null) return false;
            this.memoryCache.Set(regionName + "." + key, value, DateTimeOffset.Now.AddMinutes(minute));
            return true;
        }

        public bool Remove(string key, string regionName = null)
        {
            this.memoryCache.Remove(regionName + "." + key);
            return true;
        }

        public void ClearRegion(string regionName)
        {
            if (string.IsNullOrWhiteSpace(regionName)) return;

            foreach (var item in this.memoryCache.Where(item => item.Key.StartsWith(regionName)))
            {
                this.memoryCache.Remove(item.Key);
            }
        }
    }
}