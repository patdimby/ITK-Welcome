using Intitek.Welcome.UI.Web.Security;
using Intitek.Welcome.Infrastructure.Helpers;
using System;
using System.Web;
using System.Web.Mvc;
using Intitek.Welcome.UI.Resources;
using System.Collections.Generic;
using Intitek.Welcome.Service.Back;
using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.UI.Web.Admin.Models;
using Intitek.Welcome.UI.ViewModels.Admin;
using Intitek.Welcome.UI.Web.Areas.Admin;

namespace Intitek.Welcome.UI.Web.Controllers
{
    public class HomeController : CommunController
    {
        public static readonly string USER_FO_CONNECTED = "UserFOConnected";
        private readonly IUserService _userService;
        private const string DEFAULT_ENTITY = "Intitek";

        private readonly IImportManagerService _importManagerService;
        private readonly IProfilService _profilService;
        private readonly IEntiteService _entiteService;
        private readonly IMailTemplateService _mailTemplateService;
        private readonly IRelanceService _relanceService;
        private FileLogger _logger;

        public HomeController()
        {
            _logger = new FileLogger();
            _userService = new UserService(new FileLogger());
            _profilService = new ProfilService(new FileLogger());
            _entiteService = new EntiteService(new FileLogger());
            _mailTemplateService = new MailTemplateService(new FileLogger());
            _relanceService = new RelanceService(new FileLogger());
            _importManagerService = new ImportManagerService(new FileLogger());
        }
        public string EntityNameSession
        {
            get
            {
                if (Session["EntityNameSessionForUser"] != null)
                    return (string)Session["EntityNameSessionForUser"];
                return null;
            }
            set { Session["EntityNameSessionForUser"] = value; }
        }
        public string AgencyNameSession
        {
            get
            {
                if (Session["AgencyNameSessionForUser"] != null)
                    return (string)Session["AgencyNameSessionForUser"];
                return null;
            }
            set { Session["AgencyNameSessionForUser"] = value; }
        }

        public string EntryDateSession
        {
            get
            {
                if (Session["EntryDateSessionForUser"] != null)
                    return Convert.ToString(Session["EntryDateSessionForUser"]);
                return null;
            }
            set { Session["EntryDateSessionForUser"] = value; }
        }

        public string ExitDateSession
        {
            get
            {
                if (Session["ExitDateSessionForUser"] != null)
                    return Convert.ToString(Session["ExitDateSessionForUser"]);
                return null;
            }
            set { Session["ExitDateSessionForUser"] = value; }
        }

