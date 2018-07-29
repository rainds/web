using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using FrameWork.Core.Mvc.Result;

namespace FrameWork.Core.Mvc.Invoker
{
    /// <summary>
    /// 调用服务接口上下文
    /// </summary>
    public class ServiceInvokerContext
    {
        private readonly string typeFormat;
        private readonly IList<ICustomModelBind> modelBindList;

        /// <summary>
        /// 调用服务上下文
        /// </summary>
        internal ServiceInvokerContext()
        {
            this.typeFormat = ConfigurationManager.AppSettings["InterfaceTypeFormat"];
            this.modelBindList = new List<ICustomModelBind>();
        }

        //添加数据自定义绑定
        public IList<ICustomModelBind> ModelBinds { get { return this.modelBindList; } }

        /// <summary>
        /// 服务接口的命名空间
        /// </summary>
        //private string[] ServiceNS { set; get; }

        /// <summary>
        /// 服务接口
        /// </summary>
        public string ServiceArea { set; get; }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { set; get; }

        /// <summary>
        /// 服务方法名称
        /// </summary>
        public string MethodName { set; get; }

        /// <summary>
        /// 服务接口的类型
        /// </summary>
        public Type InterfaceType { set; get; }

        /// <summary>
        /// 服务实现类
        /// </summary>
        public object ServiceObj { set; get; }

        /// <summary>
        /// 服务接口的方法
        /// </summary>
        public MethodInfo InterfaceMethod { set; get; }

        /// <summary>
        /// 服务实现的方法
        /// </summary>
        public MethodInfo ServiceMethod { set; get; }

        /// <summary>
        /// 接口是否已实现
        /// </summary>
        public bool IsRegisteredService { set; get; }

        /// <summary>
        /// 返回信息
        /// </summary>
        public ResponseMessage ResponseMessage { set; get; }

        /// <summary>
        /// 运行接口所需要的参数
        /// </summary>
        public object[] ArgsObj { set; get; }

        public string ServiceKey { get { return this.ServiceArea + "." + this.ServiceName; } }

        public string MethodKey { get { return this.ServiceKey + "." + this.MethodName; } }

        /// <summary>
        /// 服务类型的字符串
        /// </summary>
        public string InterfaceTypeStr
        {
            get
            {
                return this.typeFormat.Replace("{ServiceArea}", this.ServiceArea).Replace("{ServiceName}", this.ServiceName);
            }
        }
    }
}