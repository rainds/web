using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FrameWork.CacheService;
using FrameWork.ConfigService;
using FrameWork.Core.Data;
using FrameWork.Core.Data.Migrations;
using FrameWork.DataService;
using FrameWork.IocService;
using Rainds.Service;

namespace Rainds.ApiSite
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //初始化实体
            ServiceInitializer.RegisterEntities();
            DatabaseMigrator<EfDbContext, EfConfiguration>.MigrateDatabase();
            RegisterService();
        }

        public void RegisterService()
        {
            new AutofacInitializer().Init();
            new EfInitializer().Init();
            new ConfigInitializer().Init();
            new CacheInitializer().Init();

            new ServiceInitializer().Init();
        }
    }
}