namespace FrameWork.Core.Plugin
{
    /// <summary>
    /// 插件加载者
    /// </summary>
    public interface IPluginLoader
    {
        /// <summary>
        /// 加载插件
        /// </summary>
        bool Load();
    }
}
