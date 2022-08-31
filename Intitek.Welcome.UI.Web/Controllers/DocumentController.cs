using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Service.Front;
using Intitek.Welcome.UI.ViewModels;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Intitek.Welcome.UI.Web.Controllers
{
    public class DocumentController : CommunController
    {
        private readonly IDocumentService _docService;
        public DocumentController()
        {
            _docService = new DocumentService(new FileLogger());
  
        }
        public ActionResult SearchCategories()
        {
            int idLang = GetIdLang();
            int idDefaultLang = GetDefaultLang();
            var categoryModels = _docService.GetAllCategory(idLang, idDefaultLang);
            var viewCategoryModels = AutoMapperConfig.Mapper.Map<List<CategoryViewModel>>(categoryModels);
            viewCategoryModels.Add(new CategoryViewModel() { ID = -1, Name = Resources.Resource.NoCategory });
            return Json(new { Items = viewCategoryModels });
        }
        public ActionResult SearchSousCategories()
        {
            int idLang = GetIdLang();
            int idDefaultLang = GetDefaultLang();
            var subcategoryModels = _docService.GetAllSubCategory(idLang, idDefaultLang);
            var viewSubCategoryModels = AutoMapperConfig.Mapper.Map<List<SubCategoryViewModel>>(subcategoryModels);
            viewSubCategoryModels.Add(new SubCategoryViewModel() { ID = -1, ID_DocumentCategory=-1, Name = Resources.Resource.NoSubCategory });
            return Json(new { Items = viewSubCategoryModels });
        }
        public ActionResult SearchVersions()
        {
            var versions = _docService.GetAllVersion();
            return Json(new { Items = versions });
        }
        
    }
}