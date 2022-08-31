using Intitek.Welcome.Infrastructure.Helpers;
using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Service.Back;
using Intitek.Welcome.UI.ViewModels;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Intitek.Welcome.UI.Web.Areas.Admin.Controllers
{


    [Authorize(Roles= "BACKOFFICE")]
    public class CommunController : Controller
    {
        private LangService _langService;
        private ADService _adService;
       

        public ActiveDirectoryService GetActiveDirectoryService(int id)
        {
            _adService = new ADService(new FileLogger());           
           
            var ad = _adService.Get(new GetADRequest()
            {
                Id = id,
                //PwdIV = "qf6bYB7dJxer+CQjoVhAdQ==", //ConfigurationManager.AppSettings["ivPwd"],
                PwdIV = ConfigurationManager.AppSettings["ivPwd"],
                //PwdKey = "x2pV8E8NK5Y85oHVqM0B2agPDDX9e1mJk0bJO75Hr+M=" //ConfigurationManager.AppSettings["keyPwd"]
                PwdKey = ConfigurationManager.AppSettings["keyPwd"]
            });

            return new ActiveDirectoryService(new FileLogger(), ad.AD.Address, ad.AD.Domain, ad.AD.Username, ad.AD.Password);
        }

        private string cultureName = string.Empty;
        public UserViewModel GetUserConnected()
        {
            var claimIdentity = this.User.Identity as ClaimsIdentity;
            var userDataConnected = JsonConvert.DeserializeObject<UserViewModel>(claimIdentity.FindFirst(ClaimTypes.UserData).Value);
            return userDataConnected;
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

        public int GetUserConnectedAD()
        {
            var claimIdentity = this.User.Identity as ClaimsIdentity;
            var userDataConnected = JsonConvert.DeserializeObject<UserViewModel>(claimIdentity.FindFirst(ClaimTypes.UserData).Value);
            return userDataConnected.ID_AD;
        }

        protected String GetAppSettings(string name)
        {
            return ConfigurationManager.AppSettings[name];
        }

        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            //string cultureName = null;
            _langService = new LangService(new FileLogger());
            var langues = _langService.GetAll(new GetAllLangRequest()).Langues;
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
        protected GridMvcRequest GetGridRequestSession(GridMvcRequest gridRequest, string action =null, string controller=null)
        {
            if (string.IsNullOrEmpty(action))
            {
                action = this.ControllerContext.RouteData.Values["action"].ToString();
            }
            if (string.IsNullOrEmpty(controller))
            {
                controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            }
            string sessionName = string.Format("{0}_{1}_{2}", gridRequest.GridName, controller, action);
            if (Session[sessionName] != null)
            {
                return (GridMvcRequest)Session[sessionName];
            }
            this.SetGridRequestSession(gridRequest, action, controller);
            return gridRequest;
        }
        protected bool HasSession(GridMvcRequest gridRequest, string action = null, string controller = null)
        {
            if (string.IsNullOrEmpty(action))
            {
                action = this.ControllerContext.RouteData.Values["action"].ToString();
            }
            if (string.IsNullOrEmpty(controller))
            {
                controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            }
            string sessionName = string.Format("{0}_{1}_{2}", gridRequest.GridName, controller, action);
            return Session[sessionName] !=null;
        }
        protected void SetGridRequestSession(GridMvcRequest gridRequest, string action = null, string controller = null)
        {
            if (string.IsNullOrEmpty(action))
            {
                action = this.ControllerContext.RouteData.Values["action"].ToString();
            }
            if (string.IsNullOrEmpty(controller))
            {
                controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            }
            string sessionName = string.Format("{0}_{1}_{2}", gridRequest.GridName, controller, action);
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
    }
}