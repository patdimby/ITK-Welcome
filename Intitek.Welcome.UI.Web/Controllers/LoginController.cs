using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Service.Back;
using Intitek.Welcome.UI.Resources;
using Intitek.Welcome.UI.ViewModels;
using Intitek.Welcome.UI.Web.Security;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Intitek.Welcome.UI.Web.Controllers
{
    public class LoginController : CommunController
    {
        const int DEFAULT_EXIPRES_REMEMBER_ME = 24;
        private readonly Service.Front.IUserService _userService;
        private readonly Service.Back.IConfigService _configService;
        private Service.Front.ActiveDirectoryService _activeDirectoryService;
        private readonly ADService _adService;
        private readonly string _ivPwd;
        private readonly string _keyPwd;
        private FileLogger _logger;
        public LoginController()
        {
            this._logger = new FileLogger();
            _userService = new Service.Front.UserService(new FileLogger());
            _configService = new Service.Back.ConfigService(new FileLogger());
           _adService = new ADService(new FileLogger());
            //_ivPwd = "qf6bYB7dJxer+CQjoVhAdQ=="; //ConfigurationManager.AppSettings["ivPwd"];
            _ivPwd = ConfigurationManager.AppSettings["ivPwd"];
            //_keyPwd = "x2pV8E8NK5Y85oHVqM0B2agPDDX9e1mJk0bJO75Hr+M="; //ConfigurationManager.AppSettings["keyPwd"];
            _keyPwd = ConfigurationManager.AppSettings["keyPwd"];
        }

        private UserViewModel UserViewModel { get; set; }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            LoginViewModel vmodel = new LoginViewModel();

            TempData["readOnly"] = null;  TempData["id"] = null;

            Service.Back.GetConfigRequest configRequest = new Service.Back.GetConfigRequest()
            {
                ConfigType = Service.Back.ConfigType.MAINTENANCE
            };
            string maintenanceMode = _configService.Get(configRequest).Config.Value;

            vmodel.RememberMe = true;
            vmodel.MaintenanceMode = maintenanceMode;
            return View(vmodel);
        }

        /// <summary>
        /// Après avoir saisie une URL normale, "login/login?ReturnUrl=%2F" se rajouter systématiquement
        /// Pour résourdre ce problème, on ajoute loginUrl=LoginRedirect dans Web.config 
        /// <system.web>...
        ///<authentication mode="Forms">
        ///<forms loginUrl = "~/Login/LoginRedirect" />
        ///</authentication >
        ///</system.web >
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult LoginRedirect()
        {
            return RedirectToAction("Login");
        }
    
        [HttpPost]
        [ValidateInput(true)]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {

            int expires = this.GetAppSettings("ExpiresRememberMeHours") != null ? Int32.Parse(this.GetAppSettings("ExpiresRememberMeHours")) : DEFAULT_EXIPRES_REMEMBER_ME;
            ViewBag.ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
            {
                var errors = new List<string>();
                foreach (var modelStateVal in ViewData.ModelState.Values)
                {
                    errors.AddRange(modelStateVal.Errors.Select(error => error.ErrorMessage));
                }
                this._logger.Info(string.Join(Environment.NewLine, errors));
                return View(model);
            }
            this._logger.Info(string.Format("Authentification {0}", model.Login));
            if (!ValidateUser(model))
            {
                this._logger.Info(string.Format("Accès impossible de l'utilisateur {0}. Veuillez contacter l'administrateur.", model.Login));
                ModelState.AddModelError(string.Empty, Resource.login_msg_AutentificationFailed);
                return View(model);
            }
            // L'authentification est réussie, 
            // injecter l'identifiant utilisateur dans le cookie d'authentification :
            var userClaims = new List<Claim>();
            // Identifiant utilisateur :
            userClaims.Add(new Claim(ClaimTypes.NameIdentifier, model.Login));
            userClaims.Add(new Claim(ClaimTypes.UserData, JsonConvert.SerializeObject(this.UserViewModel)));

            // Rôles utilisateur :

            userClaims.AddRange(LoadRoles(model.Login));

            var claimsIdentity = new ClaimsIdentity(userClaims, DefaultAuthenticationTypes.ApplicationCookie);
            var ctx = Request.GetOwinContext();
            var authenticationManager = ctx.Authentication;
            authenticationManager.SignIn(new Microsoft.Owin.Security.AuthenticationProperties() { IsPersistent = model.RememberMe, ExpiresUtc = DateTimeOffset.UtcNow.AddHours(expires) }, claimsIdentity);
            var _userIdentity = new UserIdentity(User);
            // Rediriger vers l'URL d'origine :
            if (Url.IsLocalUrl(ViewBag.ReturnUrl))
                return Redirect(ViewBag.ReturnUrl);



            var cultureName = Request.UserLanguages != null && Request.UserLanguages.Length > 0 ?
                       Request.UserLanguages[0] :  // obtain it from HTTP header AcceptLanguages
                       "fr-FR";

            // cas particulier du UserLanguages : fr-FR pour Chrome, fr seulement pour Firefox par exemple
            if (cultureName.Equals("fr"))
                cultureName = "fr-FR";
            if (cultureName.Equals("en"))
                cultureName = "en-GB";

            HttpCookie cookie = Request.Cookies["_culture"];
            if (cookie != null)
            {
                var cookieValue = Request.Cookies["_culture"].Value;
                if (cookieValue.Equals("fr"))
                    cookieValue = "fr-FR";
                if (cookieValue.Equals("en"))
                    cookieValue = "en-GB";
                cookie.Value = cookieValue;   // update cookie value
            }
            else
            {
                HttpCookie myCookie = new HttpCookie("_culture");
                myCookie.HttpOnly = true;
                cookie = myCookie;
                cookie.Value = cultureName;
                cookie.Expires = DateTime.Now.AddYears(1);
            }

            Response.Cookies.Add(cookie);
            // Validate culture name

            GetConfigRequest configRequest = new GetConfigRequest()
            {
                ConfigType = ConfigType.MAINTENANCE
            };
            string maintenanceMode = _configService.Get(configRequest).Config.Value;

            // Si en maintenance, rediriger vers le back office, sinon vers la page d'accueil :
            if (maintenanceMode == "true")
            {
                if(this.UserViewModel.Status == 0)
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                else
                    return RedirectToAction("Login");
            }
           if (UserViewModel.Status == 0)
           {    TempData["reader"] = false;
                return RedirectToAction("Index", "Home");
           }
           if (UserViewModel.isReader & UserViewModel.Status == 10)
           {                
                TempData["reader"] = true;
                return RedirectToAction("Index", "Home");
           }
            return RedirectToAction("Index", "UserDocument");
        }

       
        private bool ValidateUser(LoginViewModel model)
        {
            // TODO : Active Directory...
            String Entity = String.Empty;
            String Agency = String.Empty;
            String LongName = String.Empty;

            var user = _userService.GetIntitekUserByEmail(model.Login, true);

            if (user == null || (user != null && user.User == null))
            {
                _logger.Error(string.Format("L'email {0} n'existe pas dans la table IntitekUser ou utilisateur non valide.", model.Login));
                return false;
            }

            var ad = _adService.Get(new GetADRequest()
            {
                Id = user.User.ID_AD,
                PwdIV = _ivPwd,
                PwdKey = _keyPwd
            });

            _activeDirectoryService = new Service.Front.ActiveDirectoryService(new FileLogger(), ad.AD.Address, ad.AD.Domain, ad.AD.Username, ad.AD.Password);

            if (Convert.ToBoolean(ConfigurationManager.AppSettings["LDAPVerificationDisabled"]) ||
                _activeDirectoryService.IsAllowedInActiveDirectory(user.User.Username, model.Password, ref Entity, ref Agency, ref LongName))
            {
                this.UserViewModel = AutoMapperConfig.Mapper.Map<UserViewModel>(user.User);
                this.UserViewModel.CompanyLogo = user.Logo; 
                this.UserViewModel.NomPrenom = user.User.FullName; 
                this.UserViewModel.EntityName = user.User.EntityName;
                this.UserViewModel.AgencyName = user.User.AgencyName;
                this.UserViewModel.CompanyLogo = user.Logo;
                this.UserViewModel.Status = user.User.Status.Value;
                this.UserViewModel.ID_AD = user.User.ID_AD;
                UserViewModel.isReader = user.User.isReader;
                return true;
            }

            return false;
        }

        private IEnumerable<Claim> LoadRoles(string login)
        {
            //// TODO : Charger ici les rôles de l'utilisateur...
            if (UserViewModel.Status == (int)Status.BACKOFFICE)
            {
                yield return new Claim(ClaimTypes.Role, UserTypes.BACKOFFICE);
            }
            if (UserViewModel.Status == (int)Status.FRONTOFFICE)
            {
                yield return new Claim(ClaimTypes.Role, UserTypes.FRONTOFFICE);
            }
            if (UserViewModel.Status == (int)Status.FRONTOFFICE && UserViewModel.isReader)
            {
                yield return new Claim(ClaimTypes.Role, UserTypes.SPYOFFICE);
            }
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session.Clear();
            //Remove Cookies session
            int cookieCount = Request.Cookies.Count;
            if (cookieCount > 0)
            {
                var cookies  = Request.Cookies.AllKeys.Where(x => x.StartsWith("UserDocument")).ToList();
                foreach (var cookieName in cookies)
                {
                    var expiredCookie = new HttpCookie(cookieName)
                    {
                        Expires = DateTime.Now.AddDays(-1),
                    };
                    expiredCookie.HttpOnly = true;
                    Response.Cookies.Add(expiredCookie); 
                }
            }
            var ctx = Request.GetOwinContext();
            var authenticationManager = ctx.Authentication;
            authenticationManager.SignOut();
            
            // Rediriger vers la page d'accueil :
            return RedirectToAction("Login", "Login");
        }

        [HttpPost]
        [ValidateInput(true)]
        [AllowAnonymous]
        public ActionResult Onboard(string emailAD)
        {
            return RedirectToAction("Login", "Login");
        }
    }
}
