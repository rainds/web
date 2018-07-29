namespace FrameWork.Core.Ioc
{
    /// <summary>
    /// 受容器管理的服务对象的生命周期
    /// </summary>
    public enum LifeTime
    {
        /// <summary>
        /// 全局单例
        /// </summary>
        Singleton = 0,

        /// <summary>
        /// 每次调用创建一个新的实例
        /// </summary>
        Transient = 1,

        /// <summary>
        /// 在一个生命周期域内单例
        /// </summary>
        LifetimeScope = 2
    }
}