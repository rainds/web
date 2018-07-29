namespace FrameWork.Core.Cache
{
    /// <summary>
    /// 缓存存放位置
    /// </summary>
    public enum CacheLocation
    {
        /// <summary>
        /// 本地共享
        /// </summary>
        Local = 0,

        /// <summary>
        /// 远程共享
        /// </summary>
        Remote = 1,

        /// <summary>
        /// 上下文专用
        /// </summary>
        Context = 2
    }
}