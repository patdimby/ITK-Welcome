using Intitek.Welcome.DataAccess;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.Helpers;
using Intitek.Welcome.Infrastructure.Log;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;

namespace Intitek.Welcome.Service.Back
{
    public class ProfilService : BaseService, IProfilService
    {
        private readonly UserDataAccess _userrepository;
        private readonly ProfileDataAccess _profilerepository;
        private readonly ProfileDocumentDataAccess _profilDocrepository;
        private readonly UserProfileDataAccess _profilUserrepository;
        private readonly DocumentDataAccess _docrepository;
        private readonly UserDocumentDataAccess _userDocrepository;
        private readonly DocumentCategoryDataAccess _doccategoryrepository;
        private readonly HistoActionsDataAccess _histoactionsrepository;
        private readonly DocumentLangDataAccess _doclangrepository;
        public readonly SubCategoryDataAccess _subcategrepository;
        private static string SQL_STATS_PROFIL = "SELECT * FROM( " + Environment.NewLine +
            "SELECT " + Environment.NewLine +
            "1 as Num, " + Environment.NewLine +
            "doc.ID, " + Environment.NewLine +
            "doc.Approbation, " + Environment.NewLine +
            "doc.Test, " + Environment.NewLine +
            "doc.Date, " + Environment.NewLine +
            "usr.ID as IdUser, " + Environment.NewLine +
            "pu.ID_Profile, " + Environment.NewLine +
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
            "left join ProfileUser pu on pu.ID_IntitekUser= usr.ID " + Environment.NewLine +
            "WHERE usr.Active=1 and doc.Inactif='false' and " + Environment.NewLine +
            "    (" +
            "       EXISTS (SELECT* FROM EntityDocument " + Environment.NewLine +
            "        WHERE EntityDocument.ID_Document=ud.ID_Document AND " + Environment.NewLine +
            "        EntityName = (SELECT EntityName FROM IntitekUser WHERE IntitekUser.ID= usr.ID) AND " + Environment.NewLine +
            "         (AgencyName= (SELECT AgencyName from IntitekUser where IntitekUser.ID= usr.ID) OR AgencyName IS NULL)) " + Environment.NewLine +
            "     OR " + Environment.NewLine +
            "        EXISTS(SELECT* FROM ProfileDocument WHERE ProfileDocument.ID_Document= ud.ID_Document AND " + Environment.NewLine +
            "            ProfileDocument.ID_Profile IN (SELECT ID_Profile FROM ProfileUser WHERE ProfileUser.ID_IntitekUser= usr.ID))" + Environment.NewLine +
            "    )" +
            "UNION " + Environment.NewLine +
                "SELECT " + Environment.NewLine +
                "2 as Num, " + Environment.NewLine +
                "doc.ID, " + Environment.NewLine +
                "doc.Approbation, " + Environment.NewLine +
                "doc.Test, " + Environment.NewLine +
                "doc.Date, " + Environment.NewLine +
                "pu.ID_IntitekUser as IdUser, " + Environment.NewLine +
                "pd.ID_Profile, " + Environment.NewLine +
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
            "pu.ID_Profile, " + Environment.NewLine +
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
            " left join IntitekUser usr on (ed.EntityName= usr.EntityName and (ed.AgencyName= usr.AgencyName or ed.AgencyName is null))  " + Environment.NewLine +
            " left join ProfileUser pu on pu.ID_IntitekUser= usr.ID " + Environment.NewLine +
            "  left join Document doc on doc.ID= ed.ID_Document " + Environment.NewLine +
            "  where usr.Active= 1 and doc.Inactif= 'false' AND doc.IsNoActionRequired= 'false' " +
            " AND (ID_Document not in (select ID_Document from  UserDocument where ID_IntitekUser = usr.ID) ) " + Environment.NewLine +
            "  ) as tbl " + Environment.NewLine +
            "  where(tbl.IsStructure= 1 or tbl.IsMetier= 1) and tbl.ID_Profile>0 " + Environment.NewLine;
            
