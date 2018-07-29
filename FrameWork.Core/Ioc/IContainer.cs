using System;

namespace FrameWork.Core.Ioc
{
    /// <summary>
    /// IOC容器
    /// </summary>
    public interface IContainer
    {
        /// <summary>
        /// 获取实例
        /// </summary>
        T Get<T>();

        /// <summary>
        /// 获取实例
        /// </summary>
        T Get<T>(object serviceKey);

        /// <summary>
        /// 获取实例
        /// </summary>
        object Get(Type serviceType);

        /// <summary>
        /// 判断类型是否注册
        /// </summary>
        bool IsRegistered(Type serviceType);

        /// <summary>
        /// 判断类型是否注册
        /// </summary>
        bool IsRegistered(Type serviceType, object serviceKey);

        /// <summary>
        /// 将注册者的配置更新到当前容器
        /// </summary>
        void Update(IRegister register);
    }
}