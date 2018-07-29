using System;
using System.Reflection;

namespace FrameWork.Core.Ioc
{
    /// <summary>
    /// Ioc注册者
    /// </summary>
    public interface IRegister
    {
        /// <summary>
        /// 注册类型
        /// </summary>
        void Set<TImpl, TContract>(LifeTime lifetime);

        /// <summary>
        /// 注册类型
        /// </summary>
        void Set<TImpl, TContract>(LifeTime lifetime, object serviceKey);

        /// <summary>
        /// 注册实例
        /// </summary>
        void Set<TContract>(TContract instance, LifeTime lifetime) where TContract : class;

        /// <summary>
        /// 注册实例
        /// </summary>
        void Set<TContract>(TContract instance, LifeTime lifetime, object serviceKey) where TContract : class;

        /// <summary>
        /// 注册泛型类型
        /// </summary>
        void SetGeneric(Type typeImpl, Type contractType, LifeTime lifetime);

        /// <summary>
        /// 注册普通类型
        /// </summary>
        void Set(Type typeImpl, Type contractType, LifeTime lifetime);

        /// <summary>
        /// 注册普通类型，并使用别名
        /// </summary>
        void Set(Type typeImpl, Type contractType, LifeTime lifetime, object name);

        /// <summary>
        /// 注册多个实例
        /// </summary>
        void Set(Type typeImpl, Type[] contractTypes, LifeTime lifetime);

        /// <summary>
        /// 注册多个实例，并使用别名
        /// </summary>
        void Set(Type typeImpl, Type[] contractTypes, LifeTime lifetime, object name);

        /// <summary>
        ///  根据程序集通用注册多接口类型
        /// </summary>
        void Set(Assembly[] assemblies, Type[] contractTypes, LifeTime lifetime);
    }
}