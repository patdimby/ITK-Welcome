using Intitek.Welcome.DataAccess;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.Helpers;
using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Infrastructure.Specification;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic;


namespace Intitek.Welcome.Service.Front
{
    public class DocumentService : BaseService, IDocumentService
    {
        private readonly UserDataAccess _userrepository;
        private readonly DocumentDataAccess _docrepository;
        private readonly DocumentLangDataAccess _doclangrepository;
        private readonly LangDataAccess _langrepository;
        private readonly DocumentCategoryDataAccess _doccategoryrepository;
        private readonly DocumentCategoryLangDataAccess _doccategorylangrepository;
        private readonly EntityDocumentDataAccess _entityDocrepository;
        private readonly ProfileDocumentDataAccess _profilDocrepository;
        private readonly UserProfileDataAccess _profilUserrepository;
        private readonly UserDocumentDataAccess _userDocrepository;
        private readonly UserQcmDataAccess _userqcmrepository;
        private readonly HistoUserQcmDocVersionDataAccess _histoUserqcmrepository;
        private readonly QcmDataAccess _qcmrepository;
        private readonly QcmLangDataAccess _qcmlangRepository;
        private readonly SubCategoryDataAccess _subcategoryrepository;

        private string SQL_USERDOC_ALL = "SELECT * FROM(SELECT UDocument.*, {{schema}}GetCategoryLang(dc.ID, {0}, {1}) as NameCategory," +
            "CASE WHEN (dc.OrdreCategory IS NOT NULL) THEN dc.OrdreCategory ELSE 1000 END AS OrdreCategory, " +
            "CASE WHEN (sub.Ordre IS NOT NULL) THEN sub.Ordre ELSE 1000 END AS OrdreSubCategory FROM " +
            "(SELECT 1 as Num, " +
            "		doc.ID, " +
            "       ud.ID_IntitekUser as IdUser, " +
            "		{{schema}}GetNameDocument(doc.ID, {0}, {1}) as [Name], " +
            "		{{schema}}GetSubCategoryLang(doc.ID_SubCategory, {0}, {1}) as [NameSubCategory], " +
            "       doc.IsNoActionRequired," +
            "       doc.ID_Category," +
            "       doc.ID_SubCategory," +
            "       doc.Inactif," +
            "		doc.Version, " +
            "		{{schema}}GetNomOrigineFichierDocument(doc.ID, {0}, {1}) as NomOrigineFichier, " +
            "		doc.Extension, " +
            "		doc.isMetier, " +
            "		doc.isStructure, " +
            "		ud.IsRead, " +
            "		ud.IsApproved, " +
            "		ud.IsTested, " +
            " CASE WHEN(ud.[IsApproved] IS NOT NULL) THEN cast(1 as bit) ELSE cast(0 as bit) END AS [IsBoolApproved], " +
            " CASE WHEN(ud.[IsTested] IS NOT NULL) THEN cast(1 as bit) ELSE cast(0 as bit) END AS [IsBoolTested], " +
            " CASE WHEN(ud.[IsRead] IS NOT NULL) THEN cast(1 as bit) ELSE cast(0 as bit) END AS [IsBoolRead], " +
            "		doc.Approbation, " +
            "		doc.Test, " +
            "		doc.IdQcm, " +
            "		doc.ReadBrowser, " +
            "		doc.ReadDownload " +
            "	FROM  UserDocument ud " +
            "left join Document doc on doc.ID= ud.ID_Document " +
            "WHERE ud.ID_IntitekUser = @userId AND " +
            "      doc.Inactif='false' AND  " +
            "	  ( " +
            "	  EXISTS (SELECT * FROM EntityDocument " +
            "	          WHERE EntityDocument.ID_Document=ud.ID_Document AND " +
            "	                EntityName = (SELECT EntityName FROM IntitekUser WHERE IntitekUser.ID=@userId) AND " +
            "					(AgencyName= (SELECT AgencyName from IntitekUser where IntitekUser.ID=@userId) OR AgencyName IS NULL)) " +
            "	  OR " +
            "	  EXISTS (SELECT * FROM ProfileDocument " +
            "	          WHERE ProfileDocument.ID_Document=ud.ID_Document AND " +
            "			        ProfileDocument.ID_Profile IN (SELECT ID_Profile FROM ProfileUser WHERE ProfileUser.ID_IntitekUser=@userId)) " +
            "	  ) " +
            "union " +
            "SELECT 2 as Num, " +
            "		doc.ID, " +
            "		pu.ID_IntitekUser as IdUser, " +
            "		{{schema}}GetNameDocument(doc.ID, {0}, {1}) as [Name], " +
            "		{{schema}}GetSubCategoryLang(doc.ID_SubCategory, {0}, {1}) as [NameSubCategory], " +
            "       doc.IsNoActionRequired," +
            "       doc.ID_Category," +
            "       doc.ID_SubCategory," +
            "       doc.Inactif," +
            "		doc.Version, " +
            "		{{schema}}GetNomOrigineFichierDocument(doc.ID, {0}, {1}) as NomOrigineFichier, " +
            "		doc.Extension, " +
            "		doc.isMetier, " +
            "		doc.isStructure, " +
            "		NULL as IsRead, " +
            "		NULL as IsApproved, " +
            "		NULL as IsTested, " +
            "       cast(0 as bit) as IsBoolApproved, " +
            "       cast(0 as bit) as IsBoolTested, " +
            "       cast(0 as bit) as IsBoolRead, " +
            "		doc.Approbation, " +
            "		doc.Test, " +
            "		doc.IdQcm, " +
            "		doc.ReadBrowser, " +
            "		doc.ReadDownload " +
            "	FROM ProfileDocument pd " +
            "left join ProfileUser pu on pu.ID_Profile=pd.ID_Profile " +
            "left join Document doc on doc.ID= pd.ID_Document " +
            "where pd.ID_Profile in( select ID_Profile from ProfileUser where ID_IntitekUser= @userId) and " +
            "(ID_Document not in(select ID_Document from  UserDocument " +
            "where ID_IntitekUser = @userId ) ) " +
            "and pu.ID_IntitekUser=@userId " +
            "and doc.Inactif='false' " +
            "union " +
            "SELECT 3 as Num, " +
            "		ID_Document as ID," +
            "		iu.ID as IdUser, " +
            "		{{schema}}GetNameDocument(doc.ID, {0}, {1}) as [Name], " +
            "		{{schema}}GetSubCategoryLang(doc.ID_SubCategory, {0}, {1}) as [NameSubCategory], " +
            "       doc.IsNoActionRequired," +
            "       doc.ID_Category," +
            "       doc.ID_SubCategory," +
            "       doc.Inactif," +
            "		doc.Version, " +
            "		{{schema}}GetNomOrigineFichierDocument(doc.ID, {0}, {1}) as NomOrigineFichier, " +
            "		doc.Extension, " +
            "		doc.isMetier, " +
            "		doc.isStructure, " +
            "		NULL as IsRead, " +
            "		NULL  as IsApproved, " +
            "		NULL as IsTested, " +
            "       cast(0 as bit) as IsBoolApproved, " +
            "       cast(0 as bit) as IsBoolTested, " +
            "       cast(0 as bit) as IsBoolRead, " +
            "		doc.Approbation, " +
            "		doc.Test, " +
            "		doc.IdQcm, " +
            "		doc.ReadBrowser, " +
            "		doc.ReadDownload " +
            "	FROM EntityDocument ed " +
            "left join IntitekUser iu on iu.ID= @userId " +
            "left join Document doc on doc.ID= ed.ID_Document " +
            "where ed.EntityName = (select EntityName from IntitekUser where IntitekUser.ID=@userId) and " +
            "		(ed.AgencyName=(select AgencyName from IntitekUser where IntitekUser.ID=@userId ) or ed.AgencyName is null )and " +
            "(ID_Document not in(select ID_Document from  UserDocument " +
            "where ID_IntitekUser = @userId ) ) " +
            "and iu.ID=@userId " +
            "and doc.Inactif='false') as UDocument " +
            " LEFT JOIN SubCategory sub on (sub.ID= UDocument.ID_SubCategory) " +
            " LEFT JOIN DocumentCategory dc on (dc.ID= UDocument.ID_Category)) as Tbl";
        public DocumentService(ILogger logger) : base(logger)
        {
            _docrepository = new DocumentDataAccess(uow);
            _doclangrepository = new DocumentLangDataAccess(uow);
            _doccategoryrepository = new DocumentCategoryDataAccess(uow);
            _doccategorylangrepository = new DocumentCategoryLangDataAccess(uow);
            _entityDocrepository = new EntityDocumentDataAccess(uow);
            _userDocrepository = new UserDocumentDataAccess(uow);
            _profilDocrepository = new ProfileDocumentDataAccess(uow);
            _profilUserrepository = new UserProfileDataAccess(uow);
            _userrepository = new UserDataAccess(uow);
            _qcmrepository = new QcmDataAccess(uow);
            _userqcmrepository = new UserQcmDataAccess(uow);
            _histoUserqcmrepository = new HistoUserQcmDocVersionDataAccess(uow);
            _qcmlangRepository = new QcmLangDataAccess(uow);
            _langrepository = new LangDataAccess(uow);
            _subcategoryrepository =  new SubCategoryDataAccess(uow);
 
            //path for System.Link.Dynamic pour reconnaitre la classe EdmxFunction
            this.DynamicLinkPatch();
        }
        public List<DocumentCategoryDTO> GetAllCategory(int idLang, int idDefaultLang)
        {
            return _doccategoryrepository.RepositoryQuery.Select(x=> new DocumentCategoryDTO() {
                ID = x.ID,
                OrdreCategory = x.OrdreCategory,
                Name = EdmxFunction.GetCategoryLang(x.ID, idLang, idDefaultLang)
            }).OrderBy(p => p.OrdreCategory).ToList();
        }
        public List<DocumentSubCategoryDTO> GetAllSubCategory(int idLang, int idDefaultLang)
        {
            return _subcategoryrepository.RepositoryQuery
                .Join(_doccategoryrepository.RepositoryTable,
                    sub=> sub.ID_DocumentCategory,
                    cat=> cat.ID,
                    (sub, cat)=>  new { sub, cat }
                )
            .Select(x => new DocumentSubCategoryDTO()
            {
                ID = x.sub.ID,
                ID_DocumentCategory = x.sub.ID_DocumentCategory,
                NameCategory = EdmxFunction.GetCategoryLang(x.sub.ID_DocumentCategory, idLang, idDefaultLang),
                OrdreSubCategory = x.sub.Ordre,
                OrdreCategory = x.cat.OrdreCategory,
                Name = EdmxFunction.GetSubCategoryLang(x.sub.ID, idLang, idDefaultLang)
            }).OrderBy(p=> p.OrdreCategory).ThenBy(p => p.OrdreSubCategory).ToList();
        }
        public List<String> GetAllVersion()
        {
            var query = _docrepository.RepositoryTable.Select(p => p.Version.ToUpper()).Distinct().OrderBy(p => p);
            var versions = query.ToList();
            return versions;
        }
        private string FiltrerQueryAction(ColumnFilter columnFilter, string where)
        {
            if (!string.IsNullOrEmpty(columnFilter.FilterValue))
            {
                if (columnFilter.FilterValue.Contains("|"))
                {
                    var filterValues = columnFilter.FilterValue.Split('|');
                    string orWhere = "";
                    for (int i = 0; i < filterValues.Count(); i++)
                    {
                        int value = Int32.Parse(filterValues[i]);
                        if (i == 0)
                        {
                            orWhere = columnFilter.Where(value);
                        }
                        else
                        {
                            orWhere += " OR " + columnFilter.Where(value);
                        }
                    }
                    if (!string.IsNullOrEmpty(where))
                        where += " AND (" + orWhere + ")";
                    else
                        where = "(" + orWhere + ")";
                }
                else
                {
                    int value = Int32.Parse(columnFilter.FilterValue);
                    if (!string.IsNullOrEmpty(where))
                        where += " AND " + columnFilter.Where(value);
                    else
                        where = columnFilter.Where(value);
                }
            }
            return where;
        }
        private IQueryable<DocumentDTO> FiltrerQuery(string[] filterColumns, IQueryable<DocumentDTO> query)
        {
            int index = 0;
            string where = "";
            List<object> values = new List<object>();
            if (filterColumns != null && filterColumns.Count() > 0)
            {
                foreach (var queryFilter in filterColumns)
                {
                    ColumnFilter columnFilter = ColumnFilter.CreateColumnFilter(queryFilter);
                    if (columnFilter == null) continue;
                    System.Reflection.PropertyInfo property = typeof(DocumentDTO).GetProperty(columnFilter.ColumnName);
                    if (property == null)
                    {
                        where = FiltrerQueryAction(columnFilter, where);
                    }
                    else
                    {
                        if (columnFilter.FilterValue.Contains("|"))
                        {
                            string orWhere = "";
                            var filterValues = columnFilter.FilterValue.Split('|');
                            for (int i = 0; i < filterValues.Count(); i++)
                            {
                                if (i == 0)
                                {
                                    orWhere = columnFilter.GetOperator(index, filterValues[i]);
                                }
                                else
                                {
                                    orWhere += " OR " + columnFilter.GetOperator(index, filterValues[i]);
                                }
                                if (property.PropertyType == typeof(Int32) || property.PropertyType == typeof(Int32?))
                                {
                                    values.Add(Int32.Parse(filterValues[i]));
                                }
                                else
                                {
                                    values.Add(filterValues[i]);
                                }
                                index++;
                            }
                            if (!string.IsNullOrEmpty(where))
                                where += " AND (" + orWhere + ")";
                            else
                                where = "(" + orWhere + ")";
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(where))
                                where += " AND " + columnFilter.GetOperator(index, columnFilter.FilterValue);
                            else
                                where = columnFilter.GetOperator(index, columnFilter.FilterValue);
                            if (property.PropertyType == typeof(Int32) || property.PropertyType == typeof(Int32?))
                            {
                                values.Add(Int32.Parse(columnFilter.FilterValue));
                            }
                            else
                            {
                                values.Add(columnFilter.FilterValue);
                            }
                            index++;
                        }
                    }

                }
                if (!string.IsNullOrEmpty(where))
                    query = query.Where(where, values.ToArray());
            }
            return query;
        }
        private IQueryable<DocumentDTO> GetListDocumentByUserLoginQueryable(bool noAction, bool luRevu, GetUserDocumentRequest request)
        {
            int userId = request.UserID;
            string rechercheMultiCol = request.Search;
            string[] filterColumns = request.Filtres;
            int idLang = request.IDLang;
            int idDefaultLang = _langrepository.FindBy(new Specification<Lang>(lg => lg.ID != request.IDLang)).FirstOrDefault().ID; // request.IDDefaultLang;
            IntitekUser user = _userrepository.FindBy(userId);
            var query1_0 = this._entityDocrepository.RepositoryQuery.Where(e =>
                    e.EntityName.Equals(user.EntityName)
                    && (e.AgencyName.Equals(user.AgencyName) || string.IsNullOrEmpty(e.AgencyName)));

            var query1 = this._userDocrepository.RepositoryQuery
                .Where(x => x.ID_IntitekUser.HasValue && x.ID_IntitekUser.Value == user.ID
                    && (
                    query1_0.Where(e => e.ID_Document.HasValue && e.ID_Document.Value == x.ID_Document.Value).Any()
                    || this._profilDocrepository.RepositoryQuery.Where(d => d.ID_Document.HasValue && d.ID_Document == x.ID_Document
                        && this._profilUserrepository.RepositoryQuery.Where(u => u.ID_IntitekUser.Value == user.ID && u.ID_Profile.HasValue).Select(u => u.ID_Profile).Contains(d.ID_Profile.Value)).Any()
                    )
            )
            .GroupJoin(this._docrepository.RepositoryTable.GroupJoin(
                    this._doccategoryrepository.RepositoryTable,
                    doc => doc.ID_Category,
                    categ => categ.ID,
                    (doc, categ) => new { doc, categ }).SelectMany(x => x.categ.DefaultIfEmpty(), (parent, child) => new {
                        ID = parent.doc.ID,
                        Name = EdmxFunction.GetNameDocument(parent.doc.ID, idLang, idDefaultLang),
                        NameSubCategory = EdmxFunction.GetSubCategoryLang(parent.doc.ID_SubCategory.Value, idLang, idDefaultLang),
                        Version = parent.doc.Version,
                        Extension = parent.doc.Extension,
                        isMetier = parent.doc.isMetier,
                        isStructure = parent.doc.isStructure,
                        NomOrigineFichier = EdmxFunction.GetNomOrigineFichierDocument(parent.doc.ID, idLang, idDefaultLang),
                        Approbation = parent.doc.Approbation,
                        Test = parent.doc.Test,
                        IdQcm = parent.doc.IdQcm,
                        ID_Category = parent.doc.ID_Category,
                        ID_SubCategory = parent.doc.ID_SubCategory,
                        Inactif = parent.doc.Inactif,
                        IsNoActionRequired = parent.doc.IsNoActionRequired,
                        ReadBrowser = parent.doc.ReadBrowser,
                        ReadDownload = parent.doc.ReadDownload,
                        NameCategory = EdmxFunction.GetCategoryLang(child.ID, idLang, idDefaultLang),
                        OrdreCategory = child.OrdreCategory != null ? child.OrdreCategory : 1000,
                        OrdreSubCategory = child.SubCategory.Where(x=> x.ID== parent.doc.ID_SubCategory).Select(x=>x.Ordre).FirstOrDefault()
                    }),
                ud => ud.ID_Document,
                doc => doc.ID,
                (ud, doc) => new { ud, doc })
           .SelectMany(x => x.doc.DefaultIfEmpty(), (parent, child) => new DocumentDTO
           {
               Num = 1,
               IdUser = parent.ud.ID_IntitekUser.Value,
               ID = child.ID,
               Name = child.Name.Trim(),
               NameSubCategory = child.NameSubCategory,
               Version = child.Version,
               Extension = child.Extension,
               isMetier = child.isMetier,
               isStructure = child.isStructure,
               NomOrigineFichier = child.NomOrigineFichier,
               Approbation = child.Approbation,
               Test = child.Test,
               IdQcm = child.IdQcm,
               ID_Category = child.ID_Category,
               ID_SubCategory = child.ID_SubCategory,
               Inactif = child.Inactif,
               IsNoActionRequired = child.IsNoActionRequired,
               NameCategory = child.NameCategory,
               OrdreCategory = child.OrdreCategory,
               OrdreSubCategory = child.OrdreSubCategory != null ? child.OrdreSubCategory : 1000,
               ReadBrowser = child.ReadBrowser,
               ReadDownload = child.ReadDownload,
               IsBoolApproved = parent.ud.IsApproved.HasValue,
               IsBoolTested = parent.ud.IsTested.HasValue,
               IsBoolRead = parent.ud.IsRead.HasValue,
               IsRead = parent.ud.IsRead,
               IsApproved = parent.ud.IsApproved,
               IsTested = parent.ud.IsTested
               //Score = parent.ud.Score,
               //ScoreMinimal = parent.ud.ScoreMinimal,
           })
           .Where(x => x.Inactif == false);

            if (!string.IsNullOrEmpty(rechercheMultiCol))
            {
                var search = Utils.RemoveAccent(rechercheMultiCol);
                query1 = query1.Where(x => (EdmxFunction.RemoveAccent(x.Name).Contains(search) ||
                   EdmxFunction.RemoveAccent(x.Version).Contains(search) ||
                   EdmxFunction.RemoveAccent(x.NameCategory).Contains(search)
               ));
            }
            if (luRevu)
            {
                query1 = query1.Where(x => (x.IsRead != null
                  && (x.Approbation == 1 ? x.IsApproved != null : true)
                  && (x.Test == 1 ? x.IsTested != null : true)
                 // && (x.Score.HasValue ? x.Score.Value >= x.ScoreMinimal.Value : true)
                 ));
                return this.FiltrerQuery(filterColumns, query1);
            }
            else
            {
                if (noAction)
                {
                    query1 = query1.Where(x => x.IsNoActionRequired);
                }
                else
                {
                    query1 = query1.Where(x => !x.IsNoActionRequired);
                }
                query1 = query1.Where(x => !(x.IsRead != null
                  && (x.Approbation == 1 ? x.IsApproved != null : true)
                  && (x.Test == 1 ? x.IsTested != null : true)
                 // && (x.Score.HasValue ? x.Score.Value >= x.ScoreMinimal.Value : true)
                 ));
            }

            var query2 = this._profilDocrepository.RepositoryQuery
                 .GroupJoin(
                     this._profilUserrepository.RepositoryTable,
                     pd => pd.ID_Profile,
                     pu => pu.ID_Profile,
                     (pd, pu) => new { pd, pu })
                     .SelectMany(x => x.pu.DefaultIfEmpty(), (parent, child) => new { pd = parent.pd, pu = child })
                     .Where(x => x.pu.ID_IntitekUser == userId
                       && this._profilUserrepository.RepositoryQuery.Where(p => p.ID_IntitekUser.HasValue && p.ID_IntitekUser == userId).Select(p => p.ID_Profile).Contains(x.pu.ID_Profile)
                        && !this._userDocrepository.RepositoryQuery.Where(u => u.ID_IntitekUser.HasValue && u.ID_IntitekUser == userId).Select(u => u.ID_Document).Contains(x.pd.ID_Document))
                 .GroupJoin(
                    this._docrepository.RepositoryTable.GroupJoin(
                            this._doccategoryrepository.RepositoryTable,
                            doc => doc.ID_Category,
                            categ => categ.ID,
                            (doc, categ) => new { doc, categ }).SelectMany(x => x.categ.DefaultIfEmpty(), (parent, child) => new {
                                ID = parent.doc.ID,
                                Name = EdmxFunction.GetNameDocument(parent.doc.ID, idLang, idDefaultLang),
                                NameSubCategory = EdmxFunction.GetSubCategoryLang(parent.doc.ID_SubCategory.Value, idLang, idDefaultLang),
                                Version = parent.doc.Version,
                                Extension = parent.doc.Extension,
                                isMetier = parent.doc.isMetier,
                                isStructure = parent.doc.isStructure,
                                NomOrigineFichier = EdmxFunction.GetNomOrigineFichierDocument(parent.doc.ID, idLang, idDefaultLang),
                                Approbation = parent.doc.Approbation,
                                Test = parent.doc.Test,
                                IdQcm = parent.doc.IdQcm,
                                ID_Category = parent.doc.ID_Category,
                                ID_SubCategory = parent.doc.ID_SubCategory,
                                Inactif = parent.doc.Inactif,
                                IsNoActionRequired = parent.doc.IsNoActionRequired,
                                ReadBrowser = parent.doc.ReadBrowser,
                                ReadDownload = parent.doc.ReadDownload,
                                NameCategory = EdmxFunction.GetCategoryLang(child.ID, idLang, idDefaultLang),
                                OrdreCategory = child.OrdreCategory != null ? child.OrdreCategory : 1000,
                                OrdreSubCategory = child.SubCategory.Where(x => x.ID == parent.doc.ID_SubCategory).Select(x => x.Ordre).FirstOrDefault()
                            }),
                    pdD => pdD.pd.ID_Document,
                    doc => doc.ID,
                    (pdD, doc) => new { pdD, doc })
                   .SelectMany(x => x.doc.DefaultIfEmpty(), (parent, child) => new DocumentDTO
                   {
                       Num = 2,
                       IdUser = parent.pdD.pu.ID_IntitekUser.Value,
                       ID = child.ID,
                       Name = child.Name.Trim(),
                       NameSubCategory = child.NameSubCategory,
                       Version = child.Version,
                       Extension = child.Extension,
                       isMetier = child.isMetier,
                       isStructure = child.isStructure,
                       NomOrigineFichier = child.NomOrigineFichier,
                       Approbation = child.Approbation,
                       Test = child.Test,
                       IdQcm = child.IdQcm,
                       ID_Category = child.ID_Category,
                       ID_SubCategory = child.ID_SubCategory,
                       Inactif = child.Inactif,
                       IsNoActionRequired = child.IsNoActionRequired,
                       NameCategory = child.NameCategory,
                       OrdreCategory = child.OrdreCategory,
                       OrdreSubCategory = child.OrdreSubCategory != null ? child.OrdreSubCategory : 1000,
                       ReadBrowser = child.ReadBrowser,
                       ReadDownload = child.ReadDownload,
                       IsBoolApproved = false,
                       IsBoolTested = false,
                       IsBoolRead = false,
                       IsRead = null,
                       IsApproved = null,
                       IsTested = null
                       //Score = null,
                       //ScoreMinimal = null
                   })
                   .Where(x => x.Inactif == false);
            if (!string.IsNullOrEmpty(rechercheMultiCol))
            {
                var search = Utils.RemoveAccent(rechercheMultiCol);
                query2 = query2.Where(x => (EdmxFunction.RemoveAccent(x.Name).Contains(search) ||
                   EdmxFunction.RemoveAccent(x.Version).Contains(search) ||
                   EdmxFunction.RemoveAccent(x.NameCategory).Contains(search)
               ));
            }

            var query3 = this._entityDocrepository.RepositoryQuery
                .Where(x => x.EntityName.Equals(user.EntityName)
                    && (x.AgencyName.Equals(user.AgencyName) || string.IsNullOrEmpty(x.AgencyName))
                    && !this._userDocrepository.RepositoryQuery.Where(u => u.ID_IntitekUser == user.ID).Select(u => u.ID_Document).Contains(x.ID_Document)
                )
                .GroupJoin(this._docrepository.RepositoryTable.GroupJoin(
                        this._doccategoryrepository.RepositoryTable,
                        doc => doc.ID_Category,
                        categ => categ.ID,
                        (doc, categ) => new { doc, categ }).SelectMany(x => x.categ.DefaultIfEmpty(), (parent, child) => new {
                            ID = parent.doc.ID,
                            Name = EdmxFunction.GetNameDocument(parent.doc.ID, idLang, idDefaultLang),
                            NameSubCategory = EdmxFunction.GetSubCategoryLang(parent.doc.ID_SubCategory.Value, idLang, idDefaultLang),
                            Version = parent.doc.Version,
                            Extension = parent.doc.Extension,
                            isMetier = parent.doc.isMetier,
                            isStructure = parent.doc.isStructure,
                            NomOrigineFichier = EdmxFunction.GetNomOrigineFichierDocument(parent.doc.ID, idLang, idDefaultLang),
                            Approbation = parent.doc.Approbation,
                            Test = parent.doc.Test,
                            IdQcm = parent.doc.IdQcm,
                            ID_Category = parent.doc.ID_Category,
                            ID_SubCategory = parent.doc.ID_SubCategory,
                            Inactif = parent.doc.Inactif,
                            IsNoActionRequired = parent.doc.IsNoActionRequired,
                            ReadBrowser = parent.doc.ReadBrowser,
                            ReadDownload = parent.doc.ReadDownload,
                            NameCategory = EdmxFunction.GetCategoryLang(child.ID, idLang, idDefaultLang),
                            OrdreCategory = child.OrdreCategory != null ? child.OrdreCategory : 1000,
                            OrdreSubCategory = child.SubCategory.Where(x => x.ID == parent.doc.ID_SubCategory).Select(x => x.Ordre).FirstOrDefault()
                        }),
                    ed => ed.ID_Document,
                    doc => doc.ID,
                    (ed, doc) => new { ed, doc })
               .SelectMany(x => x.doc.DefaultIfEmpty(), (parent, child) => new DocumentDTO
               {
                   Num = 3,
                   IdUser = user.ID,
                   ID = child.ID,
                   Name = child.Name.Trim(),
                   NameSubCategory = child.NameSubCategory,
                   Version = child.Version,
                   Extension = child.Extension,
                   isMetier = child.isMetier,
                   isStructure = child.isStructure,
                   NomOrigineFichier = child.NomOrigineFichier,
                   Approbation = child.Approbation,
                   Test = child.Test,
                   IdQcm = child.IdQcm,
                   ID_Category = child.ID_Category,
                   ID_SubCategory = child.ID_SubCategory,
                   Inactif = child.Inactif,
                   IsNoActionRequired = child.IsNoActionRequired,
                   NameCategory = child.NameCategory,
                   OrdreCategory = child.OrdreCategory,
                   OrdreSubCategory = child.OrdreSubCategory != null ? child.OrdreSubCategory : 1000,
                   ReadBrowser = child.ReadBrowser,
                   ReadDownload = child.ReadDownload,
                   IsBoolApproved = false,
                   IsBoolTested = false,
                   IsBoolRead = false,
                   IsRead = null,
                   IsApproved = null,
                   IsTested = null
                   //Score = null,
                   //ScoreMinimal = null
               })
               .Where(x => x.Inactif == false);
            if (!string.IsNullOrEmpty(rechercheMultiCol))
            {
                var search = Utils.RemoveAccent(rechercheMultiCol);
                query3 = query3.Where(x => (EdmxFunction.RemoveAccent(x.Name).Contains(search) ||
                   EdmxFunction.RemoveAccent(x.Version).Contains(search) ||
                   EdmxFunction.RemoveAccent(x.NameCategory).Contains(search)
               ));
            }
            if (noAction)
            {
                query2 = query2.Where(x => x.IsNoActionRequired);
                query3 = query3.Where(x => x.IsNoActionRequired);
            }
            else
            {
                query2 = query2.Where(x => !x.IsNoActionRequired);
                query3 = query3.Where(x => !x.IsNoActionRequired);
            }
            var query = query1.Union(query2);
            query = query.Union(query3);
            if (!luRevu)
            {
                if (user.Type == Constante.UserType_METIER)
                {
                    query = query.Where(x => x.isMetier);
                }
                else if (user.Type == Constante.UserType_STRUCTURE)
                {
                    query = query.Where(x => x.isStructure);
                }
            }
            
            query = this.FiltrerQuery(filterColumns, query);
            return query;
        }
        private List<DocumentDTO> GetListAllDocumentByUser(int userId, int idLang, int idDefaultLang)
        {
            var sql = SQL_USERDOC_ALL.Replace("{{schema}}", EdmxFunction.SCHEMA + ".");
            sql = string.Format(sql, idLang, idDefaultLang);
            var query = _docrepository.Database.SqlQuery<DocumentDTO>(sql, new SqlParameter("@userId", userId)).AsQueryable();
            var lst = query.ToList();
            return lst;
        }

