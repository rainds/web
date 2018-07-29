using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FrameWork.Core;
using FrameWork.Core.Ioc;
using FrameWork.IocService;

namespace FrameWork.CoreTest.Ioc
{
    #region 测试对象

    public interface IIocClass
    {
        string TestValue { get; set; }
    }

    public class IocClassImpl : IIocClass
    {
        public string TestValue { get; set; }
    }

    public interface IIocRepository<in T> where T : IIocClass
    {
        void Update(T obj, string value);
    }

    public class IocRepository<T> : IIocRepository<T> where T : IIocClass
    {
        public void Update(T obj, string value)
        {
            obj.TestValue = value;
        }
    }

    #endregion 测试对象

    [TestClass]
    public class TestIoc
    {
        [TestInitialize]
        public void InitIoc()
        {
            new AutofacInitializer().Init();
        }

        [TestMethod]
        public void TestRegister()
        {
            var register = Locator.Get<IRegister>();
            register.Set<IocClassImpl, IIocClass>(LifeTime.Transient);
            Locator.Container.Update(register);

            var obj = Locator.Get<IIocClass>();
            obj.TestValue = "Test";
        }

        [TestMethod]
        public void TestRegisterGeneric()
        {
            var register = Locator.Get<IRegister>();
            register.SetGeneric(typeof(IocRepository<>), typeof(IIocRepository<>), LifeTime.Transient);
            Locator.Container.Update(register);

            var repository = Locator.Get<IIocRepository<IIocClass>>();
            repository.Update(new IocClassImpl(), "Test");
        }

        [TestMethod]
        public void TestLifeTime()
        {
            var register = Locator.Get<IRegister>();
            register.Set<IocClassImpl, IIocClass>(LifeTime.LifetimeScope, "LifetimeScope");
            register.Set<IocClassImpl, IIocClass>(LifeTime.Singleton, "Singleton");
            register.Set<IocClassImpl, IIocClass>(LifeTime.Transient, "Transient");
            Locator.Container.Update(register);

            var obj10 = Locator.Get<IIocClass>("LifetimeScope");
            obj10.TestValue = "10";

            var obj20 = Locator.Get<IIocClass>("Singleton");
            obj20.TestValue = "20";

            var obj30 = Locator.Get<IIocClass>("Transient");
            obj30.TestValue = "30";

            var thread = new Thread(delegate()
            {
                var obj11 = Locator.Get<IIocClass>("LifetimeScope");
                obj11.TestValue = "11";

                var obj21 = Locator.Get<IIocClass>("Singleton");
                obj21.TestValue = "21";

                var obj31 = Locator.Get<IIocClass>("Transient");
                obj31.TestValue = "31";
            });
            thread.Start();
            thread.Join();

            var obj12 = Locator.Get<IIocClass>("LifetimeScope");
            Assert.IsTrue(obj12.TestValue == "10"); //上下文单例只有同一个线程上下文才会被修改

            var obj22 = Locator.Get<IIocClass>("Singleton");
            Assert.IsTrue(obj22.TestValue == "21"); //全局单例在多个线程中共享

            var obj32 = Locator.Get<IIocClass>("Transient");
            Assert.IsNull(obj32.TestValue);
        }
    }
}