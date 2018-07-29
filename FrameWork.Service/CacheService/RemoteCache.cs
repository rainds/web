using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;
using FrameWork.Core;
using FrameWork.Core.Cache;
using FrameWork.Core.Config;

namespace FrameWork.CacheService
{
    public class RemoteCache : ICache
    {
        private static MemcachedClient memClient;
        private static readonly object ObjLock = new object();

        private const string ConfigKey = "FrameWork.Service.MemCacheServer";

        public RemoteCache()
        {
            if (memClient != null) return;

            lock (ObjLock)
            {
                if (memClient != null) return;
                try
                {
                    // 配置-协议
                    var memConfig = new MemcachedClientConfiguration { Protocol = MemcachedProtocol.Binary };

                    var serverConfig = ConfigurationManager.AppSettings[ConfigKey];
                    if (!string.IsNullOrWhiteSpace(serverConfig)) //尝试读取配置文件
                    {
                        IPAddress serverIp;
                        if (IPAddress.TryParse(serverConfig, out serverIp))
                        {
                            var ipEndPoint = new IPEndPoint(serverIp, 11211);
                            memConfig.Servers.Add(ipEndPoint);
                        }
                        else
                        {
                            var ipAddress = Dns.GetHostEntry(serverConfig)
                                .AddressList.First(e => e.AddressFamily == AddressFamily.InterNetwork);
                            var ipEndPoint = new IPEndPoint(ipAddress, 11211);
                            memConfig.Servers.Add(ipEndPoint);

                            memConfig.Authentication.Type = typeof(PlainTextAuthenticator);
                            memConfig.Authentication.Parameters["zone"] = "";
                            memConfig.Authentication.Parameters["userName"] =
                                ConfigurationManager.AppSettings["FrameWork.CacheService.MemCacheUser"];
                            memConfig.Authentication.Parameters["password"] =
                                ConfigurationManager.AppSettings["FrameWork.CacheService.MemCachePwd"];
                        }
                    }
                    else //尝试读取数据库配置
                    {
                        var info = Locator.Get<IConfig>().Get<MemcachedConfig>();

                        // 配置-ip
                        var ipAddress = Dns.GetHostEntry(info.Server)
                            .AddressList.First(e => e.AddressFamily == AddressFamily.InterNetwork);
                        var ipEndPoint = new IPEndPoint(ipAddress, 11211);
                        memConfig.Servers.Add(ipEndPoint);

                        // 配置-权限
                        memConfig.Authentication.Type = typeof(PlainTextAuthenticator);
                        memConfig.Authentication.Parameters["zone"] = "";
                        memConfig.Authentication.Parameters["userName"] = info.UserName;
                        memConfig.Authentication.Parameters["password"] = info.Password;

                        //配置-连接数
                        memConfig.SocketPool.MinPoolSize = info.MinPoolSize;
                        memConfig.SocketPool.MaxPoolSize = info.MaxPoolSize;
                    }

                    memClient = new MemcachedClient(memConfig);
                }
                catch (Exception ex)
                {
                    FrameWorkLogger.Log(ex);
                    throw;
                }
            }
        }

        private static string GetCacheKey(string key, string regionName)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("key is null");

            if (string.IsNullOrWhiteSpace(regionName))
            {
                return key;
            }

            var version = memClient.Get<int>(regionName);
            return regionName + version + "." + key;
        }

        public object Get(string key, string regionName = null)
        {
            return memClient.Get(GetCacheKey(key, regionName));
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
            return value != null &&
                   memClient.Store(StoreMode.Set, GetCacheKey(key, regionName), value, TimeSpan.FromMinutes(minute));
        }

        public bool Remove(string key, string regionName = null)
        {
            return memClient.Remove(GetCacheKey(key, regionName));
        }

        public void ClearRegion(string regionName)
        {
            if (string.IsNullOrWhiteSpace(regionName))
                throw new ArgumentException("regionName is null");

            var version = Convert.ToInt32(memClient.Get(regionName)) + 1;

            memClient.Store(StoreMode.Set, regionName, version, TimeSpan.FromDays(7));
        }
    }
}