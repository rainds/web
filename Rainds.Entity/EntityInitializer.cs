using FrameWork.Core;
using FrameWork.Core.Data;
using System.Reflection;

namespace Rainds.Entity
{
    /// <summary>
    /// 实体初始化对应的表结构
    /// </summary>
    public class EntityInitializer : BaseInitializer
    {
        protected override void OnInit()
        {
            Locator.Get<IDbFactory>().RegisterEntities(Assembly.GetExecutingAssembly());
        }
    }
}