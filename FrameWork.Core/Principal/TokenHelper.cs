using System;
using System.Web;
using System.Web.Security;
using Newtonsoft.Json;
using FrameWork.Core.Utility.Common;
using FrameWork.Core.Utility.Security;

namespace FrameWork.Core.Principal
{
    /// <summary>
    /// 令牌助手
    /// </summary>
    public class TokenHelper
    {
        private static readonly string TokenKey = FormsAuthentication.FormsCookieName;

        #region 写

        public static void ClearToken(HttpResponseBase response)
        {
            CookieHelper.SetCookie(TokenKey, "", response);
        }

        public static void SetToken(int id, string name, IdentityType identityType, HttpResponseBase response)
        {
            //生成一个60分钟内有效的令牌
            var token = CreateToken(id, name, identityType);

            CookieHelper.SetCookie(TokenKey, token, response);
        }

        public static string CreateToken(int id, string name, IdentityType identityType)
        {
            //生成一个60分钟内有效的令牌
            var info = new TokenInfo
            {
                IdentityId = id,
                IdentityName = name,
                ExpirationTime = DateTime.Now.AddMinutes(60),
                IdentityType = identityType
            };

            //将令牌序列化成字符串并加密
            return RijndaelHelper.Encrypt(JsonConvert.SerializeObject(info));
        }

        #endregion 写

        #region 读

        public static string GetToken(HttpRequestBase request)
        {
            return CookieHelper.GetCookie(TokenKey, request);
        }

        #endregion 读
    }
}