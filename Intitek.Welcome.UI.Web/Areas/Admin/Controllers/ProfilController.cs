using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Service.Back;
using Intitek.Welcome.UI.Resources;
using Intitek.Welcome.UI.ViewModels.Admin;
using Intitek.Welcome.UI.Web.Admin.Models;

namespace Intitek.Welcome.UI.Web.Areas.Admin.Controllers
{
    public class ProfilController : CommunController
    {
        private readonly IProfilService _profilService;
        private readonly IMailTemplateService _mailTemplateService;
        private readonly IRelanceService _relanceService;
       public List<DocCheckState> SessionAffectProfil
        {
            get
            {
                if (Session["SessionAffectProfil"] != null)
                    return (List<DocCheckState>)Session["SessionAffectProfil"];
                return null;
            }
            set { Session["SessionAffectProfil"] = value; }
        }
        public ProfilController()
        {
            _profilService = new ProfilService(new FileLogger());
            _mailTemplateService = new MailTemplateService(new FileLogger());
            _relanceService = new RelanceService(new FileLogger());
         

        }
        // GET: Admin/Profil
        public ActionResult Index()
        {
            string nameGrid = "prfGrid";
            GridMvcRequest initrequest = GridBORequest.GetRequestGrid(Request, nameGrid, "Name");
            GridMvcRequest request = base.GetGridRequestSession(initrequest);
            var profils = _profilService.GetAll(request);
            var viewModels = AutoMapperConfigAdmin.Mapper.Map<List<ProfilViewModel>>(profils);
            var grid = new GridBO<ProfilViewModel>(request, viewModels, null, request.Limit);
            return View(grid);
        }
        [HandleError(View = "~/Areas/Admin/Views/Shared/AjaxError.cshtml")]
        public ActionResult AjaxProfilGrid()
        {
            string nameGrid = "prfGrid";
            GridMvcRequest request = GridBORequest.GetRequestGrid(Request, nameGrid, "Name");
            base.SetGridRequestSession(request, "Index");
            var profils = _profilService.GetAll(request);
            var viewModels = AutoMapperConfigAdmin.Mapper.Map<List<ProfilViewModel>>(profils);
            var grid = new GridBO<ProfilViewModel>(request, viewModels, null, request.Limit);
            return Json(new { Html = grid.ToJson("_ProfilGrid", this) }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Edit(int? Id)
        {
            
            SaveProfilRequest profilRequest = new SaveProfilRequest();
            profilRequest.InitAffectDocs();
            this.SessionAffectProfil = profilRequest.DocsAffected;
           
            string nameGrid = "docGrid";
            GridMvcRequest request = GridBORequest.GetRequestGrid(Request, nameGrid, "Name");
            var profil = _profilService.Get(new GetProfilRequest() { Id = Id, GridRequest = request, IdLang=GetIdLang(), IdDefaultLang= GetDefaultLang() });
            var pVm = new ProfilResponseViewModel();
            pVm.Profile = AutoMapperConfigAdmin.Mapper.Map<ProfilViewModel>(profil.Profile);
            var listUser = AutoMapperConfigAdmin.Mapper.Map<List<UserViewModel>>(profil.ListUser);
            listUser = listUser.Skip((request.Page - 1) * request.Limit).Take(request.Limit).ToList();
            var listDocument = AutoMapperConfigAdmin.Mapper.Map<List<DocumentViewModel>>(profil.ListDocument);
            pVm.ListDocument = new GridBO<DocumentViewModel>("docGrid", listDocument, profil.ListDocument.Count(), -1);
            pVm.ListUser = new GridBO<UserViewModel>("userGrid", listUser, profil.ListUser.Count, request.Limit);
            return View(pVm);
        }
        [HttpPost]
        [ValidateInput(true)]
        public ActionResult AffectSession(DocCheckState model)
        {
            SaveProfilRequest profilRequest = new SaveProfilRequest();
            profilRequest.DocsAffected = this.SessionAffectProfil;
            profilRequest.ToAffectDocs(model);
            this.SessionAffectProfil = profilRequest.DocsAffected;
            return Json(new { Success = 1 }, JsonRequestBehavior.AllowGet);
        }

        [HandleError(View = "~/Areas/Admin/Views/Shared/AjaxError.cshtml")]
        public ActionResult AjaxDocumentGrid()
        {
            string nameGrid = "docGrid";
            int Id = 0;
            int.TryParse(Request.QueryString["ProfilID"], out Id);
            GridMvcRequest request = GridBORequest.GetRequestGrid(Request, nameGrid, "Name");
            var allRequest = new GetProfilRequest() { Id = Id, GridRequest = request, IdLang = GetIdLang(), IdDefaultLang = GetDefaultLang() };
            allRequest.DocsAffected = this.SessionAffectProfil;
            var docs = _profilService.ListDocument(allRequest);
            var viewModels = AutoMapperConfigAdmin.Mapper.Map<List<DocumentViewModel>>(docs);
            var grid = new GridBO<DocumentViewModel>(nameGrid, viewModels, docs.Count(), -1);
            return Json(new { Html = grid.ToJson("_DocumentGrid", this), total= docs.Count() }, JsonRequestBehavior.AllowGet);
        }
        [HandleError(View = "~/Areas/Admin/Views/Shared/AjaxError.cshtml")]
        public ActionResult AjaxUserGrid()
        {
            string nameGrid = "userGrid";
            GridMvcRequest request = GridBORequest.GetRequestGrid(Request, nameGrid, "Name");
            int Id = 0;
            int.TryParse(Request.QueryString["ProfilID"], out Id);
            var users = _profilService.ListUserByProfilID(Id, false);           
            var viewModels = AutoMapperConfigAdmin.Mapper.Map<List<UserViewModel>>(users);
            viewModels = viewModels.Skip((request.Page - 1) * request.Limit).Take(request.Limit).ToList();
            var grid = new GridBO<UserViewModel>(nameGrid, viewModels, users.Count, request.Limit);
            return Json(new { Html = grid.ToJson("_UserGrid", this) }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateInput(true)]
        public ActionResult NameProfilExist(SaveProfilRequest model)
        {
            var isNameEx = false;
            if (!string.IsNullOrWhiteSpace(model.Name))
            {
                isNameEx = _profilService.IsProfilNameExist(model);
            }
            
            return Json(new { valid = !isNameEx }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult Edit(SaveProfilRequest model)
        {
            model.DocsAffected = SessionAffectProfil;
            model.ID_IntitekUser = this.GetUserIdConnected();
            _profilService.Save(model);
            return RedirectToAction("Index", "Profil");
        }
        public ActionResult DocumentState(int profilID)
        {
            string nameGrid = "stGrid";
            GridMvcRequest request = GridBORequest.GetRequestGrid(Request, nameGrid, "Name");
            GetProfilRequest allrequest = new GetProfilRequest() { Id = profilID, GridRequest = request, IdLang = GetIdLang(), IdDefaultLang=GetDefaultLang() };
            var profil = _profilService.GetProfilById(profilID, false);
            var docs = _profilService.ListDocumentByProfileId(allrequest);
            var viewModels = AutoMapperConfigAdmin.Mapper.Map<List<DocumentViewModel>>(docs);
            var pVm = new ProfilResponseViewModel(); 
            var grid = new GridBO<DocumentViewModel>(nameGrid, viewModels, docs.Count(), -1);
            pVm.ListDocument = grid;
            pVm.Profile = AutoMapperConfigAdmin.Mapper.Map<ProfilViewModel>(profil);          
            return PartialView(pVm);
        }
        public ActionResult ConfirmDelete(int profilID)
        {
            var profil = _profilService.GetProfilById(profilID, true);
            var pVm = AutoMapperConfigAdmin.Mapper.Map<ProfilViewModel>(profil);

            return PartialView("_ConfirmDelete", new ConfirmDeleteViewModel()
            {
                ID = pVm.ID,
                CanDelete = (pVm.Affecte==false),
                ControllerName = "Profil",
                EntityName = Resources.Resource.le_profil,
                Name = pVm.Name
            });
            //return PartialView(pVm);
        }
        public ActionResult Delete(int ID)
        {
            _profilService.Delete(ID);
            return RedirectToAction("Index", "Profil");
        }
        public ActionResult Relance(int Id)
        {
            var profil = _profilService.GetProfilById(Id, false);
            var lstMailTemplate = _mailTemplateService.GetAllTemplateRemind();
            var vm = new RelanceViewModel() { Profile = profil, ListTemplate = lstMailTemplate };
            return PartialView(vm);
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult Relance(GetRelanceRequest request)
        {
            request.IdLang = GetIdLang();
            request.IdDefaultLang = GetDefaultLang();
            var mail = _mailTemplateService.Get(new GetMailTemplateRequest() { Id = request.MailTemplateID });
            var users = _profilService.ListUsersForRelance(request.ProfilID);
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