using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rainds.IService.User;
using Rainds.Service.User;

namespace Rainds.ServiceTest.User
{
    [TestClass]
    public class UserServiceTest : BaseTest
    {
        [TestMethod]
        public void LoginTest()
        {
            new UserService().Login(new LoginModel()
            {
                Name = "zhangsan",
                Password = "123456"
            });
        }
    }
}