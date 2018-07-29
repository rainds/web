using System.Collections.Generic;

namespace FrameWork.Core.Task
{
    /// <summary>
    /// 后台工作者管理者
    /// </summary>
    public static class WorkerManager
    {
        private static readonly object LockObj = new object(); //锁
        private static readonly IList<BaseWorker> Workers = new List<BaseWorker>();

        /// <summary>
        /// 添加后台工作者
        /// </summary>
        public static void Add(BaseWorker worker)
        {
            lock (LockObj)
            {
                worker.Init();
                Workers.Add(worker);
            }
        }

        /// <summary>
        /// 获得所有后台工作者
        /// </summary>
        public static IEnumerable<BaseWorker> GetWorkers()
        {
            return Workers;
        }
    }
}