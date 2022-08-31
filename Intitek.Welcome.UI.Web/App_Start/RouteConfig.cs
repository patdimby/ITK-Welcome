using System.Web.Mvc;
using System.Web.Routing;

namespace Intitek.Welcome.UI.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Login",
                url: "Login/{action}",
                defaults: new { controller = "Login", action = "Login"},
                namespaces: new[] { "Intitek.Welcome.UI.Web.Controllers" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Intitek.Welcome.UI.Web.Controllers" }
            );

            
        }
    }
}
