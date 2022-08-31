using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Service.Back;
using Intitek.Welcome.UI.ViewModels.Admin;
using Intitek.Welcome.UI.Web.Admin.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Intitek.Welcome.UI.Web.Areas.Admin.Controllers
{
    public class HistoUserQcmController : CommunController
    {
        private readonly IHistoUserQcmDocVersionService _histoUserQcmService;

        public HistoUserQcmController()
        {
            _histoUserQcmService = new HistoUserQcmDocVersionService(new FileLogger());
        }

        // GET: Admin/HistoUserQcm
        public ActionResult Index()
        {
            string nameGrid = "histoUserQcmGrid";
            GridMvcRequest initrequest = GridBORequest.GetRequestGrid(Request, nameGrid, null);
            GridMvcRequest request = base.GetGridRequestSession(initrequest);
            GetAllHistoUserQcmDocVersionRequest histoUserQcmRequest = new GetAllHistoUserQcmDocVersionRequest() {
                GridRequest = request,
                IdLang = GetIdLang(),
                IdDefaultLang = GetDefaultLang()
            };
            
            var total = _histoUserQcmService.GetAllHistoUserQcmDocVersionCount(histoUserQcmRequest);
            var histoUserQcms = _histoUserQcmService.GetAllHistoUserQcmDocVersion(histoUserQcmRequest).HistoUserQcms;
            var histoUserQcmViewModels = AutoMapperConfigAdmin.Mapper.Map<List<HistoUserQcmViewModel>>(histoUserQcms);
            var grid = new GridBO<HistoUserQcmViewModel>(request, histoUserQcmViewModels, total, request.Limit);
            return View(grid);
        }

        [HandleError(View = "~/Areas/Admin/Views/Shared/AjaxError.cshtml")]
        public ActionResult AjaxHistoUserQcmGrid()
        {
            string nameGrid = "histoUserQcmGrid";
            GridMvcRequest request = GridBORequest.GetRequestGrid(Request, nameGrid, null);
            base.SetGridRequestSession(request, "Index");
            GetAllHistoUserQcmDocVersionRequest histoUserQcmRequest = new GetAllHistoUserQcmDocVersionRequest()
            {
                GridRequest = request,
                IdLang = GetIdLang(),
                IdDefaultLang = GetDefaultLang()
            };
            var total = _histoUserQcmService.GetAllHistoUserQcmDocVersionCount(histoUserQcmRequest);
            var histoUserQcms = _histoUserQcmService.GetAllHistoUserQcmDocVersion(histoUserQcmRequest).HistoUserQcms;
            var histoUserQcmViewModels = AutoMapperConfigAdmin.Mapper.Map<List<HistoUserQcmViewModel>>(histoUserQcms);
            var grid = new GridBO<HistoUserQcmViewModel>(request, histoUserQcmViewModels, total, request.Limit);
            return Json(new { Html = grid.ToJson("_HistoUserQcmGrid", this), total }, JsonRequestBehavior.AllowGet);
        }
    }
}