using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Service.Back;
using Intitek.Welcome.UI.ViewModels.Admin;
using System.Web.Mvc;

namespace Intitek.Welcome.UI.Web.Areas.Admin.Controllers
{
    public class ConfigController : CommunController
    {
        private readonly IConfigService _configService;

        public ConfigController()
        {
            _configService = new ConfigService(new FileLogger());
        }

        // GET: Admin/Config
        public ActionResult Index()
        {
            GetConfigRequest configRequest = new GetConfigRequest() { ConfigType = ConfigType.MAINTENANCE };

            var config = _configService.Get(configRequest);

            ConfigViewModel vm = new ConfigViewModel()
            {
                Id = config.Config.Id,
                Value = config.Config.Value
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateInput(true)]
        [HandleError(View = "~/Areas/Admin/Views/Shared/AjaxError.cshtml")]
        public ActionResult AjaxChangeMaintenanceMode(string value)
        {
            UpdateConfigRequest configRequest = new UpdateConfigRequest()
            {
                Config = new ConfigDTO()
                {
                    Id = ConfigType.MAINTENANCE,
                    Value = value
                }
            };

            _configService.UpdateConfig(configRequest);

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }
    }
}