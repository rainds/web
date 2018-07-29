using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FrameWork.Core.Data
{
    /// <summary>
    /// 实体操作者
    /// </summary>
    public interface IRepository<TEntity> where TEntity : class, new()
    {
        /// <summary>
        /// 插入实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>操作影响的行数</returns>
        int Insert(TEntity entity);

        int Insert(IList<TEntity> entities);

        int Delete(IList<TEntity> entities);

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>操作影响的行数</returns>
        int Delete(TEntity entity);

        /// <summary>
        /// 删除所有符合特定条件的实体
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns>操作影响的行数</returns>
        int Delete(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 更新实体对象
        /// </summary>
        /// <param name="entity">要更新的实体</param>
        /// <returns>操作影响的行数</returns>
        int Update(TEntity entity);

        int Update(Expression<Func<TEntity, TEntity>> updatePredicate);

        /// <summary>
        /// 批量局部更新所有符合特定条件的实体
        /// </summary>
        /// <param name="wherePredicate">查询条件</param>
        /// <param name="updatePredicate">更新内容</param>
        /// <returns>操作影响的行数</returns>
        int Update(Expression<Func<TEntity, bool>> wherePredicate, Expression<Func<TEntity, TEntity>> updatePredicate);

        /// <summary>
        /// 查找指定主键的实体
        /// </summary>
        /// <param name="keyValue">实体主键</param>
        /// <returns></returns>
        TEntity Find(object keyValue);

        /// <summary>
        /// 查找指定主键的实体
        /// </summary>
        /// <param name="predicate">查询条件谓语表达式</param>
        /// <returns></returns>
        TEntity Find(Expression<Func<TEntity, bool>> predicate);

        ///<summary>
        /// 获取数据查询对象
        ///</summary>
        IQueryable<TEntity> Query { get; }
    }
}