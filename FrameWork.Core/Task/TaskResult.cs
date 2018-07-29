using System;

namespace FrameWork.Core.Task
{
    /// <summary>
    /// 任务执行结果
    /// </summary>
    public class TaskResult
    {
        /// <summary>
        /// 执行时间
        /// </summary>
        public DateTime ExecuteTime { get; internal set; }

        /// <summary>
        /// 执行毫秒数
        /// </summary>
        public long Millisecond { get; internal set; }

        /// <summary>
        /// 执行消息
        /// </summary>
        public string ExecuteMsg { get; internal set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ExecuteError { get; internal set; }
    }
}