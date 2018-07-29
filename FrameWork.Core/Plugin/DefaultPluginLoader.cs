using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Xml.Linq;

namespace FrameWork.Core.Plugin
{
    /// <summary>
    /// 默认的插件加载者
    /// </summary>
    internal class DefaultPluginLoader : IPluginLoader
    {
        /// <summary>
        /// 加载插件
        /// </summary>
        public bool Load()
        {
            //加载框架相关插件
            if (!LoadPlugins(GetPath("FrameWork"))) return false;

            //加载子目录中的业务相关插件
            var path = GetPath("Plugins");
            return !Directory.Exists(path) || Directory.GetDirectories(GetPath("Plugins")).All(LoadPlugins);
        }

        /// <summary>
        /// 获得指定模块的插件目录
        /// </summary>
        private static string GetPath(string modelName)
        {
            //在配置文件中指定
            var path = ConfigurationManager.AppSettings[modelName + ".Path"];
            if (string.IsNullOrWhiteSpace(path))
            {
                //查找跟模块同名的目录
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, modelName);
            }
            return path;
        }

        private static bool LoadPlugins(string path)
        {
            try
            {
                if (!Directory.Exists(path)) return true;

                var infoFile = Path.Combine(path, "PluginInfo.xml");
                if (!File.Exists(infoFile)) return true;

                var xdoc = XDocument.Load(infoFile);
                if (xdoc.Root == null) return true;

                Engine.Log("初始化插件目录：" + path);

                var plugins = from item in xdoc.Root.Elements()
                              select new PluginInfo
                              {
                                  PluginName = item.Attribute("name").Value,
                                  Description = item.Attribute("desc").Value,
                                  FileInfo = CreateFileInfo(path, item.Attribute("file").Value),
                                  Initializer = item.Attribute("init").Value,
                                  Dependencies = (from subItem in item.Elements()
                                                  select CreateFileInfo(path, subItem.Attribute("file").Value))
                              };

                foreach (var plugin in plugins)
                {
                    foreach (var dependency in plugin.Dependencies)
                    {
                        dependency.AsemblyVersion = PluginManager.LoadAssembly(dependency.FileName,
                            File.ReadAllBytes(Path.Combine(path, dependency.FileName)));
                    }

                    var fileInfo = plugin.FileInfo;
                    fileInfo.AsemblyVersion = PluginManager.LoadAssembly(fileInfo.FileName,
                        File.ReadAllBytes(Path.Combine(path, fileInfo.FileName)));

                    PluginManager.AddPlugin(plugin);
                }
                return true;
            }
            catch (Exception exception)
            {
                Engine.Log(exception);
                return false;
            }
        }

        private static PluginFileInfo CreateFileInfo(string path, string filename)
        {
            var writeTime = new FileInfo(Path.Combine(path, filename)).LastWriteTime;
            return new PluginFileInfo(filename, writeTime);
        }
    }
}