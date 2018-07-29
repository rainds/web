using System.Web.Mvc;
using System.Web.Routing;

namespace Rainds.ApiSite
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
              name: "Default",
              url: "{controller}/{action}/{id}",
              defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
          );

            routes.MapRoute("Image", "Image/{action}", new { controller = "Image", action = "Index" });
            routes.MapRoute("Member", "Member/{action}", new { controller = "Member", });
        }
    }
}
