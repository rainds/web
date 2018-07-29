using System.Collections.Concurrent;

namespace FrameWork.Core
{
    /// <summary>
    /// 执行初始化操作的基类
    /// </summary>
    public abstract class BaseInitializer
    {
        //初始化顺序排序
        public virtual int Order { get; set; }

        private static readonly ConcurrentDictionary<string, bool> InitDict
            = new ConcurrentDictionary<string, bool>();

        /// <summary>
        /// 是否初始化
        /// </summary>
        public bool IsInit
        {
            get { return InitDict.ContainsKey(this.GetType().FullName); }
        }

        /// <summary>
        /// 初始化(自动判断是否已经初始化)
        /// </summary>
        public void Init()
        {
            if (this.IsInit) return;
            this.OnInit();
            InitDict.TryAdd(this.GetType().FullName, true);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        protected abstract void OnInit();
    }
}