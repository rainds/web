using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using FrameWork.Core.Ioc;

namespace FrameWork.IocService
{
    public class AutofacRegister : IRegister
    {
        private ContainerBuilder defaultBuilder;

        internal ContainerBuilder Builder
        {
            get { return this.defaultBuilder ?? (this.defaultBuilder = new ContainerBuilder()); }
            set { this.defaultBuilder = value; }
        }

        public void Set<TImpl, TContract>(LifeTime lifetime)
        {
            var builder = this.Builder.RegisterType<TImpl>().As<TContract>();
            SetLifeTime(builder, lifetime);
        }

        public void Set<TImpl, TContract>(LifeTime lifetime, object serviceKey)
        {
            var builder = this.Builder.RegisterType<TImpl>().Keyed<TContract>(serviceKey);
            SetLifeTime(builder, lifetime);
        }

        public void Set<TContract>(TContract instance, LifeTime lifetime) where TContract : class
        {
            var builder = this.Builder.RegisterInstance(instance).As<TContract>();
            SetLifeTime(builder, lifetime);
        }

        public void Set<TContract>(TContract instance, LifeTime lifetime, object serviceKey) where TContract : class
        {
            var builder = this.Builder.RegisterInstance(instance).Keyed<TContract>(serviceKey);
            SetLifeTime(builder, lifetime);
        }

        public void SetGeneric(Type typeImpl, Type contractType, LifeTime lifetime)
        {
            var builder = this.Builder.RegisterGeneric(typeImpl).As(contractType);
            SetLifeTime(builder, lifetime);
        }

        public void Set(Type typeImpl, Type contractType, LifeTime lifetime)
        {
            var builder = this.Builder.RegisterType(typeImpl).As(contractType);
            SetLifeTime(builder, lifetime);
        }

        public void Set(Type typeImpl, Type contractType, LifeTime lifetime, object serviceKey)
        {
            var builder = this.Builder.RegisterType(typeImpl).Keyed(serviceKey, contractType);
            SetLifeTime(builder, lifetime);
        }

        public void Set(Type typeImpl, Type[] contractTypes, LifeTime lifetime)
        {
            foreach (var contract in contractTypes)
                this.Set(typeImpl, contract, lifetime);
        }

        public void Set(Type typeImpl, Type[] contractTypes, LifeTime lifetime, object name)
        {
            foreach (var contract in contractTypes)
                this.Set(typeImpl, contract, lifetime, name);
        }

        //根据程序集通用注册接口类型
        public void Set(Assembly[] assemblies, Type[] contractTypes, LifeTime lifetime)
        {
            //自动注册接口
            var result = Builder.RegisterAssemblyTypes(assemblies).Where(type => contractTypes.Any(contractType => contractType.IsAssignableFrom(type)) && !type.IsAbstract).AsImplementedInterfaces();
            SetLifeTime(result, lifetime);
        }

        private static void SetLifeTime<T1, T2, T3>(IRegistrationBuilder<T1, T2, T3> builder, LifeTime lifetime)
        {
            switch (lifetime)
            {
                case LifeTime.Singleton:
                    builder.SingleInstance();
                    break;

                case LifeTime.Transient:
                    builder.InstancePerDependency();
                    break;

                case LifeTime.LifetimeScope:
                    builder.InstancePerLifetimeScope();
                    break;
            }
        }
    }
}