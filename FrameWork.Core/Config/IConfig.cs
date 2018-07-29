using System.Collections.Generic;

namespace FrameWork.Core.Config
{
    /// <summary>
    /// 配置提供者
    /// </summary>
    public interface IConfig
    {
        /// <summary>
        /// 根据键值从配置中取出一项配置
        /// </summary>
        /// <returns></returns>
        TConfig Get<TConfig>() where TConfig : class ,new();

        /// <summary>
        /// 实体保存到配置文件里
        /// </summary>
        /// <param name="config"></param>
        void Save<TConfig>(TConfig config) where TConfig : class ,new();
    }
}