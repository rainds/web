using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace FrameWork.Core.Plugin
{
    /// <summary>
    /// 插件管理者
    /// </summary>
    public static class PluginManager
    {
        private static readonly IList<PluginInfo> Plugins = new List<PluginInfo>();

        /// <summary>
        /// 获得所有插件信息
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<PluginInfo> GetPluginInfos()
        {
            return Plugins;
        }

        /// <summary>
        /// 添加插件
        /// </summary>
        /// <param name="pluginInfo">插件信息</param>
        public static void AddPlugin(PluginInfo pluginInfo)
        {
            Engine.Log("初始化插件：" + pluginInfo.PluginName);

            Plugins.Add(pluginInfo);

            //初始化插件
            if (string.IsNullOrWhiteSpace(pluginInfo.Initializer)) return;

            var type = Type.GetType(pluginInfo.Initializer, true, true);
            var initializer = Activator.CreateInstance(type) as BaseInitializer;
            if (initializer == null)
            {
                throw new ArgumentException(pluginInfo.Initializer + "没有继承BaseInitializer");
            }
            initializer.Init();
        }

        /// <summary>
        /// 加载程序集,并返回程序集的版本
        /// </summary>
        public static string LoadAssembly(string fileName, byte[] rawAssembly)
        {
            var filePath = Path.Combine(Engine.PrivatePath, fileName);
            Assembly assembly;

            if (File.Exists(filePath))
            {
                assembly = Assembly.Load(rawAssembly);
            }
            else
            {
                File.WriteAllBytes(filePath, rawAssembly);
                assembly = AppDomain.CurrentDomain.Load(File.ReadAllBytes(filePath));
            }

            return assembly.GetName().Version.ToString();
        }
    }
}
