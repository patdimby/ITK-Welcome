using System.Web.Mvc;

namespace Intitek.Welcome.UI.Web.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Edit",
                "Admin/{controller}/{id}",
                new { action = "Edit"},
                constraints: new { id = @"\d+" },
                namespaces: new[] { "Intitek.Welcome.UI.Web.Areas.Admin.Controllers" }
            );

            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}",
                new { action = "Index"},
                namespaces: new[] { "Intitek.Welcome.UI.Web.Areas.Admin.Controllers" }
            );
        }
    }
}