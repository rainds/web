using System.Collections;
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
    /// <seealso cref = "IPagedList{T}" />
    /// <seealso cref = "List{T}" />
    internal abstract class BasePagedList<T> : PagedListMetaData, IPagedList<T>
    {
        /// <summary>
        /// 当前页的记录列表
        /// </summary>
        protected readonly List<T> Rows = new List<T>();

        /// <summary>
        /// 无参构造函数
        /// </summary>
        protected internal BasePagedList()
        {
        }

        /// <summary>
        /// 获取当前页包含的记录条数
        /// </summary>
        public int CurrentCount
        {
            get { return this.Rows.Count; }
        }

        /// <summary>
        /// 获取指定索引的元素
        /// </summary>
        /// <param name="index">需要获取的元素的索引，从0开始计算</param>
        /// <returns>返回指定的元素</returns>
        public T this[int index]
        {
            get { return this.Rows[index]; }
        }

        #region IPagedList<T> Members

        /// <summary>
        /// 返回一个可对BasePagedList&lt;T&gt;进行迭代的enumerator
        /// </summary>
        /// <returns>一个可对BasePagedList&lt;T&gt;进行迭代的enumerator</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this.Rows.GetEnumerator();
        }

        /// <summary>
        /// 返回一个可对BasePagedList&lt;T&gt;进行迭代的enumerator
        /// </summary>
        /// <returns>一个可对BasePagedList&lt;T&gt;进行迭代的enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// 获取分页列表的非enumerable拷贝
        /// </summary>
        /// <returns>返回分页列表的非enumerable拷贝</returns>
        public IPagedList GetMetaData()
        {
            return new PagedListMetaData(this);
        }

        #endregion IPagedList<T> Members
    }
}