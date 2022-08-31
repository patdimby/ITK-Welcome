using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Service.Back;
using Intitek.Welcome.Service.Back.DTO;
using Intitek.Welcome.UI.ViewModels.Admin;
using System.Web.Mvc;

namespace Intitek.Welcome.UI.Web.Areas.Admin.Controllers
{
    public class WelcomeMessageController : CommunController
    {
        private readonly Service.Back.IWelcomeService _welcomeServiceBack;
        private readonly Service.Front.IWelcomeService _welcomeServiceFront;

        public WelcomeMessageController()
        {
            _welcomeServiceBack = new Service.Back.WelcomeService(new FileLogger());
            _welcomeServiceFront = new Service.Front.WelcomeService(new FileLogger());
        }

        // GET: Admin/WelcomeMessage
        public ActionResult Index()
        {
            var _culture = Request.Cookies["_culture"] != null ? Request.Cookies["_culture"].Value : (Request.UserLanguages != null && Request.UserLanguages.Length > 0 ?
                        Request.UserLanguages[0] : "fr-FR");

            var welcomeMessage = _welcomeServiceFront.GetWelcomeMessageByLang(_culture.Substring(0, 2));

            WelcomeMessageViewModel vm = new WelcomeMessageViewModel()
            {
                ID_Lang = GetIdLang(),
                WelcomeMessage = string.IsNullOrEmpty(welcomeMessage) ? "" : welcomeMessage
            };
            vm.WelcomeMessageHtml = vm.WelcomeMessage;

            return View(vm);
        }

        [HandleError(View = "~/Areas/Admin/Views/Shared/AjaxError.cshtml")]
        public ActionResult AjaxWelcomeMessage(int ID_Lang)
        {
            var welcomeMessage = _welcomeServiceFront.GetWelcomeMessageByLang(ID_Lang);

            var userConnected = GetUserConnected();

            var result = string.IsNullOrEmpty(welcomeMessage) ? "" : welcomeMessage;

            return Json(new { Message = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult Index(WelcomeMessageViewModel model)
        {
            var request = new UpdateWelcomeMessageRequest()
            {
                WelcomeMessage = new WelcomeMessageDTO()
                {
                    WelcomeMessage = new WelcomeMessage()
                    {
                        ID_Lang = model.ID_Lang,
                        Message = model.WelcomeMessageHtml
                    }
                }
            };

            var response = _welcomeServiceBack.UpdateWelcomeMessage(request);

            var _culture = Request.Cookies["_culture"] != null ? Request.Cookies["_culture"].Value : (Request.UserLanguages != null && Request.UserLanguages.Length > 0 ?
                        Request.UserLanguages[0] : "fr-FR");

            var welcomeMessage = _welcomeServiceFront.GetWelcomeMessageByLang(_culture.Substring(0, 2));

            WelcomeMessageViewModel vm = new WelcomeMessageViewModel()
            {
                ID_Lang = GetIdLang(),
                WelcomeMessage = string.IsNullOrEmpty(welcomeMessage) ? "" : welcomeMessage
            };
            vm.WelcomeMessageHtml = vm.WelcomeMessage;

            return View(vm);
        }
    }
}