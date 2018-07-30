using System;

namespace FrameWork.Core.Data
{
    /// <summary>
    /// 数据提供者
    /// </summary>
    public interface IDbProvider : IDisposable
    {
        /// <summary>
        /// 获得实体操作者
        /// </summary>
        IRepository<TEntity> Repository<TEntity>() where TEntity : class, IEntity, new();

        /// <summary>
        /// 事务开启(同一个数据提供者共用一个事务)
        /// </summary>
        void Begin();

        /// <summary>
        /// 事务提交(同一个数据提供者共用一个事务)
        /// </summary>
        void Commit();

        /// <summary>
        /// 事务回滚(同一个数据提供者共用一个事务)
        /// </summary>
        void Rollback();
    }
}