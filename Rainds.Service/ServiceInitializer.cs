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
            //RegisterEntities();

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

        public static void RegisterEntities()
        {
            var baseType = typeof(IEntity);
            foreach (var entityType in Assembly.GetExecutingAssembly().GetTypes().Where(m => baseType.IsAssignableFrom(m) && !m.IsAbstract))
            {
                BaseDbContext.RegisterEntity(entityType);
            }
        }
    }
}