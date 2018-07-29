using System.Collections.Generic;

namespace FrameWork.Core.Mvc.Paged
{
    /// <summary>
    /// 表示一个可以使用索引访问的集合的子集，该子集包含集合的元数据，如页码、记录数等等
    /// </summary>
    /// <remarks>
    /// 返回一个可以使用索引访问的集合的子集
    /// </remarks>
    /// <typeparam name="T">集合中需要包含的类型</typeparam>
    /// <seealso cref="IEnumerable{T}"/>
    public interface IPagedList<out T> : IPagedList, IEnumerable<T>
    {
        /// <summary>
        /// 获取当前页包含的记录条数
        /// </summary>
        int CurrentCount { get; }

        /// <summary>
        /// 获取指定索引的元素
        /// </summary>
        /// <param name="index">需要获取的元素的索引，从0开始计算</param>
        /// <returns>返回指定索引的元素</returns>
        T this[int index] { get; }

        /// <summary>
        /// 获取分页列表的非enumerable拷贝
        /// </summary>
        /// <returns>返回分页列表的非enumerable拷贝</returns>
        IPagedList GetMetaData();
    }

    /// <summary>
    /// 表示一个可以使用索引访问的集合的子集，该子集包含集合的元数据，如页码、记录数等等
    /// </summary>
    /// <remarks>
    /// 返回一个可以使用索引访问的集合的子集
    /// </remarks>
    public interface IPagedList
    {
        /// <summary>
        /// 分页总数
        /// </summary>
        /// <value>
        /// 返回分页总数
        /// </value>
        int PageCount { get; }

        /// <summary>
        /// 记录总条数
        /// </summary>
        /// <value>
        /// 返回记录总条数
        /// </value>
        int TotalCount { get; }

        /// <summary>
        /// 当前页码，从1开始计算
        /// </summary>
        /// <value>
        /// 返回当前页码
        /// </value>
        int PageIndex { get; }

        /// <summary>
        /// 每页显示的行数
        /// </summary>
        /// <value>
        /// 返回每页显示的行数
        /// </value>
        int PageSize { get; }
    }
}