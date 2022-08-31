using Intitek.Welcome.Infrastructure.Log;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace Intitek.Welcome.Service.Back
{
    public class MailService : BaseService, IMailService
    {
        private SmtpClient _smtpClient;
        public Int32 SmtpTimeout { get; set; }
        public Int32 MailIntervalDelay { get; set; }
        public HistoEmailBO HistoEmail { get; private set; }
       
        public MailService(ILogger logger, SmtpClient smtpClient) : base(logger)
        {
            //valeurs par défaut
            this.SmtpTimeout = smtpClient.Timeout;
            this.MailIntervalDelay = 200;
            _smtpClient = smtpClient;
            //logger.Info("Host : " + smtpClient.Host);
            //logger.Info("Port : " + smtpClient.Port);
            //logger.Info("UseDefaultCredentials : " + smtpClient.UseDefaultCredentials);
            HistoEmail = new HistoEmailBO(uow);
        }
        public void SetCredential(string user, string pwd)
        {
            var credential = new NetworkCredential(user, pwd);
            credential.UserName = user;
            credential.Password = pwd;
            //_logger.Info("mail credential :" + user);
            //_logger.Info("mail credential pwd :" + pwd);
            _smtpClient.UseDefaultCredentials = false;
            _smtpClient.Credentials = credential;
            _smtpClient.EnableSsl = true;
        }
        public void Send(MailMessage mail)
        {
            try
            {
                _smtpClient.Send(mail);
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "Send",
                    ServiceName = "MailService",

                }, ex);
                throw ex;
            }
        }
        public void SendBulk(MailMessage mail)
        {
            try
            {
                // An System.Int32 that specifies the time-out value in milliseconds. 
                //The default value is 100,000 (100 seconds).
                _smtpClient.Timeout = this.SmtpTimeout;
                var ta = Task.Run(() =>
                {
                    _smtpClient.Send(mail);
                    Thread.Sleep(this.MailIntervalDelay);
                });
                ta.Wait();
            }

            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "Send",
                    ServiceName = "MailService",
                    UserName = mail.To[0].Address,
                }, ex);
                throw ex;
            }
        }

        public void Historize()
        {
            try
            {
                HistoEmail.SaveHisto();
            }

            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "Historize",
                    ServiceName = "MailService",

                }, ex);
                throw ex;
            }

        }
    }
}
