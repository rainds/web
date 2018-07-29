namespace FrameWork.Core.Mvc.Result
{
    public class ResponseMessage
    {
        /// <summary>
        /// 成功
        /// </summary>
        public ResponseMessage()
            : this(ResponseCode.Success, null, null, null)
        {
        }

        /// <summary>
        /// 成功
        /// </summary>
        public ResponseMessage(object data)
            : this(ResponseCode.Success, null, null, data)
        {
        }

        public ResponseMessage(ResponseCode code, string msg)
            : this(code, msg, null, null)
        {
        }

        public ResponseMessage(ResponseCode code, string msg, object data)
            : this(code, msg, null, data)
        {
        }

        public ResponseMessage(ResponseCode code, string msg, string errorId)
            : this(code, msg, errorId, null)
        {
        }

        private ResponseMessage(ResponseCode code, string msg, string errorId, object data)
        {
            this.Code = code;
            this.Msg = msg ?? string.Empty;
            this.ErrorId = errorId ?? string.Empty;
            this.Data = data ?? string.Empty;
        }

        /// <summary>
        /// 标准HTTP响应状态码：
        /// 200正常响应，401未经授权，500服务端错误
        /// </summary>
        public ResponseCode Code { get; private set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Msg { get; private set; }

        /// <summary>
        /// 错误Id
        /// </summary>
        public string ErrorId { get; private set; }

        /// <summary>
        /// 数据
        /// </summary>
        public object Data { get; private set; }
    }

    /// <summary>
    /// 响应状态码
    /// </summary>
    public enum ResponseCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success = 200,

        /// <summary>
        /// 未经授权
        /// </summary>
        Unauthorized = 401,

        /// <summary>
        /// 请求被拒绝
        /// </summary>
        Forbidden = 403,

        /// <summary>
        /// 服务端错误
        /// </summary>
       ServerError = 500,
    }
}
