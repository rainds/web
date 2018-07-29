using FrameWork.Core;

namespace FrameWork.IocService
{
    public class AutofacInitializer : BaseInitializer
    {
        public override int Order { get => -1; }

        protected override void OnInit()
        {
            Locator.Container = new AutofacContainer();
        }
    }
}