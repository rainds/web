using System.Collections.Generic;
using System.Web.Mvc;

namespace FrameWork.Core.Mvc.Invoker
{
    public class ServiceInvokerModelBinder : IModelBinder
    {
        internal ServiceInvokerModelBinder(){ }

        private static readonly List<IServiceInvokeModule> 
            InvokeModuleList = new List<IServiceInvokeModule>();

        /// <summary>
        /// 添加自定义模块
        /// </summary>
        public void AddInvokeModule(IServiceInvokeModule module)
        {
            InvokeModuleList.Add(module);
        }

        /// <summary>
        /// 绑定ServiceInvoker
        /// </summary>
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var si = new ServiceInvoker(controllerContext, bindingContext);
            InvokeModuleList.ForEach(m => m.ProcessInvoker(si));
            return si;
        }
    }
}
