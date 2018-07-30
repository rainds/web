using System;
using System.Linq;
using FrameWork.Core.Mvc.Paged;
using FrameWork.Core.Mvc.Result;
using FrameWork.Core.Principal;
using FrameWork.Core.Utility.Security;
using Rainds.IService.User;

namespace Rainds.Service.User
{
    public class UserService : IUserService, IPrincipal
    {
        /// <summary>
        /// 登陆
        /// </summary>
        public OperationResult<int> Login(LoginModel model)
        {
            model.Name = model.Name.Trim();
            model.Password = model.Password.Trim();
            if (!LoginHelper.CheckTimes(model.Name))
            {
                return OperationResult<int>.Failure("密码输入错误超过5次后5分钟内禁止登陆");
            }
            using (var provider = DbService.MySqlProvider)
            {
                var repository = provider.Repository<UserEntity>();
                var entity = repository.Find(e => e.UserName == model.Name);
                if (entity == null)
                {
                    return OperationResult<int>.Failure("用户名不存在");
                }

                var passwordMd5 = model.Password.GetMd5();
                if (entity.Password != passwordMd5)
                {
                    LoginHelper.SetTimes(model.Name);
                    return OperationResult<int>.Failure("密码不正确");
                }

                repository.Update(w => w.UserId == entity.UserId, m => new UserEntity { LastLoginTime = DateTime.Now });
                LoginHelper.RemoveTimes(model.Name);
                return new OperationResult<int>(entity.UserId);
            }
        }

        /// <summary>
        /// 会员注册
        /// </summary>
        public OperationResult<int> Register(RegisterModel model)
        {
            if (model == null)
            {
                return OperationResult<int>.Failure("会员信息为空");
            }

            using (var provider = DbService.MySqlProvider)
            {
                var repository = provider.Repository<UserEntity>();

                if (repository.Query.Count(e => e.UserName == model.Name) > 0)
                {
                    return OperationResult<int>.Failure("当前会员已经被注册");
                }

                var entity = new UserEntity
                {
                    UserName = model.Name,
                    Password = SecurityHelper.GetMd5(model.Password)
                };
                repository.Insert(entity);
                return new OperationResult<int>(entity.UserId);
            }
        }

        /// <summary>
        /// 获得会员信息
        /// </summary>
        /// <returns></returns>
        public OperationResult<UserModel> GetUser(int memberId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取会员分页信息列表
        /// </summary>
        public OperationResult<IPagedList<UserModel>> GetPagedList(int pageIndex, int pageSize)
        {
            using (var provider = DbService.MySqlProvider)
            {
                var pagedList = provider.Repository<UserEntity>().Query.Select(m => new UserModel
                {
                    UserId = m.UserId,
                    UserName = m.UserName
                }).ToPagedList(pageIndex, pageSize);

                return new OperationResult<IPagedList<UserModel>>(pagedList);
            }
        }

        public TokenInfo TokenInfo { get; set; }
    }
}