using System.Data.Entity.Infrastructure.Interception;
using FrameWork.Core;
using FrameWork.Core.Data;
using FrameWork.Core.Ioc;

namespace FrameWork.DataService
{
    public class EfInitializer : BaseInitializer
    {
        protected override void OnInit()
        {
            var register = Locator.Get<IRegister>();
            register.Set<EfDbFactory, IDbFactory>(LifeTime.LifetimeScope);
            Locator.Container.Update(register);

            // Database.SetInitializer<EfDbContext>(null);

#if DEBUG
            DbInterception.Add(new EfIntercepterLogging());
#endif
        }
    }
}