        public GetUserDocumentResponse GetListDocumentByUser(int UserID, int IDLang = 1)
        {
            IntitekUser user = _userrepository.FindBy(UserID);
            var defaultLangID = _langrepository.FindBy(new Specification<Lang>(lg => lg.ID != IDLang)).FirstOrDefault().ID;
            var lst = GetListAllDocumentByUser(UserID, IDLang, defaultLangID);
            var query1 = lst.AsQueryable();
            var query2 = lst.AsQueryable();
            var query3 = lst.AsQueryable();
            GetUserDocumentResponse response = new GetUserDocumentResponse();
            response.Email = user.Email;
            response.Id = UserID;
            //DOCUMENTS NÉCESSITANT UNE ACTION DE VOTRE PART
            query1 = query1.Where(x => !x.IsNoActionRequired);
            query1 = query1.Where(x => !(x.IsRead != null
                   && (x.Approbation == 1 ? x.IsApproved != null : true)
                   && (x.Test == 1 ? x.IsTested != null : true)
                  // && (x.Score.HasValue ? x.Score.Value >= x.ScoreMinimal.Value : true)
                  ));
            
            if (user.Type == Constante.UserType_METIER)
            {
                query1 = query1.Where(x => x.isMetier);
            }
            else if (user.Type == Constante.UserType_STRUCTURE)
            {
                query1 = query1.Where(x => x.isStructure);
            }           
            response.LstActionDocuments = query1.ToList();
            response.ActionsCount = response.LstActionDocuments.Count;
            foreach (DocumentDTO item in response.LstActionDocuments)
            {
                if (item.Test.HasValue && item.Test == 1)
                {
                    item.UserQcm = this.FindByUserAndIdQcm(item.IdUser, item.ID, item.IdQcm.HasValue ? item.IdQcm.Value : -1, false);
                }
            }
            //DOCUMENTS INFORMATIFS
            query2 = query2.Where(x => x.IsNoActionRequired);
            query2 = query2.Where(x => !(x.IsRead != null
                  && (x.Approbation == 1 ? x.IsApproved != null : true)
                  && (x.Test == 1 ? x.IsTested != null : true)
                 ));
           
            if (user.Type == Constante.UserType_METIER)
            {
                query2 = query2.Where(x => x.isMetier);
            }
            else if (user.Type == Constante.UserType_STRUCTURE)
            {
                query2 = query2.Where(x => x.isStructure);
            }          
            response.NbInformatifDocuments = query2.Count();         
            response.LstInformatifDocuments = query2.ToList();
           
            //DOCUMENTS DÉJÀ LUS ET REVUS
            query3 = query3.Where(x => (x.IsRead != null
                  && (x.Approbation == 1 ? x.IsApproved != null : true)
                  && (x.Test == 1 ? x.IsTested != null : true)
                 //&& (x.Score.HasValue ? x.Score.Value >= x.ScoreMinimal.Value : true)
                 ));            
            response.NbReadDocuments = query3.Count();           
            response.LstReadDocuments = query3.ToList();
            foreach (DocumentDTO item in response.LstReadDocuments)
            {
                if (item.Test.HasValue && item.Test == 1 && item.IsBoolTested)
                {
                    item.UserQcm = this.FindByUserAndIdQcm(item.IdUser, item.ID, item.IdQcm.HasValue ? item.IdQcm.Value : -1, false);
                }
            }
            return response;
        }

