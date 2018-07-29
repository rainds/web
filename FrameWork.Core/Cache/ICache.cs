using System;

namespace FrameWork.Core.Cache
{
    /// <summary>
    /// 缓存提供者
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// 获取一个对象
        /// </summary>
        object Get(string key, string regionName = null);

        /// <summary>
        /// 获取一个对象
        /// </summary>
        T Get<T>(string key, string regionName = null);

        /// <summary>
        /// 获取一个对象,如果对象不存在，则执行func，并且缓存结果和返回
        /// </summary>
        T Get<T>(string key,Func<T> func, double minute = 10d, string regionName = null);

        /// <summary>
        /// 添加一个对象到缓存中
        /// </summary>
        /// <param name="key">缓存键值</param>
        /// <param name="value">缓存对象</param>
        /// <param name="minute">缓存过期时间(单位:分钟),默认缓存10分钟</param>
        /// <param name="regionName">分区名称</param>
        /// <returns>返回操作是否成功</returns>
        bool Set(string key, object value, double minute = 10d, string regionName = null);

        /// <summary>
        /// 从缓存中移除一个对象
        /// </summary>
        bool Remove(string key, string regionName = null);

        /// <summary>
        /// 清除指定分区的全部缓存项
        /// </summary>
        void ClearRegion(string regionName);
    }
}