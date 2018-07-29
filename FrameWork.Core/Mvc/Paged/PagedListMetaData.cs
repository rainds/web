using System;

namespace FrameWork.Core.Mvc.Paged
{
    /// <summary>
    /// PagedList类的非enumerable版本
    /// </summary>
    internal class PagedListMetaData : IPagedList
    {
        /// <summary>
        /// PagedList类的非enumerable版本
        /// </summary>
        /// <param name="pagedList">分页列表</param>
        public PagedListMetaData(IPagedList pagedList)
        {
            if (pagedList == null)
            {
                throw new ArgumentNullException("pagedList");
            }

            this.PageCount = pagedList.PageCount;
            this.TotalCount = pagedList.TotalCount;
            this.PageIndex = pagedList.PageIndex;
            this.PageSize = pagedList.PageSize;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        protected PagedListMetaData()
        {
        }

        #region IPagedList Members

        /// <summary>
        /// 分页总数
        /// </summary>
        /// <value>
        /// 返回分页总数
        /// </value>
        public int PageCount { get; protected set; }

        /// <summary>
        /// 记录总条数
        /// </summary>
        /// <value>
        /// 返回记录总条数
        /// </value>
        public int TotalCount { get; protected set; }

        /// <summary>
        /// 当前页码，从1开始计算
        /// </summary>
        /// <value>
        /// 返回当前页码
        /// </value>
        public int PageIndex { get; protected set; }

        /// <summary>
        /// 每页显示的行数
        /// </summary>
        /// <value>
        /// 返回每页显示的行数
        /// </value>
        public int PageSize { get; protected set; }

        #endregion IPagedList Members
    }
}