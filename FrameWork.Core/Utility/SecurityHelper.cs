using System;
using System.Security.Cryptography;
using System.Text;

namespace FrameWork.Core.Utility.Security
{
    /// <summary>
    /// 安全助手类。
    /// </summary>
    public static class SecurityHelper
    {
        /// <summary>
        /// 获得字符串的MD5码
        /// </summary>
        public static string GetMd5(this string data)
        {
            var result = Encoding.Default.GetBytes(data);
            var md5 = new MD5CryptoServiceProvider();
            var output = md5.ComputeHash(result);
            return BitConverter.ToString(output).Replace("-", "");
        }
    }
}