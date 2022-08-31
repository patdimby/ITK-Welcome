using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Service.Back;
using Intitek.Welcome.UI.ViewModels.Admin;
using Intitek.Welcome.UI.Web.Admin.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Intitek.Welcome.UI.Web.Areas.Admin.Controllers
{
    public class HistoBatchsController : CommunController
    {
        private readonly IHistoBatchsService _histoBatchsService;

        public HistoBatchsController()
        {
            _histoBatchsService = new HistoBatchsService(new FileLogger());
        }

        // GET: Admin/HistoBatchs
        public ActionResult Index()
        {
            string nameGrid = "histoBatchsGrid";
            GridMvcRequest initrequest = GridBORequest.GetRequestGrid(Request, nameGrid, null);
            GridMvcRequest request = base.GetGridRequestSession(initrequest);
            GetAllHistoBatchsRequest histoBatchsRequest = new GetAllHistoBatchsRequest() { GridRequest = request };
            var total = _histoBatchsService.GetAllCount(histoBatchsRequest);
            var histoBatchs = _histoBatchsService.GetAll(histoBatchsRequest).HistoBatchs;
            var histoBatchsViewModels = AutoMapperConfigAdmin.Mapper.Map<List<HistoBatchsViewModel>>(histoBatchs);
            var grid = new GridBO<HistoBatchsViewModel>(request, histoBatchsViewModels, total, request.Limit);
            return View(grid);
        }

        [HandleError(View = "~/Areas/Admin/Views/Shared/AjaxError.cshtml")]
        public ActionResult AjaxHistoBatchsGrid()
        {
            string nameGrid = "histoBatchsGrid";
            GridMvcRequest request = GridBORequest.GetRequestGrid(Request, nameGrid, null);
            base.SetGridRequestSession(request, "Index");
            GetAllHistoBatchsRequest histoBatchsRequest = new GetAllHistoBatchsRequest() { GridRequest = request };
            var total = _histoBatchsService.GetAllCount(histoBatchsRequest);
            var histoBatchs = _histoBatchsService.GetAll(histoBatchsRequest).HistoBatchs;
            var histoBatchsViewModels = AutoMapperConfigAdmin.Mapper.Map<List<HistoBatchsViewModel>>(histoBatchs);
            var grid = new GridBO<HistoBatchsViewModel>(request, histoBatchsViewModels, total, request.Limit);
            return Json(new { Html = grid.ToJson("_HistoBatchsGrid", this), total }, JsonRequestBehavior.AllowGet);
        }
    }
}