using Intitek.Welcome.DataAccess;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.Helpers;
using Intitek.Welcome.Infrastructure.Log;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic;

namespace Intitek.Welcome.Service.Back
{
    public class StatsService : BaseService, IStatsService
    {
        private readonly DocumentCategoryDataAccess _doccategoryrepository;
        private readonly DocumentDataAccess _docrepository;
        private readonly UserDataAccess _userrepository;
        private readonly EntityDocumentDataAccess _entityDocrepository;
        private readonly UserDocumentDataAccess _userDocrepository;
        private readonly ProfileDocumentDataAccess _profilDocrepository;
        private readonly UserProfileDataAccess _profilUserrepository;
        private readonly DocumentLangDataAccess _doclangrepository;
        private static string SQL_STATS = "SELECT * FROM( " + Environment.NewLine +
            "SELECT " + Environment.NewLine +
            "1 as Num, " + Environment.NewLine +
            "doc.ID, " + Environment.NewLine +
            "doc.Approbation, " + Environment.NewLine +
            "doc.Test, " + Environment.NewLine +
            "doc.Date, " + Environment.NewLine +
            "usr.ID as IdUser, " + Environment.NewLine +
            "usr.InactivityStart, " + Environment.NewLine +
            "usr.InactivityEnd, " + Environment.NewLine +
            "usr.EntryDate, " + Environment.NewLine +
            "usr.ExitDate, " + Environment.NewLine +
            "usr.EntityName, " + Environment.NewLine +
            "usr.AgencyName, " + Environment.NewLine +
            "usr.Departement, " + Environment.NewLine +
            "usr.ID_Manager, " + Environment.NewLine +
            "usr.Division, " + Environment.NewLine +
            "ud.UpdateDate, " + Environment.NewLine +
            "ud.IsRead, " + Environment.NewLine +
            "ud.IsApproved, " + Environment.NewLine +
            "ud.IsTested, " + Environment.NewLine +
            "NULL as PrfDate, " + Environment.NewLine +
            "NULL as EntityDate, " + Environment.NewLine +
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
            "   (EXISTS (SELECT* FROM EntityDocument " + Environment.NewLine +
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
                "usr.InactivityStart, " + Environment.NewLine +
                "usr.InactivityEnd, " + Environment.NewLine +
                "usr.EntryDate, " + Environment.NewLine +
                "usr.ExitDate, " + Environment.NewLine +
                "usr.EntityName, " + Environment.NewLine +
                "usr.AgencyName, " + Environment.NewLine +
                "usr.Departement, " + Environment.NewLine +
                "usr.ID_Manager, " + Environment.NewLine +
                "usr.Division, " + Environment.NewLine +
                "NULL as UpdateDate, " + Environment.NewLine +
                "NULL as IsRead, " + Environment.NewLine +
                "NULL as IsApproved, " + Environment.NewLine +
                "NULL as IsTested, " + Environment.NewLine +
                "pd.Date as PrfDate, " + Environment.NewLine +
                "NULL as EntityDate, " + Environment.NewLine +
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
            "usr.InactivityStart, " + Environment.NewLine +
            "usr.InactivityEnd, " + Environment.NewLine +
            "usr.EntryDate, " + Environment.NewLine +
            "usr.ExitDate, " + Environment.NewLine +
            "usr.EntityName, " + Environment.NewLine +
            "usr.AgencyName, " + Environment.NewLine +
            "usr.Departement, " + Environment.NewLine +
            "usr.ID_Manager, " + Environment.NewLine +
            "usr.Division, " + Environment.NewLine +
            "NULL as UpdateDate, " + Environment.NewLine +
            "NULL as IsRead, " + Environment.NewLine +
            "NULL  as IsApproved, " + Environment.NewLine +
            "NULL as IsTested, " + Environment.NewLine +
            "NULL as PrfDate, " + Environment.NewLine +
            "ed.EntityDocDate as EntityDate, " + Environment.NewLine +
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
            "  where(tbl.IsStructure= 1 or tbl.IsMetier= 1) " + Environment.NewLine ;
        public StatsService(ILogger logger) : base(logger)
        {
            _docrepository = new DocumentDataAccess(uow);
            _doccategoryrepository = new DocumentCategoryDataAccess(uow);
            _userrepository = new UserDataAccess(uow);
            _entityDocrepository = new EntityDocumentDataAccess(uow);
            _userDocrepository = new UserDocumentDataAccess(uow);
            _profilDocrepository = new ProfileDocumentDataAccess(uow);
            _profilUserrepository = new UserProfileDataAccess(uow);
            _doclangrepository = new DocumentLangDataAccess(uow);
        }

