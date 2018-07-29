namespace FrameWork.Core.Cache
{
    /// <summary>
    /// Memcached客户端配置
    /// </summary>
    public class MemcachedConfig
    {
        /// <summary>
        /// 服务端地址
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 最小连接数
        /// </summary>
        public int MinPoolSize { get; set; }

        /// <summary>
        /// 最大连接数
        /// </summary>
        public int MaxPoolSize { get; set; }
    }
}