        public GetUserDocumentResponse GetAllListDocumentByUser(GetUserDocumentRequest request1, GetUserDocumentRequest request2, GetUserDocumentRequest request3)
        {
            IntitekUser user = _userrepository.FindBy(request1.UserID);
            var defaultLangID = _langrepository.FindBy(new Specification<Lang>(lg => lg.ID != request1.IDLang)).FirstOrDefault().ID;
            var lst = GetListAllDocumentByUser(request1.UserID, request1.IDLang, defaultLangID);
            var query1 = lst.AsQueryable();
            var query2 = lst.AsQueryable();
            var query3 = lst.AsQueryable();
            GetUserDocumentResponse response = new GetUserDocumentResponse();
            //DOCUMENTS NÉCESSITANT UNE ACTION DE VOTRE PART
            query1 = query1.Where(x => !x.IsNoActionRequired);
            query1 = query1.Where(x => !(x.IsRead!=null
                   && (x.Approbation == 1? x.IsApproved!=null : true)
                   && (x.Test == 1 ? x.IsTested!=null : true)
                  // && (x.Score.HasValue ? x.Score.Value >= x.ScoreMinimal.Value : true)
                  ));
            if (!string.IsNullOrEmpty(request1.Search))
            {
                var search = Utils.RemoveAccent(request1.Search);
                query1 = query1.Where(x => (
                    (x.Name != null && Utils.RemoveAccent(x.Name).Contains(search)) ||
                    (x.Version != null && Utils.RemoveAccent(x.Version).Contains(search)) ||
                    (x.NameCategory != null && Utils.RemoveAccent(x.NameCategory).Contains(search))
                ));
            }
            if(user.Type== Constante.UserType_METIER)
            {
                query1 = query1.Where(x => x.isMetier);
            }
            else if (user.Type == Constante.UserType_STRUCTURE)
            {
                query1 = query1.Where(x => x.isStructure);
            }
            query1 = this.FiltrerQuery(request1.Filtres, query1);
            response.NbActionDocuments = query1.Count();
            string orderBy1 = "OrdreCategory, OrdreSubCategory, " + request1.OrderColumn + request1.SortAscDesc;
            query1 = query1.OrderBy(orderBy1).Skip((request1.Page - 1) * request1.Limit).Take(request1.Limit);
            response.LstActionDocuments = query1.ToList();
            foreach (DocumentDTO item in response.LstActionDocuments)
            {
                if (item.Test.HasValue && item.Test == 1)
                {
                    item.UserQcm = this.FindByUserAndIdQcm(item.IdUser, item.ID, item.IdQcm.HasValue ? item.IdQcm.Value : -1, false);
                }
            }

            //DOCUMENTS INFORMATIFS
            query2 = query2.Where(x => x.IsNoActionRequired);
            query2 = query2.Where(x => !(x.IsRead != null
                  && (x.Approbation == 1 ? x.IsApproved != null : true)
                  && (x.Test == 1 ? x.IsTested != null : true)
                 ));
            if (!string.IsNullOrEmpty(request2.Search))
            {
                var search = Utils.RemoveAccent(request2.Search);
                query2 = query2.Where(x => (
                    (x.Name != null && Utils.RemoveAccent(x.Name).Contains(search)) ||
                    (x.Version != null && Utils.RemoveAccent(x.Version).Contains(search)) ||
                    (x.NameCategory != null && Utils.RemoveAccent(x.NameCategory).Contains(search))
                ));
            }
            if (user.Type == Constante.UserType_METIER)
            {
                query2 = query2.Where(x => x.isMetier);
            }
            else if (user.Type == Constante.UserType_STRUCTURE)
            {
                query2 = query2.Where(x => x.isStructure);
            }
            query2 = this.FiltrerQuery(request2.Filtres, query2);
            response.NbInformatifDocuments = query2.Count();
            string orderBy2 = "OrdreCategory, OrdreSubCategory, " + request2.OrderColumn + request2.SortAscDesc;
            query2 = query2.OrderBy(orderBy2).Skip((request2.Page - 1) * request2.Limit).Take(request2.Limit);
            response.LstInformatifDocuments = query2.ToList();

            //DOCUMENTS DÉJÀ LUS ET REVUS
            query3 = query3.Where(x => (x.IsRead != null
                  && (x.Approbation == 1 ? x.IsApproved != null : true)
                  && (x.Test == 1 ? x.IsTested != null : true)
                  //&& (x.Score.HasValue ? x.Score.Value >= x.ScoreMinimal.Value : true)
                 ));
            if (!string.IsNullOrEmpty(request3.Search))
            {
                var search = Utils.RemoveAccent(request3.Search);
                query3 = query3.Where(x => (
                    (x.Name != null && Utils.RemoveAccent(x.Name).Contains(search)) || 
                    (x.Version != null && Utils.RemoveAccent(x.Version).Contains(search)) ||
                    (x.NameCategory != null && Utils.RemoveAccent(x.NameCategory).Contains(search))
                ));
            }
            query3 = this.FiltrerQuery(request3.Filtres, query3);
            response.NbReadDocuments = query3.Count();
            string orderBy3 = "OrdreCategory, OrdreSubCategory, " + request3.OrderColumn + request3.SortAscDesc;
            query3 = query3.OrderBy(orderBy3).Skip((request3.Page - 1) * request3.Limit).Take(request3.Limit);
            response.LstReadDocuments = query3.ToList();
            foreach (DocumentDTO item in response.LstReadDocuments)
            {
                if (item.Test.HasValue && item.Test == 1 && item.IsBoolTested)
                {
                    item.UserQcm = this.FindByUserAndIdQcm(item.IdUser, item.ID, item.IdQcm.HasValue ? item.IdQcm.Value : -1, false);
                }
            }
            return response;
        }

