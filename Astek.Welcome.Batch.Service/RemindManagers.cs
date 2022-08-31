using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.Config;
using Intitek.Welcome.Infrastructure.Histo;
using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Service.Back;
using Intitek.Welcome.UI.Resources;

namespace Astek.Welcome.Batch.Service
{
    public class RemindManagers : IBatch, IDisposable
    {
        private string _mailName;
        private List<string> _emailList;
        private readonly MailTemplateService _mailTemplateService;
        private readonly MailKeywordsService _mailKeywordService;
        private readonly MailService _mailService;
        private readonly UserService _userService;
        private readonly IStatsService _statsService;
        private WelcomeDB Context;
        private readonly ILogger _logger;

        public RemindManagers(string templateMailName, string emailList)
        {
            this._logger = new FileLogger();
            _mailName = templateMailName;
            if (!string.IsNullOrEmpty(emailList))
            {
                _emailList = emailList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.ToLower()).ToList();
            }
            _mailTemplateService = new MailTemplateService(this._logger);
            _mailKeywordService = new MailKeywordsService(this._logger);
            _mailService = new MailService(this._logger, new SmtpClient()
            {
                Host = ConfigurationManager.AppSettings["SmtpHost"], // "smtp.astek.mu",
                Port = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]), //587,
            });
            _mailService.SmtpTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpTimeout"]);
            _mailService.MailIntervalDelay = Convert.ToInt32(ConfigurationManager.AppSettings["SendMailIntervalDelay"]);

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

            Context = new WelcomeDB(ConfigurationEncryption.DecryptConfig(config.DBServer),
                ConfigurationEncryption.DecryptConfig(config.DBName),
                ConfigurationEncryption.DecryptConfig(config.DBUserID),
                ConfigurationEncryption.DecryptConfig(config.DBPassword));

            _mailService.SetCredential(ConfigurationEncryption.DecryptConfig(config.SMTPUser), ConfigurationEncryption.DecryptConfig(config.SMTPPassword));
            //_mailService.SetCredential(ConfigurationManager.AppSettings["SmtpUsername"], ConfigurationManager.AppSettings["SmtpPassword"]);

            _userService = new UserService(new FileLogger());
            _statsService = new StatsService(new FileLogger());
        }

        private string addORQuery(string where, string addWhere)
        {
            if (!string.IsNullOrEmpty(where))
            {
                where = where + " OR " + addWhere;
            }
            else
                where = where + addWhere;
            return where;
        }

        private List<UserDTO> GetListAllDocumentByUser(MailTemplateDTO mailTemplate)
        {
            //utilisateur active
            var sql = RemindSQL.SQL_STATS ;
            var whereUserInactive = " (tbl.[InactivityStart] IS NOT NULL) AND((cast(cast(tbl.[InactivityStart] as date) as datetime2)) <= (cast(cast(GETDATE() as date) as datetime2))) " +
              " AND((tbl.[InactivityEnd] IS NULL) OR((tbl.[InactivityEnd] IS NOT NULL) AND((cast(cast(tbl.[InactivityEnd] as date) as datetime2)) >= (cast(cast(GETDATE() as date) as datetime2)))))";
            sql = sql + " and " + string.Format("NOT({0})", whereUserInactive);
            if (_emailList != null && _emailList.Count() > 0)
            {
                var emails = string.Join(",", _emailList.Select(x => string.Format("'{0}'", x)).ToArray());
                sql += string.Format(" AND (EXISTS(SELECT ID from IntitekUser where tbl.ID_Manager = ID and LOWER([Email]) IN({0})))", emails);

            }
            //418 : Lorsque le template est utilisé par le batch, il ne déclenche une relance que pour les documents présents dans le(s) catégorie(s) ou sous-catégorie(s) choisies.
            var whereCateg = "";
            if (mailTemplate.IsGlobal==false)
            {
                if (mailTemplate.IsDocNoCategory)
                {
                    whereCateg = addORQuery(whereCateg, "(tbl.[ID_Category] IS NULL OR tbl.[ID_Category]='')") ;
                }
                if (mailTemplate.IsDocNoSubCategory)
                {
                    whereCateg = addORQuery(whereCateg, "(tbl.[ID_SubCategory] IS NULL OR tbl.[ID_SubCategory]='')");
                }
                if (mailTemplate.Categories != null && mailTemplate.Categories.Any())
                {
                    var categs = string.Join(",", mailTemplate.Categories.Select(x => x.ID).ToArray());
                    whereCateg = addORQuery(whereCateg, string.Format("tbl.[ID_Category] IN ({0})", categs));
                }
                if (mailTemplate.SubCategories != null && mailTemplate.SubCategories.Any())
                {
                    var subcategs = string.Join(",", mailTemplate.SubCategories.Select(x => x.ID).ToArray());
                    whereCateg = addORQuery(whereCateg, string.Format("tbl.[ID_SubCategory] IN ({0})", subcategs));
                }
                if (!string.IsNullOrEmpty(whereCateg))
                {
                    sql += " AND (" + whereCateg + ")";
                }
            }
           
            List<UserDTO> managers = new List<UserDTO>();
            var query = Context.Database.SqlQuery<StatistiquesDTO>(sql).AsQueryable();
            //***on récupère le nombre exact de ToApproved, ToTested
            //query = query.Where(x => !x.IsRead.HasValue ||
            //       (x.Approbation.HasValue && x.Approbation == 1 && !x.IsApproved.HasValue) ||
            //       (x.Test.HasValue && x.Test == 1 && !x.IsTested.HasValue));
            var stats = query.ToList();
            if (stats != null && stats.Count > 0)
            {
                var userManagerIds = stats.Where(x=> x.ID_Manager.HasValue).GroupBy(x => new { ID_Manager = x.ID_Manager.Value}).Select(x => x.Key.ID_Manager).ToList();
                List<UserDTO> userManagers = _statsService.ListUsersForStat(userManagerIds, false, true, StatsRequestType.Manager);
                foreach (var manager in userManagers)
                {
                    var statsManager = stats.Where(x => x.ID_Manager.HasValue && x.ID_Manager.Value == manager.ID).ToList();
                    if (statsManager!=null & statsManager.Any())
                    {
                        List<Statistiques> statistques = new List<Statistiques>();
                        List<UserDTO> users = _statsService.ListUsersForStat(statsManager.Select(x => x.IdUser).Distinct().ToList(), false, true, StatsRequestType.Manager);
                        foreach (var user in users)
                        {
                            List<StatistiquesDTO> statsUser = statsManager.Where(x => x.IdUser == user.ID).ToList();
                            Statistiques st = new Statistiques();
                            st.UserId = user.ID;
                            st.Name = user.FullName;
                            st.NotRead = statsUser.Where(x => !x.IsRead.HasValue).Count();
                            st.NotApproved = statsUser.Where(x => (x.Approbation.HasValue && x.Approbation == 1) && !x.IsApproved.HasValue).Count();
                            st.NotTested = statsUser.Where(x => (x.Test.HasValue && x.Test == 1) && !x.IsTested.HasValue).Count();

                            st.ToRead = statsUser.Count();
                            st.ToApproved = statsUser.Where(x => x.Approbation.HasValue && x.Approbation == 1).Count();
                            st.ToTested = statsUser.Where(x => x.Test.HasValue && x.Test == 1).Count();
                            if (st.NotRead > 0 || st.NotApproved > 0 || st.NotTested > 0)
                            {
                                List<int> docsId = statsUser.Select(x => x.ID).Distinct().ToList();
                                List<DocumentDTO> documents = _statsService.ListDocumentsByDocs(1, 1, docsId);
                                List<StatistiqueDocs> statUserDocs = new List<StatistiqueDocs>();
                                foreach (var doc in documents)
                                {
                                    List<StatistiquesDTO> statDoc = statsUser.Where(x => x.ID == doc.ID).ToList();
                                    var toRead = statDoc.Count();
                                    var toApproved = statDoc.Where(x => x.Approbation.HasValue && x.Approbation == 1).Count();
                                    var toTested = statDoc.Where(x => x.Test.HasValue && x.Test == 1).Count();
                                    var notRead = 0;
                                    var notAproved = 0;
                                    var notTested = 0;

                                    StatistiqueDocs stDoc = new StatistiqueDocs();
                                    stDoc.ID = doc.ID;
                                    stDoc.Name = doc.Name;
                                    notRead = statDoc.Where(x => !x.IsRead.HasValue).Count();
                                    if (toApproved > 0)
                                    {
                                        notAproved = statDoc.Where(x => (x.Approbation.HasValue && x.Approbation == 1) && !x.IsApproved.HasValue).Count();
                                    }
                                    if (toTested > 0)
                                    {
                                        notTested = statDoc.Where(x => (x.Test.HasValue && x.Test == 1) && !x.IsTested.HasValue).Count();
                                    }
                                    stDoc.NotRead = notRead;
                                    stDoc.NotApproved = notAproved;
                                    stDoc.NotTested = notTested;
                                    stDoc.ToRead = toRead;
                                    stDoc.ToApproved = toApproved;
                                    stDoc.ToTested = toTested;

                                    stDoc.LibRead = Resource.AlreadyDone;
                                    stDoc.LibApproved = "";
                                    stDoc.LibTested = "";
                                    if (stDoc.ToRead > 0)
                                    {
                                        if (stDoc.NotRead > 0)
                                        {
                                            stDoc.LibRead = Resource.toDO;
                                            if (stDoc.ToApproved > 0)
                                            {
                                                stDoc.LibApproved = Resource.toDO;
                                            }
                                            if (stDoc.ToTested > 0)
                                            {
                                                stDoc.LibTested = Resource.toDO;
                                            }
                                        }
                                        else
                                        {
                                            if (stDoc.ToApproved > 0)
                                            {
                                                stDoc.LibApproved = Resource.AlreadyDone;
                                                if (stDoc.NotApproved>0)
                                                {
                                                    stDoc.LibApproved = Resource.toDO;
                                                }
                                            }
                                            if (stDoc.ToTested > 0)
                                            {
                                                stDoc.LibTested = Resource.AlreadyDone;
                                                if (stDoc.NotTested>0)
                                                {
                                                    stDoc.LibTested = Resource.toDO;
                                                }
                                            }
                                        }
                                    }
                                    statUserDocs.Add(stDoc);
                                }
                                st.StatistiqueDocs = statUserDocs;
                                statistques.Add(st);
                            }
                        }
                        if (statistques.Any())
                        {
                            manager.Statistiques = statistques;
                            managers.Add(manager);
                        }
                    }
                   
                }            
            }
            return managers;
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;   
            }
            int check = 0;
            foreach (char c in email)
            {
                if (c == '@')
                {
                    check++;
                }
            }
            return check == 1 && email.Split('@')[0].Length > 1 && email.Split('@')[1].Length > 1;
        }

        private bool GetTestMode()
        {
            var bTestMode = false;
            var testModeConfig = ConfigurationManager.AppSettings["TestMode"];
            if (!string.IsNullOrEmpty(testModeConfig))
            {
                Boolean.TryParse(testModeConfig, out bTestMode);
            }
            return bTestMode;
        }

        public BatchResponse Execute(BatchRequest request, Synthese synthese)
        {
            var testModeRecipient = ConfigurationManager.AppSettings["TestModeRecipient"];
            var batchResponse = new BatchResponse()
            {
                Result = 0,
                Errors = new List<string>()
            };
            var keywords = _mailKeywordService.GetAll().MailKeywords.Select(mk => mk.Code).ToList();
            var mail = _mailTemplateService.Get(new GetMailTemplateRequest() { TemplateName = _mailName });
            var lst = GetListAllDocumentByUser(mail.MailTemplate);
                  
            if (string.IsNullOrEmpty(mail.MailTemplate.Content))
            {
                _logger.Error(string.Format("Le contenu du mailTemplate {0} ne doit pas être vide", _mailName));
                batchResponse.Result = 1;
                return batchResponse;
            }
            lst = (_emailList != null && _emailList.Count() > 0) ? lst.Where(l => l.Email != null && _emailList.Contains(l.Email.ToLower())).ToList() : lst;
            synthese.NbPersonnes = lst.Count;
            foreach (var user in lst)
            {
                var name = string.Format("{0}", user.FullName).Trim();
                var mv = new MailVariablesDTO()
                {
                    User = user
                };


                var emailAddress = user.Email;
                if (!string.IsNullOrEmpty(user.Email))
                {
                    var mailMessage = FillMessage(mail.MailTemplate.Content, keywords, mv);
                    var remindMail = new MailMessage();

                    remindMail.From = new MailAddress(ConfigurationManager.AppSettings["SmtpFromMail"]);
                    bool isValidMail = false;
                    var subject = "";
                    if (this.GetTestMode())
                    {
                        if (this.IsValidEmail(testModeRecipient))
                        {
                            //_logger.Info("adress mail send to :" + testModeRecipient);
                            remindMail.To.Add(testModeRecipient);
                            subject = string.Format("[welcome test {0}] {1}", emailAddress, mail.MailTemplate.Object);
                            isValidMail = true;
                        }
                        else
                        {
                            var ex = new Exception("L'adresse email TestModeRecipient est invalide dans le fichier de configuration!");
                            _logger.Error(new ExceptionLogger()
                            {
                                ExceptionDateTime = DateTime.Now,
                                ExceptionMessage = ex.Message,
                                MethodName = "Execute",
                                ServiceName = "RemindManagers",

                            }, ex);
                            throw ex;
                        }
                    }
                    else
                    {
                        if (IsValidEmail(emailAddress))
                        {
                            //_logger.Info("adress mail send to :" + emailAddress);
                            remindMail.To.Add(emailAddress);
                            subject = mail.MailTemplate.Object;
                            isValidMail = true;
                        }
                        else
                        {
                            _logger.Info(string.Format("L'utilisateur {0} : Adresse email non valide '{1}'." + Environment.NewLine + "Veuillez contacter l'administrateur pour corriger cette adresse !", user.FullName, emailAddress));
                            //Console.WriteLine("L'utilisateur {0} : Adresse email non valide '{1}'." + Environment.NewLine + "Veuillez contacter l'administrateur pour corriger cette adresse !", user.FullName, emailAddress);
                        }
                    }

                    remindMail.Body = this.GetBody(mailMessage);
                    remindMail.IsBodyHtml = true;
                    remindMail.Subject = subject;
                    if (isValidMail)
                    {
                        _mailService.SendBulk(remindMail);
                        synthese.NbEmailSend = synthese.NbEmailSend + 1;
                        _mailService.HistoEmail.Emails.Add(new EmailBO()
                        {
                            ID_Batch = request.ID_Batch,
                            Id_IntitekUser = user.ID,
                            Id_MailTemplate = mail.MailTemplate.Id,
                            Message = string.Format("Objet: [{0}] <br> {1}", mail.MailTemplate.Object, mailMessage)
                        });
                    }
                    else
                    {
                        _mailService.HistoEmail.Emails.Add(new EmailBO()
                        {
                            ID_Batch = request.ID_Batch,
                            Id_IntitekUser = user.ID,
                            Id_MailTemplate = mail.MailTemplate.Id,
                            Message = string.Format("L'utilisateur {0} : Adresse email non valide '{1}'.<br/>Veuillez contacter l'administrateur pour corriger cette adresse !", user.FullName, emailAddress)
                        });
                        synthese.Errors.Add(string.Format("L'utilisateur {0} : Adresse email non valide '{1}'", user.FullName, emailAddress));
                    }
                }

            }

            _mailService.Historize();
            return batchResponse;

        }
              
        private string GetBody(string body)
        {
            string str = "<!doctype html><html>" + 
             "<head>"+
             "<meta charset=\"utf-8\">"+ 
             "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1, shrink-to-fit=no\">"+
             "   <link href='https://fonts.googleapis.com/css?family=Poppins' rel='stylesheet'>" +
             "    <style type=\"text/css\" >" +
             "        .table {" +
             "            font-family:'Poppins';" +
             "            font-size:10pt;" +
             "            border-collapse:collapse;" +
             "        }" +
             "       .table td, .table th {" +
             "            font-weight:normal;" +
             "            padding: 4px 8px;" +
             "       }" +
             "       .grid-header {" +
             "           color: #333;" +
             "           font-weight:bold !important;" +
             "           padding-right:0.5em;" +
             "        }" +
             "        tr.paire td {" +
             "           background-color: #e3e3e3;" +
             "       }" +
             "       table .progress {"+
             "          width:100%;" +
             "          height:26px;" +
             "       }" +
             "       table .progress td {" +
             "          margin:0px;" +
             "          padding:0px;" +
             "       }" +
             "       table td.progress-danger {" +
             "           background-color:#35bf35 !important;" +
             "        }" +
             "       table td.progress-warning {" +
             "           background-color:#35bf35 !important;" +
             "        }" +
             "       table td.progress-success {" +
             "           background-color:#009c49 !important;" +
             "       }" +
             "       .progress-bar {" +
             "           height:26px;" +
             "           text-align:left;" +
             "           color:#fff;" +
             "           white-space: nowrap;" +
             "       }" +
             "       table td.progress-bar-danger{" +
             "            background-color:#ff0000 !important;" + //red
             "        }" +
             "       table td.progress-bar-warning {" +
             "            background-color:#ffa500 !important;" + //orange
             "        }" +
             "       table td.progress-bar-success {" +
             "            background-color:#009c49 !important;" +
             "       }" +
             "       .tablerelance {" +
             "            font-family:'Poppins';" +
             "            font-size:10pt;" +
             "            border-collapse:collapse;" +
             "       }" +
             "      .tablerelance td, .tablerelance th {" +
             "            font-weight:normal;" +
             "            padding:4px 8px;" +
             "            border: 1px solid #00c072;" +
             "      }" +
             "      .tablerelance th.grid-header {" +
             "           color: #fff;" +
             "           background-color:#00c072;" +
             "           font-weight:bold !important;" +
             "           padding-right:0.5em;" +
             "      }" +
             "      tr.paire td {" +
             "           background-color: #e3e3e3;" +
             "      }" +
             "   </style>" +
            "</head>" +
            "<body>" + body + "</body>" +"</html>";
            return str;

        }

        private string FillMessage(string message, List<string> keywords, MailVariablesDTO mv)
        {
            var result = message;
            foreach (var keyword in keywords)
            {
                switch (keyword)
                {
                    case "Manager":
                        result = result.Replace(string.Format("[{0}]", keyword), mv.User.FullName);
                        break;
                    case "Collaborateur":
                        result = result.Replace(string.Format("[{0}]", keyword), string.Format("{0}", mv.User.FullName));
                        break;
                    case "Agence":
                        result = result.Replace(string.Format("[{0}]", keyword), mv.User.AgencyName);
                        break;
                    case "Entite":
                        result = result.Replace(string.Format("[{0}]", keyword), mv.User.EntityName);
                        break;
                    case "Jour":
                        result = result.Replace(string.Format("[{0}]", keyword), DateTime.Now.Date.ToString());
                        break;
                    case "Liste_Collaborateurs":
                        result = result.Replace(string.Format("[{0}]", keyword), this.GetTableCollaborator(mv.User.Statistiques));
                        break;
                    case "Liste_Collaborateurs_Detaillee":
                        result = result.Replace(string.Format("[{0}]", keyword), this.GetTableCollaboratorDetails(mv.User.Statistiques));
                        break;
                    default:
                        result = result.Replace(string.Format("[{0}]", keyword), string.Empty);
                        break;

                }
            }
            return result;
        }

        private string GetTableCollaborator(List<Statistiques> stats)
        {
            if (!stats.Any()) return string.Empty;
            var str = "<table class=\"table\">" +
                   "<thead>" +
                   "    <tr>" +
                   "        <th class=\"grid-header\"><span>" + @Resource.NameCollabotor + "</span></th>" +
                   "        <th class=\"grid-header\"><span>" + @Resource.NbDocRead + "</span></th>" +
                   "        <th class=\"grid-header\"><span>" + @Resource.NbDocApproved + "</span></th>" +
                   "        <th class=\"grid-header\"><span>" + @Resource.NbDocTested + "</span></th>" +
                   "    </tr>" +
                   "</thead>" +
                   "<tbody>";
            int i = 1;
            foreach (var stat in stats)
            {
                var classe = "";
                if (i % 2 == 0)
                {
                    classe = "paire";
                }
                str+= "    <tr class=\""+ classe + "\">";
                str += "    <td>"+ stat.Name +"</td>";
                str += "    <td>" + this.ProgressBar(stat.NotRead, stat.ToRead, 200) + "</td>";
                str += "    <td>" + this.ProgressBar(stat.NotApproved, stat.ToApproved, 200) + "</td>";
                str += "    <td>" + this.ProgressBar(stat.NotTested, stat.ToTested, 200) + "</td>";
                str += "   </tr>";
                i++;
            }                   
            str+= "</tbody>" +
                "</table>";
            return str;
        }

        private string GetTableCollaboratorDetails(List<Statistiques> stats)
        {
            if (!stats.Any()) return string.Empty;
            var str = "<table class=\"table\">" +
                   "<thead>" +
                   "    <tr>" +
                   "        <th class=\"grid-header\"><span>" + @Resource.NameCollabotor + "</span></th>" +
                   "        <th class=\"grid-header\"><span>" + @Resource.NbDocRead + "</span></th>" +
                   "        <th class=\"grid-header\"><span>" + @Resource.NbDocApproved + "</span></th>" +
                   "        <th class=\"grid-header\"><span>" + @Resource.NbDocTested + "</span></th>" +
                   "    </tr>" +
                   "</thead>" +
                   "<tbody>";
            int i = 1;
            foreach (var stat in stats)
            {
                var classe = "paire";
                //if (i % 2 == 0)
                //{
                //    classe = "paire";
                //}
                str += "    <tr class=\"" + classe + "\">";
                str += "    <td>" + stat.Name + "</td>";
                str += "    <td>" + this.ProgressBar(stat.NotRead, stat.ToRead, 200) + "</td>";
                str += "    <td>" + this.ProgressBar(stat.NotApproved, stat.ToApproved, 200) + "</td>";
                str += "    <td>" + this.ProgressBar(stat.NotTested, stat.ToTested, 200) + "</td>";
                str += "   </tr>";
                if(stat.StatistiqueDocs!=null && stat.StatistiqueDocs.Any())
                {
                    str += "    <tr>";
                    str += "    <td>&nbsp;</td>";
                    str += "    <td colspan=\"3\" style=\"padding-bottom:15px;\">" + GetTableDocument(stat.StatistiqueDocs) + "</td>";
                    str += "   </tr>";
                }
                i++;
            }
            str += "</tbody>" +
                "</table>";
            return str;
        }

        private string GetTableDocument(List<StatistiqueDocs> stats)
        {
            var red = "#C84B34";
            if (!stats.Any()) return string.Empty;
            var str = "<table class=\"tablerelance\">" +
                   "<thead>" +
                   "    <tr>" +
                   "        <th class=\"grid-header\"><span>" + @Resource.document + "</span></th>" +
                   "        <th class=\"grid-header\"><span>" + @Resource.ToRead + "</span></th>" +
                   "        <th class=\"grid-header\"><span>" + @Resource.ToApproved + "</span></th>" +
                   "        <th class=\"grid-header\"><span>" + @Resource.ToTest + "</span></th>" +
                   "    </tr>" +
                   "</thead>" +
                   "<tbody>";
            int i = 1;
            foreach (var stat in stats)
            {
                var colorDoc = "";
                var colorRead = "";
                var colorApproved = "";
                var colorTested = "";
                if (stat.LibRead.Equals(Resource.toDO) || stat.LibApproved.Equals(Resource.toDO) || stat.LibTested.Equals(Resource.toDO))
                {
                    colorDoc = string.Format("style=\"color:{0};font-weight:bold;\"", red);
                    if (stat.LibRead.Equals(Resource.toDO))
                    {
                        colorRead = string.Format("style=\"color:{0};font-weight:bold;\"", red);
                    }
                    if (stat.LibApproved.Equals(Resource.toDO))
                    {
                        colorApproved = string.Format("style=\"color:{0};font-weight:bold;\"", red);
                    }
                    if (stat.LibTested.Equals(Resource.toDO))
                    {
                        colorTested = string.Format("style=\"color:{0};font-weight:bold;\"", red);
                    }
                }
                var classe = "";
                if (i % 2 == 0)
                {
                    classe = "paire";
                }
                str += "    <tr class=\"" + classe + "\">";
                str += string.Format("<td {0}>" + stat.Name + "</td>", colorDoc);
                str += string.Format("<td {0}>" + stat.LibRead + "</td>", colorRead);
                str += string.Format("<td {0}>" + stat.LibApproved + "</td>", colorApproved);
                str += string.Format("<td {0}>" + stat.LibTested + "</td>", colorTested);
                str += "   </tr>";
                i++;
            }
            str += "</tbody>" +
                "</table>";
            return str;
        }

        private string ProgressBar(int Value, int Total, int widthTable)
        {
            var classe = string.Empty;
            var classeBar = string.Empty;
            var width = "";
            decimal valPixel = 0;
            if (Total > 0)
            {
                classeBar = (Value * 100 / Total >= 20 ? (Value * 100 / Total >= 70 ? "progress-bar-danger" : "progress-bar-warning") : "progress-bar-success");
                classe = (Value * 100 / Total >= 20 ? (Value * 100 / Total >= 70 ? "progress-danger" : "progress-warning") : "progress-success");
                //valP = Decimal.Divide(Value * 100, Total);
                valPixel = Decimal.Divide(Value * widthTable, Total);
                //Minimum width
                //if (valP < 3) valP = 3;
                width = (Value == 0) ? "0%" : string.Format("{0}px", Math.Round(valPixel));
            }
            else
            {
                width = "0%";
                classeBar = "progress-bar-success";
                classe =  "progress-success";
            }
            return string.Format("<table cellpadding=\"0\" cellspacing=\"0\" style=\"width:{5}px\" class=\"progress\"><tr><td class=\"{4}\"><table cellspacing=\"0\" cellpadding=\"0\" class=\"progress-bar\" style=\"width:{1}\"><tr><td class=\"{0}\" style=\"padding-left:2px;\">{2} / {3}</td></tr></table></td></tr></table>", classeBar, width, Value, Total, classe, widthTable);          
        }

        public void Dispose()
        {
            ((IDisposable)_mailTemplateService).Dispose();
            ((IDisposable)_mailKeywordService).Dispose();
            ((IDisposable)_mailService).Dispose();
            ((IDisposable)_userService).Dispose();
            ((IDisposable)Context).Dispose();
        }
    }
}
