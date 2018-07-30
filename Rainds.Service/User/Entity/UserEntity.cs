using System;
using System.ComponentModel.DataAnnotations;
using FrameWork.Core.Data;

namespace Rainds.Service.User
{
    /// <summary>
    /// 验证类型
    /// </summary>
    [Flags]
    public enum ValidationType
    {
        //无
        None = 0,

        //手机验证
        Moblie = 1,

        //邮箱验证
        Email = 2,

        //公司验证
        Company = 4
    }

    public class UserEntity : IEntity
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        [Key]
        public int UserId
        {
            get;
            set;
        }

        /// <summary>
        /// 用户名称
        /// </summary>
        [Required]
        public string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// 用户手机
        /// </summary>
        [Required]
        public string Mobile
        {
            get;
            set;
        }

        /// <summary>
        /// 用户邮箱
        /// </summary>
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email
        {
            get;
            set;
        }

        /// <summary>
        /// 用户登录密码
        /// </summary>
        [DataType(DataType.Password)]
        public string Password
        {
            get;
            set;
        }

        /// <summary>
        /// 验证类型
        /// </summary>
        [EnumDataType(typeof(ValidationType))]
        public ValidationType ValidationType
        {
            get;
            set;
        }

        /// <summary>
        /// 注册时间
        /// </summary>
        [DataType(DataType.DateTime)]
        public DateTime RegisterTime
        {
            get;
            set;
        }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        [DataType(DataType.DateTime)]
        public DateTime LastLoginTime
        {
            get;
            set;
        }
    }
}