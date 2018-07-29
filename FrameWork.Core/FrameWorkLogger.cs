using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace FrameWork.Core
{
    /// <summary>
    /// 记录框架日志
    /// </summary>
    public static class FrameWorkLogger
    {
        private static readonly object LockObj = new object();

        private static void WriteLog(string msg, string logtype = null)
        {
            var logMsg = string.Format("{0}：{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), msg);
            lock (LockObj)
            {
                var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FrameWork_Log");
                if (!Directory.Exists(logPath)) Directory.CreateDirectory(logPath);

                var logFile = Path.Combine(logPath,
                    DateTime.Now.ToString("yyyyMMddHH") + "_" +
                    (logtype ?? AppDomain.CurrentDomain.Id.ToString(CultureInfo.InvariantCulture)) + ".log");

                //写入日志
                using (var writer = new StreamWriter(logFile, true, Encoding.Default))
                {
                    writer.WriteLine(logMsg);
                    writer.Close();
                }
            }
        }

        /// <summary>
        /// 记录框架的调试信息
        /// </summary>
        /// <param name="msg">调试信息字符串</param>
        public static void Log(string msg)
        {
            Log(msg, null);
        }

        /// <summary>
        /// 记录框架的调试信息
        /// </summary>
        /// <param name="msg">调试信息字符串</param>
        /// <param name="logtype">日志类型</param>
        public static void Log(string msg, string logtype)
        {
            if (string.IsNullOrEmpty(msg)) return;
            WriteLog(msg, logtype);
        }

        /// <summary>
        /// 记录框架的异常信息
        /// </summary>
        /// <param name="ex">异常对象</param>
        public static void Log(Exception ex)
        {
            Log(ex, "Exception");
        }

        /// <summary>
        /// 记录框架的异常信息
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <param name="logtype">日志类型</param>
        public static void Log(Exception ex, string logtype)
        {
            if (ex == null) return;
            WriteLog(GetErrorMsg(ex), logtype);
        }

        /// <summary>
        ///  获得异常的详细信息
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetErrorMsg(Exception ex)
        {
            var builder = new StringBuilder(4096);
            builder.AppendFormat("发生［{0}］异常，异常相关信息如下：", ex.GetType());
            builder.AppendFormat("\r\n异常描述：{0}", ex.Message);
            builder.AppendFormat("\r\n异 常 源：{0}", ex.Source);
            builder.AppendFormat("\r\n堆栈跟踪：\r\n{0}", ex.StackTrace);
            if (ex.InnerException != null)
            {
                builder.AppendFormat("\r\n内含异常：{0}", ex.InnerException.GetType());
                builder.AppendFormat("\r\n异常描述：{0}", ex.InnerException.Message);
                builder.AppendFormat("\r\n异 常 源：{0}", ex.InnerException.Source);
                builder.AppendFormat("\r\n堆栈跟踪：\r\n{0}\r\n", ex.InnerException.StackTrace);
            }
            else
            {
                builder.Append("\r\n内含异常：无\r\n");
            }

            return builder.ToString();
        }
    }
}