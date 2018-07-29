namespace FrameWork.Core.Mvc.Result
{
    public class OperationResult
    {
        /// <summary>
        /// 操作成功
        /// </summary>
        public OperationResult()
        {
            this.IsSuccess = true;
        }

        /// <summary>
        /// 操作失败
        /// </summary>
        public OperationResult(string message)
        {
            this.IsSuccess = false;
            this.FailureMessage = message;
        }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; protected set; }

        /// <summary>
        /// 失败消息
        /// </summary>
        public string FailureMessage { get; protected set; }
    }

    /// <summary>
    /// 操作结果
    /// </summary>
    public class OperationResult<T> : OperationResult
    {
        /// <summary>
        /// 操作成功
        /// </summary>
        public OperationResult(T data)
        {
            this.Data = data;
        }

        /// <summary>
        /// 操作结果
        /// </summary>
        protected OperationResult()
        {
        }

        /// <summary>
        /// 操作失败
        /// </summary>
        public static OperationResult<T> Failure(string message)
        {
            return new OperationResult<T> { IsSuccess = false, FailureMessage = message };
        }

        /// <summary>
        /// 返回数据
        /// </summary>
        public T Data { get; protected set; }
    }
}