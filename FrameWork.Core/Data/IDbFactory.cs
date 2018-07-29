using System.Reflection;

namespace FrameWork.Core.Data
{
    /// <summary>
    /// 数据提供者工厂
    /// </summary>
    public interface IDbFactory
    {
        /// <summary>
        /// 获得数据提供者
        /// </summary>
        /// <param name="dbkey"></param>
        /// <returns></returns>
        IDbProvider GetDbProvider(string dbkey);

        /// <summary>
        /// 注册程序集中所有实体
        /// </summary>
        void RegisterEntities(Assembly assembly);
    }
}