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
    public class EntiteService : BaseService, IEntiteService
    {
        private readonly UserDataAccess _userrepository;
        private readonly EntityDocumentDataAccess _entityDocrepository;
        private readonly DocumentDataAccess _docrepository;
        private readonly UserDocumentDataAccess _userDocrepository;
        private readonly ProfileDocumentDataAccess _profilDocrepository;
        private readonly UserProfileDataAccess _profilUserrepository;
        private readonly DocumentCategoryDataAccess _doccategoryrepository;
        private readonly HistoActionsDataAccess _histoactionsrepository;
        private readonly DocumentLangDataAccess _doclangrepository;
        public readonly SubCategoryDataAccess _subcategrepository;
        private static string SQL_STATE_STATS = "SELECT * FROM( " + Environment.NewLine +
            "SELECT " + Environment.NewLine +
            "1 as Num, " + Environment.NewLine +
            "doc.ID, " + Environment.NewLine +
            "doc.Approbation, " + Environment.NewLine +
            "doc.Test, " + Environment.NewLine +
            "doc.Date, " + Environment.NewLine +
            "usr.ID as IdUser, " + Environment.NewLine +
            "usr.EntityName, " + Environment.NewLine +
            "usr.AgencyName, " + Environment.NewLine +
            "NULL Ed_EntityName, " + Environment.NewLine +
            "NULL Ed_AgencyName, " + Environment.NewLine +
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
                "NULL Ed_EntityName, " + Environment.NewLine +
                "NULL Ed_AgencyName, " + Environment.NewLine +
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
            "ed.EntityName as Ed_EntityName, " + Environment.NewLine +
            "ed.AgencyName as Ed_AgencyName, " + Environment.NewLine +
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
        public EntiteService(ILogger logger) : base(logger)
        {
            _userrepository = new UserDataAccess(uow);
            _entityDocrepository = new EntityDocumentDataAccess(uow);
            _userDocrepository = new UserDocumentDataAccess(uow);
            _docrepository = new DocumentDataAccess(uow);
            _profilDocrepository = new ProfileDocumentDataAccess(uow);
            _profilUserrepository = new UserProfileDataAccess(uow);
            _doccategoryrepository = new DocumentCategoryDataAccess(uow);
            _histoactionsrepository = new HistoActionsDataAccess(uow);
            _doclangrepository = new DocumentLangDataAccess(uow);
            _subcategrepository = new SubCategoryDataAccess(uow);

            //path for System.Link.Dynamic pour reconnaitre la classe EdmxFunction
            this.DynamicLinkPatch();

        }
        public List<string> AgencyByEntity(string entity, bool isactive, bool hasEmpty, bool showAgencyNull)
        {
            IQueryable<IntitekUser> query = null;
            if (string.IsNullOrEmpty(entity))
            {
                query = _userrepository.RepositoryQuery.Where(x => !string.IsNullOrEmpty(x.AgencyName));
            }
            else {
                if (showAgencyNull)
                {
                    query = _userrepository.RepositoryQuery.Where(x => x.EntityName.ToLower().Equals(entity.ToLower()));
                }
                else
                {
                    query = _userrepository.RepositoryQuery.Where(x => !string.IsNullOrEmpty(x.AgencyName) && x.EntityName.ToLower().Equals(entity.ToLower()));
                }

            }
            if (isactive)
                query = query.Where(x => x.Active);
            var queryR = query.Select(x => x.AgencyName).Distinct().OrderBy(x => x);
            var lst = queryR.ToList();
            if (hasEmpty == true)
                lst.Insert(0, "");
            return lst;
        }
        public List<UserDTO> ListUsersForRelance(string entityName)
        {
            var query = this._userrepository.RepositoryQuery;
            if (!string.IsNullOrEmpty(entityName))
            {
                query = query.Where(x => x.EntityName.ToLower().Equals(entityName.ToLower()));
            }
            query = query.Where(x => x.Active && 
                !(x.InactivityStart.HasValue && DbFunctions.TruncateTime(x.InactivityStart) <= DbFunctions.TruncateTime(System.Data.Entity.SqlServer.SqlFunctions.GetDate()) &&
                                 (!x.InactivityEnd.HasValue || (x.InactivityEnd.HasValue && DbFunctions.TruncateTime(x.InactivityEnd) >= DbFunctions.TruncateTime(System.Data.Entity.SqlServer.SqlFunctions.GetDate())))));

            var lst = query.Select(x => new UserDTO { ID = x.ID, Name=x.Username, Status = x.Status,
                EmailOnBoarding = x.EmailOnBoarding,
                IsOnBoarding = x.isOnBoarding,
                EntityName = x.EntityName, AgencyName = x.AgencyName,
                Type = x.Type
            }).ToList();
            return lst;
        }
        public List<UserDTO> ListUsersForRelance(string entityName, string agenceName)
        {
            if (!string.IsNullOrEmpty(entityName) && string.IsNullOrEmpty(agenceName))
            {
                return ListUsersForRelance(entityName);
            }
            else if (!string.IsNullOrEmpty(entityName) && !string.IsNullOrEmpty(agenceName))
            {
                var query = this._userrepository.RepositoryQuery;
                query = query.Where(x => x.EntityName.ToLower().Equals(entityName.ToLower()) && x.AgencyName.ToLower().Equals(agenceName.ToLower()));
                query = query.Where(x => x.Active && 
                    !(x.InactivityStart.HasValue && DbFunctions.TruncateTime(x.InactivityStart) <= DbFunctions.TruncateTime(System.Data.Entity.SqlServer.SqlFunctions.GetDate()) &&
                                (!x.InactivityEnd.HasValue || (x.InactivityEnd.HasValue && DbFunctions.TruncateTime(x.InactivityEnd) >= DbFunctions.TruncateTime(System.Data.Entity.SqlServer.SqlFunctions.GetDate())))));
                var lst = query.Select(x => new UserDTO { ID = x.ID, Name = x.Username, Status = x.Status,
                    EmailOnBoarding = x.EmailOnBoarding,
                    IsOnBoarding = x.isOnBoarding,
                    EntityName = x.EntityName, AgencyName = x.AgencyName }).ToList();
                return lst;
            }
            return null;            
        }

        private List<DocumentDTO> ListDocumentByEntityAgence(GetEntityRequest request)
        {
            if (string.IsNullOrEmpty(request.AgencyName))
            {
                var query = this._entityDocrepository.RepositoryQuery.Join(
                                _docrepository.RepositoryTable,
                                ed => ed.ID_Document,
                                doc => doc.ID,
                                (ed, doc) => new { ed, doc }).Where(e =>
                                    e.ed.EntityName.Equals(request.EntityName) && e.doc.Inactif == false).Select(x => new DocumentDTO { ID = x.doc.ID, AgencyName = x.ed.AgencyName });
                return query.ToList();
            }
            else
            {
                var query = this._entityDocrepository.RepositoryQuery.Join(
                _docrepository.RepositoryTable,
                ed => ed.ID_Document,
                doc => doc.ID,
                (ed, doc) => new { ed, doc }).Where(e =>
                    e.ed.EntityName.Equals(request.EntityName) && (e.ed.AgencyName.Equals(request.AgencyName) || string.IsNullOrEmpty(e.ed.AgencyName))
                    && e.doc.Inactif == false).Select(x => new DocumentDTO() { ID = x.doc.ID, AgencyName = x.ed.AgencyName, EntityName = x.ed.EntityName });
                return query.ToList();
            }           
        }
        public override string GetOperator(Type type, ColumnFilter columnFilter, int index, string filterValue)
        {
            if (type == typeof(DocumentDTO))
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
        /// Assigner les documents à l'agence EntityName
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public List<DocumentDTO> ListDocumentEntity(GetEntityRequest request)
        {
            List<DocumentDTO> lst = this.ListDocument(request);
            foreach (var item in lst)
            {
                List<DocumentDTO> lstDoc = this.ListDocumentByEntityAgence(request);
                if (lstDoc != null)
                {
                    if (lstDoc.Exists(x => x.ID == item.ID))
                    {
                        item.IsChecked = true;
                        item.IsSessionChecked = true;
                        string agencyName = lstDoc.Where(x => x.ID == item.ID).Select(x => x.AgencyName).FirstOrDefault();
                        if (!string.IsNullOrEmpty(agencyName))
                        {
                            item.IsDisabled = true;
                        }
                    }
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
            List<ColumnFilter> columnsFilters = this.GetColumnFilters(ReplaceFiltre(request.Request.Filtres), new List<string>() { "IsSessionChecked" });
            lst = this.FiltrerQuery(columnsFilters, lst.AsQueryable<DocumentDTO>()).ToList();
            return lst;
        }
        /// <summary>
        /// Assigner les documents à l'agence AgencyName
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public List<DocumentDTO> ListDocumentAgence(GetEntityRequest request)
        {
            List<DocumentDTO> lst = this.ListDocument(request);
            foreach (var item in lst)
            {
                List<DocumentDTO> lstDoc = this.ListDocumentByEntityAgence(request);
                if (lstDoc != null)
                {
                    if (lstDoc.Exists(x => x.ID == item.ID))
                    {
                        item.IsChecked = true;
                        item.IsSessionChecked = true;
                        var doc = lstDoc.Where(x => x.ID == item.ID).FirstOrDefault();
                        if (!string.IsNullOrEmpty(doc.AgencyName) || !string.IsNullOrEmpty(doc.EntityName))
                        {
                            item.IsChecked = true;
                        }
                        //Affecté à l'entité
                        if (string.IsNullOrEmpty(doc.AgencyName))
                        {
                            item.IsDisabled = true;
                        }
                    }
                }
                //Affecté : Coché ou non en session
                if (request.DocsAffected != null)
                {
                    var docState = request.DocsAffected.Find(x => x.DocId == item.ID);
                    if (docState!=null && docState.OldCheckState!= docState.NewCheckState)
                    {
                        item.IsSessionChecked = docState.NewCheckState;
                    }
                }
            }
            //Le filtre column "IsSessionChecked" est traité après l'execution de la requete
            List<ColumnFilter> columnsFilters = this.GetColumnFilters(ReplaceFiltre(request.Request.Filtres), new List<string>() { "IsSessionChecked" });
            lst = this.FiltrerQuery(columnsFilters, lst.AsQueryable<DocumentDTO>()).ToList();
            return lst;
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
        private List<DocumentDTO> ListDocument(GetEntityRequest request)
        {
            string TYPE_AFFECTATION = "Entité/Agence";
            string orderBy = "OrdreCategory, OrdreSubCategory, " + request.Request.OrderColumn + request.Request.SortAscDesc;
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
                   (docCat, categ) => new { doc = docCat.docSub, categ, sub = docCat.sub })
               .Where(x => x.doc.docL.TypeAffectation.Equals(TYPE_AFFECTATION))
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
                   //ContentType = parent.doc.ContentType,
                   //TypeAffectation = parent.doc.TypeAffectation,
                   //Qcm = parent.doc.Qcm,
                   Inactif = parent.doc.docL.Inactif,
                   IsMetier =parent.doc.docL.isMetier,
                   IsStructure = parent.doc.docL.isStructure,
                   //Extension = parent.doc.Extension,
                   ID_Category = parent.doc.docL.ID_Category,
                   ID_SubCategory = parent.doc.docL.ID_SubCategory,
                   IsNoActionRequired = parent.doc.docL.IsNoActionRequired,
                   DocumentCategoryLang = child.DocumentCategoryLang.FirstOrDefault(f => f.ID_Lang == request.IdLang),
                   DefaultNameCategory = EdmxFunction.GetCategoryLang(parent.doc.docL.ID_Category.Value, request.IdDefaultLang, request.IdDefaultLang),
                   OrdreCategory = child.OrdreCategory != null ? child.OrdreCategory : 1000,
                   OrdreSubCategory = parent.sub.Ordre != null ? parent.sub.Ordre : 1000
                   //Affecte = (parent.doc.ProfileDocument.Where(y => y.ID_Document.Value == parent.doc.ID && y.Document.Inactif == false && y.Document).Any() 
                   //|| parent.doc.EntityDocument.Where(y => y.ID_Document.Value == parent.doc.ID && y.Document.Inactif == false && y.Document).Any()) ? true : false,
               });
            List<ColumnFilter> columnsFilters = this.GetColumnFilters(ReplaceFiltre(request.Request.Filtres), null);
            //Le filtre column "IsSessionChecked" est traité après l'execution de la requete
            columnsFilters.Remove(new ColumnFilter() { ColumnName= "IsSessionChecked" });
            query = this.FiltrerQuery(columnsFilters, query).OrderBy(orderBy);
            List<DocumentDTO> lst = query.ToList();
            return lst;
        }

        public void SaveEntity(SaveEntityRequest request)
        {
            List<DocCheckState> lstdocs = request.DocsAffected;
            if (lstdocs != null && lstdocs.Count > 0)
            {
                var lstEntityDB = this._entityDocrepository.RepositoryQuery.Join(
                _docrepository.RepositoryTable,
                ed => ed.ID_Document,
                doc => doc.ID,
                (ed, doc) => new { ed, doc }).Where(e =>
                    e.ed.EntityName.Equals(request.EntityName) && string.IsNullOrEmpty(e.ed.AgencyName)
                    && e.doc.Inactif == false).Select(x => x.ed);

                //Ajout
                var docs = lstdocs.Where(x => x.NewCheckState && x.OldCheckState == false).Select(x => x.DocId).ToList();
                foreach (var doc in docs)
                {
                    if (!lstEntityDB.Any(x => x.ID_Document == doc))
                    {
                        EntityDocument ed = new EntityDocument()
                        {
                            ID_Document = doc,
                            EntityName = request.EntityName,
                            EntityDocDate = DateTime.Now
                        };
                        _entityDocrepository.Add(ed);
                        //Histo Actions Ajoutées
                        HistoActions hA = new HistoActions() { ID_Object = ed.ID, ObjectCode = ObjectCode.ENTITY_DOCUMENT, ID_IntitekUser = request.ID_IntitekUser, DateAction = DateTime.Now, Action = Actions.Create };
                        _histoactionsrepository.Save(hA);
                    }
                }
                var docsR = lstdocs.Where(x => x.NewCheckState == false && x.OldCheckState).Select(x => x.DocId).ToList();
                var entityDocumentToDelete = lstEntityDB.Where(x => docsR.Contains(x.ID_Document.Value)).Select(x => new EntityDocumentDTO()
                {
                    ID = x.ID,
                    EntityDocDate = x.EntityDocDate,
                    EntityName = x.EntityName,
                    ID_Document = x.ID_Document
                }).ToList();
                _entityDocrepository.RemoveAll(lstEntityDB.Where(x => docsR.Contains(x.ID_Document.Value)));
                foreach (var ed in entityDocumentToDelete)
                {
                    HistoActions hA = new HistoActions() { ID_Object = ed.ID_Document.Value, LinkedObjects = ed.EntityName, ObjectCode = ObjectCode.ENTITY_DOCUMENT, ID_IntitekUser = request.ID_IntitekUser, DateAction = DateTime.Now, Action = Actions.Delete };
                    _histoactionsrepository.Save(hA);
                }
            }
        }
        public void SaveAgency(SaveEntityRequest request)
        {
            List<DocCheckState> lstdocs = request.DocsAffected;
            if(lstdocs!=null && lstdocs.Count > 0)
            {
                var lstEntityDB = this._entityDocrepository.RepositoryQuery.Join(
                _docrepository.RepositoryTable,
                ed => ed.ID_Document,
                doc => doc.ID,
                (ed, doc) => new { ed, doc }).Where(e =>
                    e.ed.EntityName.Equals(request.EntityName) && e.ed.AgencyName.Equals(request.AgencyName)
                    && e.doc.Inactif == false).Select(x => x.ed);

                //Ajout
                var docs = lstdocs.Where(x => x.NewCheckState && x.OldCheckState == false).Select(x=> x.DocId).ToList();
                foreach (var doc in docs)
                {
                    if (!lstEntityDB.Any(x => x.ID_Document == doc))
                    {
                        EntityDocument ed = new EntityDocument()
                        {
                            ID_Document = doc,
                            EntityName = request.EntityName,
                            AgencyName = request.AgencyName,
                            EntityDocDate = DateTime.Now
                        };
                        _entityDocrepository.Add(ed);
                        //Histo Actions Ajoutées
                        HistoActions hA = new HistoActions() { ID_Object = ed.ID, ObjectCode = ObjectCode.ENTITY_DOCUMENT, ID_IntitekUser = request.ID_IntitekUser, DateAction = DateTime.Now, Action = Actions.Create };
                        _histoactionsrepository.Save(hA);
                    }
                }
                var docsR = lstdocs.Where(x => x.NewCheckState==false && x.OldCheckState).Select(x => x.DocId).ToList();
                var entityDocumentToDelete = lstEntityDB.Where(x => docsR.Contains(x.ID_Document.Value)).Select(x => new EntityDocumentDTO() {
                    ID = x.ID,
                    AgencyName = x.AgencyName,
                    EntityDocDate = x.EntityDocDate,
                    EntityName = x.EntityName,
                    ID_Document = x.ID_Document
                }).ToList();
                _entityDocrepository.RemoveAll(lstEntityDB.Where(x => docsR.Contains(x.ID_Document.Value)));
                foreach (var ed in entityDocumentToDelete)
                {
                    HistoActions hA = new HistoActions() { ID_Object = ed.ID_Document.Value, LinkedObjects = ed.EntityName+"-"+ ed.AgencyName, ObjectCode = ObjectCode.ENTITY_DOCUMENT, ID_IntitekUser = request.ID_IntitekUser, DateAction = DateTime.Now, Action = Actions.Delete };
                    _histoactionsrepository.Save(hA);
                }
            }
        }
        private List<DocumentDTO> GetDocumentsByIds(int idLang, int idDefaultLang, List<int> docsId)
        {
            string orderBy = "OrdreCategory, OrdreSubCategory, Name";
            IQueryable<DocumentDTO> query = this._docrepository.RepositoryTable.Where(x=> docsId.Contains(x.ID))
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
        private List<DocumentDTO> GetStatsDocuments(int idLang, int idDefaultLang, List<int> docsId, List<StatistiquesDTO> lst, bool agence)
        {
            List<DocumentDTO> docsDTO = GetDocumentsByIds(idLang, idDefaultLang, docsId);
            foreach (var dto in docsDTO)
            {
                var itemID = dto.ID;
                //DocumentDTO dto = docsDTO.Where(x => x.ID == itemID).FirstOrDefault();
                var updateDate = lst.Where(x => x.ID == itemID).Max(x => x.UpdateDate);
                if (updateDate.HasValue)
                {
                    dto.Date = updateDate;
                }
                else
                {
                    dto.Date = lst.Where(x => x.ID == itemID).Max(x => x.Date);
                }
                dto.EntityName = lst.Where(x => x.ID == itemID).Select(x => x.EntityName).FirstOrDefault();
                Statistiques stat = new Statistiques();
                stat.ToRead = lst.Where(x => x.ID == dto.ID).Select(x => x.IdUser).Distinct().Count();
                stat.ToApproved = lst.Where(x => x.ID == dto.ID && (x.Approbation.HasValue && x.Approbation == 1)).Select(x => x.IdUser).Distinct().Count();
                stat.ToTested = lst.Where(x => x.ID == dto.ID && (x.Test.HasValue && x.Test == 1)).Select(x => x.IdUser).Distinct().Count();
                
                dto.UserRead = lst.Where(x => x.ID == itemID && x.IsRead.HasValue).Select(x => x.IdUser).Distinct().Count();
                dto.UserApproved = lst.Where(x => x.ID == itemID && x.IsApproved.HasValue && x.Approbation.HasValue && x.Approbation == 1).Select(x => x.IdUser).Distinct().Count();
                dto.UserTested = lst.Where(x => x.ID == itemID && x.IsTested.HasValue && x.Test.HasValue && x.Test == 1).Select(x => x.IdUser).Distinct().Count();
                //if (agence)
                //{
                //    dto.AgencyName = string.Join(", ", lst.Where(x => x.ID == itemID).Select(x => x.AgencyName).Distinct().ToList());
                //}
                //else
                //{
                //    dto.AgencyName = string.Join(", ", lst.Where(x => x.ID == itemID).Select(x => x.AgencyName).Distinct().ToList());
                //}
                dto.AgencyName = string.Join(", ", lst.Where(x => x.ID == itemID).Select(x => x.AgencyName).Distinct().ToList());

                dto.Statistiques = stat;
            }
            return docsDTO;
        }
        public List<DocumentDTO> DocumentStateEntity(GetEntityRequest request, bool agence)
        {
            var query = this.DocumentStateEntityQueryable(request).Where(x => x.EntityName.Equals(request.EntityName)) ;
            if (agence)
            {
                query = query.Where(x => !string.IsNullOrEmpty(x.Ed_AgencyName));
            }
            else
            {
                query = query.Where(x => string.IsNullOrEmpty(x.Ed_AgencyName));
            }
            List<StatistiquesDTO> statsDTO = query.ToList();
            var lstIDDocs = query.Select(x => x.ID).Distinct().ToList();
            List<DocumentDTO> lstdocs = GetStatsDocuments(request.IdLang, request.IdDefaultLang, lstIDDocs, statsDTO, false);
            return lstdocs;
        }
        public List<DocumentDTO> DocumentStateAgency(GetEntityRequest request)
        {
           var query = this.DocumentStateEntityQueryable(request).Where(x => x.EntityName.Equals(request.EntityName) && x.AgencyName.Equals(request.AgencyName));
            List<StatistiquesDTO> statsDTO = query.ToList();
            var lstIDDocs = query.Select(x => x.ID).Distinct().ToList();
            List<DocumentDTO> lstdocs = GetStatsDocuments(request.IdLang, request.IdDefaultLang, lstIDDocs, statsDTO, true);
            return lstdocs;
        }
        private IQueryable<StatistiquesDTO> DocumentStateEntityQueryable(GetEntityRequest request)
        {
            string sql = SQL_STATE_STATS;
            var query = _docrepository.Database.SqlQuery<StatistiquesDTO>(sql).AsQueryable();
            return query;          
            
        }
        
        /// <summary>
        /// Nombre d'utilisateurs qui ont lu le document
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private int ListReadDocumentUser(GetEntityRequest request)
        {
            var query = this._userrepository.RepositoryQuery.Where(x => x.Active && x.EntityName.Equals(request.EntityName)
                && _userDocrepository.RepositoryQuery.Join(_docrepository.RepositoryTable,
                ud => ud.ID_Document,
                doc => doc.ID,
                (ud, doc) => new { ud, doc }).Where(u => u.ud.ID_IntitekUser == x.ID && u.ud.ID_Document == request.ID_Document && u.ud.IsRead.HasValue &&
                u.doc.Inactif == false).Any()).Select(x=>x.ID);
            var nb = query.Count();
            return nb;
       }
        /// <summary>
        ///  Nombre d'utilisateurs qui ont approuvé le document
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private int ListApprovedDocumentUser(GetEntityRequest request)
        {
            var query = this._userrepository.RepositoryQuery.Where(x => x.Active && x.EntityName.Equals(request.EntityName)
                && _userDocrepository.RepositoryQuery.Join(_docrepository.RepositoryTable,
                ud => ud.ID_Document,
                doc => doc.ID,
                (ud, doc) => new { ud, doc }).Where(u => u.ud.ID_IntitekUser == x.ID && u.ud.ID_Document == request.ID_Document && u.ud.IsApproved.HasValue &&
                u.doc.Approbation==1 && u.doc.Inactif == false).Any()).Select(x => x.ID);
            var nb = query.Count();
            return nb;
        }
        private int ListTestedDocumentUser(GetEntityRequest request)
        {
            var query = this._userrepository.RepositoryQuery.Where(x => x.Active && x.EntityName.Equals(request.EntityName)
                && _userDocrepository.RepositoryQuery.Join(_docrepository.RepositoryTable,
                ud => ud.ID_Document,
                doc => doc.ID,
                (ud, doc) => new { ud, doc }).Where(u => u.ud.ID_IntitekUser == x.ID && u.ud.ID_Document == request.ID_Document && u.ud.IsTested.HasValue &&
                u.doc.Test == 1 && u.doc.Inactif == false).Any()).Select(x => x.ID);
            var nb = query.Count();
            return nb;
        }
        /// <summary>
        /// Nombre les utilisateurs d'une entité
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private int NbUserEntity(GetEntityRequest request)
        {
            var query = this._userrepository.RepositoryQuery.Where(x => x.Active && x.EntityName.ToLower().Equals(request.EntityName.ToLower()));
            var nb = query.Count();
            return nb;
        }

        public GetAllEntityResponse GetAllEntity(GetAllEntityRequest request)
        {
            var reponse = new GetAllEntityResponse();
            var query = _userrepository.RepositoryQuery.Where(x=> x.Active).Select(e => new EntityAgencyDTO()
            {
                EntityName = e.EntityName.Trim(),
                AgencyName = request.WithAgency ? e.AgencyName.Trim() : string.Empty
            });

            if (request.WithAgency)
            {
                query = query.GroupBy(p => new { p.EntityName, p.AgencyName })
                    .Select(r => r.FirstOrDefault());
            }
            else
            {
                query = query.GroupBy(p => new { p.EntityName })
                    .Select(r => r.FirstOrDefault());
            }

            reponse.Entites = query.OrderBy(e => e.EntityName).ToList();
            return reponse;
        }
    }
}
