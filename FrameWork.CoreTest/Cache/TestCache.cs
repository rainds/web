using Microsoft.VisualStudio.TestTools.UnitTesting;
using FrameWork.Core;
using FrameWork.Core.Cache;
using FrameWork.CacheService;
using FrameWork.IocService;

namespace FrameWork.CoreTest.Cache
{
    [TestClass]
    public class TestCache
    {
        [TestInitialize]
        public void InitIoc()
        {        
            new CacheInitializer().Init();
            new AutofacInitializer().Init();
        }

        [TestMethod]
        public void TestLocalCache()
        {
            RunTest(CacheLocation.Local);
        }

        [TestMethod]
        public void TestContextCache()
        {
            RunTest(CacheLocation.Context);
        }

        [TestMethod]
        public void TestRemoteCache()
        {
            RunTest(CacheLocation.Remote);
        }

        private static void RunTest(CacheLocation location)
        {
            var provider = Locator.Get<ICache>(location);

            provider.Set("TestCache1", new CacheObj { Name = "TestCache1" });
            var obj1 = provider.Get<CacheObj>("TestCache1");
            Assert.IsNotNull(obj1);
            Assert.IsTrue(obj1.Name == "TestCache1");


            provider.Set("TestCache2", new CacheObj { Name = "TestCache2" }, 1, "TestRegion");
            provider.ClearRegion("TestRegion");
            var obj2 = provider.Get<CacheObj>("TestCache2", "TestRegion");
            Assert.IsNull(obj2);
        }
    }
}