using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Intitek.Welcome.UI.Web.Filters;
using Intitek.Welcome.UI.ViewModels;
using Intitek.Welcome.Infrastructure.Helpers;

namespace Intitek.Welcome.UI.Web.Controllers
{
    [AllowAnonymous]
    public class ErrorCodeController : Controller
    {
        // GET: ErrorCode
       
        public ActionResult InternalServerError()
        {
            if (Request.UrlReferrer != null)
            {
                var values = RouteDataContextHelper.RouteValuesFromUri(Request.UrlReferrer);

                var controllerName = values["controller"];
                var actionName = values["action"];
                var id = values["id"];
                var qs = values["qs"];
                return View(new ErrorCodeViewModel()
                {
                    ControllerName = controllerName.ToString(),
                    ActionName = actionName.ToString(),
                    Id = id != null ? id.ToString() : null,
                    QueryString = qs.ToString()

                });
            }
            return View();                     
       }
    }
}