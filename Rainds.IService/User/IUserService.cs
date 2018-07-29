using System.ComponentModel.DataAnnotations;
using FrameWork.Core.Mvc.Paged;
using FrameWork.Core.Mvc.Result;

namespace Rainds.IService.User
{
    /// <summary>
    /// 会员服务
    /// </summary>
    public interface IUserService : IService
    {
        /// <summary>
        /// 登陆
        /// </summary>
        OperationResult<int> Login(LoginModel model);

        /// <summary>
        /// 会员注册
        /// </summary>
        OperationResult<int> Register(RegisterModel model);

        /// <summary>
        /// 会员注册
        /// </summary>
        OperationResult<IPagedList<UserModel>> GetPagedList(int pageIndex, int pageSize);

        /// <summary>
        /// 获得会员信息
        /// </summary>
        /// <returns></returns>
        OperationResult<UserModel> GetUser(int userId);
    }

    public class LoginModel
    {
        /// <summary>
        /// 用户名称
        /// </summary>
        [Required(ErrorMessage = "用户名不能为空")]
        public string Name { get; set; }

        /// <summary>
        /// 用户密码
        /// </summary>
        [Required(ErrorMessage = "用户密码不能为空")]
        public string Password { get; set; }
    }

    /// <summary>
    /// 会员注册实体
    /// </summary>
    public class RegisterModel
    {
        /// <summary>
        /// 用户名称
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 用户密码
        /// </summary>
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// 用户手机
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 用户邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string ValidateCode { get; set; }

        /// <summary>
        /// 是否同意用户协议
        /// </summary>
        public bool IsCheck { get; set; }
    }

    /// <summary>
    /// 用户实体
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// 用户密码
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}