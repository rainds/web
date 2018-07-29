using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using FastReflection;
using FrameWork.Core.Mvc.Result;

namespace FrameWork.Core.Mvc.Invoker
{
    public class ServiceInvoker : IDisposable
    {
        //protected static readonly ConcurrentDictionary<string, object> ServiceObjects
        //= new ConcurrentDictionary<string, object>();

        protected static readonly ConcurrentDictionary<string, MethodInfo> ServiceMethodInfos
            = new ConcurrentDictionary<string, MethodInfo>();

        protected static readonly ConcurrentDictionary<string, ResponseMessage> MethodReturnValues
            = new ConcurrentDictionary<string, ResponseMessage>();

        protected static readonly ConcurrentDictionary<string, Type> InterfaceTypes
            = new ConcurrentDictionary<string, Type>();

        protected static readonly ConcurrentDictionary<string, MethodInfo> InterfaceMethodInfos
            = new ConcurrentDictionary<string, MethodInfo>();

        /// <summary>
        /// mvc 上下文
        /// </summary>
        public ControllerContext ControllerContext { set; get; }

        /// <summary>
        /// 数据绑定上下文
        /// </summary>
        public ModelBindingContext BindingContext { set; get; }

        /// <summary>
        /// 运行服务上下文
        /// </summary>
        public ServiceInvokerContext InvokerContext { set; get; }

        /// <summary>
        /// 服务调用之前执行的事件,执行顺序1
        /// </summary>
        public event EventHandler BeginInvoker;

        /// <summary>
        /// 查找接口之前执行的事务,执行顺序2
        /// </summary>
        public event EventHandler OnFindInterface;

        /// <summary>
        /// 查找接口方法之前执行的事件,执行顺序3
        /// </summary>
        public event EventHandler OnFindInterfaceMethod;

        /// <summary>
        /// 参数绑定前执行的事件,执行顺序4
        /// </summary>
        public event EventHandler OnBindViewModel;

        /// <summary>
        /// 检查接口是否有实现前执行的事件,执行顺序5
        /// </summary>
        public event EventHandler OnCheckRegisteredService;

        /// <summary>
        /// 接口还没有实现事件
        /// </summary>
        public event EventHandler OnNotRegisterService;

        /// <summary>
        /// 查找实现类方法之前执行的事件,执行顺序6
        /// </summary>
        public event EventHandler OnFindServiceMethod;

        /// <summary>
        /// 运行服务实现类之前执行的事件,执行顺序7
        /// </summary>
        public event EventHandler OnInvokeService;

        private bool isComplete;
        private Action[] step;
        private EventHandler[] eventHandler;

        internal ServiceInvoker(ControllerContext controllerContext,
            ModelBindingContext bindingContext)
        {
            this.ControllerContext = controllerContext;
            this.BindingContext = bindingContext;
            this.InvokerContext = new ServiceInvokerContext();
            this.isComplete = false;
        }

        /// <summary>
        /// 绑定运行步骤
        /// </summary>
        private void BindStep()
        {
            this.step = new Action[] {
                this.GetServiceParam,
                this.FindInterface,
                this.FindInterfaceMethod,
                this.BindViewModel,
                this.CheckRegisteredService,
                this.FindServiceMethod,
                this.InvokeService };

            this.eventHandler = new[] {
                this.BeginInvoker,
                this.OnFindInterface,
                this.OnFindInterfaceMethod,
                this.OnBindViewModel,
                this.OnCheckRegisteredService,
                this.OnFindServiceMethod,
                this.OnInvokeService };
        }

        /// <summary>
        /// 运行各步骤
        /// </summary>
        internal ResponseMessage Execute()
        {
            this.BindStep();

            int i = 0;
            foreach (var step in this.step)
            {
                var e = this.eventHandler[i++];
                if (e != null)
                    e(this, null);

                if (this.isComplete)
                    break;

                step();

                if (this.isComplete)
                    break;
            }

            return this.InvokerContext.ResponseMessage;
        }

