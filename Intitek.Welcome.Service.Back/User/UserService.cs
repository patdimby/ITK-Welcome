using Intitek.Welcome.DataAccess;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.Helpers;
using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Infrastructure.Specification;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using static Intitek.Welcome.Service.Back.ColumnFilter;

namespace Intitek.Welcome.Service.Back
{
    public class UserService : BaseService, IUserService
    {

        private readonly UserDataAccess _userrepository;
        private readonly ImportManagerDataAccess _importManagerrepository;
        private readonly CityEntityBlackListedDataAccess _cityEntityBlacklistedrepository;
        private readonly UserProfileDataAccess _profilUserrepository;
        private readonly ProfileDataAccess _profilerepository;
        private readonly DocumentDataAccess _docrepository;
        private readonly UserDocumentDataAccess _userDocrepository;
        private readonly EntityDocumentDataAccess _entityDocrepository;
        private readonly ProfileDocumentDataAccess _profilDocrepository;
        private readonly DocumentCategoryDataAccess _doccategoryrepository;
        private readonly DocumentLangDataAccess _doclangrepository;

      

        public UserService(ILogger logger) : base(logger)
        {
            _userrepository = new UserDataAccess(uow);
            _importManagerrepository = new ImportManagerDataAccess(uow);
            _cityEntityBlacklistedrepository = new CityEntityBlackListedDataAccess(uow);
            _profilUserrepository = new UserProfileDataAccess(uow);
            _profilerepository = new ProfileDataAccess(uow);
            _userDocrepository = new UserDocumentDataAccess(uow);
            _docrepository = new DocumentDataAccess(uow);
            _entityDocrepository = new EntityDocumentDataAccess(uow);
            _profilDocrepository = new ProfileDocumentDataAccess(uow);
            _doccategoryrepository = new DocumentCategoryDataAccess(uow);
            _doclangrepository = new DocumentLangDataAccess(uow);

            //path for System.Link.Dynamic pour reconnaitre la classe EdmxFunction
            this.DynamicLinkPatch();
        }

        public GetUserResponse Get(GetUserRequest request)
        {
            var response = new GetUserResponse();
            try
            {

                var user = _userrepository.FindBy(request.Id);
                if (user != null)
                {
                    response = new GetUserResponse()
                    {
                        User = user,
                    };

                }
                else
                {
                    response = new GetUserResponse()
                    {
                        User = new IntitekUser()                     
                      
                    };
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "Get",
                    ServiceName = "UserService",

                }, ex);
                throw ex;
            }
        }
        public IntitekUser GetIntitekUserByLogin(string login)
        {
            var user = _userrepository.FindBy(new Specification<IntitekUser>(p => p.Username.Equals(login))).FirstOrDefault();
            if (user == null)
            {
                user = new IntitekUser();
                user.Username = login;
                user.FullName = login;
            }
            return user ;           
        }
        public List<string> EntityNameList(bool isactive)
        {
            var query = _userrepository.RepositoryQuery.Where(x => !string.IsNullOrEmpty(x.EntityName));
            if (isactive)
                query = query.Where(x=>x.Active);
            var queryR = query.Select(x => x.EntityName).Distinct().OrderBy(x => x);
            return queryR.ToList();
        }
        public int GetCountUserActiveByEntityName(string entityName)
        {
            var query = _userrepository.RepositoryQuery.Where(x => x.Active && x.EntityName.Equals(entityName));
            return query.Select(x => x.ID).Distinct().Count();
        }
        public List<string> DepartementNameList(bool hasEmpty)
        {
            var query = _userrepository.RepositoryQuery.Where(x => x.Active && !string.IsNullOrEmpty(x.Departement));
            var queryR = query.Select(x => x.Departement).Distinct().OrderBy(x => x);
            var lst = queryR.ToList();
            if (hasEmpty == true)
                lst.Add(Constante.NO_DIRECTION_ID);
            return lst;
        }
        public int GetCountUserActiveByDepartementName(string department)
        {
            if (String.IsNullOrEmpty(department) || department.Equals(Constante.NO_DIRECTION_ID))
            {
                return _userrepository.RepositoryQuery.Where(x => x.Active && string.IsNullOrEmpty(x.Departement))
                    .Select(x => x.ID).Distinct().Count();
            }
            return _userrepository.RepositoryQuery.Where(x => x.Active && x.Departement.Equals(department))
                .Select(x => x.ID).Distinct().Count();
        }
        public List<DepartementDTO> ManagerList()
        {
            var query = _userrepository.RepositoryQuery.Where(x => x.Active && !string.IsNullOrEmpty(x.Departement));
            var queryR = query.Where(x=> x.ID_Manager.HasValue).Select(x => new DepartementDTO() { Departement = x.Departement, Division=x.Division, ID_Manager=x.ID_Manager }).OrderBy(x => x.Departement).ThenBy(x=>x.Division);
            var lst = queryR.ToList();
            return lst;
        }
        public int GetCountUserActiveByIDManager(string departement, int idManager)
        {
            int count = _userrepository.RepositoryQuery.Where(x => x.Active && x.ID_Manager.HasValue && x.ID_Manager.Value.Equals(idManager) && x.Departement.Equals(departement))
                .Select(x => x.ID).Distinct().Count();
            return count;
        }

        public List<string> SetManagers(string[] ListManager)
        {
            var result = new List<string>();
            foreach (var p in ListManager.ToList())
            {
                var s = p.Split('|');
                var id = int.Parse(s[1]);               
                var element = _userrepository.RepositoryQuery.Where(x => x.Active && x.ID == id).FirstOrDefault();
                var label = element.Departement + "|" + element.ID_Manager;
                result.Add(label);
            }
            return result;
        }

