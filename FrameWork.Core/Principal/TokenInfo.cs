using System;

namespace FrameWork.Core.Principal
{
    /// <summary>
    /// 令牌信息
    /// </summary>
    public class TokenInfo
    {
        /// <summary>
        /// 身份Id
        /// </summary>
        public int IdentityId { get; set; }

        /// <summary>
        /// 身份名称
        /// </summary>
        public string IdentityName { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExpirationTime { get; set; }

        /// <summary>
        /// 身份类型
        /// </summary>
        public IdentityType IdentityType { get; set; }
    }

    /// <summary>
    /// 身份类型
    /// </summary>
    public enum IdentityType
    {
        /// <summary>
        /// 所有
        /// </summary>
        All = 9,

        /// <summary>
        /// 会员
        /// </summary>
        Member = 0,

        /// <summary>
        /// 用户
        /// </summary>
        User = 1
    }
}