using System;
using System.Collections.Generic;
using System.Linq;

namespace FrameWork.Core.Mvc.Paged
{
    /// <summary>
    /// 表示一个可以使用索引访问的集合的子集，该子集包含集合的元数据，如页码、记录数等等
    /// </summary>
    /// <remarks>
    /// 返回一个可以使用索引访问的集合的子集
    /// </remarks>
    /// <typeparam name="T">集合中需要包含的类型</typeparam>
    /// <seealso cref="IPagedList{T}"/>
    /// <seealso cref="BasePagedList{T}"/>
    /// <seealso cref="StaticPagedList{T}"/>
    /// <seealso cref="List{T}"/>
    internal class PagedList<T> : BasePagedList<T>
    {
        /// <summary>
        /// 初始化 PagedList 类的新实例
        /// </summary>
        /// <param name = "superset">IQueryable&lt;T&gt;结果集</param>
        /// <param name = "pageIndex">开始的页数，从1开始计算</param>
        /// <param name = "pageSize">每页显示的条数</param>
        /// <exception cref="ArgumentOutOfRangeException">索引不能小于0</exception>
        /// <exception cref="ArgumentOutOfRangeException">每页记录条数不能小于1</exception>
        public PagedList(IQueryable<T> superset, int pageIndex, int pageSize)
        {
            if (pageIndex < 1)
            {
                throw new ArgumentOutOfRangeException(
                    "pageIndex",
                    pageIndex,
                    "分页页码不能小于1");
            }

            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException(
                    "pageSize",
                    pageSize,
                    "分页条数不能小于1");
            }
            // set source to blank list if superset is null to prevent exceptions
            this.TotalCount = superset == null ? 0 : superset.Count();
            this.PageSize = pageSize;
            this.PageCount = this.TotalCount > 0
                        ? (int)Math.Ceiling(this.TotalCount / (double)this.PageSize)
                        : 0;
            pageIndex = pageIndex > this.PageCount ? this.PageCount : pageIndex;
            this.PageIndex = pageIndex;

            // add items to internal list
            if (superset != null && this.TotalCount > 0)
            {
                this.Rows.AddRange(pageIndex == 1
                    ? superset.Skip(0).Take(pageSize).ToList()
                    : superset.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList());
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedList{T}"/> class that divides the supplied superset into subsets the size of the supplied pageSize. The instance then only containes the objects contained in the subset specified by index.
        /// </summary>
        /// <param name = "superset">IEnumerable&lt;T&gt;>结果集</param>
        /// <param name = "pageNumber">开始的页数，从1开始计算</param>
        /// <param name = "pageSize">每页显示的条数</param>
        /// <exception cref="ArgumentOutOfRangeException">索引不能小于0</exception>
        /// <exception cref="ArgumentOutOfRangeException">每页记录条数不能小于1</exception>
        public PagedList(IEnumerable<T> superset, int pageNumber, int pageSize)
            : this(superset.AsQueryable(), pageNumber, pageSize)
        {
        }
    }
}