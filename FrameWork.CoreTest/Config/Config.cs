using Microsoft.VisualStudio.TestTools.UnitTesting;
using FrameWork.Core;
using FrameWork.Core.Config;
using FrameWork.IocService;
using FrameWork.ConfigService;

namespace FrameWork.CoreTest.Config
{
    [TestClass]
    public class Config
    {
        [TestInitialize]
        public void InitConfig()
        {
            new AutofacInitializer().Init();
            new ConfigInitializer().Init();
        }

        [TestMethod]
        public void SaveTest()
        {
            var info = new Info()
            {
                Id = 2,
                Name = "hah"
            };
            Locator.Get<IConfig>().Save(info);
            var result = Locator.Get<IConfig>().Get<Info>();
            Assert.AreEqual(result.Id, 2);
        }

        [TestMethod]
        public void GetTest()
        {
            var result = Locator.Get<IConfig>().Get<Info>();
            Assert.AreEqual(result.Id, 2);
        }

        public class Info
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }
    }
}