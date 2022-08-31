using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Service.Back;
using Intitek.Welcome.UI.Resources;
using Intitek.Welcome.UI.ViewModels.Admin;
using Intitek.Welcome.UI.Web.Admin;
using Intitek.Welcome.UI.Web.Admin.Models;

namespace Intitek.Welcome.UI.Web.Areas.Admin.Controllers
{
    public class EntiteController : CommonStatController
    {
        private const string DEFAULT_ENTITY = "Intitek";
        private readonly IUserService _userService;
        private readonly IEntiteService _entiteService;
        private readonly IMailTemplateService _mailTemplateService;
        private readonly IRelanceService _relanceService;
        public string EntityNameSession
        {
            get
            {
                if (Session["EntityNameSessionForEntity"] != null)
                    return (string)Session["EntityNameSessionForEntity"];
                return null;
            }
            set { Session["EntityNameSessionForEntity"] = value; }
        }
        public List<DocCheckState> SessionAffectAgency
        {
            get
            {
                if (Session["SessionAffectAgency"] != null)
                    return (List<DocCheckState>)Session["SessionAffectAgency"];
                return null;
            }
            set { Session["SessionAffectAgency"] = value; }
        }
        public List<DocCheckState> SessionAffectEntity
        {
            get
            {
                if (Session["SessionAffectEntity"] != null)
                    return (List<DocCheckState>)Session["SessionAffectEntity"];
                return null;
            }
            set { Session["SessionAffectEntity"] = value; }
        }
        public EntiteController()
        {
            _userService = new UserService(new FileLogger());
            _entiteService = new EntiteService(new FileLogger());
            _mailTemplateService = new MailTemplateService(new FileLogger());
            _relanceService = new RelanceService(new FileLogger());
        }
        // GET: Admin/Entite
        public ActionResult Index(GetEntityRequest request)
        {
            var entities = _userService.EntityNameList(true);
            if (string.IsNullOrEmpty(request.EntityName))
            {
                request.EntityName = EntityNameSession;
                if (string.IsNullOrEmpty(request.EntityName))
                {
                    request.EntityName = entities.Where(x => x.Equals(DEFAULT_ENTITY, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    EntityNameSession = request.EntityName;
                }             
            }
            else
            {
                EntityNameSession = request.EntityName;
            }
            this.GetEntityStats(request);
            GetEntityResponse reponse = new GetEntityResponse();
            reponse.EntityName = request.EntityName;
            reponse.EntityNameList = entities;
            reponse.AgencyNameList = _entiteService.AgencyByEntity(request.EntityName, true, false, false);
            reponse.EntityStat = this.EntityStat;
            reponse.AgencyStats = this.AgencyStats;
            EntiteResponseViewModel vm = new EntiteResponseViewModel()
            {
                EntityName = reponse.EntityName,
                EntityNameList = reponse.EntityNameList,
                EntityGrid = new GridBO<Statistiques>("entGrid", reponse.EntityStats, 1, 1),
                ListAgency = new GridBO<Statistiques>("agGrid", reponse.AgencyStats, null, -1)
            };
            return View(vm);
        }

        public ActionResult Edit(int Id = 0)
        {
            return View(new EntiteViewModel() {
                ID = Id,
                Name = string.Empty
            });
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult Edit(EntiteViewModel model)
        {
            return View();
        }
        public ActionResult AffectEntity(GetEntityRequest allrequest)
        {

            SaveEntityRequest entityRequest = new SaveEntityRequest();
            entityRequest.InitAffectDocs();
            this.SessionAffectEntity = entityRequest.DocsAffected;

            string nameGrid = "docGrid";
            GridMvcRequest request = GridBORequest.GetRequestGrid(Request, nameGrid, "Name");
            allrequest.Request = request;
            allrequest.IdLang = GetIdLang();
            allrequest.IdDefaultLang = GetDefaultLang();
            List<DocumentDTO> lst = _entiteService.ListDocumentEntity(allrequest);
            var viewmodels = AutoMapperConfigAdmin.Mapper.Map<List<DocumentViewModel>>(lst);
            var grid = new GridBO<DocumentViewModel>(nameGrid, viewmodels, lst.Count(), -1);
            ViewBag.EntityName = allrequest.EntityName;
            return View(grid);
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult AffectEntity(SaveEntityRequest model)
        {
            model.DocsAffected = SessionAffectEntity;
            model.ID_IntitekUser = this.GetUserIdConnected();
            _entiteService.SaveEntity(model);
            return RedirectToAction("Index", "Entite");
        }
        public ActionResult AffectAgency(GetEntityRequest allrequest)
        {
            SaveEntityRequest entityRequest = new SaveEntityRequest();
            entityRequest.InitAffectDocs();
            this.SessionAffectAgency = entityRequest.DocsAffected;

            string nameGrid = "docGrid";
            GridMvcRequest request = GridBORequest.GetRequestGrid(Request, nameGrid, "Name");
            allrequest.Request = request;
            allrequest.IdLang = GetIdLang();
            allrequest.IdDefaultLang = GetDefaultLang();
            List<DocumentDTO> lst = _entiteService.ListDocumentAgence(allrequest);
            var viewmodels = AutoMapperConfigAdmin.Mapper.Map<List<DocumentViewModel>>(lst);
            var grid = new GridBO<DocumentViewModel>(nameGrid, viewmodels, lst.Count(), -1);
            ViewBag.EntityName = allrequest.EntityName;
            ViewBag.AgencyName = allrequest.AgencyName;
            return View(grid);
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult AffectAgencySession(DocCheckState model)
        {
            SaveEntityRequest entityRequest = new SaveEntityRequest();
            entityRequest.DocsAffected = this.SessionAffectAgency;
            entityRequest.ToAffectDocs(model);
            this.SessionAffectAgency = entityRequest.DocsAffected;
            return Json(new {Success = 1 }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateInput(true)]
        public ActionResult AffectEntitySession(DocCheckState model)
        {
            SaveEntityRequest entityRequest = new SaveEntityRequest();
            entityRequest.DocsAffected = this.SessionAffectEntity;
            entityRequest.ToAffectDocs(model);
            this.SessionAffectEntity = entityRequest.DocsAffected;
            return Json(new { Success = 1 }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateInput(true)]
        public ActionResult AffectAgency(SaveEntityRequest model)
        {
            model.DocsAffected = SessionAffectAgency;
            model.ID_IntitekUser = this.GetUserIdConnected();
            _entiteService.SaveAgency(model);
            return RedirectToAction("Index", "Entite");
        }
        [HandleError(View = "~/Areas/Admin/Views/Shared/AjaxError.cshtml")]
        public ActionResult AjaxDocumentGrid(GetEntityRequest allrequest)
        {
            string nameGrid = "docGrid";
            GridMvcRequest request = GridBORequest.GetRequestGrid(Request, nameGrid, "Name");
            allrequest.Request = request;
            allrequest.IdLang = GetIdLang();
            allrequest.IdDefaultLang = GetDefaultLang();
            List<DocumentDTO> lst = null;
            if (string.IsNullOrEmpty(allrequest.AgencyName))
            {
                allrequest.DocsAffected = this.SessionAffectEntity;
                lst = _entiteService.ListDocumentEntity(allrequest);
            }
            else
            {
                allrequest.DocsAffected = this.SessionAffectAgency;
                lst = _entiteService.ListDocumentAgence(allrequest);
            }
            var viewModels = AutoMapperConfigAdmin.Mapper.Map<List<DocumentViewModel>>(lst);
            var grid = new GridBO<DocumentViewModel>(nameGrid, viewModels, lst.Count(), -1);
            return Json(new { Html = grid.ToJson("_DocumentGrid", this) }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DocumentStateEntity(GetEntityRequest allrequest)
        {
            string nameGrid = "stGrid";
            string nameGridAg = "stAgGrid";
            GridMvcRequest request = GridBORequest.GetRequestGrid(Request, nameGrid, "Name");
            allrequest.Request = request;
            allrequest.IdLang = GetIdLang();
            allrequest.IdDefaultLang = GetDefaultLang();
            var docs = _entiteService.DocumentStateEntity(allrequest, false);
            var viewModels = AutoMapperConfigAdmin.Mapper.Map<List<DocumentViewModel>>(docs);

            var docsAg = _entiteService.DocumentStateEntity(allrequest, true);
            var viewModelsAg = AutoMapperConfigAdmin.Mapper.Map<List<DocumentViewModel>>(docsAg);

            var grid = new GridBO<DocumentViewModel>(nameGrid, viewModels, docs.Count(), -1);
            var gridAg = new GridBO<DocumentViewModel>(nameGridAg, viewModelsAg, docsAg.Count(), -1);

            EntiteResponseViewModel vm = new EntiteResponseViewModel();
            vm.EntityName = allrequest.EntityName;
            vm.ListDocuments = grid;
            vm.ListDocumentAgences = gridAg;
            return PartialView(vm);
        }
        public ActionResult DocumentStateAgency(GetEntityRequest allrequest)
        {
            string nameGrid = "stGrid";
            GridMvcRequest request = GridBORequest.GetRequestGrid(Request, nameGrid, "Name");
            allrequest.Request = request;
            allrequest.IdLang = GetIdLang();
            allrequest.IdDefaultLang = GetDefaultLang();
            var docs = _entiteService.DocumentStateAgency(allrequest);
            var viewModels = AutoMapperConfigAdmin.Mapper.Map<List<DocumentViewModel>>(docs);
            var grid = new GridBO<DocumentViewModel>(nameGrid, viewModels, docs.Count(), -1);

            EntiteResponseViewModel vm = new EntiteResponseViewModel();
            vm.EntityName = allrequest.EntityName;
            vm.AgencyName = allrequest.AgencyName;
            vm.ListDocuments = grid;
            return PartialView(vm);
        }
        public ActionResult Relance(string EntityName, string AgencyName)
        {
            var lstMailTemplate = _mailTemplateService.GetAllTemplateRemind();
            var vm = new RelanceViewModel() { EntityName=EntityName, AgencyName= AgencyName, ListTemplate = lstMailTemplate };
            return PartialView(vm);
        }
        [HttpPost]
        [ValidateInput(true)]
        public ActionResult Relance(GetRelanceRequest request)
        {
            request.IdLang = GetIdLang();
            request.IdDefaultLang = GetDefaultLang();
            var mail = _mailTemplateService.Get(new GetMailTemplateRequest() { Id = request.MailTemplateID });
            var users = _entiteService.ListUsersForRelance(request.EntityName, request.AgencyName);
            request.LstUsers = users;
            request.MailTemplate = mail.MailTemplate;
            string message = "";
            bool ret = _relanceService.Execute(request, out message);
            if (ret)
            {
                return Json(new { success = 1, Message = Resource.relanceMessageSuccess }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = 0, Message = string.Format(Resource.relanceMessageFailed, "<div>" + message + "</div>") }, JsonRequestBehavior.AllowGet);
        }
    }
}