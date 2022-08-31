using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Service.Back;
using Intitek.Welcome.UI.ViewModels.Admin;
using Intitek.Welcome.UI.Web.Admin.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Intitek.Welcome.UI.Web.Areas.Admin.Controllers
{
    public class HistoEmailsController : CommunController
    {
        private readonly IHistoEmailsService _histoEmailsService;
        private readonly IBatchService _batchsService;

        public HistoEmailsController()
        {
            _histoEmailsService = new HistoEmailsService(new FileLogger());
            _batchsService = new BatchService(new FileLogger());
        }

        // GET: Admin/HistoEmails
        public ActionResult Index()
        {
            string nameGrid = "histoEmailsGrid";
            GridMvcRequest initrequest = GridBORequest.GetRequestGrid(Request, nameGrid, null);
            GridMvcRequest request = base.GetGridRequestSession(initrequest);
            GetAllHistoEmailsRequest histoEmailsRequest = new GetAllHistoEmailsRequest() { GridRequest = request };
            var total = _histoEmailsService.GetAllCount(histoEmailsRequest);
            var histoEmails = _histoEmailsService.GetAll(histoEmailsRequest).HistoEmails;
            var histoEmailsViewModels = AutoMapperConfigAdmin.Mapper.Map<List<HistoEmailsViewModel>>(histoEmails);
            var grid = new GridBO<HistoEmailsViewModel>(request, histoEmailsViewModels, total, request.Limit);
            return View(grid);
        }

        [HandleError(View = "~/Areas/Admin/Views/Shared/AjaxError.cshtml")]
        public ActionResult AjaxHistoEmailsGrid()
        {
            string nameGrid = "histoEmailsGrid";
            GridMvcRequest request = GridBORequest.GetRequestGrid(Request, nameGrid, null);
            base.SetGridRequestSession(request, "Index");
            GetAllHistoEmailsRequest histoEmailsRequest = new GetAllHistoEmailsRequest() { GridRequest = request };
            var total = _histoEmailsService.GetAllCount(histoEmailsRequest);
            var histoEmails = _histoEmailsService.GetAll(histoEmailsRequest).HistoEmails;
            var histoEmailsViewModels = AutoMapperConfigAdmin.Mapper.Map<List<HistoEmailsViewModel>>(histoEmails);
            var grid = new GridBO<HistoEmailsViewModel>(request, histoEmailsViewModels, total, request.Limit);
            return Json(new { Html = grid.ToJson("_HistoEmailsGrid", this), total }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchBatchProgNames()
        {
            var progNames = _batchsService.GetAllProgNames();
            var result = progNames.Select(name => new { ID = name, Label = name });
            return Json(new { Items = result });
        }
    }
}