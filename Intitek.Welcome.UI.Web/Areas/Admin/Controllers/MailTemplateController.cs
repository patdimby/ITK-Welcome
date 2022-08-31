using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Service.Back;
using Intitek.Welcome.UI.ViewModels.Admin;
using Intitek.Welcome.UI.Web.Admin.Models;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;

namespace Intitek.Welcome.UI.Web.Areas.Admin.Controllers
{
    public class MailTemplateController : CommunController
    {
        private readonly IMailTemplateService _mailTemplateService;
        private readonly IMailKeywordsService _mailKeywordsService;
        private readonly IDocumentService _docService;

        public MailTemplateController()
        {
            _mailTemplateService = new MailTemplateService(new FileLogger());
            _mailKeywordsService = new MailKeywordsService(new FileLogger());
            _docService = new DocumentService(new FileLogger());
        }

        // GET: Admin/HistoBatchs
        public ActionResult Index()
        {
            string nameGrid = "mailTemplateGrid";
            GridMvcRequest initrequest = GridBORequest.GetRequestGrid(Request, nameGrid, null);
            GridMvcRequest request = base.GetGridRequestSession(initrequest);
            GetAllMailTemplateRequest mailTemplateRequest = new GetAllMailTemplateRequest() { GridRequest = request };
            var mailTemplates = _mailTemplateService.GetAll(mailTemplateRequest).MailTemplates;
            var total = mailTemplates.Count;
            var mailTemplateViewModels = AutoMapperConfigAdmin.Mapper.Map<List<MailTemplateViewModel>>(mailTemplates);
            var grid = new GridBO<MailTemplateViewModel>(request, mailTemplateViewModels, total, request.Limit);
            return View(grid);
        }

        [HandleError(View = "~/Areas/Admin/Views/Shared/AjaxError.cshtml")]
        public ActionResult AjaxMailTemplateGrid()
        {
            string nameGrid = "mailTemplateGrid";
            GridMvcRequest request = GridBORequest.GetRequestGrid(Request, nameGrid, null);
            base.SetGridRequestSession(request, "Index");
            GetAllMailTemplateRequest mailTemplateRequest = new GetAllMailTemplateRequest() { GridRequest = request };
            var mailTemplates = _mailTemplateService.GetAll(mailTemplateRequest).MailTemplates;
            var total = mailTemplates.Count;
            var mailTemplateViewModels = AutoMapperConfigAdmin.Mapper.Map<List<MailTemplateViewModel>>(mailTemplates);
            var grid = new GridBO<MailTemplateViewModel>(request, mailTemplateViewModels, total, request.Limit);
            return Json(new { Html = grid.ToJson("_MailTemplateGrid", this), total }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int Id = 0)
        {
            int idLang = GetIdLang();
            int idDefaultLang = GetDefaultLang();
            var categoryModels = _docService.GetAllCategory(new GetAllCategoryRequest()
            {
                IdLang = GetIdLang(),
                IdDefaultLang = GetDefaultLang()
            });
            var viewCategoryModels = AutoMapperConfigAdmin.Mapper.Map<List<CategoryViewModel>>(categoryModels);
            //viewCategoryModels.Add(new CategoryViewModel() { ID = -1, Name = Resources.Resource.NoCategory });

            var subcategoryModels = _docService.GetAllSubCategory(idLang, idDefaultLang);
            var viewSubCategoryModels = AutoMapperConfigAdmin.Mapper.Map<List<SubCategoryViewModel>>(subcategoryModels);
          
            var response = _mailTemplateService.Get(new GetMailTemplateRequest()
            {
                Id = Id
            });

            var mailTemplateViewModel = new MailTemplateViewModel();

            if(response.MailTemplate != null)
                mailTemplateViewModel = AutoMapperConfigAdmin.Mapper.Map<MailTemplateViewModel>(response.MailTemplate);

            var mailKeywords = _mailKeywordsService.GetAll().MailKeywords;
            mailTemplateViewModel.MailKeywords = mailKeywords;
            mailTemplateViewModel.Categories = viewCategoryModels;
            mailTemplateViewModel.SubCategories = viewSubCategoryModels;
            if(response.MailTemplate.Categories!=null)
                mailTemplateViewModel.SelectedCategories = response.MailTemplate.Categories.Select(x => x.ID).ToList();
            if (response.MailTemplate.SubCategories != null)
                mailTemplateViewModel.SelectedSubCategories = response.MailTemplate.SubCategories.Select(x => x.ID).ToList();

            return View(mailTemplateViewModel);
        }

        [ValidateInput(false)]
        public ActionResult AjaxGetMailPreview(string content)
        {
            string preview = _mailTemplateService.GetMailPreview(content);

            return Json(new { Preview = preview }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult Edit(MailTemplateViewModel model)
        {
            List<string> unrecognizedKeywords = _mailTemplateService.GetUnrecognizedKeywords(model.Content);
            if (unrecognizedKeywords.Count != 0)
            {
                Response.StatusCode = 500;
                return Json(new { InvalidKeywords = string.Join(", ", unrecognizedKeywords) });
            }
            var request = new SaveMailTemplateRequest()
            {
                MailTemplate = new Domain.MailTemplate()
                {
                    ID = model.Id,
                    Name = model.Name,
                    Commentaire = model.Comment,
                    Content = model.Content,
                    Object = model.Object,
                    isGlobal = model.IsGlobal ? 1 : 0,
                    isDocNoCategory = model.IsDocNoCategory? 1 : 0,
                    isDocNoSubCategory = model.IsDocNoSubCategory ? 1 :0
                },
                CategorySubCategories = model.CategorySubCategories
             };
            //Toutes les documents
            if (model.IsGlobal)
            {
                request.MailTemplate.isDocNoCategory = 0;
                request.MailTemplate.isDocNoSubCategory = 0;
                request.CategorySubCategories = new List<string>();
            }

            var response = _mailTemplateService.Save(request);

            return RedirectToAction("Index", "MailTemplate");
        }

        public ActionResult ConfirmDelete(int Id)
        {
            var response = _mailTemplateService.Get(new GetMailTemplateRequest()
            {
                Id = Id
            });

            return PartialView("_ConfirmDelete", new ConfirmDeleteViewModel()
            {
                ID = response.MailTemplate.Id,
                CanDelete = true,
                ControllerName = "MailTemplate",
                ActionName = "Delete",
                EntityName = Resources.Resource.the_mailTemplate,
                Name = response.MailTemplate.Name
            });
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult Delete(int id)
        {
            var reponse = _mailTemplateService.Delete(new DeleteMailTemplateRequest()
            {
                Id = id
            });

            if (Request.IsAjaxRequest())
                return Json(new { Id = id }, JsonRequestBehavior.AllowGet);

            return RedirectToAction("Index");
        }
    }
}