        /// <summary>
        /// 取得需要运行的服务参数
        /// </summary>
        private void GetServiceParam()
        {
            this.InvokerContext.ServiceArea = this.BindingContext.ValueProvider.GetValue("serviceArea").AttemptedValue.Trim();
            this.InvokerContext.ServiceName = this.BindingContext.ValueProvider.GetValue("serviceName").AttemptedValue.Trim();
            this.InvokerContext.MethodName = this.BindingContext.ValueProvider.GetValue("methodName").AttemptedValue.Trim();

            if (
                string.IsNullOrWhiteSpace(this.InvokerContext.ServiceArea)
                ||
                string.IsNullOrWhiteSpace(this.InvokerContext.ServiceName)
                ||
                string.IsNullOrWhiteSpace(this.InvokerContext.MethodName)
                )
            {
                this.InvokerContext.ResponseMessage = new ResponseMessage(ResponseCode.Forbidden, "未知的服务调用");
                this.Complete();
            }
        }

        private void FindInterface()
        {
            Type interfaceType;
            if (!InterfaceTypes.TryGetValue(this.InvokerContext.ServiceKey, out interfaceType))
            {
                interfaceType = Type.GetType(this.InvokerContext.InterfaceTypeStr, false, true);
                if (interfaceType == null)
                {
                    this.InvokerContext.ResponseMessage = new ResponseMessage(ResponseCode.Forbidden, "服务接口不存在");
                    this.Complete();
                    return;
                }
                InterfaceTypes.TryAdd(this.InvokerContext.ServiceKey, interfaceType);//原本是没有的
            }
            this.InvokerContext.InterfaceType = interfaceType;
        }

        private void FindInterfaceMethod()
        {
            if (!this.FindMethod(this.InvokerContext.InterfaceType, InterfaceMethodInfos))
                this.Complete();
        }

        private void CheckRegisteredService()
        {
            this.InvokerContext.IsRegisteredService = Locator.Container.IsRegistered(this.InvokerContext.InterfaceType);

            if (this.InvokerContext.IsRegisteredService)
            {
                this.InvokerContext.ServiceObj = Locator.Container.Get(this.InvokerContext.InterfaceType);
                return;
            }

            //接口没实现的用户自定义处理
            if (this.OnNotRegisterService != null)
            {
                this.OnNotRegisterService(this, null);
                this.Complete();
                return;
            }

            //取得缓存后的默认值
            ResponseMessage returnObj;
            if (MethodReturnValues.TryGetValue(this.InvokerContext.MethodKey, out returnObj))
            {
                this.InvokerContext.ResponseMessage = returnObj;
                this.Complete();
                return;
            }

            //接口没有实现的默认处理
            do
            {
                //如果是不带返回值的非泛型返回结果，则设置返回值为空
                var returnType = this.InvokerContext.InterfaceMethod.ReturnType;
                if (!returnType.IsGenericType)
                {
                    this.InvokerContext.ResponseMessage = new ResponseMessage();
                    break;
                }

                var dataType = returnType.GenericTypeArguments[0];

                //如果不是泛型或者不是IList的子类
                if (!dataType.IsGenericType ||
                    !typeof(IList<>).IsAssignableFrom(dataType.GetGenericTypeDefinition()))
                {
                    this.InvokerContext.ResponseMessage =
                        new ResponseMessage(Activator.CreateInstance(dataType));
                    break;
                }

                var itemType = dataType.GenericTypeArguments[0];
                var itemObj = Activator.CreateInstance(itemType);
                var listType = typeof(List<>).MakeGenericType(itemType);
                var dataObj = Activator.CreateInstance(listType);
                listType.GetMethod("Add").FastInvoke(dataObj, new[] { itemObj });

                this.InvokerContext.ResponseMessage = new ResponseMessage(dataObj);
            } while (false);

            MethodReturnValues.TryAdd(this.InvokerContext.MethodKey, this.InvokerContext.ResponseMessage);
            this.Complete();
        }