        public int GetCountUserActiveByIDManagerByDepartement(string departement)
        {
            return _userrepository.RepositoryQuery.Where(x => x.Active && x.ID_Manager.HasValue && x.Departement.Equals(departement))
                .Select(x => x.ID).Distinct().Count();
        }
        public List<string> AgencyNameList(bool hasEmpty)
        {
            var query = _userrepository.RepositoryQuery.Where(x=> !string.IsNullOrEmpty(x.AgencyName)).Select(x => x.AgencyName).Distinct().OrderBy(x => x);
            var lst = query.ToList();
            if (hasEmpty == true)
                lst.Insert(0, "");
            return lst;
        }
        public int GetCountUserActiveByAgencyName(string entity, string agence)
        {
            if (String.IsNullOrEmpty(agence))
            {
                return _userrepository.RepositoryQuery.Where(x => x.Active && x.EntityName.Equals(entity) && string.IsNullOrEmpty(x.AgencyName))
                    .Select(x => x.ID).Distinct().Count();
            }
            return _userrepository.RepositoryQuery.Where(x => x.Active && x.EntityName.Equals(entity) && x.AgencyName.Equals(agence))
                .Select(x => x.ID).Distinct().Count();
        }

        public List<Profile> ListProfileByUserId(int userId)
        {
            var query = _profilUserrepository.RepositoryQuery.Join(
                _profilerepository.RepositoryTable,
                pu => pu.ID_Profile,
                prf => prf.ID,
                (pu, prf) => new { pu, prf }).Where(x => x.pu.ID_IntitekUser == userId).Select(x => x.prf).OrderBy(x => x.Name);
            return query.ToList();
        }
        public int ListUsersCount(GetUserRequest allrequest)
        {
            var query = ListUsersQueryable(allrequest);
            var nb = query.Count();
            return nb;
            
        }
        public override string GetOperator(Type type, ColumnFilter columnFilter, int index, string filterValue)
        {
            if (type== typeof(UserDTO) && "ProfilListFilter".Equals(columnFilter.ColumnName))
            {
                columnFilter.FilterType = GridFilterType.Contains;
            }
            else if (type == typeof(DocumentDTO))
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
        public List<UserDTO> ListUsers(GetUserRequest allrequest)
        {
            var request = allrequest.Request;
            string orderBy = request.OrderColumn + request.SortAscDesc;
            var query = ListUsersQueryable(allrequest).OrderBy(orderBy).Skip((request.Page - 1) * request.Limit).Take(request.Limit);
            var lst = query.ToList();
            foreach (var user in lst)
            {
                user.ProfilList = this.ListProfileByUserId(user.ID);
                GetUserRequest requestUser = new GetUserRequest() { EntityName = allrequest.EntityName, AgencyName = allrequest.AgencyName, Id = user.ID };
                List<DocumentDTO> userReadDocList = this.ListDocuments(false, requestUser);
                //NotRead -NotApproved -NotTested
                user.DocumentRead = userReadDocList.Where(x => !x.IsRead.HasValue).Count();
                user.DocumentApproved = userReadDocList.Where(x => (x.Approbation.HasValue && x.Approbation == 1) && !x.IsApproved.HasValue).Count();
                user.DocumentTested = userReadDocList.Where(x => (x.Test.HasValue && x.Test == 1) && !x.IsTested.HasValue).Count();

            }
            return lst;
        }
        protected new IQueryable<UserDTO> FiltrerQuery<UserDTO>(string[] filterColumns, IQueryable<UserDTO> query)
        {
            List<ColumnFilter> columnFilters = this.GetColumnFilters(filterColumns, null);
            //Replace IsRoot by Status
            var columRooot = columnFilters.Where(x => x.ColumnName.Equals("IsRoot")).FirstOrDefault();
            if (columRooot != null)
            {
                var filterValue = "10"; //Admin=0, FO=10
                if ("true".Equals(columRooot.FilterValue))
                {
                    filterValue = "0";
                }
                var columnStatus = new ColumnFilter() { ColumnName="Status", FilterType= columRooot.FilterType, FilterValue= filterValue };
                columnFilters.Remove(columRooot);
                columnFilters.Add(columnStatus);
            }
            
            query = this.FiltrerQuery(columnFilters, query);
            return query;
        }
        public bool GetActivityUser(int idUser)
        {
            //si inactif :
            //InactivityStart is not null 
            //and InactivityStart <= CONVERT (date, GETDATE()) 
            //and(InactivityEnd is null or (InactivityEnd is not null and CONVERT(date, GETDATE()) <= InactivityEnd)
            var query = this._userrepository.RepositoryQuery.Where(x=> x.ID==idUser);
            var query1 = query.Select(x => !(x.InactivityStart.HasValue && DbFunctions.TruncateTime(x.InactivityStart)<= DbFunctions.TruncateTime(System.Data.Entity.SqlServer.SqlFunctions.GetDate())  &&
              (!x.InactivityEnd.HasValue || (x.InactivityEnd.HasValue && DbFunctions.TruncateTime(x.InactivityEnd)>= DbFunctions.TruncateTime(System.Data.Entity.SqlServer.SqlFunctions.GetDate()))))
            );
            bool activity = query1.FirstOrDefault();
            return activity;
        }


        public IntitekUser GetManager(int idManager)
        {          
            var query = _userrepository.RepositoryQuery.Where(x => x.ID_Manager == idManager);
            var users = query.ToList<IntitekUser>();
            return users.FirstOrDefault();
        }

        public List<IntitekUser> GetManagerList(int idManager)
        {
            var query = _userrepository.RepositoryQuery.Where(x => x.ID_Manager == idManager);
            var users = query.ToList<IntitekUser>();
            return users.ToList();
        }

        public IntitekUser GetUser(int idUser)
        {
            var query = _userrepository.RepositoryQuery.Where(x => x.ID == idUser);
            var users = query.ToList<IntitekUser>();
            return users.FirstOrDefault();
        }


        private IQueryable<UserDTO> ListUsersQueryable(GetUserRequest allrequest)
        {
            var request = allrequest.Request;
            string orderBy = request.OrderColumn + request.SortAscDesc;
            string entityName = allrequest.EntityName;
            string agencyName = allrequest.AgencyName;
            DateTime? entryDate = allrequest.EntryDate;
            DateTime? exitDate = allrequest.ExitDate;
            var query = this._userrepository.RepositoryQuery;
            if (!string.IsNullOrEmpty(entityName))
            {
                query = query.Where(x => x.EntityName.ToLower().Equals(entityName.ToLower()));
            }
            if (!string.IsNullOrEmpty(agencyName))
            {
                query = query.Where(x => x.AgencyName.ToLower().Equals(agencyName.ToLower()));
            }
            if (entryDate != null)
            {
                var entryDt = Convert.ToDateTime(entryDate);
                query = query.Where(x => x.EntryDate >= entryDt);
            }
            if (exitDate != null)
            {
                var exitDt = Convert.ToDateTime(exitDate);
                query = query.Where(x => x.ExitDate <= exitDt);
            }
            if (allrequest.Actif==1)
            {
                query = query.Where(x => x.Active);
            }
            else if (allrequest.Actif == 2)
            {
                query = query.Where(x => !x.Active);
            }
            
            var query1 = query.Select(x => new UserDTO()
            {
                ID = x.ID,
                Name = x.Username,
                FullName = x.FullName,
                Status = x.Status,
                EntityName = x.EntityName,
                AgencyName = x.AgencyName,
                IsOnBoarding = x.isOnBoarding,
                Active =x.Active,
                Activity = !(x.InactivityStart.HasValue && DbFunctions.TruncateTime(x.InactivityStart) <= DbFunctions.TruncateTime(System.Data.Entity.SqlServer.SqlFunctions.GetDate()) &&
              (!x.InactivityEnd.HasValue || (x.InactivityEnd.HasValue && DbFunctions.TruncateTime(x.InactivityEnd) >= DbFunctions.TruncateTime(System.Data.Entity.SqlServer.SqlFunctions.GetDate())))),
                Type = x.Type,
                EmailOnBoarding =x.EmailOnBoarding,
                Email = x.Email,
                ExitDate = x.ExitDate,
                EntryDate=x.EntryDate,
                ProfilListFilter = x.ProfileUser.Select(p => p.Profile!=null? p.Profile.ID:0).ToList(),
                Manager= x.ID_Manager!=null? this._userrepository.RepositoryQuery.Where(y=>y.ID==x.ID_Manager).AsEnumerable().FirstOrDefault():null,
                ImportManager=(x.ID_Manager!=null && this._userrepository.RepositoryQuery.Where(y => y.ID == x.ID_Manager).AsEnumerable().FirstOrDefault().Email != null)? 
                    (this._importManagerrepository.RepositoryQuery.Where(z=>z.accountUser==this._userrepository.RepositoryQuery.Where(y => y.ID == x.ID_Manager).AsEnumerable().FirstOrDefault().Email).FirstOrDefault()):null
            });
            //si inactif :
            //InactivityStart is not null 
            //and InactivityStart <= CONVERT (date, GETDATE()) 
            //and(InactivityEnd is null or (InactivityEnd is not null and CONVERT(date, GETDATE()) <= InactivityEnd)

            if (allrequest.Activity == 1)
            {
                // query = query.Where(x => !(x.InactivityStart.HasValue && DbFunctions.DiffDays(x.InactivityStart, System.Data.Entity.SqlServer.SqlFunctions.GetDate()) <= 0 &&
                //(!x.InactivityEnd.HasValue || (x.InactivityEnd.HasValue && DbFunctions.DiffDays(x.InactivityEnd, System.Data.Entity.SqlServer.SqlFunctions.GetDate()) >= 0))));
                query1 = query1.Where(x => x.Activity);
            }
            //Inactive
            else if (allrequest.Activity == 2)
            {
                //query = query.Where(x => x.InactivityStart.HasValue && DbFunctions.DiffDays(x.InactivityStart, System.Data.Entity.SqlServer.SqlFunctions.GetDate()) <= 0 && 
                //(!x.InactivityEnd.HasValue || (x.InactivityEnd.HasValue && DbFunctions.DiffDays(x.InactivityEnd, System.Data.Entity.SqlServer.SqlFunctions.GetDate()) >= 0)));
                query1 = query1.Where(x => !x.Activity);
            }
            query1 = this.FiltrerQuery(request.Filtres, query1);
            return query1;
        }
        
        public int ListDocumentReadByLoginCount(GetUserRequest request)
        {
            var qury = this.ListDocumentReadByLoginQueryable(request);
            var nb = qury.Count();
            return nb;

        }
        public List<DocumentDTO> ListDocumentReadByLogin(GetUserRequest allrequest)
        {
            var request = allrequest.Request;
            string orderBy = "OrdreCategory, OrdreSubCategory, " + request.OrderColumn + request.SortAscDesc;
            var query = this.ListDocumentReadByLoginQueryable(allrequest);
            query = query.OrderBy(orderBy).Skip((request.Page - 1) * request.Limit).Take(request.Limit); ;
            var list = query.ToList();
            return list;
        }
        private IQueryable<DocumentDTO> ListDocumentReadByLoginQueryable(GetUserRequest allrequest)
        {
            int idLang = allrequest.IdLang;
            int idDefaultLang = allrequest.IdDefaultLang;
            IntitekUser user = _userrepository.FindBy(allrequest.Id);
            int userId = user.ID;
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
                        IsDefaultLangName = string.IsNullOrEmpty(EdmxFunction.GetNameDocument(parent.doc.ID, idLang, idLang)),
                        NameSubCategory = EdmxFunction.GetSubCategoryLang(parent.doc.ID_SubCategory.Value, idLang, idLang),
                        DefaultNameSubCategory = EdmxFunction.GetSubCategoryLang(parent.doc.ID_SubCategory.Value, idDefaultLang, idDefaultLang),
                        Version = parent.doc.Version,
                        Date = parent.doc.Date,
                        isMetier = parent.doc.isMetier,
                        isStructure = parent.doc.isStructure,
                        Approbation = parent.doc.Approbation,
                        Test = parent.doc.Test,
                        ID_Category = parent.doc.ID_Category,
                        ID_SubCategory = parent.doc.ID_SubCategory,
                        Inactif = parent.doc.Inactif,
                        IsNoActionRequired = parent.doc.IsNoActionRequired,
                        DocumentCategoryLang = child.DocumentCategoryLang.FirstOrDefault(f => f.ID_Lang == idLang),
                        DefaultNameCategory = EdmxFunction.GetCategoryLang(parent.doc.ID_Category.Value, idDefaultLang, idDefaultLang),
                        OrdreCategory = child.OrdreCategory != null ? child.OrdreCategory : 1000,
                        OrdreSubCategory = child.SubCategory.Where(x => x.ID == parent.doc.ID_SubCategory).Select(x => x.Ordre).FirstOrDefault()
                    }),
                ud => ud.ID_Document,
                doc => doc.ID,
                (ud, doc) => new { ud, doc })
           .SelectMany(x => x.doc.DefaultIfEmpty(), (parent, child) => new DocumentDTO
           {
               Num = 1,
               ID = child.ID,
               Name = child.Name.Trim(),
               IsDefaultLangName = child.IsDefaultLangName,
               NameSubCategory = child.NameSubCategory,
               DefaultNameSubCategory = child.DefaultNameSubCategory,
               Version = child.Version,
               Date = parent.ud.UpdateDate,
               IsMetier = child.isMetier,
               IsStructure = child.isStructure,
               Approbation = child.Approbation,
               Test = child.Test,
               ID_Category = child.ID_Category,
               ID_SubCategory = child.ID_SubCategory,
               Inactif = child.Inactif,
               IsNoActionRequired = child.IsNoActionRequired,
               DocumentCategoryLang = child.DocumentCategoryLang,
               DefaultNameCategory = child.DefaultNameCategory,
               OrdreCategory = child.OrdreCategory,
               OrdreSubCategory = child.OrdreSubCategory != null ? child.OrdreSubCategory : 1000,
               IsRead = parent.ud.IsRead,
               IsApproved = parent.ud.IsApproved,
               IsTested = parent.ud.IsTested
           })
           .Where(x => x.Inactif == false);

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
                                IsDefaultLangName = string.IsNullOrEmpty(EdmxFunction.GetNameDocument(parent.doc.ID, idLang, idLang)),
                                NameSubCategory = EdmxFunction.GetSubCategoryLang(parent.doc.ID_SubCategory.Value, idLang, idLang),
                                DefaultNameSubCategory = EdmxFunction.GetSubCategoryLang(parent.doc.ID_SubCategory.Value, idDefaultLang, idDefaultLang),
                                Version = parent.doc.Version,
                                Date = parent.doc.Date,
                                isMetier = parent.doc.isMetier,
                                isStructure = parent.doc.isStructure,
                                Approbation = parent.doc.Approbation,
                                Test = parent.doc.Test,
                                ID_Category = parent.doc.ID_Category,
                                ID_SubCategory = parent.doc.ID_SubCategory,
                                Inactif = parent.doc.Inactif,
                                IsNoActionRequired = parent.doc.IsNoActionRequired,
                                DocumentCategoryLang = child.DocumentCategoryLang.FirstOrDefault(f => f.ID_Lang == idLang),
                                DefaultNameCategory = EdmxFunction.GetCategoryLang(parent.doc.ID_Category.Value, idDefaultLang, idDefaultLang),
                                OrdreCategory = child.OrdreCategory != null ? child.OrdreCategory : 1000,
                                OrdreSubCategory = child.SubCategory.Where(x => x.ID == parent.doc.ID_SubCategory).Select(x => x.Ordre).FirstOrDefault()
                            }),
                    pdD => pdD.pd.ID_Document,
                    doc => doc.ID,
                    (pdD, doc) => new { pdD, doc })
                   .SelectMany(x => x.doc.DefaultIfEmpty(), (parent, child) => new DocumentDTO
                   {
                       Num = 2,
                       ID = child.ID,
                       Name = child.Name.Trim(),
                       IsDefaultLangName = child.IsDefaultLangName,
                       NameSubCategory = child.NameSubCategory,
                       DefaultNameSubCategory = child.DefaultNameSubCategory,
                       Version = child.Version,
                       Date = child.Date,
                       IsMetier = child.isMetier,
                       IsStructure = child.isStructure,
                       Approbation = child.Approbation,
                       Test = child.Test,
                       ID_Category = child.ID_Category,
                       ID_SubCategory = child.ID_SubCategory,
                       Inactif = child.Inactif,
                       IsNoActionRequired = child.IsNoActionRequired,
                       DocumentCategoryLang = child.DocumentCategoryLang,
                       DefaultNameCategory = child.DefaultNameCategory,
                       OrdreCategory = child.OrdreCategory,
                       OrdreSubCategory = child.OrdreSubCategory != null ? child.OrdreSubCategory : 1000,
                       IsRead = null,
                       IsApproved = null,
                       IsTested = null
                    })
                   .Where(x => x.Inactif == false && !x.IsNoActionRequired);

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
                            IsDefaultLangName = string.IsNullOrEmpty(EdmxFunction.GetNameDocument(parent.doc.ID, idLang, idLang)),
                            NameSubCategory = EdmxFunction.GetSubCategoryLang(parent.doc.ID_SubCategory.Value, idLang, idLang),
                            DefaultNameSubCategory = EdmxFunction.GetSubCategoryLang(parent.doc.ID_SubCategory.Value, idDefaultLang, idDefaultLang),
                            Version = parent.doc.Version,
                            Date = parent.doc.Date,
                            isMetier = parent.doc.isMetier,
                            isStructure = parent.doc.isStructure,
                            Approbation = parent.doc.Approbation,
                            Test = parent.doc.Test,
                            ID_Category = parent.doc.ID_Category,
                            ID_SubCategory = parent.doc.ID_SubCategory,
                            Inactif = parent.doc.Inactif,
                            IsNoActionRequired = parent.doc.IsNoActionRequired,
                            DocumentCategoryLang = child.DocumentCategoryLang.FirstOrDefault(f => f.ID_Lang == idLang),
                            DefaultNameCategory = EdmxFunction.GetCategoryLang(parent.doc.ID_Category.Value, idDefaultLang, idDefaultLang),
                            OrdreCategory = child.OrdreCategory != null ? child.OrdreCategory : 1000,
                            OrdreSubCategory = child.SubCategory.Where(x => x.ID == parent.doc.ID_SubCategory).Select(x => x.Ordre).FirstOrDefault()
                        }),
                    ed => ed.ID_Document,
                    doc => doc.ID,
                    (ed, doc) => new { ed, doc })
               .SelectMany(x => x.doc.DefaultIfEmpty(), (parent, child) => new DocumentDTO
               {
                   Num = 3,
                   ID = child.ID,
                   Name = child.Name.Trim(),
                   IsDefaultLangName = child.IsDefaultLangName,
                   NameSubCategory = child.NameSubCategory,
                   DefaultNameSubCategory = child.DefaultNameSubCategory,
                   Version = child.Version,
                   Date = child.Date,
                   IsMetier = child.isMetier,
                   IsStructure = child.isStructure,
                   Approbation = child.Approbation,
                   Test = child.Test,
                   ID_Category = child.ID_Category,
                   ID_SubCategory = child.ID_SubCategory,
                   Inactif = child.Inactif,
                   IsNoActionRequired = child.IsNoActionRequired,
                   DocumentCategoryLang = child.DocumentCategoryLang,
                   DefaultNameCategory = child.DefaultNameCategory,
                   OrdreCategory = child.OrdreCategory,
                   OrdreSubCategory = child.OrdreSubCategory != null ? child.OrdreSubCategory : 1000,
                   IsRead = null,
                   IsApproved = null,
                   IsTested = null
               })
               .Where(x => x.Inactif == false && !x.IsNoActionRequired);
            var query = query1.Union(query2).Union(query3);
            if (user.Type == Constante.UserType_METIER)
            {
                query = query.Where(x => x.IsMetier);
            }
            else if (user.Type == Constante.UserType_STRUCTURE)
            {
                query = query.Where(x => x.IsStructure);
            }
            var request = allrequest.Request;
            query = this.FiltrerQuery(request.Filtres, query);
            return query;
        }
        
        public List<DocumentDTO> ListDocuments(bool lu, GetUserRequest request)
        {
            IntitekUser user = _userrepository.FindBy(request.Id);
            int idLang = request.IdLang;
            int userId = user.ID;
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
            .GroupJoin(this._docrepository.RepositoryTable,
                ud => ud.ID_Document,
                doc => doc.ID,
                (ud, doc) => new { ud, doc })
           .SelectMany(x => x.doc.DefaultIfEmpty(), (parent, child) => new DocumentDTO
           {
               Num = 1,
               ID = child.ID,
               Version = child.Version,
               IsMetier = child.isMetier,
               IsStructure = child.isStructure,
               Approbation = child.Approbation,
               Test = child.Test,
               ID_Category = child.ID_Category,
               ID_SubCategory = child.ID_SubCategory,
               Inactif = child.Inactif,
               IsNoActionRequired = child.IsNoActionRequired,
               IsRead = parent.ud.IsRead,
               IsApproved = parent.ud.IsApproved,
               IsTested = parent.ud.IsTested
           })
           .Where(x => x.Inactif == false);

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
                        this._docrepository.RepositoryTable,
                        pdD => pdD.pd.ID_Document,
                        doc => doc.ID,
                        (pdD, doc) => new { pdD, doc })
                       .SelectMany(x => x.doc.DefaultIfEmpty(), (parent, child) => new DocumentDTO
                       {
                           Num = 2,
                           ID = child.ID,
                           Version = child.Version,
                           IsMetier = child.isMetier,
                           IsStructure = child.isStructure,
                           Approbation = child.Approbation,
                           Test = child.Test,
                           ID_Category = child.ID_Category,
                           ID_SubCategory = child.ID_SubCategory,
                           Inactif = child.Inactif,
                           IsNoActionRequired = child.IsNoActionRequired,
                           IsRead = null,
                           IsApproved = null,
                           IsTested = null
                       })
                       .Where(x => x.Inactif == false && !x.IsNoActionRequired);

            var query3 = this._entityDocrepository.RepositoryQuery
                .Where(x => x.EntityName.Equals(user.EntityName)
                    && (x.AgencyName.Equals(user.AgencyName) || string.IsNullOrEmpty(x.AgencyName))
                    && !this._userDocrepository.RepositoryQuery.Where(u => u.ID_IntitekUser == user.ID).Select(u => u.ID_Document).Contains(x.ID_Document)
                )
                .GroupJoin(this._docrepository.RepositoryTable,
                    ed => ed.ID_Document,
                    doc => doc.ID,
                    (ed, doc) => new { ed, doc })
               .SelectMany(x => x.doc.DefaultIfEmpty(), (parent, child) => new DocumentDTO
               {
                   Num = 3,
                   ID = child.ID,
                   Version = child.Version,
                   IsMetier = child.isMetier,
                   IsStructure = child.isStructure,
                   Approbation = child.Approbation,
                   Test = child.Test,
                   ID_Category = child.ID_Category,
                   ID_SubCategory = child.ID_SubCategory,
                   Inactif = child.Inactif,
                   IsNoActionRequired = child.IsNoActionRequired,
                   IsRead = null,
                   IsApproved = null,
                   IsTested = null
               })
               .Where(x => x.Inactif == false && !x.IsNoActionRequired);
            
            var query = query1;
            if(!lu)
            {
                query = query1.Union(query2).Union(query3);
            }
            if (user.Type == Constante.UserType_METIER)
            {
                query = query.Where(x => x.IsMetier);
            }
            else if (user.Type == Constante.UserType_STRUCTURE)
            {
                query = query.Where(x => x.IsStructure);
            }
            var lst = query.ToList();
            return lst;
        }
        public List<ProfilDTO> ListProfil(GetUserRequest request)
        {
            var query = this._profilerepository.RepositoryTable.Select(x => new ProfilDTO()
            {
                ID = x.ID,
                Name = x.Name,
                Affecte = x.ProfileUser.Where(y=> y.ID_IntitekUser.Value == request.Id).Any()? true: false
            }).OrderBy(x=> x.Name);
            var lst = query.ToList();
            foreach (var item in lst)
            {
                item.IsSessionChecked = item.Affecte;
                //Affecté : Coché ou non en session
                if (request.DocsAffected != null)
                {
                    var prfState = request.DocsAffected.Find(x => x.DocId == item.ID);
                    if (prfState != null && prfState.OldCheckState != prfState.NewCheckState)
                    {
                        item.IsSessionChecked = prfState.NewCheckState;
                    }
                }
            }
            return lst;
        }
        public void Save(SaveUserRequest request)
        {
            List<DocCheckState> lstdocs = request.DocsAffected;
            if (lstdocs != null && lstdocs.Count > 0)
            {
                var lstEntityDB = this._profilUserrepository.RepositoryQuery.Where(x => x.ID_IntitekUser == request.Id.Value);
                //Ajout
                var docs = lstdocs.Where(x => x.NewCheckState && x.OldCheckState == false).Select(x => x.DocId).ToList();
                foreach (var prf in docs)
                {
                    if (!lstEntityDB.Any(x=> x.ID_Profile == prf))
                    {
                        ProfileUser pu = new ProfileUser()
                        {
                            ID_IntitekUser = request.Id.Value,
                            ID_Profile = prf
                        };
                        this._profilUserrepository.Add(pu);
                    }
                }
                var docsR = lstdocs.Where(x => x.NewCheckState == false && x.OldCheckState).Select(x => x.DocId).ToList();
                this._profilUserrepository.RemoveAll(lstEntityDB.Where(x => docsR.Contains(x.ID_Profile.Value)));
            }
            //modification status
            if (request.Id.HasValue && request.Id.Value > 0)
            {
                var userToUpdate = _userrepository.RepositoryTable.Find(request.Id.Value);               
                userToUpdate.Id = userToUpdate.ID;
                bool isUpdating = false;
                if (userToUpdate.Status.HasValue && userToUpdate.isReader != request.isReader)
                {
                    userToUpdate.isReader = request.isReader;
                    isUpdating = true;
                }
                if (userToUpdate.Status.HasValue && userToUpdate.Status.Value!= request.Status)
                {
                    userToUpdate.Status = request.Status;
                    isUpdating = true;
                }
                if(request.ExitDate!=null)
                {
                    userToUpdate.ExitDate = request.ExitDate;
                }
                if (isUpdating)
                {
                    _userrepository.Save(userToUpdate);
                }
            }
            
        }

        
        public SynchronizeADResponse SynchronizeWithAd(SynchronizeADRequest request)
        {
            var adMailDiscarded = new List<UserAdDTO>();
            var toAdd = new List<IntitekUser>();
            var toUpdate = new List<IntitekUser>();
            var toDelete = new List<IntitekUser>();
            var operation = string.Empty;
            var crudErrors = new List<string>();
            var syntheseErrors = new List<string>();
            var currentUser = new IntitekUser();
            try
            {
                // User out of LDAP
                var userstoDelete = _userrepository.FindBy(new Specification<IntitekUser>(iu => iu.ID_AD == request.Id_AD && iu.Active))
                    .Select(u => new { Username = u.Username.Trim().ToLower(), Email = u.Email.Trim().ToLower(), u.ID_AD })
                    .Except(request.ADUsers.Select(u => new
                    {
                        Username = u.UserName.Trim().ToLower(),
                        Email = u.Email.Trim().ToLower(),
                        u.ID_AD,
                    })).ToList();

                foreach (var userToDelete in userstoDelete)
                {

                    var user = _userrepository.FindBy(new Specification<IntitekUser>(u => u.Email == userToDelete.Email 
                            && u.Username == userToDelete.Username 
                            && u.ID_AD == userToDelete.ID_AD)).FirstOrDefault();
                    if (user != null)
                    {
                        currentUser = user;
                        operation = "Delete";

                        user.Id = user.ID;
                        user.Active = false;

                        user.ExitDate = DateTime.Now;

                        var res = _userrepository.Save(user);
                        if (!string.IsNullOrEmpty(res))
                        {                    
                            crudErrors.Add(string.Format("DELETE ERROR: {1}{0}{2}", Environment.NewLine, user.Username, res));
                            syntheseErrors.Add(string.Format("ERREUR DE SUPPRESSION : {1}{0}{2}", Environment.NewLine, user.Username, res));
                        }
                        else{
                            toDelete.Add(user);
                        }
                    }
                }

                foreach (var userAD in request.ADUsers.Where(u => string.IsNullOrEmpty(u.Email)))
                {
                    adMailDiscarded.Add(userAD);
                }
                //Sauvegarde users AD vers Databse
                foreach (var userAD in request.ADUsers.Where(u => !string.IsNullOrEmpty(u.Email)))
                {
                    if (!string.IsNullOrEmpty(userAD.Division) && "#N/A".Equals(userAD.Division))
                    {
                        userAD.Division = string.Empty;
                    }
                    var user = _userrepository.FindBy(new Specification<IntitekUser>(u => u.Username == userAD.UserName && u.ID_AD == request.Id_AD)).FirstOrDefault();
                    currentUser = new IntitekUser()
                    {

                        Username = userAD.UserName,
                        EntityName = userAD.EntityName,
                        AgencyName = userAD.AgencyName,
                        Departement = userAD.Departement,
                        Division = userAD.Division,
                        Pays = userAD.Pays,
                        Plaque = userAD.Plaque,
                        Email = userAD.Email,
                        FirstName = userAD.FirstName,
                        FullName = userAD.FullName,
                        ID_AD = userAD.ID_AD,
                        Type = userAD.Type,
                       
                        Active = !userAD.IsBlackListed // to be verified tested qnd confirmed
                    };

                    if (user != null)
                    {

                        operation = "Update";

                        user.Id = user.ID;
                        user.Username = userAD.UserName;
                        user.EntityName = userAD.EntityName;
                        user.AgencyName = userAD.AgencyName;
                        user.Departement = userAD.Departement;
                        user.Division = userAD.Division;
                        user.Pays = userAD.Pays;
                        user.Plaque = userAD.Plaque;
                        user.Email = userAD.Email;
                        user.FirstName = userAD.FirstName;
                        user.FullName = userAD.FullName;
                        user.ID_AD = userAD.ID_AD;
                        user.Type = userAD.Type;

                        user.EntryDate = userAD.EntryDate;
                        user.ExitDate = null;

                        user.Active = !userAD.IsBlackListed; // to be verified tested qnd confirmed
                        
                        var res = _userrepository.Save(user);
                        if (!string.IsNullOrEmpty(res))
                        {
                            crudErrors.Add(string.Format("UPDATE ERROR: {1}{0}{2}", Environment.NewLine, user.Username, res));
                            syntheseErrors.Add(string.Format("ERREUR DE MODIFICATION : {1}{0}{2}", Environment.NewLine, user.Username, res));
                        }
                        else{
                            toUpdate.Add(user);
                        }
                    }
                    else
                    {
                        var userToAdd = new IntitekUser()
                        {
                            //Id = user.ID,
                            Username = userAD.UserName,
                            EntityName = userAD.EntityName,
                            AgencyName = userAD.AgencyName,
                            Departement = userAD.Departement,
                            Division = userAD.Division,
                            Pays = userAD.Pays,
                            Plaque = userAD.Plaque,
                            Email = userAD.Email,
                            FirstName = userAD.FirstName,
                            FullName = userAD.FullName,
                            Type = userAD.Type,
                            Status = 10,
                            ID_AD = userAD.ID_AD,
                            EntryDate = DateTime.Now,
                            ExitDate = null,
                            Active = true
                        };

                        operation = "Add";
                        if (!userAD.IsBlackListed)
                        { // to be verified tested qnd confirmed)
                            var userMailExist = _userrepository.FindBy(new Specification<IntitekUser>(u => u.Email.Equals(userAD.Email, StringComparison.InvariantCultureIgnoreCase))).Any();
                            if (userMailExist)
                            {
                                var res = string.Format("Email [{0}] de l'utilisateur (Fullname : {1}, Username : {2}, AD : {3}) existe déjà dans la base", userAD.Email, userAD.FullName, userAD.UserName, request.Name_AD);
                                _logger.Error(res);
                                crudErrors.Add(string.Format("ADD ERROR: {1}{0}{2}", Environment.NewLine, userToAdd.Username, res));
                                syntheseErrors.Add(res);
                            }
                            else
                            {
                                var res = _userrepository.Add(userToAdd);
                                if (!string.IsNullOrEmpty(res))
                                {
                                    crudErrors.Add(string.Format("ADD ERROR : {1}{0}{2}", Environment.NewLine, userToAdd.Username, res));
                                    syntheseErrors.Add(string.Format("ERREUR D'INSERTION : {1}{0}{2}", Environment.NewLine, userToAdd.Username, res));
                                }
                                else
                                {
                                    toAdd.Add(userToAdd);
                                }
                            }
                            
                        }
                    }
                }
                var cityEntityblackListed = request.BlackListedAgencies.Select(x => new { x.EntityName, x.AgencyName }).Distinct().ToList();

                // Removing all City/Entity BlackListed
                _cityEntityBlacklistedrepository.RemoveAll();

                foreach (var ec in cityEntityblackListed)
                {
                    var ecBlackListed = new CityEntityBlackListed()
                    {
                        City = ec.AgencyName,
                        Entity = ec.EntityName,
                        DateCre = DateTime.Now
                    };
                    //_cityEntityBlacklistedrepository.Add(ecBlackListed);
                    CityEntityBlackListed ecBlackListedExist = _cityEntityBlacklistedrepository
                        .FindBy(new Specification<CityEntityBlackListed>(c => c.Entity == ecBlackListed.Entity && c.City == ecBlackListed.City )).FirstOrDefault();
                    if (ecBlackListedExist == null)
                    {
                        _cityEntityBlacklistedrepository.Add(ecBlackListed);
                    }
                    //
                }
                return new SynchronizeADResponse()
                {
                    Result = 0,
                    ErrorMessage = string.Empty,
                    UsersADToSync = request.ADUsers,
                    UsersADMailDiscarded = adMailDiscarded,
                    BlackListedUsers = request.ADBlackListedUsers,
                    UsersToAdd = toAdd,
                    UsersToDelete = toDelete,
                    UsersToUpdate = toUpdate,
                    CRUDErrors = crudErrors,
                    SyntheseADErrors = syntheseErrors,
                    Count = _userrepository.Count(new Specification<IntitekUser>(u => u.Active && u.ID_AD == request.Id_AD))
                };

            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "SynchronozeWithAd",
                    ServiceName = "UserService",

                }, ex);
                return new SynchronizeADResponse()
                {
                    Result = -1,
                    ErrorMessage = string.Format("Error on ID_AD: {1} {0} User: {2} {0} Operation: {3} {0} {4} {0}", Environment.NewLine, request.Id_AD, UserInfo(currentUser), operation, ex.Message),
                    UsersADToSync = request.ADUsers,
                    UsersADMailDiscarded = adMailDiscarded,
                    BlackListedUsers = request.ADBlackListedUsers,
                    UsersToAdd = toAdd,
                    UsersToDelete = toDelete,
                    UsersToUpdate = toUpdate,                    
                    Count = 0
                };
            }
        }
        public void UpdateIdManager(SynchronizeADResponse response, int id_AD, string name_AD)
        {
            //Test l'existance d'un manager
            var listUsersDB = response.UsersToAdd.Union(response.UsersToUpdate).ToList();
            foreach (var userAD in listUsersDB)
            {
                //if (!string.IsNullOrEmpty(userAD.FullName))
                //{
                //    var nbre = GetNbreUserByFullnameAD(userAD.FullName, userAD.ID_AD);
                //    if (nbre > 1)
                //    {
                //        _logger.Info(string.Format("L'utilisateur [{0}] se répète [{1} fois] dans la base (AD {2})", userAD.FullName, nbre, name_AD));
                //    }
                //}
                if (!string.IsNullOrEmpty(userAD.Division))
                {
                    if (userAD.Division.Contains("@"))
                    {
                        var users = GetIntitekUserByEmail(userAD.Division);
                        if (users == null)
                        {
                            var msg = string.Format("Manager [{0}] inconnu pour utilisateur [{1}]  (AD : {2})", userAD.Division, userAD.FullName, name_AD);
                            _logger.Info(msg);
                            response.SyntheseADErrors.Add(msg);
                        }
                        //MAJ ID_Manager
                        else
                        {
                            IntitekUser userDB = null;
                            if (userAD.ID > 0)
                            {
                                userDB = _userrepository.FindBy(userAD.ID);
                            }
                            else
                            {
                                userDB = _userrepository.FindBy(new Specification<IntitekUser>(u => u.Username == userAD.Username && u.ID_AD == id_AD)).FirstOrDefault();
                            }
                            if (userDB != null && userDB.ID > 0)
                            {
                                userDB.Id = userDB.ID;
                                userDB.ID_Manager = users.ID;
                                var res = _userrepository.Save(userDB);
                                if (!string.IsNullOrEmpty(res))
                                {
                                    response.CRUDErrors.Add(string.Format("UPDATE ID_Manager ERROR: {1}{0}{2}", Environment.NewLine, userDB.Username, res));
                                    response.SyntheseADErrors.Add(string.Format("ERREUR MODIFICATION DE L' ID_MANAGER : {1}{0}{2}", Environment.NewLine, userDB.Username, res));
                                };
                            }
                        }
                    } else
                    {
                        var users = GetIntitekUserByFullname(userAD.Division);
                        if (users == null || !users.Any())
                        {
                            var msg = string.Format("Manager [{0}] inconnu pour utilisateur [{1}]  (AD : {2})", userAD.Division, userAD.FullName, name_AD);
                            _logger.Info(msg);
                            response.SyntheseADErrors.Add(msg);
                        }
                        //MAJ ID_Manager
                        else
                        {
                            IntitekUser userDB = null;
                            if (userAD.ID > 0)
                            {
                                userDB = _userrepository.FindBy(userAD.ID);
                            }
                            else
                            {
                                userDB = _userrepository.FindBy(new Specification<IntitekUser>(u => u.Username == userAD.Username && u.ID_AD == id_AD)).FirstOrDefault();
                            }
                            if (userDB != null && userDB.ID > 0)
                            {
                                userDB.Id = userDB.ID;
                                userDB.ID_Manager = users.FirstOrDefault().ID;
                                var res = _userrepository.Save(userDB);
                                if (!string.IsNullOrEmpty(res))
                                {
                                    response.CRUDErrors.Add(string.Format("UPDATE ID_Manager ERROR: {1}{0}{2}", Environment.NewLine, userDB.Username, res));
                                    response.SyntheseADErrors.Add(string.Format("ERREUR MODIFICATION DE L' ID_MANAGER : {1}{0}{2}", Environment.NewLine, userDB.Username, res));
                                };
                            }
                        }
                    }
                }

            }
        }
        private string UserInfo(IntitekUser user)
        {
            return string.Format("Username: {0}, EntityName: {1}, AgencyName: {2}, Email: {3}, Type: {4}", user.Username, user.EntityName, user.AgencyName, user.Email, user.Type);
        }
        public IntitekUser  GetIntitekUserByEmail(string email)
        {
            var user = _userrepository.RepositoryQuery.Where(p=> p.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            return user;           
        }

        public List<string> GetUsersByName(){
            var users = _userrepository.RepositoryQuery.Select(p => p.FullName).Distinct().ToList();
            users.Sort();
            return users;
        }

        public List<IntitekUser> GetIntitekUsers()
        {           
            var result = _userrepository.RepositoryQuery.ToList();
            result.Sort();
            return result;
        }

        public List<IntitekUser> GetIntitekUserTeam(string fullname)
        {
            var users = GetIntitekUserByFullname(fullname);
            var item = users.FirstOrDefault();
            var result = _userrepository.RepositoryQuery.Where(x => x.ID_Manager == item.ID_Manager).ToList();
            result.Sort();
            return users;
        }

        public List<string> GetUsersTeam(int idManager)
        {
            var users = _userrepository.RepositoryQuery.Where(x=> x.ID_Manager == idManager).Select(p => p.FullName).Distinct().ToList();
            users.Sort();
            return users;
        }

        public List<IntitekUser> GetIntitekUserByFullname(string fullname)
        {
            var users = _userrepository.RepositoryQuery.Where(p => p.FullName.Equals(fullname, StringComparison.InvariantCultureIgnoreCase)).ToList();
            return users;
        }

        private int GetNbreUserByFullnameAD(string fullname, int id_AD)
        {
            var users = _userrepository.RepositoryQuery.Where(p => p.FullName.Equals(fullname, StringComparison.InvariantCultureIgnoreCase) && p.ID_AD==id_AD).Count();
            return users;
        }
        
        public void RemoveAllInactivity()
        {
            _userrepository.RemoveAllInactivity();
        }
        public void UpdateAll(List<IntitekUser> users)
        {
               try
                {
                    _userrepository.UpdateAll(users);
                }
                catch (Exception ex)
                {
                    _logger.Error(new ExceptionLogger()
                    {
                        ExceptionDateTime = DateTime.Now,
                        ExceptionStackTrack = ex.StackTrace,
                        MethodName = "ImporterInactivity",
                        ServiceName = "UserService",

                    }, ex);
                    throw ex;
                }
        }

        
    }
}
