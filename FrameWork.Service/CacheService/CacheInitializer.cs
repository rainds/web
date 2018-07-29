using FrameWork.Core;
using FrameWork.Core.Cache;
using FrameWork.Core.Ioc;

namespace FrameWork.CacheService
{
    public class CacheInitializer : BaseInitializer
    {
        protected override void OnInit()
        {
            var register = Locator.Get<IRegister>();
            register.Set<ContextCache, ICache>(LifeTime.LifetimeScope, CacheLocation.Context);
            register.Set<LocalCache, ICache>(LifeTime.Singleton, CacheLocation.Local);
            register.Set<RemoteCache, ICache>(LifeTime.Singleton, CacheLocation.Remote);

            Locator.Container.Update(register);
        }
    }
}