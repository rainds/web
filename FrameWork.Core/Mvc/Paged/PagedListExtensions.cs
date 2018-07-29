using System.Collections.Generic;
using System.Linq;

namespace FrameWork.Core.Mvc.Paged
{
    /// <summary>
    /// 简化 <see cref="PagedList{T}"/> 类实例化的扩展方法的容器
    /// </summary>
    public static class PagedListExtensions
    {
        /// <summary>
        /// 为 <see cref="IEnumerable{T}"/> 结果集创建一个分页列表
        /// </summary>
        /// <typeparam name="T">集合中需要包含的类型</typeparam>
        /// <param name = "superset"><see cref="IEnumerable{T}"/>结果集</param>
        /// <param name = "pageNumber">开始的页数，从1开始计算</param>
        /// <param name = "pageSize">每页上所显示的记录的数目</param>
        /// <returns>返回一个分页列表</returns>
        public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> superset, int pageNumber = 1, int pageSize = 10)
        {
            return new PagedList<T>(superset, pageNumber, pageSize);
        }

        /// <summary>
        /// 为 <see cref="IQueryable{T}"/> 结果集创建一个分页列表
        /// </summary>
        /// <typeparam name="T">集合中需要包含的类型</typeparam>
        /// <param name = "superset"><see cref="IQueryable{T}"/>结果集</param>
        /// <param name = "pageNumber">开始的页数，从1开始计算</param>
        /// <param name = "pageSize">每页上所显示的记录的数目</param>
        /// <returns>返回一个分页列表</returns>
        public static IPagedList<T> ToPagedList<T>(this IQueryable<T> superset, int pageNumber = 1, int pageSize = 10)
        {
            return new PagedList<T>(superset, pageNumber, pageSize);
        }
    }
}