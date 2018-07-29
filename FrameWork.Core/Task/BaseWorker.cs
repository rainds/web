using System;
using System.Diagnostics;
using System.Threading;

namespace FrameWork.Core.Task
{
    /// <summary>
    /// 后台工作者
    /// </summary>
    public abstract class BaseWorker : BaseInitializer
    {
        #region 任务参数属性

        /// <summary>
        /// 工作间隔(单位：秒)
        /// </summary>
        public abstract int Interval { get; }

        /// <summary>
        /// 超时时间(单位：秒)
        /// </summary>
        public abstract int TimeOut { get; }

        /// <summary>
        /// 工作者名称
        /// </summary>
        public abstract string WorkerName { get; }

        /// <summary>
        /// 工作者描述
        /// </summary>
        public abstract string Description { get; }

        #endregion 任务参数属性

        private System.Timers.Timer timer;
        private readonly object lockObj = new object(); //锁
        private bool isWorking; //标志

        /// <summary>
        /// 任务执行结果
        /// </summary>
        public TaskResult Result { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void OnInit()
        {
            //启动定时器
            this.timer = new System.Timers.Timer { Interval = this.Interval * 1000d };
            this.timer.Elapsed += this.Timer_Elapsed;
            this.timer.Start();

            this.Result = new TaskResult();
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Execute();
        }

        private void Execute()
        {
            lock (this.lockObj)
            {
                if (this.isWorking) return;
                this.isWorking = true;
            }

            try
            {
                Thread thread = null;
                string msg = null;

                Action wrappedAction = () =>
                {
                    thread = Thread.CurrentThread;
                    msg = this.OnExecute();
                };

                var watch = new Stopwatch();
                watch.Start();

                var result = wrappedAction.BeginInvoke(null, null);
                if (result.AsyncWaitHandle.WaitOne(this.TimeOut * 1000))
                {
                    wrappedAction.EndInvoke(result);
                }
                else
                {
                    thread.Abort();
                }

                watch.Stop();

                this.Result.Millisecond = watch.ElapsedMilliseconds;
                this.Result.ExecuteError = "";
                this.Result.ExecuteMsg = msg;
            }
            catch (Exception ex)
            {
                this.Result.Millisecond = -1;
                this.Result.ExecuteError = FrameWorkLogger.GetErrorMsg(ex);
                this.Result.ExecuteMsg = "";
            }
            finally
            {
                this.Result.ExecuteTime = DateTime.Now;
            }

            lock (this.lockObj)
            {
                this.isWorking = false;
            }
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        protected abstract string OnExecute();
    }
}