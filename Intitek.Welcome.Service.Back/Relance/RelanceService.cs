using Intitek.Welcome.Infrastructure;
using Intitek.Welcome.Infrastructure.Histo;
using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.UI.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace Intitek.Welcome.Service.Back
{
    public class RelanceService : BaseService, IRelanceService
    {
        private readonly MailKeywordsService _mailKeywordService;
        private readonly MailService _mailService;
        private readonly BatchService _batchService;
        private readonly ActiveDirectoryService _adUserService;
        private readonly IUserService _userService;
        private readonly StatsService _statsService;

        private const string BATCH_PROGNAME_MANUEL = "Manuel";
        public static readonly string SQL_STATS = "SELECT * FROM( " + Environment.NewLine +
            "SELECT " + Environment.NewLine +
            "1 as Num, " + Environment.NewLine +
            "doc.ID, " + Environment.NewLine +
            "doc.Approbation, " + Environment.NewLine +
            "doc.Test, " + Environment.NewLine +
            "doc.Date, " + Environment.NewLine +
            "usr.ID as IdUser, " + Environment.NewLine +
            "usr.EntityName, " + Environment.NewLine +
            "usr.AgencyName, " + Environment.NewLine +
            "usr.Division, " + Environment.NewLine +
            "usr.ID_Manager, " + Environment.NewLine +
            "usr.InactivityStart, " + Environment.NewLine +
            "usr.InactivityEnd, " + Environment.NewLine +
            "ud.UpdateDate, " + Environment.NewLine +
            "ud.IsRead, " + Environment.NewLine +
            "ud.IsApproved, " + Environment.NewLine +
            "ud.IsTested, " + Environment.NewLine +
            "CASE(usr.Type) " + Environment.NewLine +
            "   WHEN 'STR' THEN doc.isStructure " + Environment.NewLine +
            "   ELSE 0" + Environment.NewLine +
            "  END as IsStructure, " + Environment.NewLine +
            "CASE (usr.Type)" + Environment.NewLine +
            "   WHEN 'MET' THEN doc.isMetier " + Environment.NewLine +
            "   ELSE 0 " + Environment.NewLine +
            " END as IsMetier " + Environment.NewLine +
            "FROM Document doc " +
            "left join UserDocument ud on (doc.ID= ud.ID_Document) " + Environment.NewLine +
            "join IntitekUser usr on(usr.ID= ud.ID_IntitekUser) " + Environment.NewLine +
            "WHERE usr.Active=1 and doc.Inactif='false' and " + Environment.NewLine +
            "    (EXISTS (SELECT* FROM EntityDocument " + Environment.NewLine +
            "        WHERE EntityDocument.ID_Document=ud.ID_Document AND " + Environment.NewLine +
            "        EntityName = (SELECT EntityName FROM IntitekUser WHERE IntitekUser.ID= usr.ID) AND " + Environment.NewLine +
            "         (AgencyName= (SELECT AgencyName from IntitekUser where IntitekUser.ID= usr.ID) OR AgencyName IS NULL)) " + Environment.NewLine +
            "     OR " + Environment.NewLine +
            "        EXISTS(SELECT* FROM ProfileDocument WHERE ProfileDocument.ID_Document= ud.ID_Document AND " + Environment.NewLine +
            "            ProfileDocument.ID_Profile IN (SELECT ID_Profile FROM ProfileUser WHERE ProfileUser.ID_IntitekUser= usr.ID))) " + Environment.NewLine +
            "UNION " + Environment.NewLine +
                "SELECT " + Environment.NewLine +
                "2 as Num, " + Environment.NewLine +
                "doc.ID, " + Environment.NewLine +
                "doc.Approbation, " + Environment.NewLine +
                "doc.Test, " + Environment.NewLine +
                "doc.Date, " + Environment.NewLine +
                "pu.ID_IntitekUser as IdUser, " + Environment.NewLine +
                "usr.EntityName, " + Environment.NewLine +
                "usr.AgencyName, " + Environment.NewLine +
                "usr.Division, " + Environment.NewLine +
                "usr.ID_Manager, " + Environment.NewLine +
                "usr.InactivityStart, " + Environment.NewLine +
                "usr.InactivityEnd, " + Environment.NewLine +
                "NULL as UpdateDate, " + Environment.NewLine +
                "NULL as IsRead, " + Environment.NewLine +
                "NULL as IsApproved, " + Environment.NewLine +
                "NULL as IsTested, " + Environment.NewLine +
                "CASE(usr.Type) " + Environment.NewLine +
                "  WHEN 'STR' THEN doc.isStructure " + Environment.NewLine +
                "       ELSE 0 " + Environment.NewLine +
                "  END as IsStructure, " + Environment.NewLine +
                "CASE (usr.Type) " + Environment.NewLine +
                "  WHEN 'MET' THEN doc.isMetier " + Environment.NewLine +
                "  ELSE 0 " + Environment.NewLine +
                "  END as IsMetier " + Environment.NewLine +
                "  FROM ProfileDocument pd " + Environment.NewLine +
                "  left join ProfileUser pu on pu.ID_Profile= pd.ID_Profile " + Environment.NewLine +
                "  join IntitekUser usr on(usr.ID= pu.ID_IntitekUser) " + Environment.NewLine +
                "  left join Document doc on doc.ID= pd.ID_Document " + Environment.NewLine +
                "  WHERE " + Environment.NewLine +
                "  usr.Active= 1 and doc.Inactif= 'false' and doc.IsNoActionRequired= 'false' and " + Environment.NewLine +
                "  pd.ID_Profile in (select ID_Profile from ProfileUser where ID_IntitekUser= pu.ID_IntitekUser) " + Environment.NewLine +
                "  and (ID_Document not in (select ID_Document from  UserDocument where ID_IntitekUser = pu.ID_IntitekUser) ) " + Environment.NewLine +
            "UNION " + Environment.NewLine +
            "SELECT  " + Environment.NewLine +
            "3 as Num, " + Environment.NewLine +
            "ID_Document as ID, " + Environment.NewLine +
            "doc.Approbation, " + Environment.NewLine +
            "doc.Test, " + Environment.NewLine +
            "doc.Date, " + Environment.NewLine +
            "usr.ID as IdUser, " + Environment.NewLine +
            "usr.EntityName, " + Environment.NewLine +
            "usr.AgencyName, " + Environment.NewLine +
            "usr.Division, " + Environment.NewLine +
            "usr.ID_Manager, " + Environment.NewLine +
            "usr.InactivityStart, " + Environment.NewLine +
            "usr.InactivityEnd, " + Environment.NewLine +
            "NULL as UpdateDate, " + Environment.NewLine +
            "NULL as IsRead, " + Environment.NewLine +
            "NULL  as IsApproved, " + Environment.NewLine +
            "NULL as IsTested, " + Environment.NewLine +
            "CASE(usr.Type) " + Environment.NewLine +
            "  WHEN 'STR' THEN doc.isStructure " + Environment.NewLine +
            "  ELSE 0 " + Environment.NewLine +
            "  END as IsStructure, " + Environment.NewLine +
            "CASE (usr.Type) " + Environment.NewLine +
            "  WHEN 'MET' THEN doc.isMetier " + Environment.NewLine +
            "  ELSE 0 " + Environment.NewLine +
            "  END as IsMetier " + Environment.NewLine +
            "  FROM EntityDocument ed " + Environment.NewLine +
            "  left join IntitekUser usr on (ed.EntityName= usr.EntityName and (ed.AgencyName= usr.AgencyName or ed.AgencyName is null))  " + Environment.NewLine +
            "  left join Document doc on doc.ID= ed.ID_Document " + Environment.NewLine +
            "  where usr.Active= 1 and doc.Inactif= 'false' AND doc.IsNoActionRequired= 'false' " +
            " AND (ID_Document not in (select ID_Document from  UserDocument where ID_IntitekUser = usr.ID) ) " + Environment.NewLine +
            "  ) as tbl " + Environment.NewLine +
            "  where(tbl.IsStructure= 1 or tbl.IsMetier= 1) " + Environment.NewLine;

        public RelanceService(ILogger logger): base(logger)
        {
            _mailKeywordService = new MailKeywordsService(logger);
            _mailService = new MailService(logger, new SmtpClient()
            {

            });
            _batchService = new BatchService(logger);
            _adUserService = new ActiveDirectoryService(logger);
            _userService = new UserService(logger);
            _statsService = new StatsService(logger);
        }

        private string GetBody(string body)
        {
            string str = "<!doctype html><html><head>" +
                "<meta charset=\"utf-8\">" +
                "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1, shrink-to-fit=no\">" +
                "   <link href='https://fonts.googleapis.com/css?family=Poppins' rel='stylesheet'>" +
                "  <style type=\"text/css\" >" +
                 "        .tablerelance {" +
                 "            font-family:'Poppins';" +
                 "            font-size:10pt;" +
                 "            border-collapse:collapse;" +
                 "        }" +
                 "       .tablerelance td, .tablerelance th {" +
                 "            font-weight:normal;" +
                 "            padding:4px 8px;" +
                 "            border: 1px solid #00c072;" +
                 "       }" +
                 "       .tablerelance th.grid-header {" +
                 "           color: #fff;" +
                 "           background-color:#00c072;" +
                 "           font-weight:bold !important;" +
                 "           padding-right:0.5em;" +
                 "        }" +
                 "        tr.paire td {" +
                 "           background-color: #e3e3e3;" +
                 "       }" +
                 "   </style>" +
                 "</head><body>" + body + "</body>" + "</html>";
            return str;
        }

        private List<StatistiquesDTO> GetStats(List<UserDTO> usersDTO)
        {
            var sql = SQL_STATS;
            var whereUserInactive = " (tbl.[InactivityStart] IS NOT NULL) AND((cast(cast(tbl.[InactivityStart] as date) as datetime2)) <= (cast(cast(GETDATE() as date) as datetime2))) " +
              " AND((tbl.[InactivityEnd] IS NULL) OR((tbl.[InactivityEnd] IS NOT NULL) AND((cast(cast(tbl.[InactivityEnd] as date) as datetime2)) >= (cast(cast(GETDATE() as date) as datetime2)))))";
            sql = sql + " and " + string.Format("NOT({0})", whereUserInactive);

            var query = this.Database.SqlQuery<StatistiquesDTO>(sql).AsQueryable();
            query = query.Where(x => usersDTO.Select(us => us.ID).FirstOrDefault() == x.IdUser
            //query = query.Where(x => usersDTO.Select(u => u.ID).Contains(x.IdUser)
                && ((!x.IsRead.HasValue || x.Approbation.HasValue && x.Approbation == 1 && !x.IsApproved.HasValue) ||
                   (x.Test.HasValue && x.Test == 1 && !x.IsTested.HasValue)));
            var stats = new List<StatistiquesDTO>();
            stats = query.ToList();
            return stats;
        }
        
        public bool Execute(GetRelanceRequest request, out string Message)
        {
            bool ret = false;
            Message = string.Empty;
            var startedAt = DateTime.Now;
            var theBatch = _batchService.Get(new GetBatchRequest() { ProgName = BATCH_PROGNAME_MANUEL });
            if (theBatch.Batch != null)
            {
                var mail = new Mail(this._logger);
                var batchId = theBatch.Batch.ID;
                try
                {
                    this._logger.Info(string.Format("Batch {0} started at {1}....", theBatch.Batch.ProgName, DateTime.Now));
                    var keywords = _mailKeywordService.GetAll().MailKeywords.Select(mk => mk.Code).ToList();
                    var stats = this.GetStats(request.LstUsers);
                    foreach (var user in request.LstUsers)
                    {
                        var intitekUser = _userService.GetIntitekUserByLogin(user.Name);
                        if (stats != null && stats.Any())
                        {
                            List<int> docsId = stats.Where(x=> x.IdUser==user.ID).Select(x => x.ID).Distinct().ToList();
                            if (docsId.Any())
                            {
                                List<DocumentDTO> documents = _statsService.ListDocumentsByDocs(request.IdLang, request.IdDefaultLang, docsId);
                                List<StatistiqueDocs> statUserDocs = new List<StatistiqueDocs>();
                                foreach (var doc in documents)
                                {
                                    List<StatistiquesDTO> statDoc = stats.Where(x => x.ID == doc.ID && x.IdUser == user.ID).ToList();
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
                                                if (stDoc.NotApproved > 0)
                                                {
                                                    stDoc.LibApproved = Resource.toDO;
                                                }
                                            }
                                            if (stDoc.ToTested > 0)
                                            {
                                                stDoc.LibTested = Resource.AlreadyDone;
                                                if (stDoc.NotTested > 0)
                                                {
                                                    stDoc.LibTested = Resource.toDO;
                                                }
                                            }
                                        }
                                    }
                                    statUserDocs.Add(stDoc);
                                }
                                user.StatistiqueDocs = statUserDocs;
                            }
                        }
                        var mv = new MailVariablesDTO()
                        {
                            User = user,
                            ADUser = intitekUser != null ?
                                new UserAdDTO()
                                {
                                    UserName = intitekUser.FullName,
                                    Email = intitekUser.Email
                                } :
                                new UserAdDTO()
                                {
                                    UserName = "Unknown",
                                    Email = ""
                                }
                        };

                        var mailMessage = FillMessage(request.MailTemplate.Content, keywords, mv);
                        var body = this.GetBody(mailMessage);
                        var emailTo = !string.IsNullOrEmpty(mv.ADUser.Email) ? mv.ADUser.Email : user.EmailOnBoarding;
                        //Send mail
                        mail.SendMailDepuisServer(emailTo, body, request.MailTemplate.Object, true);

                        _mailService.HistoEmail.Emails.Add(new EmailBO()
                        {
                            ID_Batch = batchId,
                            Id_IntitekUser = user.ID,
                            Id_MailTemplate = request.MailTemplate.Id,
                            Message = mailMessage
                        });

                    }
                    _mailService.Historize();
                    var endedAt = DateTime.Now;
                    _batchService.HistoBatch._batches.Add(new BatchBO()
                    {
                        ID_Batch = batchId,
                        Start = startedAt,
                        Finish = endedAt,
                        Message = string.Empty,
                        ReturnCode = 0
                    });
                    ret = true;                   
                }
                catch (Exception ex)
                {
                    Message = ex.Message;
                    var abortedAt = DateTime.Now;
                    _batchService.HistoBatch._batches.Add(new BatchBO()
                    {
                        ID_Batch = batchId,
                        Start = startedAt,
                        Finish = abortedAt,
                        Message = ex.Message,
                        ReturnCode = -1
                    });
                }
                finally
                {
                    mail.Dispose();
                    _batchService.Historize();
                    this._logger.Info(string.Format("Batch execution ended at {0}....", DateTime.Now));
                }
            }
            return ret;
        }
        private string GetTableDocument(List<StatistiqueDocs> stats)
        {
            //if (stats is null || !stats.Any()) return string.Empty;
            //var str = "<table class=\"tablerelance\">" +
            //       "<thead>" +
            //       "    <tr>" +
            //       "        <th class=\"grid-header\"><span>" + @Resource.document + "</span></th>" +
            //       "        <th class=\"grid-header\"><span>" + @Resource.ToRead + "</span></th>" +
            //       "        <th class=\"grid-header\"><span>" + @Resource.ToApproved + "</span></th>" +
            //       "        <th class=\"grid-header\"><span>" + @Resource.ToTest + "</span></th>" +
            //       "    </tr>" +
            //       "</thead>" +
            //       "<tbody>";
            //int i = 1;
            //foreach (var stat in stats)
            //{
            //    var classe = "";
            //    if (i % 2 == 0)
            //    {
            //        classe = "paire";
            //    }
            //    str += "    <tr class=\"" + classe + "\">";
            //    str += "    <td>" + stat.Name + "</td>";
            //    str += "    <td>" + stat.LibRead + "</td>";
            //    str += "    <td>" + stat.LibApproved + "</td>";
            //    str += "    <td>" + stat.LibTested + "</td>";
            //    str += "   </tr>";
            //    i++;
            //}
            //str += "</tbody>" +
            //    "</table>";
            //return str;
            var red = "#C84B34";
            if (stats is null ||!stats.Any()) return string.Empty;
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
        private string FillMessage(string message, List<string> keywords, MailVariablesDTO mv)
        {
            var result = message;
            foreach (var keyword in keywords)
            {
                switch (keyword)
                {
                    case "Collaborateur":
                        result = result.Replace(string.Format("[{0}]", keyword), string.Format("{0}", mv.ADUser.UserName));
                        break;
                    case "Agence":
                        result = result.Replace(string.Format("[{0}]", keyword), mv.User.EntityName);
                        break;
                    case "Entite":
                        result = result.Replace(string.Format("[{0}]", keyword), mv.User.AgencyName);
                        break;
                    case "Jour":
                        result = result.Replace(string.Format("[{0}]", keyword), DateTime.Now.Date.ToString());
                        break;
                    case "Liste_Documents":
                        result = result.Replace(string.Format("[{0}]", keyword), this.GetTableDocument(mv.User.StatistiqueDocs));
                        break;
                    default:
                        result = result.Replace(string.Format("[{0}]", keyword), string.Empty);
                        break;

                }
            }
            return result;
        }
    }
}
