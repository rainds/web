namespace FrameWork.Core.Plugin
{
    /// <summary>
    /// 不加载任何插件的插件实现者，主要用于单元测试
    /// </summary>
    public class NonePluginLoader : IPluginLoader
    {
        /// <summary>
        /// 不加载任何插件
        /// </summary>
        public bool Load()
        {
            return true;
        }
    }
}
