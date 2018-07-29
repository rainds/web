using System;
using System.Web;

namespace FrameWork.Core.Utility.Common
{
    /// <summary>
    /// Cookie助手
    /// </summary>
    public static class CookieHelper
    {
        /// <summary>
        /// 设置Cookie
        /// </summary>
        public static void SetCookie(string name, string value, HttpResponseBase response)
        {
            var cookie = string.IsNullOrWhiteSpace(value)
                ? new HttpCookie(name) { Expires = DateTime.Now.AddDays(-1d) }
                : new HttpCookie(name, value) { Expires = DateTime.Now.AddDays(1d) };

            response.SetCookie(cookie);
        }

        /// <summary>
        /// 清除Cookie
        /// </summary>
        public static void ClearCookie(string name, HttpResponseBase response)
        {
            SetCookie(name, "", response);
        }

        /// <summary>
        /// 获取Cookie
        /// </summary>
        public static string GetCookie(string name, HttpRequestBase request)
        {
            var cookie = request.Cookies[name];
            return (cookie == null) ? string.Empty : cookie.Value;
        }
    }
}