namespace FrameWork.Core.Data
{
    /// <summary>
    /// 数据库配置信息
    /// </summary>
    public class DbConfigInfo
    {
        /// <summary>
        /// 获取或设置连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 获取或设置提供程序名称属性
        /// </summary>
        public string ProviderName { get; set; }
    }
}