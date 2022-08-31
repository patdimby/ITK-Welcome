using Intitek.Welcome.UI.Web.Areas.Admin;
using Intitek.Welcome.Infrastructure.Config;
using System;
using System.IO;
using System.Security.Claims;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Intitek.Welcome.UI.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(MvcApplication));
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
            AutoMapperConfig.RegisterMappings();
            AutoMapperConfigAdmin.RegisterMappings();
            log4net.Config.XmlConfigurator.Configure();
            if (!File.Exists(Server.MapPath("~/Configs/Config.bin"))
                /*&& !File.Exists(Server.MapPath("~/Configs/Config.bin"))*/)
            {
                ConfigurationEncryption.EncryptConfigurationFile(Server.MapPath("~/Configs/Config.txt"), Server.MapPath("~/Configs/Config.bin"));              
                //File.Delete("Configs\\Config.txt");
                //log.Info("App_Start...");
            }
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError().GetBaseException();
            log.Error("App_Error", ex);
        }

    }
}