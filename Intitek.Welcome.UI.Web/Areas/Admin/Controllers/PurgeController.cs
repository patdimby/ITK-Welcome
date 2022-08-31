using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Service.Back;
using Intitek.Welcome.UI.ViewModels.Admin;
using System;
using System.Web.Mvc;

namespace Intitek.Welcome.UI.Web.Areas.Admin.Controllers
{
    public class PurgeController : CommunController
    {

        private readonly IHistoActionsService _histoActionsService;
        private readonly IHistoEmailsService _histoEmailsService;
        private readonly IHistoBatchsService _histoBatchsService;
        private readonly IHistoUserQcmDocVersionService _histoUserQcmService;

        public PurgeController()
        {
            _histoActionsService = new HistoActionsService(new FileLogger());
            _histoEmailsService = new HistoEmailsService(new FileLogger());
            _histoBatchsService = new HistoBatchsService(new FileLogger());
            _histoUserQcmService = new HistoUserQcmDocVersionService(new FileLogger());
        }

        // GET: Admin/Purge
        public ActionResult Index(string limitDate = null)
        {
            DateTime? limitDateTime = null;
            DateTime outLimitDate;
            if (DateTime.TryParse(limitDate, out outLimitDate))
                limitDateTime = DateTime.Parse(limitDate);

            GetAllHistoActionsRequest histoActionsRequest = new GetAllHistoActionsRequest() { LimitDate = limitDateTime };
            int histoActionsCount = _histoActionsService.GetAllCount(histoActionsRequest);

            GetAllHistoEmailsRequest histoEmailsRequest = new GetAllHistoEmailsRequest() { LimitDate = limitDateTime };
            int histoEmailsCount = _histoEmailsService.GetAllCount(histoEmailsRequest);

            GetAllHistoBatchsRequest histoBatchsRequest = new GetAllHistoBatchsRequest() { LimitDate = limitDateTime };
            int histoBatchsCount = _histoBatchsService.GetAllCount(histoBatchsRequest);

            GetAllHistoUserQcmDocVersionRequest histoUserQcmRequest = new GetAllHistoUserQcmDocVersionRequest() { LimitDate = limitDateTime };
            int histoUserQcmCount = _histoUserQcmService.GetAllHistoUserQcmDocVersionCount(histoUserQcmRequest);

            PurgeViewModel purgeVM = new PurgeViewModel()
            {
                LimitDateString = limitDate,
                HistoActionsCount = histoActionsCount,
                HistoEmailsCount = histoEmailsCount,
                HistoBatchsCount = histoBatchsCount,
                HistoUserQcmCount = histoUserQcmCount
            };

            return View(purgeVM);
        }

