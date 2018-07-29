using System;
using System.Collections.Generic;

namespace FrameWork.Core.Plugin
{
    /// <summary>
    /// 插件文件信息
    /// </summary>
    public class PluginFileInfo
    {
        /// <summary>
        /// 插件文件信息
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <param name="writeTime">修改时间</param>
        public PluginFileInfo(string fileName, DateTime writeTime)
        {
            FileName = fileName;
            LastWriteTime = writeTime;
        }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime LastWriteTime { get; private set; }

        /// <summary>
        /// 程序集版本
        /// </summary>
        public string AsemblyVersion { get; set; }
    }

    /// <summary>
    /// 插件信息
    /// </summary>
    public class PluginInfo
    {
        /// <summary>
        /// 插件名称
        /// </summary>
        public string PluginName { get; set; }

        /// <summary>
        /// 插件描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 初始化提供者
        /// </summary>
        public string Initializer { get; set; }

        /// <summary>
        /// 文件信息
        /// </summary>
        public PluginFileInfo FileInfo { get; set; }

        /// <summary>
        /// 依赖的程序集
        /// </summary>
        public IEnumerable<PluginFileInfo> Dependencies { get; set; }
    }
}
