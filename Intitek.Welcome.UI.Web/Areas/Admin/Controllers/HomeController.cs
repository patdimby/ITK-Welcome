using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.UI.Web.Admin.Models;
using Intitek.Welcome.Service.Back;
using System.Web.Mvc;
using System;
using System.Collections.Generic;

using Intitek.Welcome.UI.Resources;
using Intitek.Welcome.UI.ViewModels.Admin;

namespace Intitek.Welcome.UI.Web.Areas.Admin.Controllers
{

    public class HomeController : CommunController
    {
        private readonly IUserService _userService;
        private const string DEFAULT_ENTITY = "Intitek";
        
        private readonly IImportManagerService _importManagerService;
        private readonly IProfilService _profilService;
        private readonly IEntiteService _entiteService;
        private readonly IMailTemplateService _mailTemplateService;
        private readonly IRelanceService _relanceService;
        private FileLogger _logger;
        private int _id;

        public HomeController()
        {
            _logger = new FileLogger();           
            _userService = new UserService(new FileLogger());          
            _profilService = new ProfilService(new FileLogger());
            _entiteService = new EntiteService(new FileLogger());
            _mailTemplateService =new MailTemplateService(new FileLogger());
            _relanceService = new RelanceService(new FileLogger());
            _importManagerService = new ImportManagerService(new FileLogger());
        }

        public ActionResult ChangeAgences(string EntityName)
        {
            var agencyNameList = _entiteService.AgencyByEntity(EntityName, false, true, false);
            return Json(new { Agences = agencyNameList, success = 1 }, JsonRequestBehavior.AllowGet);
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

        // GET: Admin/Home
        public ActionResult Index()
        {          
            if (TempData["id"] != null)
            {
                int idManager = (int)TempData["id"];
                _id = idManager;              
            }
            return View();
        }

      

    }
}