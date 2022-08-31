using Intitek.Welcome.Infrastructure.Histo;
using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Service.Back;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Text;
using RazorEngine.Templating;
using System.IO;
using System;
using Intitek.Welcome.Infrastructure.Config;

namespace Astek.Welcome.Batch.Service
{
    public class SendStatistics : IBatch, IDisposable
    {
        private readonly MailService _mailService;
        private readonly ActiveDirectoryService _adUserService;
        private readonly UserService _userService;
        private IRazorEngineService _razor;

        public SendStatistics()
        {
            Intitek.Welcome.Infrastructure.Config.Config config;
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["ConfigurationFilePath"], "config.bin");
            if (!File.Exists(path))
            {
                var txtfilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["ConfigurationFilePath"], "config.txt");

                if (File.Exists(txtfilename)
                   /*&& !File.Exists(Server.MapPath("~/Configs/Config.bin"))*/)
                {
                    ConfigurationEncryption.EncryptConfigurationFile(txtfilename, path);
                }
            }
            config = Intitek.Welcome.Infrastructure.Config.Config.Deserialize(path);
            //_mailService.SetCredential(ConfigurationEncryption.DecryptConfig(config.DBUserID), ConfigurationEncryption.DecryptConfig(config.DBPassword));

            _mailService = new MailService(new FileLogger(), new SmtpClient()
            {
                Host = ConfigurationManager.AppSettings["SmtpHost"], // "smtp.astek.mu",
                Port = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]), //587,
                Credentials = new NetworkCredential(
                        ConfigurationEncryption.DecryptConfig(config.SMTPUser), ConfigurationEncryption.DecryptConfig(config.SMTPPassword))
                //Credentials = new NetworkCredential(
                //        ConfigurationManager.AppSettings["SmtpFromMail"], ConfigurationManager.AppSettings["SmtpPassword"])
            });
            _userService = new UserService(new FileLogger());
            _adUserService = new ActiveDirectoryService(new FileLogger());

            TemplateServiceConfiguration templateConfig = new TemplateServiceConfiguration();
            templateConfig.EncodedStringFactory = new RawStringFactory();
            _razor = RazorEngineService.Create(templateConfig);
            Engine.Razor = _razor;
        }

        public void Dispose()
        {
            ((IDisposable)_mailService).Dispose();
            ((IDisposable)_adUserService).Dispose();
            ((IDisposable)_userService).Dispose();
        }

        public BatchResponse Execute(BatchRequest request, Synthese synthese)
        {
            var batchResponse = new BatchResponse() {
                Errors = new List<string>(),
                Result = 0
            };

            var body = GenerateMailBody("SendStatisticsTemplate.cshtml", new SendStatisticModel()
            {
                AgencyName = "IFI",
                EntityName = "FRANCE",
                ItemsCount = string.Empty,
                ReadItemsCount = string.Empty,
                PercentItemsRead = "10",
                ItemsToApproveCount = string.Empty,
                ApprovedItemsCount = string.Empty,
                PercentItemsApproved = "20",
                ItemsToTestCount = string.Empty,
                TestedPassedItemsCount = string.Empty,
                PercentItemsTested = "50"

            });

            _mailService.HistoEmail.Emails.Add(new EmailBO()
            {
                ID_Batch = request.ID_Batch,
                Id_IntitekUser = 0,
                
                Message = string.Format("Objet: [{0}] <br> {1}", string.Empty, string.Empty)
            });

            _mailService.Historize();

            return batchResponse;
        }

        
        private string GenerateMailBody(string fileTemplate, SendStatisticModel model)
        {
            string path = string.Format("D:\\Projects\\Intitek\\source\\Astek.Welcome.Batch.Service\\{0}",  fileTemplate);
            string template = System.Text.Encoding.UTF8.GetString(System.IO.File.ReadAllBytes(path));
            string returnedView = Engine.Razor.RunCompile(template, "SendStatistics", typeof(SendStatisticModel), model, null);
            
            return returnedView;
        }
    }
}
