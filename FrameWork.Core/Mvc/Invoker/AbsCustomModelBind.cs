using System.Web.Mvc;

namespace FrameWork.Core.Mvc.Invoker
{
    public interface ICustomModelBind
    {
        bool IsRunThisBind(ModelBindingContext mbc);

        object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext);
    }
}
