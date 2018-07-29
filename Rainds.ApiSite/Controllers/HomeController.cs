using System.Web.Mvc;
using FrameWork.Core;
using FrameWork.Core.Mvc.Controller;
using FrameWork.Core.Principal;
using Rainds.IService.User;

namespace Rainds.ApiSite.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return Content("Controller");
        }

        public ActionResult Login(LoginModel model)
        {
            var result = Locator.Get<IUserService>().Login(model);
            if (result.IsSuccess)
            {
                SetToken(result.Data, model.Name);
            }

            return Json(result);
        }

        public ActionResult Register(RegisterModel model)
        {
            var result = Locator.Get<IUserService>().Register(model);
            if (result.IsSuccess)
            {
                SetToken(result.Data, model.Name);
            }

            return this.Json();
        }

        private void SetToken(int id, string name)
        {
            TokenHelper.SetToken(id, name, IdentityType.Member, Response);
        }
    }
}