        public List<DocumentDTO> ListDocuments(int idLang, int idDefaultLang)
        {
            string orderBy = "OrdreCategory, Name";
           IQueryable<DocumentDTO> query = this._docrepository.RepositoryTable.GroupJoin(
                    this._doclangrepository.RepositoryQuery.Where(l => l.ID_Lang == idLang),
                    docL => docL.ID,
                    lang => lang.ID_Document,
                    (docL, lang) => new { docL, lang }).SelectMany(x => x.lang.DefaultIfEmpty(), (parent, child) => new { docL = parent.docL, lang = child })
                .Where(d=>d.docL.Inactif==false && (d.docL.EntityDocument.Any() || d.docL.ProfileDocument.Any()))
                .GroupJoin(this._doccategoryrepository.RepositoryTable.Include("DocumentCategoryLang"),
               doc => doc.docL.ID_Category,
               categ => categ.ID,
               (doc, categ) => new { doc, categ })
               .SelectMany(x => x.categ.DefaultIfEmpty(), (parent, child) => new DocumentDTO()
               {
                   ID = parent.doc.docL.ID,
                   Name = EdmxFunction.GetNameDocument(parent.doc.docL.ID, idLang, idDefaultLang),
                   IsDefaultLangName = string.IsNullOrEmpty(parent.doc.lang.Name),
                   ID_Category = parent.doc.docL.ID_Category,
                   DefaultNameCategory = EdmxFunction.GetCategoryLang(parent.doc.docL.ID_Category.Value, idDefaultLang, idDefaultLang),
                   DocumentCategoryLang = child.DocumentCategoryLang.FirstOrDefault(f => f.ID_Lang == idLang),
                   OrdreCategory = child.OrdreCategory != null ? child.OrdreCategory : 1000
               }).OrderBy(orderBy);
            var lst = query.ToList();
            return lst;
        }
        public List<DocumentDTO> ListDocumentsByDocs(int idLang, int idDefaultLang, List<int> docs)
        {
            IQueryable<DocumentDTO> query = this._docrepository.RepositoryTable
               .Where(d => d.Inactif == false && (d.EntityDocument.Any() || d.ProfileDocument.Any()) && docs.Contains(d.ID))
               .GroupJoin(
                    this._doclangrepository.RepositoryQuery.Where(l => l.ID_Lang == idLang),
                    docL => docL.ID,
                    lang => lang.ID_Document,
                    (docL, lang) => new { docL, lang }).SelectMany(x => x.lang.DefaultIfEmpty(), (parent, child) => new { docL = parent.docL, lang = child })
               .Select(x => new DocumentDTO()
               {
                   ID = x.docL.ID,
                   Name = EdmxFunction.GetNameDocument(x.docL.ID, idLang, idDefaultLang),
                   IsDefaultLangName = string.IsNullOrEmpty(x.lang.Name)
               }).OrderBy(x=>x.Name);
            var lst = query.ToList();
            return lst;
        }
        public List<UserDTO> ListUsersForStat(List<int> usersId, bool bAll, bool bActivity, StatsRequestType statype)
        {
            var query = this._userrepository.RepositoryQuery.Where(x => x.Active && usersId.Contains(x.ID));
            if (!bAll)
            {
                //si inactif :
                //InactivityStart is not null 
                //and InactivityStart <= CONVERT (date, GETDATE()) 
                //and(InactivityEnd is null or (InactivityEnd is not null and CONVERT(date, GETDATE()) <= InactivityEnd)
                if (bActivity)
                {
                    query = query.Where(x => !(x.InactivityStart.HasValue && DbFunctions.TruncateTime(x.InactivityStart) <= DbFunctions.TruncateTime(System.Data.Entity.SqlServer.SqlFunctions.GetDate()) &&
                                 (!x.InactivityEnd.HasValue || (x.InactivityEnd.HasValue && DbFunctions.TruncateTime(x.InactivityEnd) >= DbFunctions.TruncateTime(System.Data.Entity.SqlServer.SqlFunctions.GetDate())))));
                }
                else
                {
                    query = query.Where(x => x.InactivityStart.HasValue && DbFunctions.TruncateTime(x.InactivityStart) <= DbFunctions.TruncateTime(System.Data.Entity.SqlServer.SqlFunctions.GetDate()) &&
                                (!x.InactivityEnd.HasValue || (x.InactivityEnd.HasValue && DbFunctions.TruncateTime(x.InactivityEnd) >= DbFunctions.TruncateTime(System.Data.Entity.SqlServer.SqlFunctions.GetDate()))));
                }
               
            }
            var query1 = query.Select(x => new UserDTO { ID = x.ID, Name = x.Username, FullName = x.FullName, EntityName = x.EntityName, AgencyName = x.AgencyName, Type = x.Type, Email = x.isOnBoarding ? x.EmailOnBoarding : x.Email, Departement = x.Departement, Division = x.Division, ID_Manager = x.ID_Manager });
            switch (statype)
            {
                case StatsRequestType.Agency:
                    query1 = query1.OrderBy(x => x.EntityName).ThenBy(x => x.AgencyName).ThenBy(x => x.FullName);
                    break;
                case StatsRequestType.Departement:
                    query1 = query1.OrderBy(x => x.Departement).ThenBy(x => x.FullName);
                    break;
                case StatsRequestType.Manager:
                    query1 = query1.OrderBy(x => x.Departement).ThenBy(x => x.Division).ThenBy(x => x.FullName);
                    break;
                default:
                    break;
            }
            var lst = query1.ToList();
            return lst;
        }

        
        private Nullable<DateTime> GetLastDate(UserDTO user)
        {
            DateTime? dtTested = this._userDocrepository.RepositoryQuery.Where(x => x.ID_IntitekUser == user.ID).Max(x => x.IsTested).GetValueOrDefault();
            if (dtTested.HasValue)
                return dtTested.Value;
            DateTime? dtApproved = this._userDocrepository.RepositoryQuery.Where(x => x.ID_IntitekUser == user.ID).Max(x => x.IsApproved).GetValueOrDefault();
            if (dtApproved.HasValue)
                return dtApproved.Value;
            DateTime? dtRead = this._userDocrepository.RepositoryQuery.Where(x => x.ID_IntitekUser == user.ID).Max(x => x.IsRead).GetValueOrDefault();
            return dtRead;
        }
        public List<StatistiquesDTO> GetStatsEntity(GetStatsRequest request)
        {
            var sql = SQL_STATS;
            List<Statistiques> lststat = new List<Statistiques>();

            var query = _docrepository.Database.SqlQuery<StatistiquesDTO>(sql).AsQueryable();
            if (!string.IsNullOrEmpty(request.EntitySelect))
            {
                var predicate = PredicateBuilder.New<StatistiquesDTO>();
                predicate = predicate.And(x => x.EntityName.Equals(request.EntitySelect));
                query = query.Where(predicate);
            }
            if (request.MultiDocSelect != null && request.MultiDocSelect.Length > 0)
            {
                query = query.Where(x => request.MultiDocSelect.Contains(x.ID));
            }
            var lst = query.ToList();
            return lst;
        }
        public List<StatistiquesDTO> GetStats(GetStatsRequest request, bool isRelance)
        {
            var sql = SQL_STATS;
            var whereUserInactive = " (tbl.[InactivityStart] IS NOT NULL) AND((cast(cast(tbl.[InactivityStart] as date) as datetime2)) <= (cast(cast(GETDATE() as date) as datetime2))) "+ 
                " AND((tbl.[InactivityEnd] IS NULL) OR((tbl.[InactivityEnd] IS NOT NULL) AND((cast(cast(tbl.[InactivityEnd] as date) as datetime2)) >= (cast(cast(GETDATE() as date) as datetime2)))))";
            var whereDate = "";
            var whereExitDate = "";
            var whereEntryDate = "";
            var parameters = new List<SqlParameter>();
            if (request.StartDate.HasValue)
            {
                whereDate = " (tbl.[IsRead]>=@startDate OR tbl.[IsApproved]>=@startDate OR tbl.[IsTested]<=@startDate OR tbl.[PrfDate]>=@startDate OR tbl.[EntityDate]>=@startDate)";
                whereExitDate = " (tbl.[ExitDate]>= @startDate)";
                whereEntryDate = " (tbl.[EntryDate]>= @startDate)";
                parameters.Add(new SqlParameter("startDate", request.StartDate.Value));
            }
            if (request.EndDate.HasValue)
            {
                if (string.IsNullOrEmpty(whereDate))
                {
                    whereDate = " (tbl.[IsRead]<=@endDate OR tbl.[IsApproved]<=@endDate OR tbl.[IsTested]<=@endDate OR tbl.[PrfDate]<=@endDate OR tbl.[EntityDate]<=@endDate)";
                    whereExitDate = " (tbl.[ExitDate]<= @endDate)";
                    whereEntryDate = " (tbl.[EntryDate]<= @endDate)";
                }
                else
                {
                    whereDate+= " and (tbl.[IsRead]<=@endDate OR tbl.[IsApproved]<=@endDate OR tbl.[IsTested]<=@endDate OR tbl.[PrfDate]<=@endDate OR tbl.[EntityDate]<=@endDate)";
                    whereExitDate+= " and (tbl.[ExitDate]<= @endDate)";
                    whereEntryDate+= " and (tbl.[EntryDate]<= @endDate)";
                }
                parameters.Add(new SqlParameter("endDate", request.EndDate.Value));
            }
            if (!string.IsNullOrEmpty(whereDate))
            {
                sql = sql + string.Format(" AND ({0})", whereDate);
                sql = sql + string.Format(" AND (tbl.[ExitDate] IS NULL OR NOT({0}))", whereExitDate);
                sql = sql + string.Format(" AND (tbl.[EntryDate] IS NULL OR ({0}))", whereEntryDate);
            }
                
            if (request.EmployeeSelect == 1)
            {
                sql = sql + " and " + string.Format("NOT({0})", whereUserInactive) ;
            }
            else if (request.EmployeeSelect == 2)
            {
                sql = sql + " and " +whereUserInactive;
            }
            var query = _docrepository.Database.SqlQuery<StatistiquesDTO>(sql, parameters.ToArray()).AsQueryable();
            switch (request.StatType)
            {
                case StatsRequestType.Agency:
                    if (request.IsSelectAllEntity!=1 && request.MultiEntitySelect != null && request.MultiEntitySelect.Length > 0)
                    {
                        var predicate = PredicateBuilder.New<StatistiquesDTO>();
                        foreach (var item in request.MultiEntitySelect)
                        {
                            string[] tab = item.Split('|');
                            string entity = tab[0];
                            string agency = tab[1];
                            if (string.IsNullOrEmpty(agency))
                            {
                                predicate = predicate.Or(x => x.EntityName.Equals(entity) && string.IsNullOrEmpty(x.AgencyName));
                            }
                            else
                            {
                                predicate = predicate.Or(x => x.EntityName.Equals(entity) && x.AgencyName.Equals(agency));
                            }
                            
                        }
                        query = query.Where(predicate);
                    }
                    break;
                case StatsRequestType.Departement:
                    if (request.IsSelectAllDepartement!=1 && request.MultiDepartementSelect != null && request.MultiDepartementSelect.Length > 0)
                    {
                        var predicate = PredicateBuilder.New<StatistiquesDTO>();
                        if (request.MultiDepartementSelect.Contains(Constante.NO_DIRECTION_ID))
                        {
                            predicate = predicate.Or(x=> string.IsNullOrEmpty(x.Departement));
                        }
                        var lstRequiredDepartement = request.MultiDepartementSelect.ToList();
                        lstRequiredDepartement.Remove(Constante.NO_DIRECTION_ID);
                        predicate = predicate.Or(x => lstRequiredDepartement.Contains(x.Departement));
                        query = query.Where(predicate);
                    }
                    break;
                case StatsRequestType.Manager:
                    if (request.IsSelectAllManager!=1 && request.MultiManagerSelect != null && request.MultiManagerSelect.Length > 0)
                    {
                        query = query.Where(x=> x.ID_Manager.HasValue && !string.IsNullOrEmpty(x.Departement));
                        var predicate = PredicateBuilder.New<StatistiquesDTO>();
                        foreach (var item in request.MultiManagerSelect)
                        {
                            string[] tab = item.Split('|');
                            string departement = tab[0];
                            int id_manager = Int32.Parse(tab[1]);
                            predicate = predicate.Or(x => x.Departement.Equals(departement) && x.ID_Manager.Equals(id_manager));
                        }
                        query = query.Where(predicate);
                    }
                    break;
                default:
                    break;
            }
            if (request.MultiDocSelect != null && request.MultiDocSelect.Length > 0)
            {
                query = query.Where(x => request.MultiDocSelect.Contains(x.ID));
            }
            
            if (isRelance)
            {
                query = query.Where(x => !x.IsRead.HasValue ||
                    (x.Approbation.HasValue && x.Approbation == 1 && !x.IsApproved.HasValue) ||
                    (x.Test.HasValue && x.Test == 1 && !x.IsTested.HasValue));
            }
            var lst = query.ToList();
            return lst;
        }
       
