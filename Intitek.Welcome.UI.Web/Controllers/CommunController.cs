using Intitek.Welcome.UI.ViewModels;
using Intitek.Welcome.Infrastructure.Helpers;
using Newtonsoft.Json;
using System;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Intitek.Welcome.Service.Front;
using Intitek.Welcome.Service.Back;
using Intitek.Welcome.Infrastructure.Log;

namespace Intitek.Welcome.UI.Web.Controllers
{

    public class CommunController : Controller
    {
       
        private ADService _adService;
        private Service.Front.LangService _langService;

        
        public Service.Front.ActiveDirectoryService GetActiveDirectoryService(int id)
        {
            _adService = new ADService(new FileLogger());

            var ad = _adService.Get(new GetADRequest()
            {
                Id = id,
                PwdIV = ConfigurationManager.AppSettings["ivPwd"],
                PwdKey = ConfigurationManager.AppSettings["keyPwd"]
        });

            return new Service.Front.ActiveDirectoryService(new FileLogger(), ad.AD.Address, ad.AD.Domain, ad.AD.Username, ad.AD.Password);
        }

        public ActionResult Unauthorize()
        {
            return View("~/Views/shared/_Unauthorize.cshtml");
        }
        public UserViewModel GetUserConnected()
        {
            var claimIdentity = this.User.Identity as ClaimsIdentity;
            var userDataConnected = JsonConvert.DeserializeObject<UserViewModel>(claimIdentity.FindFirst(ClaimTypes.UserData).Value);
            return userDataConnected;
        }

        public bool IsReader()
        {
            var claimIdentity = this.User.Identity as ClaimsIdentity;
            var userDataConnected = JsonConvert.DeserializeObject<UserViewModel>(claimIdentity.FindFirst(ClaimTypes.UserData).Value);
            return userDataConnected.isReader;
        }
        public int GetUserIdConnected()
        {
            var claimIdentity = this.User.Identity as ClaimsIdentity;
            var userDataConnected = JsonConvert.DeserializeObject<UserViewModel>(claimIdentity.FindFirst(ClaimTypes.UserData).Value);
            return userDataConnected.ID;
        }

        public string GetUserNomPrenomConnected()
        {
            var claimIdentity = this.User.Identity as ClaimsIdentity;
            var userDataConnected = JsonConvert.DeserializeObject<UserViewModel>(claimIdentity.FindFirst(ClaimTypes.UserData).Value);
            return userDataConnected.NomPrenom;
        }
        protected String GetAppSettings(string name)
        {
            return ConfigurationManager.AppSettings[name];
        }
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {

           
                string cultureName = null;
                _langService = new Service.Front.LangService(new FileLogger());
                var langues = _langService.GetAll(new Service.Front.GetAllLangRequest()).Langues;
                // Attempt to read the culture cookie from Request
                HttpCookie cultureCookie = Request.Cookies["_culture"];
                if (cultureCookie != null)
                {
                    cultureName = cultureCookie.Value;
                }
                else
                {
                    cultureName = Request.UserLanguages != null && Request.UserLanguages.Length > 0 ?
                            Request.UserLanguages[0] :  // obtain it from HTTP header AcceptLanguages
                            null;
                }
                // Validate culture name
                cultureName = CultureHelper.GetImplementedCulture(cultureName); // This is safe

                // Modify current thread's cultures            
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
                ViewBag.CulturName = cultureName;
                ViewBag.Langues = langues;
                return base.BeginExecuteCore(callback, state);
        }
        protected GetUserDocumentRequest GetGridRequestSession(GetUserDocumentRequest gridRequest, string action = null, string controller = null)
        {
            if (string.IsNullOrEmpty(action))
            {
                action = this.ControllerContext.RouteData.Values["action"].ToString();
            }
            if (string.IsNullOrEmpty(controller))
            {
                controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            }
            string sessionName = string.Format("FO_{0}_{1}_{2}", gridRequest.GridName, controller, action);
            if (Session[sessionName] != null)
            {
                return (GetUserDocumentRequest)Session[sessionName];
            }
            this.SetGridRequestSession(gridRequest, action, controller);
            return gridRequest;
        }
        protected void SetGridRequestSession(GetUserDocumentRequest gridRequest, string action = null, string controller = null)
        {
            if (string.IsNullOrEmpty(action))
            {
                action = this.ControllerContext.RouteData.Values["action"].ToString();
            }
            if (string.IsNullOrEmpty(controller))
            {
                controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            }
            string sessionName = string.Format("FO_{0}_{1}_{2}", gridRequest.GridName, controller, action);
            Session[sessionName] = gridRequest;
        }
        public string GetCulture()
        {
            var _culture = Request.Cookies["_culture"] != null ? Request.Cookies["_culture"].Value : (Request.UserLanguages != null && Request.UserLanguages.Length > 0 ?
                       Request.UserLanguages[0] : "fr-FR");

            return _culture;

        }
        /// <summary>
        /// Retourne le code langue : fr or en
        /// </summary>
        /// <returns></returns>
        public string GetCodeLang()
        {
            var _culture = GetCulture().Substring(0, 2);
            return _culture;

        }
        public int GetIdLang()
        {
            var _culture = GetCulture();
            int ID_Lang = _culture.Equals("fr-FR") ? 1 : 2;
            return ID_Lang;
        }
        public int GetDefaultLang()
        {
            int ID_Lang = this.GetAppSettings("DefaultLangue") != null ? Int32.Parse(this.GetAppSettings("DefaultLangue")) : 0;
            return ID_Lang;
        }

        /// <summary>
        /// PR : overriding OnActionExecuting
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controllerName = filterContext.RouteData.Values["controller"];
            var actionName = filterContext.RouteData.Values["action"];
            var link = string.IsNullOrWhiteSpace(actionName.ToString()) ? controllerName.ToString() : $"{controllerName}/{actionName}";

            if (!link.Equals("Commun/BrowserNotSupported") && Utilities.IsBrowserNotSupported())
            {
                filterContext.Result = new RedirectResult("/Commun/BrowserNotSupported");
                return;
            }
        }

        /// <summary>
        /// PR : Redirect to not supported
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult BrowserNotSupported()
        {
            return View("~/Views/shared/_NotSupported.cshtml");
        }
    }
}