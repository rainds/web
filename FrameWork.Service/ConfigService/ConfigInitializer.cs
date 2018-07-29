using FrameWork.Core;
using FrameWork.Core.Config;
using FrameWork.Core.Ioc;

namespace FrameWork.ConfigService
{
    public class ConfigInitializer : BaseInitializer
    {
        protected override void OnInit()
        {
            var register = Locator.Get<IRegister>();
            register.Set<Config, IConfig>(LifeTime.Transient);

            Locator.Container.Update(register);
        }
    }
}