        public int GetAllDocumentByUserLoginCount(GetUserDocumentRequest request)
        {
            var query = this.GetListDocumentByUserLoginQueryable(false, false, request);
            return query.Count();
        }
        public List<DocumentDTO> GetAllDocumentByUserLogin(GetUserDocumentRequest request)
        {
            string orderBy = "OrdreCategory, OrdreSubCategory, " + request.OrderColumn + request.SortAscDesc;
            IQueryable<DocumentDTO> query = this.GetListDocumentByUserLoginQueryable(false, false,request).OrderBy(orderBy).Skip((request.Page - 1) * request.Limit).Take(request.Limit);
            var list = query.ToList();
            foreach (DocumentDTO item in list)
            {
                if (item.Test.HasValue && item.Test == 1)
                {
                    item.UserQcm = this.FindByUserAndIdQcm(item.IdUser, item.ID, item.IdQcm.HasValue ? item.IdQcm.Value : -1, false);
                }
            }
            return list;
        }
        public int GetNoActionDocumentByUserLoginCount(GetUserDocumentRequest request)
        {
            var query = this.GetListDocumentByUserLoginQueryable(true, false, request);
            return query.Count();
        }
        public List<DocumentDTO> GetNoActionDocumentByUserLogin(GetUserDocumentRequest request)
        {
            string orderBy = "OrdreCategory, OrdreSubCategory, " + request.OrderColumn + request.SortAscDesc;
            IQueryable<DocumentDTO> query = this.GetListDocumentByUserLoginQueryable(true, false, request).OrderBy(orderBy).Skip((request.Page - 1) * request.Limit).Take(request.Limit);
            var list = query.ToList();
            return list;
        }
        /// <summary>
        /// Nombre Documents déjà lus et revus
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="rechercheMultiCol"></param>
        /// <param name="filterColumns"></param>
        /// <returns></returns>
        public int GetReadedDocumentByUserLoginCount(GetUserDocumentRequest request)
        {
            var query = this.GetListDocumentByUserLoginQueryable(false, true, request);
            var total = query.Count();
            return total;
        }
        /// <summary>
        /// Liste Documents déjà lus et revus
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="rechercheMultiCol"></param>
        /// <param name="filterColumns"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="orderColumn"></param>
        /// <param name="sortDirection"></param>
        /// <returns></returns>
        public List<DocumentDTO> GetReadedDocumentByUserLogin(GetUserDocumentRequest request)
        {
            string orderBy = "OrdreCategory ASC, OrdreSubCategory, " + request.OrderColumn + request.SortAscDesc;
            IQueryable<DocumentDTO> query = this.GetListDocumentByUserLoginQueryable(false, true, request).OrderBy(orderBy).Skip((request.Page - 1) * request.Limit).Take(request.Limit);
            var list = query.ToList();
            foreach (DocumentDTO item in list)
            {
                if(item.Test.HasValue && item.Test==1 && item.IsBoolTested)
                {
                    item.UserQcm = this.FindByUserAndIdQcm(item.IdUser, item.ID, item.IdQcm.HasValue ? item.IdQcm.Value : -1, false);
                }                
            }
            return list;
        }


