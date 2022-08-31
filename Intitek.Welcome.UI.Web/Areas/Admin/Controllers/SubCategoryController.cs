using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Service.Back;
using Intitek.Welcome.UI.ViewModels.Admin;
using System.Linq;
using System.Web.Mvc;

namespace Intitek.Welcome.UI.Web.Areas.Admin.Controllers
{
    public class SubCategoryController :  CommunController
    {
        private readonly IDocumentService _docService;

        public SubCategoryController()
        {
            _docService = new DocumentService(new FileLogger());
        }

        public ActionResult Edit(int Id = 0)
        {
            

            var response = _docService.GetSubcategory(new GetSubCategoryRequest()
            {
               Id = Id,
               IdLang = GetIdLang()
            });


            var subcategoryViewModel = new SubCategoryViewModel()
            {
                ID = response.SubCategory.ID,
                ID_Category = response.SubCategory.ID_Category,
                Name = response.SubCategory.Name,
                IsDefaultLangName = response.SubCategory.IsDefaultLangName,
                Ordre = response.SubCategory.Ordre,
                Categories = _docService.GetAllCategories(GetIdLang()).Select(
                    ct => new CategoryViewModel() {
                        ID = ct.ID,
                        Name = ct.Name,
                        OrdreCategory = ct.OrdreCategory
                    }
               ).ToList()
            };
            

            return View(subcategoryViewModel);
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult Edit(SubCategoryViewModel model)
        {
            var request = new SaveSubCategoryRequest()
            {
                IdLang = GetIdLang(),
                IsCategoryChanged = model.ID_OldCategory != model.ID_Category,
                SubCategory = new Domain.SubCategory()
                {
                    ID = model.ID,
                    ID_DocumentCategory = model.ID_Category,
                    Ordre = model.Ordre,
                },
                SubCategoryTrad = new Domain.SubCategoryLang()
                {
                    ID_Lang = GetIdLang(),
                    ID_SubCategory = model.ID,
                    Name = model.Name
                }
            };

            var response = _docService.SaveSubcategory(request);

            return RedirectToAction("Index", "Category");
        }

        public ActionResult ConfirmDelete(int Id)
        {
            var response = _docService.GetSubcategory(new GetSubCategoryRequest()
            {
                Id = Id
            });

            return PartialView("_ConfirmDelete", new ConfirmDeleteViewModel()
            {
                ID = response.SubCategory.ID,
                CanDelete = true,
                ControllerName = "SubCategory",
                EntityName = Resources.Resource.la_categorie,
                Name = response.SubCategory.SubCategoryTrad == null ? response.SubCategory.DefaultTrad.Name : response.SubCategory.SubCategoryTrad.Name
            });
        }

        public ActionResult Delete(int Id)
        {
            var reponse = _docService.DeleteSubcategory(new DeleteSubCategoryRequest()
            {
                Id = Id,
             
            });

            return RedirectToAction("Index", "Category");
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult NexSubcatoryOrder(int IdCategory)
        {
            var nextOrder = 1;
            var allSubcategory = _docService.GetAllSubcategory(new GetAllSubCategoryRequest()
            {
                IdLang = GetIdLang(),
            });
            if(allSubcategory.SubCategories.Any())
                nextOrder = allSubcategory.SubCategories.Count(sc => sc.ID_Category == IdCategory) + 1;

            return Json(new { nextOrder }, JsonRequestBehavior.AllowGet);
        }
    }
}