using FrameWork.Core.Ioc;

namespace FrameWork.Core
{
    /// <summary>
    /// 服务工厂
    /// </summary>
    public static class Locator
    {
        /// <summary>
        /// IOC容器对象
        /// </summary>
        public static IContainer Container { get; set; }

        /// <summary>
        /// 获取服务
        /// </summary>
        public static T Get<T>()
        {
            return Container.Get<T>();
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        public static T Get<T>(object serviceKey)
        {
            return Container.Get<T>(serviceKey);
        }
    }
}