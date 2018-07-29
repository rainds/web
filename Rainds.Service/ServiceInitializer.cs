using System.Linq;
using System.Reflection;
using System.Web.Compilation;
using FrameWork.Core;
using FrameWork.Core.Data;
using FrameWork.Core.Ioc;
using FrameWork.Core.Principal;
using Rainds.Service.User;

namespace Rainds.Service
{
    /// <summary>
    /// 服务初始化
    /// </summary>
    public class ServiceInitializer : BaseInitializer
    {
        protected override void OnInit()
        {
            //初始化实体
            Locator.Get<IDbFactory>().RegisterEntities(Assembly.GetExecutingAssembly());

            //初始化服务
            var register = Locator.Get<IRegister>();
            //获取所有程序集
            var assemblies = BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToArray();
            var baseType = typeof(Rainds.IService.IService);
            register.Set(assemblies, new[] { baseType }, LifeTime.Transient);

            // register.Set<UserService, IUserService>(LifeTime.Transient);

            register.Set<UserService, IPrincipal>(LifeTime.LifetimeScope);

            Locator.Container.Update(register);
        }
    }
}