        public List<Statistiques> GetEngineerList(GetStatsRequest request, bool onlyLatePeople)
        {
            List<Statistiques> lststat = new List<Statistiques>();
            List<StatistiquesDTO> stats = this.GetStats(request, onlyLatePeople);           
            if (stats != null && stats.Count> 0)
            {
                List<UserDTO> users = new List<UserDTO>();
                if (onlyLatePeople)
                {
                    users = this.ListUsersForStat(stats.Select(x => x.IdUser).Distinct().ToList(), false, true, request.StatType);
                }
                else
                {
                    users = this.ListUsersForStat(stats.Select(x => x.IdUser).Distinct().ToList(), true, false, request.StatType);
                }
                foreach (var user in users)
                {
                    List<StatistiquesDTO> statsUser = stats.Where(x => x.IdUser==user.ID).ToList();
                    Statistiques st = new Statistiques();
                    st.LastDate = this.GetLastDate(user);
                    st.UserId = user.ID;
                    st.Name = user.FullName;
                    switch (request.StatType)
                    {
                        case StatsRequestType.Agency:
                            st.EntityName = user.EntityName;
                            st.AgencyName = user.AgencyName;
                            break;
                        case StatsRequestType.Departement:
                            st.EntityName = user.EntityName;
                            st.Departement = user.Departement;
                            break;
                        case StatsRequestType.Manager:
                            st.EntityName = user.EntityName;
                            st.Departement = user.Departement;
                            st.Division = user.Division;
                            break;
                         default:
                            break;
                    }

                    st.NotRead = statsUser.Where(x => !x.IsRead.HasValue).Count();
                    st.NotApproved = statsUser.Where(x => (x.Approbation.HasValue && x.Approbation == 1) && !x.IsApproved.HasValue).Count();
                    st.NotTested = statsUser.Where(x => (x.Test.HasValue && x.Test == 1) && !x.IsTested.HasValue).Count();
                    
                    st.ToRead = statsUser.Count(); 
                    st.ToApproved = statsUser.Where(x => x.Approbation.HasValue && x.Approbation == 1).Count();
                    st.ToTested = statsUser.Where(x => x.Test.HasValue && x.Test == 1).Count();
                    lststat.Add(st);
                }
            }
            return lststat;
        }
        public bool IsRelanceMail(UserDTO user)
        {
            var queryNotParcours1 = this._entityDocrepository.RepositoryQuery
                 .Where(x => x.EntityName.Equals(user.EntityName)
                 && (x.AgencyName.Equals(user.AgencyName) || string.IsNullOrEmpty(x.AgencyName))
                 && this._docrepository.RepositoryQuery.Where(d => d.ID == x.ID_Document && d.Inactif == false).Any()
                 && this._userDocrepository.RepositoryQuery.Where(u => u.ID_IntitekUser.HasValue && u.ID_IntitekUser == user.ID && u.ID_Document == x.ID_Document
                 && (u.IsRead == null || (u.IsApproved == null && u.Document.Approbation == 1) || (u.IsTested == null && u.Document.Test == 1))).Any());
            var bExist = queryNotParcours1.Take(1).Any();
            if (bExist)
                return true;
            var queryNotParcours2 = this._profilDocrepository.RepositoryQuery.Where(x =>
                this._profilUserrepository.RepositoryQuery.Where(p => p.ID_IntitekUser.HasValue && p.ID_IntitekUser == user.ID).Select(p => p.ID_Profile).Contains(x.ID_Profile)
                 && this._docrepository.RepositoryQuery.Where(d => d.ID == x.ID_Document && d.Inactif == false).Any()
                 && this._userDocrepository.RepositoryQuery.Where(u => u.ID_IntitekUser.HasValue && u.ID_IntitekUser == user.ID && u.ID_Document == x.ID_Document
                 && (u.IsRead == null || (u.IsApproved == null && u.Document.Approbation == 1) || (u.IsTested == null && u.Document.Test == 1))).Any());
            bExist = queryNotParcours2.Take(1).Any();
            return bExist;
        }
    }
}