        public ProfilService(ILogger logger) : base(logger)
        {
            _userrepository = new UserDataAccess(uow);
            _profilerepository = new ProfileDataAccess(uow);
            _profilDocrepository = new ProfileDocumentDataAccess(uow);
            _profilUserrepository = new UserProfileDataAccess(uow);
            _docrepository = new DocumentDataAccess(uow);
            _userDocrepository = new UserDocumentDataAccess(uow);
            _doccategoryrepository = new DocumentCategoryDataAccess(uow);
            _histoactionsrepository = new HistoActionsDataAccess(uow);
            _doclangrepository = new DocumentLangDataAccess(uow);
            _subcategrepository = new SubCategoryDataAccess(uow);

            //path for System.Link.Dynamic pour reconnaitre la classe EdmxFunction
            this.DynamicLinkPatch();
        }
        public int GetAllCount(GridMvcRequest request)
        {
            return 0;
        }
        private List<DocumentDTO> GetDocumentsByIds(int idLang, int idDefaultLang, List<int> docsId)
        {
            string orderBy = "OrdreCategory, OrdreSubCategory, Name";
            IQueryable<DocumentDTO> query = this._docrepository.RepositoryTable.Where(x => docsId.Contains(x.ID))
                .GroupJoin(
                    this._doclangrepository.RepositoryQuery.Where(l => l.ID_Lang == idLang),
                    docL => docL.ID,
                    lang => lang.ID_Document,
                    (docL, lang) => new { docL, lang }).SelectMany(x => x.lang.DefaultIfEmpty(), (parent, child) => new { docL = parent.docL, lang = child })
                .GroupJoin(this._subcategrepository.RepositoryTable,
                   docSub => docSub.docL.ID_SubCategory,
                   sub => sub.ID,
                   (docSub, sub) => new { docSub, sub }).SelectMany(x => x.sub.DefaultIfEmpty(), (parent, child) => new { docSub = parent.docSub, sub = child })
               .GroupJoin(this._doccategoryrepository.RepositoryTable.Include("DocumentCategoryLang"),
                   docCat => docCat.docSub.docL.ID_Category,
                   categ => categ.ID,
                   (docCat, categ) => new { doc = docCat.docSub.docL, categ, sub = docCat.sub, lang = docCat.docSub.lang })
               .SelectMany(x => x.categ.DefaultIfEmpty(), (parent, child) =>
                   new DocumentDTO()
                   {
                       ID = parent.doc.ID,
                       Name = string.IsNullOrEmpty(parent.lang.Name) ? this._doclangrepository.RepositoryQuery.Where(l => l.ID_Document == parent.doc.ID && l.ID_Lang != idLang).FirstOrDefault().Name : parent.lang.Name,
                       IsDefaultLangName = string.IsNullOrEmpty(parent.lang.Name),
                       NameSubCategory = EdmxFunction.GetSubCategoryLang(parent.doc.ID_SubCategory.Value, idLang, idLang),
                       DefaultNameSubCategory = EdmxFunction.GetSubCategoryLang(parent.doc.ID_SubCategory.Value, idDefaultLang, idDefaultLang),
                       Version = parent.doc.Version,
                       Commentaire = parent.doc.Commentaire,
                       ID_Category = parent.doc.ID_Category.HasValue ? parent.doc.ID_Category : 0,
                       ID_SubCategory = parent.doc.ID_SubCategory,
                       DocumentCategoryLang = child.DocumentCategoryLang.FirstOrDefault(f => f.ID_Lang == idLang),
                       DefaultNameCategory = EdmxFunction.GetCategoryLang(parent.doc.ID_Category.Value, idDefaultLang, idDefaultLang),
                       OrdreCategory = child.OrdreCategory != null ? child.OrdreCategory : 1000,
                       OrdreSubCategory = parent.sub.Ordre != null ? parent.sub.Ordre : 1000
                   }).OrderBy(orderBy);
            return query.ToList();
        }
        public Statistiques GetStatsProfil(List<StatistiquesDTO> statsAll, int idprofil)
        {
            Statistiques profilStat = new Statistiques();
            var stats = statsAll.Where(x => x.ID_Profile==idprofil).ToList();
            if (stats != null && stats.Count > 0)
            {
                List<int> users = stats.Select(x => x.IdUser).Distinct().ToList();
                profilStat.ToRead = users.Count();
                foreach (var userId in users)
                {
                    List<StatistiquesDTO> statsUser = stats.Where(x => x.IdUser == userId).ToList();
                    Statistiques st = new Statistiques();
                    st.UserId = userId;

                    st.NotRead = statsUser.Where(x => !x.IsRead.HasValue).Count();
                    st.NotApproved = statsUser.Where(x => (x.Approbation.HasValue && x.Approbation == 1) && !x.IsApproved.HasValue).Count();
                    st.NotTested = statsUser.Where(x => (x.Test.HasValue && x.Test == 1) && !x.IsTested.HasValue).Count();
                    st.ToApproved = statsUser.Where(x => (x.Approbation.HasValue && x.Approbation == 1)).Count();
                    st.ToTested = statsUser.Where(x => (x.Test.HasValue && x.Test == 1)).Count();
                    if (st.NotRead > 0)
                        profilStat.NotRead += 1;
                    if (st.NotApproved > 0)
                        profilStat.NotApproved += 1;
                    if (st.NotTested > 0)
                        profilStat.NotTested += 1;
                    if (st.ToApproved > 0)
                        profilStat.ToApproved += 1;
                    if (st.ToTested > 0)
                        profilStat.ToTested += 1;

                }
            }
            return profilStat;
        }
        
       
        public List<Profile> GetAll()
        {
            return this._profilerepository.FindAll().ToList().OrderBy(p => p.Name).ToList();
        }
        public List<ProfilDTO> GetAll(GridMvcRequest request)
        {
            var query = this._profilerepository.RepositoryQuery.Select(x => new
            ProfilDTO()
            {
                ID = x.ID,
                Name = x.Name,
                Affecte = (x.ProfileDocument.Where(y => y.ID_Profile.Value == x.ID && y.Document.Inactif == false).Any() || x.ProfileUser.Any()) ? true : false,
            });
            var lst = query.ToList().OrderBy(request.OrderColumn + request.SortAscDesc).ToList();
            var sql = SQL_STATS_PROFIL;
            var queryStat = _docrepository.Database.SqlQuery<StatistiquesDTO>(sql).AsQueryable();
            var stats = queryStat.ToList();

            foreach (ProfilDTO item in lst)
            {
                Statistiques stat = GetStatsProfil(stats, item.ID);
                item.Statistiques = stat;
            }
            return lst;
        }
        public ProfilDTO GetProfilById(int idProfil, bool getAssoc = false)
        {

            bool affecte = false;
            if (getAssoc)
            {
                var result = _profilerepository.RepositoryTable.Include("ProfileUser").Include("ProfileDocument").Include("ProfileDocument.Document").Where(x => x.ID == idProfil).SingleOrDefault();
                if (result != null)
                {
                    var profil = new ProfilDTO()
                    {
                        ID = result.ID,
                        Name = result.Name,
                        Affecte = (result.ProfileDocument.Where(y => y.ID_Profile.Value == result.ID && y.Document.Inactif == false).Any() || result.ProfileUser.Any())
                    };
                    return profil;
                }
            }
            var prf = _profilerepository.FindBy(idProfil);
            return new ProfilDTO() { ID = prf.ID, Name = prf.Name, Affecte = affecte };

        }
        public GetProfilResponse Get(GetProfilRequest request)
        {
            var response = new GetProfilResponse();
            try
            {
                if (request.Id.HasValue)
                {
                    response.Profile = this.GetProfilById(request.Id.Value);
                    response.ListUser = this.ListUserByProfilID(request.Id.Value, true);

                }
                else
                {
                    response.Profile = new ProfilDTO();
                    response.ListUser = new List<UserDTO>();
                }
                response.ListDocument = this.ListDocument(request);
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "Get",
                    ServiceName = "ProfilService",

                }, ex);
                throw ex;
            }

        }
        private string[] ReplaceFiltre(string[] filtres)
        {
            if (filtres != null)
            {
                int i = 0;
                string newFiltre = "";
                foreach (string filtre in filtres)
                {
                    newFiltre = filtre;
                    if (filtre.IndexOf("Approbation") != -1 || filtre.IndexOf("Test") != -1)
                    {
                        newFiltre = newFiltre.Replace("true", "1");
                        newFiltre = newFiltre.Replace("false", "0");
                    }
                    filtres[i] = newFiltre;
                    i++;
                }
            }
            return filtres;
        }
        public override string GetOperator(Type type, ColumnFilter columnFilter, int index, string filterValue)
        {
            if(type== typeof(DocumentDTO))
            {
                if ("ID_Category".Equals(columnFilter.ColumnName) && "-1".Equals(filterValue))
                {
                    return "(ID_Category== null OR ID_Category==0)";
                }
                else if ("ID_SubCategory".Equals(columnFilter.ColumnName) && "-1".Equals(filterValue))
                {
                    return "(ID_SubCategory== null OR ID_SubCategory==0)";
                }
                else if ("Name".Equals(columnFilter.ColumnName) && !string.IsNullOrEmpty(filterValue))
                {
                    columnFilter.Field = "EdmxFunction.RemoveAccent(Name)";
                    columnFilter.FilterValue = Utils.RemoveAccent(filterValue);
                }
            }           
            return columnFilter.GetOperator(type, index, filterValue);
        }

        /// <summary>
        /// Liste des documents dont le Type d'affectation== 'Profil'
        /// </summary>
        /// <param name="idProfil"></param>
        /// <returns></returns>
        public List<DocumentDTO> ListDocument(GetProfilRequest request)
        {
            string TYPE_AFFECTATION = "Profil";
            string orderBy = "OrdreCategory, OrdreSubCategory, " + request.GridRequest.OrderColumn + request.GridRequest.SortAscDesc;
            var query = _docrepository.RepositoryQuery.Where(x => x.Inactif == false)
                 .GroupJoin(
                    this._doclangrepository.RepositoryQuery.Where(l => l.ID_Lang == request.IdLang),
                    docL => docL.ID,
                    lang => lang.ID_Document,
                    (docL, lang) => new { docL, lang }).SelectMany(x => x.lang.DefaultIfEmpty(), (parent, child) => new { docL = parent.docL, lang = child })
              .GroupJoin(this._subcategrepository.RepositoryTable,
                   docSub => docSub.docL.ID_SubCategory,
                   sub => sub.ID,
                   (docSub, sub) => new { docSub, sub }).SelectMany(x => x.sub.DefaultIfEmpty(), (parent, child) => new { docSub = parent.docSub, sub = child })
               .GroupJoin(this._doccategoryrepository.RepositoryTable.Include("DocumentCategoryLang"),
                    docCat => docCat.docSub.docL.ID_Category,
                   categ => categ.ID,
                   (docCat, categ) => new { doc = docCat.docSub, categ, sub = docCat.sub }).Where(x => x.doc.docL.TypeAffectation.Equals(TYPE_AFFECTATION))
              .SelectMany(x => x.categ.DefaultIfEmpty(), (parent, child) => new DocumentDTO()
               {
                   ID = parent.doc.docL.ID,
                   Name = EdmxFunction.GetNameDocument(parent.doc.docL.ID, request.IdLang, request.IdDefaultLang),
                  IsDefaultLangName = string.IsNullOrEmpty(parent.doc.lang.Name),
                  NameSubCategory = EdmxFunction.GetSubCategoryLang(parent.doc.docL.ID_SubCategory.Value, request.IdLang, request.IdLang),
                   DefaultNameSubCategory = EdmxFunction.GetSubCategoryLang(parent.doc.docL.ID_SubCategory.Value, request.IdDefaultLang, request.IdDefaultLang),
                  Version = parent.doc.docL.Version,
                   Date = parent.doc.docL.Date,
                   Commentaire = parent.doc.docL.Commentaire,
                   Approbation = parent.doc.docL.Approbation,
                   Test = parent.doc.docL.Test,
                   ContentType = parent.doc.docL.ContentType,
                   TypeAffectation = parent.doc.docL.TypeAffectation,
                   //Qcm = parent.doc.Qcm,
                   Inactif = parent.doc.docL.Inactif,
                  
                   Extension = parent.doc.docL.Extension,
                   ID_Category = parent.doc.docL.ID_Category,
                   ID_SubCategory = parent.doc.docL.ID_SubCategory,
                   IsNoActionRequired = parent.doc.docL.IsNoActionRequired,
                   DocumentCategoryLang = child.DocumentCategoryLang.FirstOrDefault(f => f.ID_Lang == request.IdLang),
                  DefaultNameCategory = EdmxFunction.GetCategoryLang(parent.doc.docL.ID_Category.Value, request.IdDefaultLang, request.IdDefaultLang),
                  OrdreCategory = child.OrdreCategory != null ? child.OrdreCategory : 1000,
                   OrdreSubCategory = parent.sub.Ordre != null ? parent.sub.Ordre : 1000
              });
            
            List < ColumnFilter > columnsFilters = this.GetColumnFilters(ReplaceFiltre(request.GridRequest.Filtres), null);
            //Le filtre column "IsSessionChecked" est traité après l'execution de la requete
            columnsFilters.Remove(new ColumnFilter() { ColumnName = "IsSessionChecked" });
            query = this.FiltrerQuery(columnsFilters, query).OrderBy(orderBy);
            List<DocumentDTO> lst = query.ToList();           
            foreach (var item in lst)
            {
                List<int> lstDoc = new List<int>();
                if (request.Id.HasValue)
                    lstDoc = this.ListProfileDocument(request.Id.Value);
                if (lstDoc != null && lstDoc.Contains(item.ID))
                {
                    item.IsChecked = true;
                    item.IsSessionChecked = true;
                }
                //Affecté : Coché ou non en session
                if (request.DocsAffected != null)
                {
                    var docState = request.DocsAffected.Find(x => x.DocId == item.ID);
                    if (docState != null && docState.OldCheckState != docState.NewCheckState)
                    {
                        item.IsSessionChecked = docState.NewCheckState;
                    }
                }
            }
            //Le filtre column "IsSessionChecked" est traité après l'execution de la requete
            columnsFilters = this.GetColumnFilters(ReplaceFiltre(request.GridRequest.Filtres), new List<string>() { "IsSessionChecked" });
            lst = this.FiltrerQuery(columnsFilters, lst.AsQueryable<DocumentDTO>()).ToList();
            return lst;
        }

        private List<int> ListProfileDocument(int idProfil)
        {
            var query = _profilDocrepository.RepositoryQuery.GroupJoin(this._docrepository.RepositoryTable,
               prf => prf.ID_Document,
               doc => doc.ID,
               (prf, doc) => new { prf, doc })
               .Where(x => x.prf.ID_Profile == idProfil)
               .SelectMany(x => x.doc.DefaultIfEmpty(), (parent, child) => new
               {
                   child.ID,
                   Inactif = child.Inactif,
                
               })
               .Where(x => x.Inactif == false).Select(x => x.ID);
            var lst = query.ToList();
            return lst;
        }
        public List<UserDTO> ListUsersForRelance(int profilID)
        {
            var query = this._profilUserrepository.RepositoryQuery.GroupJoin(
                this._userrepository.RepositoryTable,
                pu => pu.ID_IntitekUser,
                iu => iu.ID,
                (pu, iu) => new { pu, iu })
                .Where(x => x.pu.ID_Profile == profilID)
                .SelectMany(x => x.iu.DefaultIfEmpty(), (parent, child) => new UserDTO()
                {
                    ID = child.ID,
                    Name = child.Username,
                    FullName = child.FullName,
                    Status = child.Status,
                    EntityName = child.EntityName,
                    AgencyName = child.AgencyName,
                    EmailOnBoarding = child.EmailOnBoarding,
                    IsOnBoarding = child.isOnBoarding,
                    Active = child.Active,
                    InactivityStart = child.InactivityStart,
                    InactivityEnd = child.InactivityEnd
                });
            query = query.Where(x => x.Active &&
                !(x.InactivityStart.HasValue && DbFunctions.TruncateTime(x.InactivityStart) <= DbFunctions.TruncateTime(System.Data.Entity.SqlServer.SqlFunctions.GetDate()) &&
                                 (!x.InactivityEnd.HasValue || (x.InactivityEnd.HasValue && DbFunctions.TruncateTime(x.InactivityEnd) >= DbFunctions.TruncateTime(System.Data.Entity.SqlServer.SqlFunctions.GetDate())))));
            var lst = query.OrderBy(x => x.Name).ToList();
           return lst;
        }
        public List<UserDTO> ListUserByProfilID(int profilID, bool isactive)
        {
            var query = this._profilUserrepository.RepositoryQuery.GroupJoin(
                this._userrepository.RepositoryTable,
                pu => pu.ID_IntitekUser,
                iu => iu.ID,
                (pu, iu) => new { pu, iu })
                .Where(x => x.pu.ID_Profile == profilID)
                .SelectMany(x => x.iu.DefaultIfEmpty(), (parent, child) => new UserDTO()
                {
                    ID = child.ID,
                    Name = child.Username,
                    FullName = child.FullName,
                    Status = child.Status,
                    EntityName = child.EntityName,
                    AgencyName = child.AgencyName,
                    EmailOnBoarding = child.EmailOnBoarding,
                    IsOnBoarding = child.isOnBoarding,
                    Active = child.Active
                });
            if (isactive)
            {
                query = query.Where(x => x.Active);
            }
            var lst = query.OrderBy(x=> x.Name).ToList();
            return lst;
        }
        /// <summary>
        /// Etats des documents
        /// </summary>
        /// <param name="profilID"></param>
        /// <returns></returns>
        public List<DocumentDTO> ListDocumentByProfileId(GetProfilRequest request)
        {
            var query = _docrepository.Database.SqlQuery<StatistiquesDTO>(SQL_STATS_PROFIL).AsQueryable();
            query = query.Where(x => x.ID_Profile.Equals(request.Id));
            List<StatistiquesDTO> statsDTO = query.ToList();
            var lstIDDocs = statsDTO.Select(x => x.ID).Distinct().ToList();
            List<DocumentDTO> docsDTO = GetDocumentsByIds(request.IdLang, request.IdDefaultLang, lstIDDocs);
            List<DocumentDTO> lstdocs = new List<DocumentDTO>();
            foreach (var dto in docsDTO)
            {
                Statistiques stat = new Statistiques();
                stat.NotRead = statsDTO.Where(x => x.ID==dto.ID && !x.IsRead.HasValue).Select(x => x.IdUser).Distinct().Count();
                stat.NotApproved = statsDTO.Where(x => x.ID == dto.ID && (x.Approbation.HasValue && x.Approbation == 1) && !x.IsApproved.HasValue).Select(x => x.IdUser).Distinct().Count();
                stat.NotTested = statsDTO.Where(x => x.ID == dto.ID && (x.Test.HasValue && x.Test == 1) && !x.IsTested.HasValue).Select(x => x.IdUser).Distinct().Count();
                stat.ToRead = statsDTO.Where(x => x.ID == dto.ID).Select(x => x.IdUser).Distinct().Count();
                stat.ToApproved = statsDTO.Where(x => x.ID == dto.ID && (x.Approbation.HasValue && x.Approbation == 1)).Select(x => x.IdUser).Distinct().Count();
                stat.ToTested = statsDTO.Where(x => x.ID == dto.ID && (x.Test.HasValue && x.Test == 1)).Select(x => x.IdUser).Distinct().Count();
                dto.Statistiques = stat;
            }
            return docsDTO;
        }
        public void Delete(int profilID)
        {
            try
            {
                var profil = this._profilerepository.FindBy(profilID);
                this._profilerepository.Remove(profil);
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "Delete",
                    ServiceName = "ProfilService",

                }, ex);
                throw ex;
            }
        }
        public bool IsProfilNameExist(SaveProfilRequest request)
        {
            var name = Utils.RemoveAccent(request.Name);
            if (request.Id.HasValue && request.Id.Value>0)
                {
                    var prf = this._profilerepository.RepositoryQuery.Where(x => x.ID != request.Id.Value && EdmxFunction.RemoveAccent(x.Name).Equals(name)).FirstOrDefault();
                    if (prf != null) return true;
                }
                else
                {
                    var prf = this._profilerepository.RepositoryQuery.Where(x => EdmxFunction.RemoveAccent(x.Name).Equals(name)).FirstOrDefault();
                    if (prf != null) return true;
                }
                return false;
        }
        public void Save(SaveProfilRequest request)
        {
            var profil = new Profile();
            List<DocCheckState> lstdocs = request.DocsAffected;
            if (request.Id.HasValue && request.Id.Value > 0)
            {
                profil.ID = request.Id.Value;
                //On affecte Id pour faire une modification
                profil.Id = request.Id.Value;
            }
            profil.Name = request.Name;
            List<int> lProfileToDelete = new List<int>();
            List<int> lDocsAdd = new List<int>();
            var profilDocumentToDelete = new List<ProfilDocumentDTO>();
            if (lstdocs != null && lstdocs.Count > 0)
            {
                //Ajout
                lDocsAdd = lstdocs.Where(x => x.NewCheckState && x.OldCheckState == false).Select(x => x.DocId).ToList();
                foreach (var doc in lDocsAdd)
                {
                    profil.ProfileDocument.Add(new ProfileDocument() { ID_Document = doc, Date = DateTime.Today });
                }
                //Suppression
                lProfileToDelete = lstdocs.Where(x => x.NewCheckState == false && x.OldCheckState).Select(x => x.DocId).ToList();
                if (request.Id.HasValue && request.Id.Value > 0)
                {
                    profilDocumentToDelete = _profilDocrepository.RepositoryQuery.Select(x => new ProfilDocumentDTO()
                    {
                        ID = x.ID,
                        ID_Document = x.ID_Document,
                        ID_Profile = x.ID_Profile,
                        Date = x.Date
                    }).Where(p => lProfileToDelete.Contains(p.ID_Document.Value) && p.ID_Profile == request.Id.Value).ToList();
                }                 
            }
            _profilerepository.SaveOrUpdate(profil, lProfileToDelete);
            if (request.Id.HasValue && request.Id.Value > 0)
            {
                //Histo Actions supprimées
                foreach (var pd in profilDocumentToDelete)
                {
                    HistoActions hA = new HistoActions() { ID_Object = pd.ID_Document.Value, LinkedObjects=pd.ID_Profile.ToString(), ObjectCode = ObjectCode.PROFILE_DOCUMENT, ID_IntitekUser = request.ID_IntitekUser, DateAction = DateTime.Now, Action = Actions.Delete };
                    _histoactionsrepository.Save(hA);
                }
                //Histo Actions Ajoutées
                if (lDocsAdd != null && lDocsAdd.Count > 0)
                {
                    var profilDocsAdd = profil.ProfileDocument.Where(p => lDocsAdd.Contains(p.ID_Document.Value)).ToList();
                    foreach (var pd in profilDocsAdd)
                    {
                        HistoActions hA = new HistoActions() { ID_Object = pd.ID, ObjectCode = ObjectCode.PROFILE_DOCUMENT, ID_IntitekUser = request.ID_IntitekUser, DateAction = DateTime.Now, Action = Actions.Create };
                        _histoactionsrepository.Save(hA);
                    }
                }
                   
            }
            else
            {
                //Histo Actions Ajoutées
                foreach (var pd in profil.ProfileDocument)
                {
                    HistoActions hA = new HistoActions() { ID_Object = pd.ID, ObjectCode = ObjectCode.PROFILE_DOCUMENT, ID_IntitekUser = request.ID_IntitekUser, DateAction = DateTime.Now, Action = Actions.Create };
                    _histoactionsrepository.Save(hA);
                }
            }
        }
    }
}
