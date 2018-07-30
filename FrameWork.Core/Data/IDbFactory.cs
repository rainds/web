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
        IDbProvider GetDbProvider<TContext>() where TContext : BaseDbContext, new();
    }
}