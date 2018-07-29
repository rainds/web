using FrameWork.Core;
using FrameWork.Core.Cache;

namespace Rainds.Service.User
{
    public class LoginHelper
    {
        /// <summary>
        /// 远程共享缓存
        /// </summary>
        private static ICache RemoteCache
        {
            get { return Locator.Get<ICache>(CacheLocation.Remote); }
        }

        private static string GetKey(string name)
        {
            return "Rainds.User.LoginTimes." + name;
        }

        public static bool CheckTimes(string name)
        {
            return (RemoteCache.Get<int>(GetKey(name)) < 5);
        }

        public static void SetTimes(string name)
        {
            var key = GetKey(name);
            var times = RemoteCache.Get<int>(key) + 1;
            RemoteCache.Set(key, times, 1d);
        }

        public static void RemoveTimes(string name)
        {
            RemoteCache.Remove(GetKey(name));
        }
    }
}