        public GetDocumentResponse GetDocument(GetDocumentRequest request)
        {
            GetDocumentResponse response = null;
            try
            {
                var doc = _docrepository.FindBy(request.Id);
                var trad = _doclangrepository.FindBy(new Specification<DocumentLang>(dl => dl.ID_Document == request.Id && dl.ID_Lang == request.IdLang)).FirstOrDefault();
                DocumentLang defaultTrad = _doclangrepository.FindBy(new Specification<DocumentLang>(dt => dt.ID_Lang != request.IdLang && dt.ID_Document == request.Id)).FirstOrDefault();
                response = new GetDocumentResponse()
                {
                    Document = doc,
                    DocumentTrad = trad,
                    DefaultDocumentTrad = defaultTrad
                };
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "GetDocument",
                    ServiceName = "DocumentService",

                }, ex);
                throw ex;
            }

        }
        private UserQcm FindByUserAndIdQcm(int idUser, int idDoc, int idQcm, bool bLoadQcm)
        {
            if(idQcm > 0)
            {
                UserQcm uqcm = _userqcmrepository.FindBy(new Specification<UserQcm>(x => x.ID_IntitekUser == idUser && x.ID_Qcm == idQcm && x.ID_Document== idDoc)).FirstOrDefault();
                if (uqcm != null && bLoadQcm)
                {
                    Qcm qcm = _qcmrepository.FindBy(uqcm.ID_Qcm);
                    uqcm.Qcm = qcm;
                }
                return uqcm;
            }
            return null;            
        }
        public UserQcm FindByUserQcm(int id)
        {
            var query = _userqcmrepository.RepositoryQuery.Include("Qcm").Where(x => x.ID==id);
            var uqcm = query.SingleOrDefault();
            return uqcm;
        }
        public HistoUserQcmDocVersion FindByHistoUserQcm(int id)
        {
            var query = _histoUserqcmrepository.RepositoryQuery.Include("Qcm").Where(x => x.ID == id);
            var uqcm = query.SingleOrDefault();
            return uqcm;
        }

        //public List<UserQcmDTO> FindAllUserQcmSuccess(int idUser, int maxDisplay, int idLang, int idDefaultLangue)
        //{
        //    List<UserQcmDTO> result = new List<UserQcmDTO>();
        //    var query = _userqcmrepository.RepositoryQuery.Include("Qcm").Where(x => x.ID_IntitekUser == idUser && x.ScoreMinimal> 0 && x.Score>= x.ScoreMinimal).Where(x=> x.Qcm.Inactif!=1).OrderByDescending(x=> x.DateFin).Take(maxDisplay);
        //    var lst = query.ToList();
        //    foreach (var userq in lst)
        //    {
        //        var doc =_docrepository.RepositoryQuery.Select(x => new DocumentDTO() { ID = x.ID, Inactif = x.Inactif, Name = string.Empty }).Where(x => x.Inactif == false).FirstOrDefault();
        //        if (doc != null)
        //        {
        //            var qcm = new UserQcmDTO() {
        //                ID = userq.ID,
        //                DocumentID = userq.ID_Document.Value,
        //                UserID = userq.ID_IntitekUser,
        //                DateCre = userq.DateCre,
        //                DateFin = userq.DateFin.Value,
        //                DocumentVersion = userq.Version,
        //                QcmID = userq.ID_Qcm,

        //                DocumentName = doc.Name,
        //            };
        //            qcm.QcmName = _qcmlangRepository.RepositoryQuery.Where(x => x.ID_Lang == idLang && x.ID_Qcm == qcm.QcmID).Select(u => u.QcmName).SingleOrDefault();

        //            if (string.IsNullOrEmpty(qcm.QcmName) && idLang != idDefaultLangue)
        //            {
        //                qcm.QcmName = _qcmlangRepository.RepositoryQuery.Where(x => x.ID_Lang == idDefaultLangue && x.ID_Qcm == qcm.QcmID).Select(u=>u.QcmName).SingleOrDefault();
        //            }
        //            result.Add(qcm);
        //        }
        //    }
        //    return result;
        //}
        public List<HistoUserQcmDocVersionDTO> FindAllUserQcmSuccess(int idUser, int maxDisplay, int idLang, int idDefaultLangue)
        {
            List<HistoUserQcmDocVersionDTO> result = new List<HistoUserQcmDocVersionDTO>();
            List<HistoUserQcmDocVersionDTO> resultTemp = new List<HistoUserQcmDocVersionDTO>();
            var query = _histoUserqcmrepository.RepositoryQuery
                .Include("Qcm")
                .Where(x => x.ID_IntitekUser == idUser && x.ScoreMinimal > 0 && x.Score >= x.ScoreMinimal)
                .Where(x => x.ID_IntitekUser == idUser && x.ScoreMinimal > 0)
                .Where(x => x.Qcm.Inactif != 1)
                .OrderByDescending(x => x.DateFin);
                //.Take(maxDisplay);
            var lst = query.ToList();
            foreach (var userq in lst)
            {
                var doc = _docrepository.RepositoryQuery.Select(x => new DocumentDTO() { ID = x.ID, Inactif = x.Inactif, Name = string.Empty }).Where(x => x.Inactif == false).FirstOrDefault();
                if (doc != null)
                {
                    var qcm = new HistoUserQcmDocVersionDTO()
                    {
                        ID = userq.ID,
                        DocumentID = userq.ID_Document,
                        UserID = userq.ID_IntitekUser,
                        DateCre = userq.DateCre,
                        DateFin = userq.DateFin,
                        DocumentVersion = userq.Version,
                        QcmID = userq.ID_Qcm,

                        DocumentName = doc.Name,
                    };
                    qcm.QcmName = _qcmlangRepository.RepositoryQuery.Where(x => x.ID_Lang == idLang && x.ID_Qcm == qcm.QcmID).Select(u => u.QcmName).SingleOrDefault();

                    if (string.IsNullOrEmpty(qcm.QcmName) && idLang != idDefaultLangue)
                    {
                        qcm.QcmName = _qcmlangRepository.RepositoryQuery.Where(x => x.ID_Lang == idDefaultLangue && x.ID_Qcm == qcm.QcmID).Select(u => u.QcmName).SingleOrDefault();
                    }
                    resultTemp.Add(qcm);
                }
                
                foreach (var resultElement in resultTemp)
                {
                    if (!result.Any(x => x.QcmID == resultElement.QcmID))
                    {
                        result.Add(resultElement);
                    }

                } 
            }
            return result;
        }
    }
}
