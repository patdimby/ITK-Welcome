using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Service.Back;
using Intitek.Welcome.UI.ViewModels.Admin;
using Intitek.Welcome.UI.Web.Admin.Models;

namespace Intitek.Welcome.UI.Web.Areas.Admin.Controllers
{
    public class CategoryController : CommunController
    {
        private readonly IDocumentService _docService;

        public CategoryController()
        {
            _docService = new DocumentService(new FileLogger());
        }

        // GET: Admin/Entite
        public ActionResult Index()
        {
            var idLang = GetIdLang();
            var categs = _docService.GetAllCategories(idLang);
            var viewModels = categs.Select(ctg => new CategoryViewModel()
            {
                ID = ctg.ID,
                Name = ctg.Name,
                OrdreCategory = ctg.OrdreCategory,
                IsDefaultLangName = ctg.IsDefaultLangName,
                IsDeleted = ctg.IsDeleted,
                NbDocuments = ctg.NbDocuments,
                SubCategories = ctg.Subcategories.Select(sc => new SubCategoryViewModel()
                    {
                        ID = sc.ID,
                        Name = sc.Name,
                        Ordre = sc.Ordre,
                        IsDeleted = sc.IsDeleted,
                        NbDocuments = sc.NbDocuments,
                        IsDefaultLangName = sc.IsDefaultLangName

                    }
                    ).ToList()

            }).ToList();
            
            return View(new ListCategoryViewModel() {
                Categories = viewModels
            });
        }

        [HandleError(View = "~/Areas/Admin/Views/Shared/AjaxError.cshtml")]
        public ActionResult AjaxCategoryGrid()
        {
            string nameGrid = "catGrid";
            GridMvcRequest request = GridBORequest.GetRequestGrid(Request, nameGrid, "OrdreCategory");
            base.SetGridRequestSession(request, "Index");
            var idLang = GetIdLang();
            var categs = _docService.GetAllCategories(idLang);
            var viewModels = AutoMapperConfigAdmin.Mapper.Map<List<CategoryViewModel>>(categs);
            var grid = new GridBO<CategoryViewModel>(request, viewModels, null, request.Limit);
            return Json(new { Html = grid.ToJson("_CategoryGrid", this) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int Id = 0)
        {
            var response = _docService.GetCategory(new GetCategoryRequest()
            {
                Id = Id,
                IdLang= GetIdLang()
            });

            var categoryViewModel = new CategoryViewModel()
                {
                    ID = response.Category.ID,
                    Name = Id == 0 ? string.Empty : (response.CategoryTrad == null ? response.DefaultTrad.Name : response.CategoryTrad.Name),
                    OrdreCategory = response.Category.OrdreCategory,
                    IsDefaultLangName = response.CategoryTrad == null
            };
  
            return View(categoryViewModel);
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult Edit(CategoryViewModel model)
        {
            var request = new SaveCategoryRequest()
            {
                Category = new Domain.DocumentCategory()
                {
                    ID = model.ID,
                    OrdreCategory = model.OrdreCategory
                },
                CategoryTrad = new Domain.DocumentCategoryLang()
                {
                    ID_DocumentCategory = model.ID,
                    ID_Lang = GetIdLang(),
                    Name = model.Name

                },
            };

            var response = _docService.SaveCategory(request);

            return RedirectToAction("Index", "Category");
        }
        [HttpPost]
        [ValidateInput(true)]
        public ActionResult NameCategoryExist(CategoryViewModel model)
        {
            var isNameEx = false;
            if (!string.IsNullOrWhiteSpace(model.Name))
            {
                var request = new SaveCategoryRequest()
                {
                    Category = new Domain.DocumentCategory()
                    {
                        ID = model.ID,
                        OrdreCategory = model.OrdreCategory
                    }
                };
                isNameEx = _docService.IsCategoryNameExist(request);
            }

            return Json(new { valid = !isNameEx }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult ConfirmDelete(int Id)
        {
            var response = _docService.GetCategory(new GetCategoryRequest()
            {
                Id = Id
            });

            return PartialView("_ConfirmDelete", new ConfirmDeleteViewModel() {
                ID = response.Category.ID,
                CanDelete = true,
                ControllerName = "Category",
                EntityName = Resources.Resource.la_categorie,
                Name = response.CategoryTrad == null ? response.DefaultTrad.Name : response.CategoryTrad.Name
            });
        }

        public ActionResult Delete(int Id)
        {
            var reponse = _docService.DeleteCategory(new DeleteCategoryRequest()
            {
                Id = Id,
                UserId = GetUserIdConnected(),
            });

            return RedirectToAction("Index");
        }
    }
}