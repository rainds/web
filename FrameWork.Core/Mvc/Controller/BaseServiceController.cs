using System.Web.Mvc;
using FrameWork.Core.Mvc.Invoker;
using JsonResult = FrameWork.Core.Mvc.Result.JsonResult;

namespace FrameWork.Core.Mvc.Controller
{
    public class BaseServiceController : BaseController
    {
        public ActionResult Index(string serviceArea, string serviceName,
            string methodName, ServiceInvoker invoker)
        {
            return new JsonResult(invoker.Execute());
        }
    }
}