        private void BindViewModel()
        {
            var parms = this.InvokerContext.InterfaceMethod.GetParameters();
            var args = new object[parms.Length];
            var builder = new StringBuilder(4096);
            var modelState = this.ControllerContext.Controller.ViewData.ModelState;
            var binder = new DefaultModelBinder();
            var isError = false;

            for (var i = 0; i < parms.Length; i++)
            {
                var paramName = parms[i].Name;
                var paramType = parms[i].ParameterType;
                var context = new ModelBindingContext
                {
                    FallbackToEmptyPrefix = true,
                    ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, paramType),
                    ModelName = paramName,
                    ModelState = modelState,
                    PropertyFilter = null,
                    ValueProvider = this.BindingContext.ValueProvider
                };

                //运行自定义数据绑定
                var running = false;
                foreach (var modelbind in this.InvokerContext.ModelBinds)
                {
                    running = modelbind.IsRunThisBind(context);
                    if (!running) continue;
                    args[i] = modelbind.BindModel(this.ControllerContext, this.BindingContext);
                    break;
                }
                if (running) continue;

                //运行默认数据绑定
                var value = binder.BindModel(this.ControllerContext, context);
                if (paramType.IsValueType && value == null)
                {
                    isError = true;
                    builder.AppendLine().AppendFormat("{0}：{1}", paramName, "没有传值或者数据格式错误");
                }
                else
                    args[i] = value;
            }

            foreach (var key in modelState.Keys)
            {
                foreach (var error in modelState[key].Errors.
                    Where(error => !string.IsNullOrWhiteSpace(error.ErrorMessage)))
                {
                    isError = true;
                    builder.AppendLine().AppendFormat("{0}：{1}", key, error.ErrorMessage);
                }
            }

            if (isError)
            {
                this.InvokerContext.ResponseMessage =
                    new ResponseMessage(ResponseCode.Forbidden, builder.ToString());
                this.Complete();
                return;
            }

            this.InvokerContext.ArgsObj = args;
        }

        private void FindServiceMethod()
        {
            if (!this.FindMethod(this.InvokerContext.ServiceObj.GetType(), ServiceMethodInfos))
                this.Complete();
        }

        private void InvokeService()
        {
            OperationResult result;

            try
            {
                result = (OperationResult)this.InvokerContext.
                    ServiceMethod.FastInvoke(this.InvokerContext.ServiceObj, this.InvokerContext.ArgsObj);

                if (!result.IsSuccess)
                {
                    this.InvokerContext.ResponseMessage = new ResponseMessage(ResponseCode.ServerError, result.FailureMessage);
                    return;
                }
            }
            catch (Exception ex)
            {
                FrameWorkLogger.Log(ex);
                this.InvokerContext.ResponseMessage =
                    new ResponseMessage(ResponseCode.ServerError,
                        "调用服务" + this.InvokerContext.MethodKey + "出错：" + ex.Message);
                return;
            }

            var type = result.GetType();
            this.InvokerContext.ResponseMessage = type.IsGenericType
                ? new ResponseMessage(type.GetProperty("Data").FastGetValue(result))
                : new ResponseMessage();
        }

        private bool FindMethod(Type type, ConcurrentDictionary<string, MethodInfo> map)
        {
            MethodInfo methodinfo;
            if (!map.TryGetValue(this.InvokerContext.MethodKey, out methodinfo))
            {
                try
                {
                    methodinfo = type.GetMethod(this.InvokerContext.MethodName, BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public);
                    if (methodinfo == null)
                    {
                        this.InvokerContext.ResponseMessage = new ResponseMessage(ResponseCode.ServerError, "服务方法找不到");
                        return false;
                    }
                }
                catch (AmbiguousMatchException)
                {
                    this.InvokerContext.ResponseMessage = new ResponseMessage(ResponseCode.ServerError, "服务方法重名");
                    return false;
                }

                map.TryAdd(this.InvokerContext.MethodKey, methodinfo);
            }

            if (map == InterfaceMethodInfos)
                this.InvokerContext.InterfaceMethod = methodinfo;

            if (map == ServiceMethodInfos)
                this.InvokerContext.ServiceMethod = methodinfo;

            return true;
        }

        /// <summary>
        /// 结束运行
        /// </summary>
        public void Complete()
        {
            this.isComplete = true;
        }

        public void Dispose()
        {
            this.ControllerContext = null;
            this.BindingContext = null;
            this.InvokerContext = null;
        }
    }
}