using System;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using Newtonsoft.Json;
using FrameWork.Core.Cache;
using FrameWork.Core.Ioc;
using FrameWork.Core.Mvc.Result;
using FrameWork.Core.Principal;
using FrameWork.Core.Utility.Common;
using FrameWork.Core.Utility.Security;
using JsonResult = FrameWork.Core.Mvc.Result.JsonResult;

namespace FrameWork.Core.Mvc.Filter
{
    /// <summary>
    /// 身份验证过滤器。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class AuthenticationAttribute : FilterAttribute, IAuthenticationFilter
    {
        private readonly IdentityType identityType;

        public AuthenticationAttribute()
        {
            this.identityType = IdentityType.All;
        }

        public AuthenticationAttribute(IdentityType identityType)
        {
            this.identityType = identityType;
        }

        /// <summary>
        /// 认证。
        /// </summary>
        /// <param name="filterContext">认证上下文实体对象。</param>
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            try
            {
                var cipherText = TokenHelper.GetToken(filterContext.HttpContext.Request);
                if (string.IsNullOrWhiteSpace(cipherText))
                {
                    throw new Exception("令牌信息为空");
                }

                //验证令牌
                var decrypt = RijndaelHelper.Decrypt(cipherText);
                if (string.IsNullOrWhiteSpace(decrypt))
                {
                    throw new Exception("解密后的令牌为空");
                }

                var tokenInfo = JsonConvert.DeserializeObject<TokenInfo>(decrypt);
                if (tokenInfo.ExpirationTime < DateTime.Now)
                {
                    throw new Exception("令牌已经过期");
                }

                if (this.identityType != IdentityType.All && tokenInfo.IdentityType != this.identityType)
                {
                    throw new Exception("令牌身份类型不正确");
                }

                //当前请求上下文生命周期内注入会员信息
                Locator.Get<IPrincipal>().TokenInfo = tokenInfo;

                //每过10分钟重新颁发新的令牌
                if ((tokenInfo.ExpirationTime - DateTime.Now).Minutes < 10)
                {
                    TokenHelper.SetToken(tokenInfo.IdentityId, tokenInfo.IdentityName, tokenInfo.IdentityType,
                        filterContext.HttpContext.Response);
                }
            }
            catch (Exception ex)
            {
                filterContext.Result = new JsonResult(new ResponseMessage(ResponseCode.Unauthorized, ex.Message));
            }
        }

        /// <summary>
        /// 认证通道。
        /// </summary>
        /// <param name="filterContext">认证通道上下文实体对象。</param>
        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
        }
    }
}