using System;
using System.Collections.Generic;
using FrameWork.Core.Data;
using FrameWork.Core.Data.Migrations;
using Rainds.Service.User;

namespace Rainds.ApiSite
{
    /// <summary>
    /// 日志数据库迁移配置类。
    /// </summary>
    public sealed class EfConfiguration : GenericConfiguration<EfDbContext>
    {
        protected override void OnSeed(EfDbContext context)
        {
            context.Set<UserEntity>().AddRange(new List<UserEntity> {
                new UserEntity{ UserName="123",Mobile="1231321",ValidationType=ValidationType.None,Password="34242432", Email="213123@qq.com",RegisterTime=DateTime.Now,LastLoginTime=DateTime.Now},
                 new UserEntity{ UserName="222",Mobile="222",ValidationType=ValidationType.None,Password="34242432", Email="213123@qq.com",RegisterTime=DateTime.Now,LastLoginTime=DateTime.Now},
                  new UserEntity{ UserName="333",Mobile="333",ValidationType=ValidationType.None,Password="34242432", Email="213123@qq.com",RegisterTime=DateTime.Now,LastLoginTime=DateTime.Now},
                   new UserEntity{ UserName="444",Mobile="444",ValidationType=ValidationType.None,Password="34242432", Email="213123@qq.com",RegisterTime=DateTime.Now,LastLoginTime=DateTime.Now},
                    new UserEntity{ UserName="555",Mobile="555",ValidationType=ValidationType.None,Password="34242432", Email="213123@qq.com",RegisterTime=DateTime.Now,LastLoginTime=DateTime.Now},
            });
            context.SaveChanges();
            base.OnSeed(context);
        }
    }
}