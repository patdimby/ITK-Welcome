using Intitek.Welcome.Infrastructure.Config;
using Intitek.Welcome.Infrastructure.Histo;
using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Service.Back;
using Intitek.Welcome.UI.Resources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Astek.Welcome.Batch.Service
{
    public class SyntheseAD
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public int NbUsersAD { get; set; }
        public int NbUsersEmptyMailAD { get; set; }
        public int NbUsersDiscardedAD { get; set; }
        public int NbUsersBlackListAD { get; set; }
        public int NbUsersDeletedDB { get; set; }
        public int NbUsersUpdatedDB { get; set; }
        public int NbUsersAddedDB { get; set; }
        public List<string> Errors { get; set; }
        public SyntheseAD()
        {
            this.Errors = new List<string>();
        }
        public string GetString(bool bHtml)
        {
            var str = Environment.NewLine + string.Format("AD {0}({1})", Name, Address) + Environment.NewLine;
            if (bHtml)
            {
                str = string.Format("<br/><b>AD {0}({1})</b>", Name, Address) + Environment.NewLine;
            }
            str+= string.Format("Nombre d'utilisateurs AD : {0}", NbUsersAD + NbUsersDiscardedAD) + Environment.NewLine;
            str += string.Format("Nombre d'utilisateurs AD à synchroniser : {0}", NbUsersAD) + Environment.NewLine;
            str += string.Format("Nombre d'utilisateurs AD avec E-mail vide : {0}", NbUsersEmptyMailAD) + Environment.NewLine;
            str += string.Format("Nombre d'utilisateurs AD Blacklistés : {0}", NbUsersBlackListAD) + Environment.NewLine;
            str += string.Format("Nombre d'utilisateurs AD supprimés de la base : {0}", NbUsersDeletedDB) + Environment.NewLine;
            str += string.Format("Nombre d'utilisateurs AD ajoutés à la base : {0}", NbUsersAddedDB) + Environment.NewLine;
            str += string.Format("Nombre d'utilisateurs AD modifiés à la base : {0}", NbUsersUpdatedDB) + Environment.NewLine;           
            if (this.Errors.Count() > 0)
            {
                str += string.Format("Quelques erreurs :") + Environment.NewLine;
            }
            if (bHtml)
            {
                str = str.Replace(Environment.NewLine, "<br/>");
            }
            foreach (var error in this.Errors)
            {
                if (bHtml)
                {
                    str += "<span style=\"margin-left:20px\">" + error + "</span><br/>";
                }
                else
                {
                    str += "\t" + error + Environment.NewLine;
                }
            }
            return str;

        }
    }
    public class Synthese
    {
        public MailService MailService { get; private set; }
        public string Subject { get; set; }
        public DateTime Debut { get; set; }
        public DateTime Fin { get; set; }
        public int NbPersonnes { get; set; }
        public int NbEmailSend { get; set; }
        public int NbEmailErrors { get; set; }
        public List<string> Errors { get; set; }
        public List<SyntheseAD> SynchronizeADs { get; set; }
        public Synthese()
        {
            this.Errors = new List<string>();
            this.SynchronizeADs = new List<SyntheseAD>();
            MailService = new MailService(new FileLogger(), new SmtpClient()
            {
                Host = ConfigurationManager.AppSettings["SmtpHost"], // "smtp.astek.mu",
                Port = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]), //587,
            });

            Intitek.Welcome.Infrastructure.Config.Config config;
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["ConfigurationFilePath"], "config.bin");
            config = Intitek.Welcome.Infrastructure.Config.Config.Deserialize(path);
            MailService.SetCredential(ConfigurationEncryption.DecryptConfig(config.SMTPUser), ConfigurationEncryption.DecryptConfig(config.SMTPPassword));
            //MailService.SetCredential(ConfigurationManager.AppSettings["SmtpUsername"], ConfigurationManager.AppSettings["SmtpPassword"]);
        }
        public string SynchronizeADString(bool bHtml)
        {
            string hostName = Dns.GetHostName();
            string myIP = GetIpv4(hostName);
            var str = string.Format(Resource.batchHostIPSendMail, hostName, myIP) + Environment.NewLine;
            str += string.Format(Resource.batch_Debut, Debut.ToString("dd/MM/yyyy HH:mm:ss")) + Environment.NewLine;
            str += string.Format(Resource.batch_Fin, Fin.ToString("dd/MM/yyyy HH:mm:ss")) + Environment.NewLine;
            foreach (var synchro in SynchronizeADs)
            {
                str+=synchro.GetString(bHtml) + Environment.NewLine;
            }
            if (this.Errors.Count() > 0)
            {
                str += Resource.batch_DetailError + Environment.NewLine;
            }
            else
            {
                str += Resource.batch_Success + Environment.NewLine;
            }
            if (bHtml)
            {
                str = str.Replace(Environment.NewLine, "<br/>");
            }
            foreach (var error in this.Errors)
            {
                if (bHtml)
                {
                    str += "<span style=\"margin-left:20px;\">" + error + "</span><br/>";
                }
                else
                {
                    str += "\t" + error + Environment.NewLine;
                }
            }
            return str;
         }
        private string GetIpv4(string hostName)
        {
            var myIPs = Dns.GetHostEntry(hostName).AddressList;
            foreach (var ip in myIPs)
            {
                if (ip.AddressFamily.Equals(AddressFamily.InterNetwork))
                {
                    return ip.ToString();
                }
            }
            return null;
        }
        public string GetString(bool bHtml)
        {
            string hostName = Dns.GetHostName(); 
            string myIP = GetIpv4(hostName);
            var str = string.Format(Resource.batchHostIPSendMail, hostName, myIP) + Environment.NewLine;
            str += string.Format(Resource.batch_Debut, Debut.ToString("dd/MM/yyyy HH:mm:ss")) + Environment.NewLine;
            str += string.Format(Resource.batch_Fin, Fin.ToString("dd/MM/yyyy HH:mm:ss")) + Environment.NewLine;
            str += string.Format(Resource.batch_NbPersonnes, NbPersonnes) + Environment.NewLine;
            str += string.Format(Resource.batch_NbSendMail, NbEmailSend) + Environment.NewLine;
            str += string.Format(Resource.batch_NbErrors, NbEmailErrors) + Environment.NewLine;
            if (this.Errors.Count() > 0)
            {
                str += Resource.batch_DetailError + Environment.NewLine;
            }
            else
            {
                str += Resource.batch_Success + Environment.NewLine;
            }
            if (bHtml)
            {
                str = str.Replace(Environment.NewLine, "<br/>");
            }
            foreach (var error in this.Errors)
            {
                if (bHtml)
                {
                    str += "<span style=\"margin-left:20px\">" + error + "</span><br/>";
                }
                else
                {
                    str += "\t" + error + Environment.NewLine;
                }
            }
            return str;
            
        }
        public void SendMail(int idBatch)
        {
            var remindMail = new MailMessage();
            remindMail.From = new MailAddress(ConfigurationManager.AppSettings["SmtpFromMail"]);
            remindMail.To.Add(ConfigurationManager.AppSettings["MailToSynthese"]);
            remindMail.Body = this.GetString(true);
            remindMail.IsBodyHtml = true;
            remindMail.Subject = this.Subject;

            MailService.Send(remindMail);
            MailService.HistoEmail.Emails.Add(new EmailBO()
            {
                ID_Batch = idBatch,
                Message = remindMail.Body
            });
            MailService.Historize();
        }
        public void SendMailSynchronizeAD(int idBatch)
        {
            var remindMail = new MailMessage();
            remindMail.From = new MailAddress(ConfigurationManager.AppSettings["SmtpFromMail"]);
            remindMail.To.Add(ConfigurationManager.AppSettings["MailToSynthese"]);
            remindMail.Body = SynchronizeADString(true);
            remindMail.IsBodyHtml = true;
            remindMail.Subject = this.Subject;

            MailService.Send(remindMail);
            MailService.HistoEmail.Emails.Add(new EmailBO()
            {
                ID_Batch = idBatch,
                Message = remindMail.Body
            });
            MailService.Historize();
        }
    }
}
