using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using FrameWork.Core.Plugin;

namespace FrameWork.Core
{
    /// <summary>
    /// 框架内核启动引擎
    /// </summary>
    public static class Engine
    {
        /// <summary>
        /// 私有程序集目录
        /// </summary>
        public static string PrivatePath { get; private set; }

        /// <summary>
        /// 配置当前程序域的私有程序集目录
        /// </summary>
        private static void SetPrivatePath()
        {
            var tempPath = "AssemblyShadow";

            PrivatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, tempPath);
            if (Directory.Exists(PrivatePath))
            {
                var files = Directory.GetFiles(PrivatePath);
                foreach (var file in files)
                {
                    File.Delete(file);
                }
            }
            else
            {
                Directory.CreateDirectory(PrivatePath);
            }

            var path = AppDomain.CurrentDomain.GetData("PRIVATE_BINPATH");
            if (path != null)
            {
                tempPath = PrivatePath + ";" + path;
            }

            AppDomain.CurrentDomain.SetData("PRIVATE_BINPATH", tempPath);
            AppDomain.CurrentDomain.SetData("BINPATH_PROBE_ONLY", tempPath);

            var m = typeof (AppDomainSetup).GetMethod("UpdateContextProperty",
                BindingFlags.NonPublic | BindingFlags.Static);

            var funsion = typeof (AppDomain).GetMethod("GetFusionContext",
                BindingFlags.NonPublic | BindingFlags.Instance);

            m.Invoke(null, new[]
            {
                funsion.Invoke(AppDomain.CurrentDomain, null), "PRIVATE_BINPATH", tempPath
            });
        }

        private static bool isInit;

        /// <summary>
        /// 通过读取配置文件进行初始化
        /// </summary>
        public static bool Initialize()
        {
            if (isInit) return true;
            Log("框架初始化开始");

            //捕获未处理的异常
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            //捕获解析失败的程序集
            AppDomain.CurrentDomain.TypeResolve += CurrentDomain_TypeResolve;

            //配置私有程序集目录
            SetPrivatePath();

            //初始插件
            if (!LoadPlugins()) return false;

            isInit = true;
            Log("框架初始化完成");
            return true;
        }

        private static bool LoadPlugins()
        {
            var typeName = ConfigurationManager.AppSettings["FrameWork.PluginLoader"];

            if (string.IsNullOrWhiteSpace(typeName))
                return new DefaultPluginLoader().Load();

            var type = Type.GetType(typeName, false, true);
            if (type == null)
            {
                Log("无法找到对象类型：" + typeName);
                return false;
            }

            var loader = Activator.CreateInstance(type) as IPluginLoader;
            if (loader != null) return loader.Load();

            Log(typeName + "没有实现IPluginLoader接口");
            return false;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            FrameWorkLogger.Log((Exception) e.ExceptionObject, "UnhandledException");
        }

        private static Assembly CurrentDomain_TypeResolve(object sender, ResolveEventArgs args)
        {
            Log("无法解析的程序集：" + args.Name);
            return null;
        }

        /// <summary>
        /// 记录引擎普通日志
        /// </summary>
        internal static void Log(string msg)
        {
            FrameWorkLogger.Log(msg, "Engine");
        }

        /// <summary>
        /// 记录引擎错误日志
        /// </summary>
        internal static void Log(Exception ex)
        {
            FrameWorkLogger.Log(ex, "Engine");
        }
    }
}
