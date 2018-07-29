using System.Web.Mvc;
using FrameWork.Core.Mvc.Result;
using JsonResult = FrameWork.Core.Mvc.Result.JsonResult;

namespace FrameWork.Core.Mvc.Controller
{
    /// <summary>
    /// controller基础类
    /// </summary>
    public class BaseController : System.Web.Mvc.Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            
            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// 将业务操作结果转换为JsonResult
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected JsonResult Json(OperationResult result)
        {
            return new JsonResult(result.IsSuccess
                ? new ResponseMessage()
                : new ResponseMessage(ResponseCode.ServerError, result.FailureMessage));
        }

        /// <summary>
        /// 将业务操作结果转换为JsonResult
        /// </summary>
        protected JsonResult Json<T>(OperationResult<T> result)
        {
            return new JsonResult(result.IsSuccess
                ? new ResponseMessage(result.Data)
                : new ResponseMessage(ResponseCode.ServerError, result.FailureMessage));
        }

        protected JsonResult Json()
        {
            return new JsonResult(new ResponseMessage());
        }

        protected JsonResult Json(ResponseCode code, string msg)
        {
            return new JsonResult(new ResponseMessage(code, msg));
        }

        protected new JsonResult Json(object data)
        {
            return new JsonResult(new ResponseMessage(data));
        }

        /// <summary>
        /// 当前客户端IP
        /// </summary>
        protected string CurrentClientIp
        {
            get
            {
                var request = this.HttpContext.Request;
                var userIp = "";

                if (string.IsNullOrWhiteSpace(userIp))
                    userIp = request.ServerVariables["X-Forwarded-For"];

                if (string.IsNullOrWhiteSpace(userIp))
                    userIp = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (string.IsNullOrWhiteSpace(userIp))
                    userIp = request.ServerVariables["REMOTE_ADDR"];

                return userIp;
            }
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            //记录错误日志
            var desc = filterContext.Exception == null ? "应用程序错误" : filterContext.Exception.Message;

            if (filterContext.Exception != null)
            {
                FrameWorkLogger.Log(filterContext.Exception);
            }

            //var stack = filterContext.Exception == null ? "" : filterContext.Exception.StackTrace;
            //var isAuthError = filterContext.Exception is AuthenticationException;
            //var requestMsg = " 请求路径：" + Request.RawUrl + " 请求类型：" + Request.RequestType + " 客户端信息：" + Request.UserAgent;

            //返回错误消息给客户端
            filterContext.ExceptionHandled = true;
            filterContext.Result = new JsonResult(new ResponseMessage(ResponseCode.ServerError, desc));

            base.OnException(filterContext);
        }
    }
}