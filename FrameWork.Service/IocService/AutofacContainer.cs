using System;
using System.Runtime.Remoting.Messaging;
using Autofac;
using FrameWork.Core.Ioc;
using IContainer = FrameWork.Core.Ioc.IContainer;

namespace FrameWork.IocService
{
    public class AutofacContainer : IContainer
    {
        #region autofac

        private readonly Autofac.IContainer afContainer;

        public AutofacContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<AutofacRegister>().As<IRegister>().InstancePerDependency();
            this.afContainer = builder.Build();
        }

        private ILifetimeScope Scope
        {
            get
            {
                const string Key = "FrameWork.IocService.Scope";

                var scope = CallContext.GetData(Key) as ILifetimeScope;

                if (scope != null) return scope;

                scope = this.afContainer.BeginLifetimeScope();
                CallContext.SetData(Key, scope);

                return scope;
            }
        }

        #endregion autofac

        public T Get<T>()
        {
            return this.Scope.Resolve<T>();
        }

        public T Get<T>(object serviceKey)
        {
            return this.Scope.ResolveKeyed<T>(serviceKey);
        }

        public object Get(Type serviceType)
        {
            return this.Scope.Resolve(serviceType);
        }

        public bool IsRegistered(Type serviceType)
        {
            return this.Scope.IsRegistered(serviceType);
        }

        public bool IsRegistered(Type serviceType, object serviceKey)
        {
            return this.Scope.IsRegisteredWithKey(serviceKey, serviceType);
        }

        public void Update(IRegister register)
        {
            var autofacRegister = (register as AutofacRegister);
            if (autofacRegister == null)
                throw new ArgumentException("register is not a AutofacRegister");

            //更新当前容器，并重置容器构造者，防止被复用
            autofacRegister.Builder.Update(afContainer);
            autofacRegister.Builder = null;
        }
    }
}