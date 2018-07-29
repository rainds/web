using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rainds.Service;

namespace Rainds.ServiceTest
{
    [TestClass]
    public abstract class BaseTest
    {
        [TestInitialize]
        public void InitCore()
        {
            new ServiceInitializer().Init();
        }
    }
}