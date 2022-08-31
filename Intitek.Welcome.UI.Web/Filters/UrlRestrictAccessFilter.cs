using Intitek.Welcome.UI.Web.Controllers;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Intitek.Welcome.UI.Web.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class UrlRestrictAccessFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.UrlReferrer == null ||
                    filterContext.HttpContext.Request.Url.Host != filterContext.HttpContext.Request.UrlReferrer.Host)
            {
                //Eviter la Boucle infinie sur l'action Index/HomeController vers Index/UserDocument pour l'utilisateur Front
                if (!HomeController.USER_FO_CONNECTED.Equals(filterContext.Controller.TempData["UserFO"])){
                    if (filterContext.ActionDescriptor.ActionName!="ShowFromLink") {
                        filterContext.Result = new RedirectToRouteResult(new
                                                RouteValueDictionary(new { controller = "Home", action = "Index", area = "" }));
                    }
                }
                
            }
        }
    }
}