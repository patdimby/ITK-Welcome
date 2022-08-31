using Intitek.Welcome.Infrastructure.Config;
using Intitek.Welcome.Infrastructure.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.Infrastructure
{
    public class Mail : IDisposable
    {
        private bool _IsMailTest;
        private string _MailTest ;
        private ILogger _logger;
        private string _MailHost;
        private int _MailPort;
        public string MailFrom { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        private const int DEFAULT_MAIL_PORT = 25;
        private SmtpClient _SmtpClient;
        protected Infrastructure.Config.Config config;

        public Mail(ILogger logger)
        {
            _logger = logger;
            _MailHost = ConfigurationManager.AppSettings["SmtpHost"];
            _MailPort = 0;
            int.TryParse(ConfigurationManager.AppSettings["SmtpPort"], out _MailPort);
            if (_MailPort == 0)
                _MailPort = DEFAULT_MAIL_PORT;

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["ConfigurationFilePath"], "config.bin");
            config = Infrastructure.Config.Config.Deserialize(path);

            MailFrom = ConfigurationManager.AppSettings["SmtpFromMail"];
            Password = ConfigurationEncryption.DecryptConfig(config.SMTPPassword); // ConfigurationManager.AppSettings["SmtpPassword"];
            Username = ConfigurationEncryption.DecryptConfig(config.SMTPUser); // ConfigurationManager.AppSettings["SmtpUsername"];

            _IsMailTest = false;
            if(ConfigurationManager.AppSettings["SmtpIsMailTest"]!=null)
                _IsMailTest = Convert.ToBoolean(ConfigurationManager.AppSettings["SmtpIsMailTest"]);
            _MailTest = ConfigurationManager.AppSettings["SmtpMailTestRelance"];

            this._SmtpClient = new SmtpClient();
            /* Recuperer depuis la conf smtpmail de IIS     */
            this._SmtpClient.Host = this._MailHost;
            this._SmtpClient.Port = this._MailPort;
            
            var credential = new System.Net.NetworkCredential();
            credential.UserName = Username;
            credential.Password = Password;
            //_logger.Info("mail credential :" + user);
            //_logger.Info("mail credential pwd :" + pwd);
            this._SmtpClient.UseDefaultCredentials = true;
            this._SmtpClient.Credentials = credential;
            this._SmtpClient.EnableSsl = true;
        }
        public SmtpClient SmtpClient
        {
            get
            {
                return _SmtpClient;
            }
        }
        public void SendMailDepuisServer(string emailTo, string body, string subject, bool isHtml)
        {
            var sendTo = emailTo;
            if (_IsMailTest)
            {
                sendTo = _MailTest;
            }
            var mailMessage = CreateMessage(sendTo, body, subject, isHtml);
            if (mailMessage != null)
            {
                _logger.Info("Envoi de message vers " + sendTo + "....");
                try
                {
                    this._SmtpClient.Send(mailMessage);
                    mailMessage.Dispose();
                    _logger.Info("Le Message a été envoyé avec succès.");
                }
                catch (Exception ex)
                {
                    _logger.Error(new ExceptionLogger()
                    {
                        ExceptionDateTime = DateTime.Now,
                        ExceptionStackTrack = ex.StackTrace,
                        MethodName = "SendMailDepuisServer",
                        ServiceName = "Mail",
                    }, ex);
                    throw ex;
                }
            }
        }
        
        private MailMessage CreateMessage(string emailTo, string body, string subject, bool isHtml)
        {

            if (string.IsNullOrEmpty(emailTo))
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = "Email To is null",
                    MethodName = "SendMailDepuisServer",
                    ServiceName = "Mail",
                }, new Exception("Email To is null"));
                return null;
            }
            // Prepare le mail
            var mail = new MailMessage();           
            mail.To.Add(emailTo) ;
            if (!string.IsNullOrEmpty(this.MailFrom))
            {
                mail.From = new MailAddress(this.MailFrom);
            }
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = isHtml;
            mail.DeliveryNotificationOptions = DeliveryNotificationOptions.Delay |
                                                DeliveryNotificationOptions.OnFailure |
                                                DeliveryNotificationOptions.OnSuccess;

            return mail;
        }
        public void Dispose()
        {
            this._SmtpClient.Dispose();
        }

    }
}