        public ActionResult AjaxHistoList(string limitDateString)
        {
            DateTime? limitDate = null;
            DateTime outLimitDate;
            if (DateTime.TryParse(limitDateString, out outLimitDate))
                limitDate = DateTime.Parse(limitDateString);

            GetAllHistoActionsRequest histoActionsRequest = new GetAllHistoActionsRequest() { LimitDate = limitDate };
            int histoActionsCount = _histoActionsService.GetAllCount(histoActionsRequest);

            GetAllHistoEmailsRequest histoEmailsRequest = new GetAllHistoEmailsRequest() { LimitDate = limitDate };
            int histoEmailsCount = _histoEmailsService.GetAllCount(histoEmailsRequest);

            GetAllHistoBatchsRequest histoBatchsRequest = new GetAllHistoBatchsRequest() { LimitDate = limitDate };
            int histoBatchsCount = _histoBatchsService.GetAllCount(histoBatchsRequest);

            GetAllHistoUserQcmDocVersionRequest histoUserQcmRequest = new GetAllHistoUserQcmDocVersionRequest() { LimitDate = limitDate };
            int histoUserQcmCount = _histoUserQcmService.GetAllHistoUserQcmDocVersionCount(histoUserQcmRequest);

            PurgeViewModel purgeVM = new PurgeViewModel()
            {
                HistoActionsCount = histoActionsCount,
                HistoEmailsCount = histoEmailsCount,
                HistoBatchsCount = histoBatchsCount,
                HistoUserQcmCount = histoUserQcmCount
            };

            return Json(new { HistoActionsCount = purgeVM.HistoActionsCount, HistoEmailsCount = purgeVM.HistoEmailsCount, HistoBatchsCount = purgeVM.HistoBatchsCount, HistoUserQcmCount = purgeVM.HistoUserQcmCount }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ConfirmPurge(PurgeViewModel vm)
        {
            return PartialView("_ConfirmPurge", vm);
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult Purge(PurgeViewModel vm)
        {
            DateTime? limitDate = null;
            DateTime outLimitDate;
            if (DateTime.TryParse(vm.LimitDateString, out outLimitDate))
                limitDate = DateTime.Parse(vm.LimitDateString);

            if(vm.HistoActionsCount != 0)
            {
                DeleteHistoActionsRequest request = new DeleteHistoActionsRequest() { LimitDate = limitDate };
                _histoActionsService.Delete(request);
            }
            if(vm.HistoEmailsCount != 0)
            {
                DeleteHistoEmailsRequest request = new DeleteHistoEmailsRequest() { LimitDate = limitDate };
                _histoEmailsService.Delete(request);
            }
            if (vm.HistoBatchsCount != 0)
            {
                DeleteHistoBatchsRequest request = new DeleteHistoBatchsRequest() { LimitDate = limitDate };
                _histoBatchsService.Delete(request);
            }
            if (vm.HistoUserQcmCount != 0)
            {
                DeleteHistoUserQcmDocVersionRequest request = new DeleteHistoUserQcmDocVersionRequest() { LimitDate = limitDate };
                _histoUserQcmService.Delete(request);
            }

            return RedirectToAction("Index", new { limitDate = vm.LimitDateString });
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult PurgeHistoActions(string limitDateString)
        {
            DateTime limitDate = DateTime.MinValue;
            DateTime outLimitDate;
            if (DateTime.TryParse(limitDateString, out outLimitDate))
                limitDate = DateTime.Parse(limitDateString);

            DeleteHistoActionsRequest request = new DeleteHistoActionsRequest()
            {
                LimitDate = limitDate
            };

            _histoActionsService.Delete(request);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult PurgeHistoEmails(string limitDateString)
        {
            DateTime limitDate = DateTime.MinValue;
            DateTime outLimitDate;
            if (DateTime.TryParse(limitDateString, out outLimitDate))
                limitDate = DateTime.Parse(limitDateString);

            DeleteHistoEmailsRequest request = new DeleteHistoEmailsRequest()
            {
                LimitDate = limitDate
            };

            _histoEmailsService.Delete(request);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult PurgeHistoBatchs(string limitDateString)
        {
            DateTime limitDate = DateTime.MinValue;
            DateTime outLimitDate;
            if (DateTime.TryParse(limitDateString, out outLimitDate))
                limitDate = DateTime.Parse(limitDateString);

            DeleteHistoBatchsRequest request = new DeleteHistoBatchsRequest()
            {
                LimitDate = limitDate
            };

            _histoBatchsService.Delete(request);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult PurgeHistoUserQcm(string limitDateString)
        {
            DateTime limitDate = DateTime.MinValue;
            DateTime outLimitDate;
            if (DateTime.TryParse(limitDateString, out outLimitDate))
                limitDate = DateTime.Parse(limitDateString);

            DeleteHistoUserQcmDocVersionRequest request = new DeleteHistoUserQcmDocVersionRequest()
            {
                LimitDate = limitDate
            };

            _histoUserQcmService.Delete(request);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult PurgeAll(string limitDateString)
        {
            DateTime limitDate = DateTime.MinValue;
            DateTime outLimitDate;
            if (DateTime.TryParse(limitDateString, out outLimitDate))
                limitDate = DateTime.Parse(limitDateString);

            DeleteHistoActionsRequest requestHistoActions = new DeleteHistoActionsRequest() { LimitDate = limitDate };
            _histoActionsService.Delete(requestHistoActions);

            DeleteHistoEmailsRequest requestHistoEmails = new DeleteHistoEmailsRequest() { LimitDate = limitDate };
            _histoEmailsService.Delete(requestHistoEmails);

            DeleteHistoBatchsRequest requestHistoBatchs = new DeleteHistoBatchsRequest() { LimitDate = limitDate };
            _histoBatchsService.Delete(requestHistoBatchs);

            DeleteHistoUserQcmDocVersionRequest requestHistoUserQcm = new DeleteHistoUserQcmDocVersionRequest() { LimitDate = limitDate };
            _histoUserQcmService.Delete(requestHistoUserQcm);

            return RedirectToAction("Index");
        }
    }
}