using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Service.Back;
using Intitek.Welcome.UI.Resources;
using Intitek.Welcome.UI.ViewModels.Admin;
using Intitek.Welcome.UI.Web.Admin.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Intitek.Welcome.UI.Web.Areas.Admin.Controllers
{
    public class HistoActionsController : CommunController
    {
        private readonly IHistoActionsService _histoActionsService;

        public HistoActionsController()
        {
            _histoActionsService = new HistoActionsService(new FileLogger());
        }

        // GET: Admin/Changes
        public ActionResult Index()
        {
            string nameGrid = "histoActionsGrid";
            GridMvcRequest initrequest = GridBORequest.GetRequestGrid(Request, nameGrid, null);
            GridMvcRequest request = base.GetGridRequestSession(initrequest);
            GetAllHistoActionsRequest histoActionsRequest = new GetAllHistoActionsRequest() { GridRequest = request };
            histoActionsRequest.IdLang = GetIdLang();
            var total = _histoActionsService.GetAllCount(histoActionsRequest);
            var histoActions = _histoActionsService.GetAll(histoActionsRequest).HistoActions;
            var histoActionsViewModels = AutoMapperConfigAdmin.Mapper.Map<List<ChangesViewModel>>(histoActions);
            var grid = new GridBO<ChangesViewModel>(request, histoActionsViewModels, total, request.Limit);
            return View(grid);
        }

        [HandleError(View = "~/Areas/Admin/Views/Shared/AjaxError.cshtml")]
        public ActionResult AjaxHistoActionsGrid()
        {
            string nameGrid = "histoActionsGrid";
            GridMvcRequest request = GridBORequest.GetRequestGrid(Request, nameGrid, null);
            base.SetGridRequestSession(request, "Index");
            GetAllHistoActionsRequest histoActionsRequest = new GetAllHistoActionsRequest() { GridRequest = request };
            histoActionsRequest.IdLang = GetIdLang();
            var total = _histoActionsService.GetAllCount(histoActionsRequest);
            var histoActions = _histoActionsService.GetAll(histoActionsRequest).HistoActions;
            var histoActionsViewModels = AutoMapperConfigAdmin.Mapper.Map<List<ChangesViewModel>>(histoActions);
            var grid = new GridBO<ChangesViewModel>(request, histoActionsViewModels, total, request.Limit);
            return Json(new { Html = grid.ToJson("_ChangesGrid", this), total }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchObjectCodes()
        {
            var objectCodes = _histoActionsService.GetAllObjectCodes();
            return Json(new { Items = objectCodes });
        }

        public ActionResult SearchActions()
        {
            var actions = _histoActionsService.GetAllActions();
            var result = actions.Select(a => new { ID = a, Label = Resource.ResourceManager.GetString("histoAction_" + a) });
            return Json(new { Items = result });
        }
    }
}