        public int ActifSession
        {
            get
            {
                if (Session["ActifSessionForUser"] != null)
                    return (int)Session["ActifSessionForUser"];
                return 0;
            }
            set { Session["ActifSessionForUser"] = value; }
        }
        public int ActivitySession
        {
            get
            {
                if (Session["ActivitySessionForUser"] != null)
                    return (int)Session["ActivitySessionForUser"];
                return 0;
            }
            set { Session["ActivitySessionForUser"] = value; }
        }
        public List<DocCheckState> SessionAffectUserProfil
        {
            get
            {
                if (Session["SessionAffectUserProfil"] != null)
                    return (List<DocCheckState>)Session["SessionAffectUserProfil"];
                return null;
            }
            set { Session["SessionAffectUserProfil"] = value; }
        }
        public ActionResult Index()
        {
             UserIdentity userIdentity = new UserIdentity(User);
            //Mise à jour des informations de l'utilisateur connecté (status, agence, entité) au cas où il y a de modification dans la base.
            bool bok = userIdentity.UpdateUser();
            if (!bok)
            {
                ModelState.AddModelError(string.Empty, Resource.login_msg_AutentificationFailed);
                return RedirectToAction("Logout", "Login");
            }           
            //Redirection au cas où l'utilisateur choisit "Se souvenir de moi"
            else if (userIdentity.IsAuthorizeFrontOffice())
            {
                if (IsReader())
                {
                    TempData["reader"] = true;
                }              
                TempData["UserFO"] = "UserFOConnected";
                if (TempData["reader"] != null)
                {
                    if((bool)TempData["reader"])
                    {
                        return View();
                    }
                }
                return RedirectToAction("Index", "UserDocument");
            }
            TempData["reader"] = false;
            return View();
        }
        public ActionResult SetCulture(string lang = "fr-FR")
        {
            // Validate input
            lang = CultureHelper.GetImplementedCulture(lang);
            // Save culture in a cookie
            HttpCookie cookie = Request.Cookies["_culture"];
            if (cookie != null && !String.IsNullOrEmpty(cookie.Value))
            {
                cookie.HttpOnly = true;
                cookie.Value = lang;   // update cookie value
            }
            else
            {
                cookie = new HttpCookie("_culture");
                cookie.HttpOnly = true;
                cookie.Value = lang;
                cookie.Expires = DateTime.Now.AddYears(1);
            }
            var cultureCook = Response.Cookies["_culture"];
            if (cultureCook != null && !String.IsNullOrEmpty(cultureCook.Value))
            {
                cultureCook.HttpOnly = true;
                cultureCook.Value = cookie.Value;
                cultureCook.Expires = DateTime.Now.AddYears(1);
            }
            else
            {
                Response.Cookies.Add(cookie);
            }
            return Redirect(Request.UrlReferrer.ToString());
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
            return Session[sessionName] != null;
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

        protected GridMvcRequest GetGridRequestSession(GridMvcRequest gridRequest, string action = null, string controller = null)
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

        protected void SetGridRequestSession2(GridMvcRequest gridRequest, string action = null, string controller = null)
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

        [HandleError(View = "~/Areas/Admin/Views/Shared/AjaxError.cshtml")]
        public ActionResult AjaxUserGrid(GetUserRequest allrequest)
        {
            string nameGrid = "usrGrid";
            //_adService = GetActiveDirectoryService(GetUserConnectedAD());
            GridMvcRequest request = GridBORequest.GetRequestGrid(Request, nameGrid, "Name");
            allrequest.Request = request;
            SetGridRequestSession2(request, "Index");
            var nbcount = _userService.ListUsersCount(allrequest);
            var users = _userService.ListUsers(allrequest);
            var viewModels = AutoMapperConfigAdmin.Mapper.Map<List<UserViewModel>>(users);
            var grid = new GridBO<UserViewModel>(request, viewModels, nbcount, allrequest.Request.Limit);
            return Json(new { Html = grid.ToJson("_UserGrid", new ViewDataDictionary { { "EntityName", allrequest.EntityName } }, this) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangeAgences(string EntityName)
        {
            var agencyNameList = _entiteService.AgencyByEntity(EntityName, false, true, false);
            return Json(new { Agences = agencyNameList, success = 1 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListUser(GetUserRequest allrequest)
        {
            string nameGrid = "usrGrid";
            GridMvcRequest initrequest = GridBORequest.GetRequestGrid(Request, nameGrid, "Name");
            //nbquery>0 si on click le bouton Rafrachir
            int nbQuery = Request.QueryString.Count;
            //Metre Actif =Oui par défaut
            if (nbQuery == 0 && string.IsNullOrEmpty(allrequest.EntityName) && string.IsNullOrEmpty(allrequest.AgencyName) && allrequest.Actif == 0 && allrequest.Activity == 0)
            {
                if (!HasSession(initrequest, "Index"))
                {
                    allrequest.Actif = Options.True;
                }
                else
                {
                    allrequest.AgencyName = AgencyNameSession;
                    allrequest.EntityName = EntityNameSession;
                    allrequest.Actif = ActifSession;
                    allrequest.Activity = ActivitySession;
                    if (!string.IsNullOrEmpty(EntryDateSession))
                        allrequest.EntryDate = Convert.ToDateTime(EntryDateSession);
                    if (!string.IsNullOrEmpty(ExitDateSession))
                        allrequest.ExitDate = Convert.ToDateTime(ExitDateSession);
                }
            }

            GridMvcRequest request = GetGridRequestSession(initrequest);
            allrequest.Request = request;
            AgencyNameSession = allrequest.AgencyName;
            EntityNameSession = allrequest.EntityName;
            ActifSession = allrequest.Actif;
            ActivitySession = allrequest.Activity;
            EntryDateSession = allrequest.EntryDate != null ? Convert.ToString(allrequest.EntryDate) : null;
            ExitDateSession = allrequest.ExitDate != null ? Convert.ToString(allrequest.ExitDate) : null;

            var nbcount = _userService.ListUsersCount(allrequest);
            var users = _userService.ListUsers(allrequest);
            var viewModels = AutoMapperConfigAdmin.Mapper.Map<List<UserViewModel>>(users);
            var grid = new GridBO<UserViewModel>(request, viewModels, nbcount, request.Limit);

            UserResponseViewModel response = new UserResponseViewModel();
            response.EntityName = allrequest.EntityName;
            response.AgencyName = allrequest.AgencyName;
            response.AgencyNameList = _entiteService.AgencyByEntity(allrequest.EntityName, false, true, false);
            response.EntityNameList = new List<string>() { string.Empty };
            response.EntityNameList.AddRange(_userService.EntityNameList(false));
            response.Actif = allrequest.Actif.ToString();
            response.EntryDate = allrequest.EntryDate;
            response.ExitDate = allrequest.ExitDate;
            response.Activity = allrequest.Activity.ToString();
            response.ActifList = new List<StringOption>() { new StringOption() { Value = "", Text = Resource.All }, new StringOption() { Value = Options.True.ToString(), Text = Resource.yes }, new StringOption() { Value = Options.False.ToString(), Text = Resource.no } };
            response.ListUser = grid;
            return View(response);
        }
    }
}