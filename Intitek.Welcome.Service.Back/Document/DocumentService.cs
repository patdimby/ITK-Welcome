using System;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using Intitek.Welcome.DataAccess;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.Helpers;
using Intitek.Welcome.Infrastructure.Histo;
using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Infrastructure.Specification;
using Z.EntityFramework.Plus;
using System.Data.Entity;
using System.Data.SqlClient;

namespace Intitek.Welcome.Service.Back
{
    public class DocumentService : BaseService, IDocumentService
    {

        private readonly DocumentDataAccess _docrepository;
        private readonly DocumentLangDataAccess _doclangrepository;
        private readonly DocumentCategoryDataAccess _doccategoryrepository;
        public readonly SubCategoryDataAccess _subcategrepository;
        public readonly SubCategoryLangDataAccess _subcateglangrepository;
        private readonly DocumentCategoryLangDataAccess _doccategorylangrepository;
        private readonly DocumentVersionLangDataAccess _docversionlangrepository;
        private readonly DocumentVersionDataAccess _docversionrepository;
        private readonly UserDocumentDataAccess _userdocrepository;
        private readonly EntityDocumentDataAccess _entitydocrepository;
        private readonly ProfileDocumentDataAccess _profiledocrepository;
        private readonly ProfileDataAccess _profilerepository;
        private readonly QcmDataAccess _qcmRepository;
        private readonly UserQcmDataAccess _userqcmRepository;
        private readonly UserQcmReponseDataAccess _userqcmreponseRepository;


        private const string MinorVersion = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public DocumentService(ILogger logger) : base(logger)
        {
            _docrepository = new DocumentDataAccess(uow);
            _doclangrepository = new DocumentLangDataAccess(uow);
            _doccategoryrepository = new DocumentCategoryDataAccess(uow);
            _doccategorylangrepository = new DocumentCategoryLangDataAccess(uow);
            _subcategrepository = new SubCategoryDataAccess(uow);
            _subcateglangrepository = new SubCategoryLangDataAccess(uow);
            _docversionrepository = new DocumentVersionDataAccess(uow);
            _docversionlangrepository = new DocumentVersionLangDataAccess(uow);
            _entitydocrepository = new EntityDocumentDataAccess(uow);
            _profiledocrepository = new ProfileDocumentDataAccess(uow);
            _profilerepository = new ProfileDataAccess(uow);
            _userdocrepository = new UserDocumentDataAccess(uow);
            _qcmRepository = new QcmDataAccess(uow);
            _userqcmRepository = new UserQcmDataAccess(uow);
            _userqcmreponseRepository = new UserQcmReponseDataAccess(uow);

            //path for System.Link.Dynamic pour reconnaitre la classe EdmxFunction
            this.DynamicLinkPatch();
        }

        private string GetNextVersion(int docId, bool isMajorVersion)
        {

            var doc = _docrepository.FindBy(docId);

            if (doc != null && !string.IsNullOrEmpty(doc.Version))
            {
                var currentVersion = doc.Version;

                var majorVersion = 0;

                var minorVersion = currentVersion.Substring(2, 1);

                if (int.TryParse(currentVersion.Substring(0, 2), out majorVersion))
                {
                    if (isMajorVersion)
                    {
                        majorVersion++;
                        return string.Format("{0:00}A", majorVersion);
                    }
                    else
                    {
                        minorVersion = MinorVersion.Substring(MinorVersion.IndexOf(minorVersion) + 1, 1);
                    }
                }

                return string.Format("{0:00}{1}", majorVersion, minorVersion);
            }
            return "01A";
        }

        public List<DocumentCategoryDTO> GetAllCategory(GetAllCategoryRequest request)
        {
            int idLang = request.IdLang;
            int idDefaultLang = request.IdDefaultLang;
            var query = this._doccategoryrepository.RepositoryQuery
                .Select(x => new DocumentCategoryDTO()
                {
                    ID = x.ID,
                    Name = EdmxFunction.GetCategoryLang(x.ID, idLang, idDefaultLang),
                    ID_Lang = request.IdLang,
                    IsDefaultLangName = string.IsNullOrEmpty(EdmxFunction.GetCategoryLang(x.ID, idLang, idLang)),
                    OrdreCategory = x.OrdreCategory
                }).OrderBy(p => p.OrdreCategory).ToList();

            var lst = query.ToList();
            return lst;
        }
        public List<DocumentSubCategoryDTO> GetAllSubCategory(int idLang, int idDefaultLang)
        {
            return this._subcategrepository.RepositoryQuery
                .Join(_doccategoryrepository.RepositoryTable,
                    sub => sub.ID_DocumentCategory,
                    cat => cat.ID,
                    (sub, cat) => new { sub, cat }
                )
            .Select(x => new DocumentSubCategoryDTO()
            {
                ID = x.sub.ID,
                ID_DocumentCategory = x.sub.ID_DocumentCategory,
                NameCategory = EdmxFunction.GetCategoryLang(x.sub.ID_DocumentCategory, idLang, idDefaultLang),
                IsDefaultLangNameCategory = string.IsNullOrEmpty(EdmxFunction.GetCategoryLang(x.sub.ID_DocumentCategory, idLang, idLang)),
                OrdreSubCategory = x.sub.Ordre,
                OrdreCategory = x.cat.OrdreCategory,
                Name = EdmxFunction.GetSubCategoryLang(x.sub.ID, idLang, idDefaultLang),
                IsDefaultLangName = string.IsNullOrEmpty(EdmxFunction.GetSubCategoryLang(x.sub.ID, idLang, idLang)),
            }).OrderBy(p => p.OrdreCategory).ThenBy(p => p.OrdreSubCategory).ToList();
        }

        public List<CategoryDTO> GetAllCategories(int idLang)
        {
            List<CategoryDTO> categsDTO = new List<CategoryDTO>();
            var query = _doccategoryrepository.RepositoryTable.OrderBy(p => p.OrdreCategory);
            var categories = query.ToList();
            foreach (var item in categories)
            {
                var subcategories = _subcategrepository.FindBy(new Specification<SubCategory>(sc => sc.ID_DocumentCategory == item.ID));
                categsDTO.Add(new CategoryDTO()
                {
                    ID = item.ID,
                    CategoryTrad = _doccategorylangrepository.FindBy(new Specification<DocumentCategoryLang>(dcl => dcl.ID_DocumentCategory == item.ID && dcl.ID_Lang == idLang)).FirstOrDefault(),
                    DefaultTrad = _doccategorylangrepository.FindBy(new Specification<DocumentCategoryLang>(dcl => dcl.ID_DocumentCategory == item.ID && dcl.ID_Lang != idLang)).FirstOrDefault(),
                    OrdreCategory = item.OrdreCategory,
                    NbDocuments = _docrepository.Count(new Specification<Document>(doc => doc.ID_Category == item.ID)),
                    IsDeleted = !this.IsAttachedDocument(item.ID) && !subcategories.Any(),

                    Subcategories = subcategories.Select(subcateg =>
                        new SubCategoryDTO()
                        {
                            ID = subcateg.ID,
                            ID_Category = subcateg.ID_DocumentCategory,
                            NbDocuments = _docrepository.Count(new Specification<Document>(doc => doc.ID_SubCategory == subcateg.ID)),
                            IsDeleted = !IsAttachedSubcategDocument(subcateg.ID),
                            Ordre = subcateg.Ordre,
                            SubCategoryTrad = _subcateglangrepository.FindBy(new Specification<SubCategoryLang>(scl => scl.ID_SubCategory == subcateg.ID && scl.ID_Lang == idLang)).FirstOrDefault(),
                            DefaultTrad = _subcateglangrepository.FindBy(new Specification<SubCategoryLang>(scl => scl.ID_SubCategory == subcateg.ID && scl.ID_Lang != idLang)).FirstOrDefault(),

                        }).ToList()
                });
            }
            return categsDTO;
        }
        private bool IsAttachedDocument(int categID)
        {
            var query = _docrepository.RepositoryTable.Where(x => x.ID_Category == categID && x.Inactif == false);
            return query.Any();
        }
        private bool IsAttachedSubcategDocument(int scategID)
        {
            var query = _docrepository.RepositoryTable.Where(x => x.ID_SubCategory == scategID && x.Inactif == false);
            return query.Any();
        }
        public List<string> GetAllVersion()
        {
            var query = _docrepository.RepositoryTable.Select(p => p.Version.ToUpper()).Distinct().OrderBy(p => p);
            var versions = query.ToList();
            return versions;
        }

        public CreateDocumentResponse Create(CreateDocumentRequest request)
        {
            var response = new CreateDocumentResponse();
            try
            {



                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "Create",
                    ServiceName = "DocumentService",

                }, ex);
                throw ex;
            }
        }

        public DeleteDocumentResponse Delete(DeleteDocumentRequest request)
        {
            var ActionsBO = new List<ActionBO>();
            var response = new DeleteDocumentResponse();
            try
            {
                var docToDelete = _docrepository.FindBy(request.Id);
                if (docToDelete != null)
                {
                    docToDelete.Id = request.Id;
                    docToDelete.ID = request.Id;
                    docToDelete.ID_UserDel = request.UserId;
                    docToDelete.DateDel = DateTime.Now;
                    docToDelete.Inactif = true;
                    docToDelete.ID_UserDel = request.UserId;
                    docToDelete.DateDel = DateTime.Now;
                    _docrepository.Save(docToDelete);
                }


                ActionsBO.Add(new ActionBO()
                {
                    Action = Actions.Delete,
                    ID_Object = request.Id,
                    ID_User = request.UserId,
                    ObjectCode = ObjectCode.DOCUMENT,
                    LinkObjects = string.Empty,
                    DateAction = DateTime.Now

                });

                _histoActionBO = new HistoActionBO(base.uow, ActionsBO);
                _histoActionBO.SaveHisto();

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "Delete",
                    ServiceName = "DocumentService",

                }, ex);
                throw ex;
            }
        }

        public GetDocumentResponse Get(GetDocumentRequest request)
        {
            var response = new GetDocumentResponse();
            try
            {

                var doc = _docrepository.FindBy(request.Id);

                if (doc != null)
                {
                    DocumentLang defaultTrad = null;
                    if (request.IdLang != request.DefaulltIdLang)
                    {
                        defaultTrad = _doclangrepository.FindBy(new Specification<DocumentLang>(dt => dt.ID_Lang == request.DefaulltIdLang && dt.ID_Document == request.Id)).FirstOrDefault();
                    }
                    response = new GetDocumentResponse()
                    {
                        DocumentTitle = _doclangrepository.FindBy(new Specification<DocumentLang>(dt => dt.ID_Document == request.Id && !string.IsNullOrEmpty(dt.Name))).FirstOrDefault() == null ?
                                string.Empty :
                                _doclangrepository.FindBy(new Specification<DocumentLang>(dt => dt.ID_Document == request.Id && !string.IsNullOrEmpty(dt.Name))).FirstOrDefault().Name,
                        Document = doc,
                        DocumentTrad = _doclangrepository.FindBy(new Specification<DocumentLang>(dt => dt.ID_Lang == request.IdLang && dt.ID_Document == request.Id)).FirstOrDefault(),
                        DefaultDocumentTrad = defaultTrad,
                        Categories = _doccategoryrepository.FindAll()
                            .Select(cat => {
                                var categ = _doccategorylangrepository.FindBy(new Specification<DocumentCategoryLang>(lng => lng.ID_DocumentCategory == cat.ID && lng.ID_Lang == request.IdLang)).FirstOrDefault();
                                var defaultCateg = _doccategorylangrepository.FindBy(new Specification<DocumentCategoryLang>(lng => lng.ID_DocumentCategory == cat.ID && lng.ID_Lang != request.IdLang)).FirstOrDefault();
                                return new DocumentCategoryDTO()
                                {
                                    ID = cat.ID,
                                    Name = categ == null ? (defaultCateg != null ? defaultCateg.Name : string.Empty) : categ.Name,
                                    IsDefaultLangName = categ == null,
                                    ID_Lang = request.IdLang,
                                    OrdreCategory = cat.OrdreCategory
                                };
                            }).OrderBy(c => c.OrdreCategory).ToList(),
                        SubCategories = _subcategrepository.FindAll()
                            .Select(scat => {
                                var ordreCategory = _doccategoryrepository.FindBy(scat.ID_DocumentCategory).OrdreCategory.Value;
                                var categ = _doccategorylangrepository.FindBy(new Specification<DocumentCategoryLang>(lng => lng.ID_DocumentCategory == scat.ID_DocumentCategory && lng.ID_Lang == request.IdLang)).FirstOrDefault();
                                var defaultCateg = _doccategorylangrepository.FindBy(new Specification<DocumentCategoryLang>(lng => lng.ID_DocumentCategory == scat.ID_DocumentCategory && lng.ID_Lang != request.IdLang)).FirstOrDefault();
                                return new SubCategoryDTO()
                                {
                                    ID = scat.ID,
                                    ID_Category = scat.ID_DocumentCategory,
                                    CategoryName = categ == null ? defaultCateg.Name : categ.Name,
                                    OrdreCategory = ordreCategory,
                                    SubCategoryTrad = _subcateglangrepository.FindBy(new Specification<SubCategoryLang>(scl => scl.ID_SubCategory == scat.ID && scl.ID_Lang == request.IdLang)).FirstOrDefault(),
                                    DefaultTrad = _subcateglangrepository.FindBy(new Specification<SubCategoryLang>(scl => scl.ID_SubCategory == scat.ID && scl.ID_Lang != request.IdLang)).FirstOrDefault(),
                                };
                            }).OrderBy(sc => sc.OrdreCategory).ToList(),
                        Versions = _docversionrepository.FindBy(new Specification<DocumentVersion>(dv => dv.ID_Document == doc.ID)).OrderBy("version DESC")
                            .Select(ver => {
                                var version = _docversionlangrepository.FindBy(new Specification<DocumentVersionLang>(lng => lng.ID_DocumentVersion == ver.ID && lng.ID_Lang == request.IdLang)).FirstOrDefault();
                                return new DocumentVersionDTO()
                                {
                                    ID = ver.ID,
                                    ID_Lang = version == null ? 0 : version.ID_Lang,
                                    ID_UserCre = ver.ID_UserCre,
                                    IsMajor = ver.IsMajor,
                                    DateCre = ver.DateCre.ToString(),
                                    ContentType = ver.ContentType,
                                    Extension = ver.Extension,
                                    Version = ver.Version,
                                    Name = version == null ? string.Empty : version.Name,
                                    NomOrigineFichier = version == null ? string.Empty : version.NomOrigineFichier,
                                    Data = version == null ? null : version.Data,
                                    ID_Document = request.Id
                                };
                            })
                            .ToList(),
                    };

                }
                else
                {
                    response = new GetDocumentResponse()
                    {
                        Document = new Document()
                        {
                            isMajor = true,
                            Approbation = null,
                            Test = null,
                            IsNoActionRequired = true,
                            TypeAffectation = string.Empty,
                            Version = GetNextVersion(0, true),
                            Inactif = false,
                            PhaseEmployee = true,
                            PhaseOnboarding = true,
                            ReadBrowser = true,
                            ReadDownload = true,
                            isMetier = true,
                            isStructure = true

                        },
                        DocumentTrad = new DocumentLang()
                        {
                            ID_Lang = request.IdLang,
                            Name = string.Empty,
                            NomOrigineFichier = string.Empty,
                        },
                        Categories = _doccategoryrepository.FindAll()
                            .Select(cat => {
                                var categ = _doccategorylangrepository.FindBy(new Specification<DocumentCategoryLang>(lng => lng.ID_DocumentCategory == cat.ID && lng.ID_Lang == request.IdLang)).FirstOrDefault();
                                var defaultCateg = _doccategorylangrepository.FindBy(new Specification<DocumentCategoryLang>(lng => lng.ID_DocumentCategory == cat.ID && lng.ID_Lang != request.IdLang)).FirstOrDefault();
                                return new DocumentCategoryDTO()
                                {
                                    ID = cat.ID,
                                    Name = categ == null ? (defaultCateg != null ? defaultCateg.Name : string.Empty) : categ.Name,
                                    ID_Lang = categ == null ? request.IdLang : categ.ID_Lang,
                                    OrdreCategory = cat.OrdreCategory
                                };
                            })
                            .OrderBy(cat => cat.OrdreCategory)
                            .ToList(),

                        SubCategories = _subcategrepository.FindAll()
                            .Select(scat => {
                                var ordrecategory = _doccategoryrepository.FindBy(scat.ID_DocumentCategory).OrdreCategory.Value;
                                var categ = _doccategorylangrepository.FindBy(new Specification<DocumentCategoryLang>(lng => lng.ID_DocumentCategory == scat.ID_DocumentCategory && lng.ID_Lang == request.IdLang)).FirstOrDefault();
                                var defaultCateg = _doccategorylangrepository.FindBy(new Specification<DocumentCategoryLang>(lng => lng.ID_DocumentCategory == scat.ID_DocumentCategory && lng.ID_Lang != request.IdLang)).FirstOrDefault();
                                return new SubCategoryDTO()
                                {
                                    ID = scat.ID,
                                    ID_Category = scat.ID_DocumentCategory,
                                    CategoryName = categ == null ? defaultCateg.Name : categ.Name,
                                    OrdreCategory = ordrecategory,
                                    SubCategoryTrad = _subcateglangrepository.FindBy(new Specification<SubCategoryLang>(scl => scl.ID_SubCategory == scat.ID && scl.ID_Lang == request.IdLang)).FirstOrDefault(),
                                    DefaultTrad = _subcateglangrepository.FindBy(new Specification<SubCategoryLang>(scl => scl.ID_SubCategory == scat.ID && scl.ID_Lang != request.IdLang)).FirstOrDefault(),
                                };
                            }).OrderBy(sc => sc.OrdreCategory).ToList(),
                        Versions = new List<DocumentVersionDTO>(),
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
                    ServiceName = "DocumentService",

                }, ex);
                throw ex;
            }
        }

        public override string GetOperator(Type type, ColumnFilter columnFilter, int index, string filterValue)
        {
            if ("ID_Category".Equals(columnFilter.ColumnName) && "-1".Equals(filterValue))
            {
                return "(ID_Category== null OR ID_Category==0)";
            }
            else if ("ID_SubCategory".Equals(columnFilter.ColumnName) && "-1".Equals(filterValue))
            {
                return "(ID_SubCategory== null OR ID_SubCategory==0)";
            }
            else if ("Inactif".Equals(columnFilter.ColumnName) && "false".Equals(filterValue))
            {
                return "(Inactif== null OR Inactif==false)";
            }
            else if ("Name".Equals(columnFilter.ColumnName) && !string.IsNullOrEmpty(filterValue))
            {
                columnFilter.Field = "EdmxFunction.RemoveAccent(Name)";
                columnFilter.FilterValue = Utils.RemoveAccent(filterValue);
            }
            return columnFilter.GetOperator(type, index, filterValue);
        }

        public IQueryable<DocumentDTO> GetAllDocumentAsQueryable(GetAllDocumentRequest allrequest)
        {
            var idLang = allrequest.IdLang;
            var idDefaultLang = allrequest.IdDefaultLang;
            var request = allrequest.GridRequest;
            IQueryable<DocumentDTO> query = this._docrepository.RepositoryTable.GroupJoin(
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
                       NameSubCategory = EdmxFunction.GetSubCategoryLang(parent.doc.ID_SubCategory.Value, idLang, idLang),
                       DefaultNameSubCategory = EdmxFunction.GetSubCategoryLang(parent.doc.ID_SubCategory.Value, idDefaultLang, idDefaultLang),
                       IsDefaultLangName = string.IsNullOrEmpty(parent.lang.Name),
                       NomOrigineFichier = parent.lang.NomOrigineFichier,
                       Version = parent.doc.Version,
                       Date = parent.doc.Date,
                       Commentaire = parent.doc.Commentaire,
                       Approbation = parent.doc.Approbation,
                       Test = parent.doc.Test,
                       TypeAffectation = parent.doc.TypeAffectation,
                       Inactif = parent.doc.Inactif,
                       Extension = parent.doc.Extension,
                       IsMetier = parent.doc.isMetier,
                       IsStructure = parent.doc.isStructure,
                       ID_Category = parent.doc.ID_Category.HasValue ? parent.doc.ID_Category : 0,
                       ID_SubCategory = parent.doc.ID_SubCategory,
                       IsNoActionRequired = parent.doc.IsNoActionRequired,
                       DocumentCategoryLang = child.DocumentCategoryLang.FirstOrDefault(f => f.ID_Lang == idLang),
                       DefaultNameCategory = EdmxFunction.GetCategoryLang(parent.doc.ID_Category.Value, idDefaultLang, idDefaultLang),
                       DateUpd = parent.doc.DateUpd,
                       DateCre = parent.doc.DateCre,
                       OrdreCategory = child.OrdreCategory != null ? child.OrdreCategory : 1000,
                       OrdreSubCategory = parent.sub.Ordre != null ? parent.sub.Ordre : 1000
                   });
            if (request != null)
            {
                Trace.WriteLine($"Search: {request.Search}");
                if (!string.IsNullOrEmpty(request.Search))
                {
                    var search = Utils.RemoveAccent(request.Search);
                    query = query.Where(x => (
                        EdmxFunction.RemoveAccent(x.Name).Contains(search) ||
                        EdmxFunction.RemoveAccent(x.Version).Contains(search) ||
                        EdmxFunction.RemoveAccent(x.Commentaire).Contains(search)
                   ));
                }
                if (!string.IsNullOrEmpty(request.Categories))
                {
                    var categs = request.Categories.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Int32.Parse(x)).ToList();
                    query = query.Where(x => !categs.Contains(x.ID_Category.Value));
                }
                query = this.FiltrerQuery(request.Filtres, query);
            }
            return query;
        }

        public int GetAllCount(GetAllDocumentRequest request)
        {
            return GetAllDocumentAsQueryable(request).Count();

        }

        public GetAllDocumentResponse GetAll(GetAllDocumentRequest allrequest)
        {
            string orderBy = "";
            var request = allrequest.GridRequest;
            if (string.IsNullOrEmpty(request.OrderColumn))
            {
                orderBy = "OrdreCategory, OrdreSubCategory, DateUpd DESC, DateCre DESC";
            }
            else
            {
                orderBy = "OrdreCategory, OrdreSubCategory, " + request.OrderColumn + request.SortAscDesc;
            }
            var response = new GetAllDocumentResponse();
            try
            {
                IQueryable<DocumentDTO> query = GetAllDocumentAsQueryable(allrequest).OrderBy(orderBy).Skip((request.Page - 1) * request.Limit).Take(request.Limit);
                var list = query.ToList();
                response.Documents = list;
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "GetAll",
                    ServiceName = "DocumentService",

                }, ex);
                throw ex;
            }
        }

        public UpdateDocumentResponse Update(UpdateDocumentRequest request)
        {
            var response = new UpdateDocumentResponse();
            try
            {



                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "Update",
                    ServiceName = "DocumentService",

                }, ex);
                throw ex;
            }
        }

        public SaveDocumentResponse Save(SaveDocumentRequest request)
        {

            var ActionsBO = new List<ActionBO>();
            var response = new SaveDocumentResponse();
            try
            {
                var docToSave = _docrepository.FindBy(request.Document.ID);

                if (docToSave == null)
                {
                    docToSave = new Document()
                    {

                        Date = DateTime.Now,
                        Approbation = request.Document.Approbation,
                        Test = request.Document.Test,
                        Commentaire = request.Document.Commentaire,
                        ContentType = request.Document.ContentType,
                        Extension = request.Document.Extension,
                        Inactif = false,
                        PhaseEmployee = request.Document.PhaseEmployee,
                        PhaseOnboarding = request.Document.PhaseOnboarding,
                        ReadBrowser = request.Document.ReadBrowser,
                        ReadDownload = request.Document.ReadDownload,
                        TypeAffectation = request.Document.TypeAffectation,
                        IdQcm = request.Document.IdQcm,
                        ID_Category = request.Document.ID_Category,
                        ID_SubCategory = request.Document.ID_SubCategory,
                        isMajor = request.Document.isMajor,
                        isMetier = request.Document.isMetier,
                        isStructure = request.Document.isStructure,
                        IsNoActionRequired = request.Document.IsNoActionRequired,
                        ID_UserCre = request.Document.ID_UserCre,
                        DateCre = DateTime.Now,
                        isMagazine = request.Document.isMagazine
                    };
                }
                else
                {
                   
                    docToSave.ID = request.Document.ID;
                    docToSave.Id = request.Document.ID;

                    docToSave.Version = request.DocumentTrad.Data.Length > 0 ? GetNextVersion(request.Document.ID, request.Document.isMajor) : docToSave.Version;
                    docToSave.Date = DateTime.Now;
                    docToSave.Approbation = request.Document.Approbation;
                    docToSave.Test = request.Document.Test;
                    docToSave.Commentaire = request.Document.Commentaire;
                    docToSave.ContentType = request.DocumentTrad.Data.Length > 0 ? request.Document.ContentType : docToSave.ContentType;
                    docToSave.Extension = request.DocumentTrad.Data.Length > 0 ? request.Document.Extension : docToSave.Extension;
                    docToSave.Inactif = request.Document.Inactif;

                    docToSave.PhaseEmployee = request.Document.PhaseEmployee;
                    docToSave.PhaseOnboarding = request.Document.PhaseOnboarding;
                    docToSave.ReadBrowser = request.Document.ReadBrowser;
                    docToSave.ReadDownload = request.Document.ReadDownload;

                    docToSave.TypeAffectation = request.Document.TypeAffectation;
                    //docToSave.IdQcm = request.Document.IdQcm;
                    docToSave.ID_Category = request.Document.ID_Category;
                    docToSave.ID_SubCategory = request.Document.ID_SubCategory;
                    docToSave.isMajor = request.Document.isMajor;
                    docToSave.isMetier = request.Document.isMetier;
                    docToSave.isStructure = request.Document.isStructure;
                    docToSave.IsNoActionRequired = request.Document.IsNoActionRequired;
                    docToSave.ID_UserUpd = request.Document.ID_UserCre;
                    docToSave.DateUpd = DateTime.Now;
                    docToSave.isMagazine = request.Document.isMagazine;
                    if (request.Document.Inactif.Value)
                    {
                        docToSave.ID_UserDel = request.Document.ID_UserCre;
                        docToSave.DateDel = DateTime.Now;
                        ActionsBO.Add(new ActionBO()
                        {
                            Action = Actions.Delete,
                            ID_Object = docToSave.ID,
                            ID_User = request.Document.ID_UserCre.HasValue ? request.Document.ID_UserCre.Value : 0,
                            ObjectCode = ObjectCode.DOCUMENT,
                            LinkObjects = string.Empty,
                            DateAction = DateTime.Now

                        });
                    }
                    else
                    {
                        docToSave.ID_UserDel = null;
                        docToSave.DateDel = null;

                        bool reinitializQuizz = docToSave.IdQcm != request.Document.IdQcm || request.Document.isMajor;

                        if (reinitializQuizz)
                        {
                            var idDocument = new SqlParameter("@ID_Document", docToSave.ID);
                            var isMajor = new SqlParameter("@isMajor", request.Document.isMajor ? 1 : 0);

                            var sql = "EXEC p_ReinitUserDocQcm @ID_Document, @isMajor";
                            _docrepository.Database.ExecuteSqlCommand(sql, idDocument, isMajor);

                        }
                    }
                    docToSave.IdQcm = request.Document.IdQcm;
                    docToSave.isMajor = request.Document.isMajor;
                }

                if (request.DocumentTrad.Data.Length > 0)
                {
                    docToSave.Version = request.Document.Version;
                    if (IsDocLangVersionned(request.Document.ID, request.DocumentTrad.ID_Lang, request.Document.Version))
                    {
                        docToSave.Version = GetNextVersion(request.Document.ID, request.Document.isMajor);
                    }
                    if (!_docversionrepository.FindBy(new Specification<DocumentVersion>(dv => dv.Version == docToSave.Version && dv.ID_Document == request.Document.ID)).Any())
                    {
                        var docVersion = new DocumentVersion()
                        {
                            IsMajor = request.Document.isMajor,
                            DateCre = DateTime.Now,
                            ContentType = request.Document.ContentType,
                            Extension = request.Document.Extension,
                            ID_UserCre = request.Document.ID_UserCre.Value,
                            Version = docToSave.Version
                        };

                        var doclangVersionToAdd = new DocumentVersionLang()
                        {
                            Data = request.Document.Extension == ".pdf" ? request.DocumentTrad.Data : null,
                            Name = request.DocumentTrad.Name,
                            NomOrigineFichier = request.DocumentTrad.NomOrigineFichier,
                            ID_Lang = request.DocumentTrad.ID_Lang,

                        };


                        docVersion.DocumentVersionLang.Add(doclangVersionToAdd);

                        docToSave.DocumentVersion.Add(docVersion);
                    }
                    else
                    {
                        var docversion = _docversionrepository.FindBy(new Specification<DocumentVersion>(dv => dv.Version == docToSave.Version && dv.ID_Document == request.Document.ID)).FirstOrDefault();
                        var doclangVersionToAdd = new DocumentVersionLang()
                        {
                            Data = request.Document.Extension == ".pdf" ? request.DocumentTrad.Data : null,
                            Name = request.DocumentTrad.Name,
                            NomOrigineFichier = request.DocumentTrad.NomOrigineFichier,
                            ID_Lang = request.DocumentTrad.ID_Lang,
                            ID_DocumentVersion = _docversionrepository.FindBy(new Specification<DocumentVersion>(dv => dv.Version == docToSave.Version && dv.ID_Document == request.Document.ID)).FirstOrDefault().ID
                        };

                        docversion.DocumentVersionLang.Add(doclangVersionToAdd);
                    }


                    ActionsBO.Add(new ActionBO()
                    {
                        Action = Actions.CreateDocVersion,
                        ID_Object = docToSave.ID,
                        ID_User = request.Document.ID_UserCre.HasValue ? request.Document.ID_UserCre.Value : 0,
                        ObjectCode = ObjectCode.DOCUMENT,
                        LinkObjects = docToSave.Version,
                        DateAction = DateTime.Now
                    });

                }

                _docrepository.Save(docToSave);
                response.Id = docToSave.ID;
                response.Version = docToSave.Version;
                var _docTrad = _doclangrepository.FindBy(new Specification<DocumentLang>(dl => dl.ID_Document == request.Document.ID && dl.ID_Lang == request.DocumentTrad.ID_Lang)).FirstOrDefault();
                if (_docTrad != null)
                {
                    _docTrad.Name = request.DocumentTrad.Name;
                    _docTrad.NomOrigineFichier = request.DocumentTrad.NomOrigineFichier;
                    if (request.DocumentTrad.Data.Length > 0)
                        _docTrad.Data = request.Document.Extension == ".pdf" ? request.DocumentTrad.Data : null;
                    _doclangrepository.Update(_docTrad);
                }
                else
                {
                    _doclangrepository.Add(new DocumentLang()
                    {
                        ID_Document = request.Document.ID == 0 ? docToSave.ID : request.Document.ID,
                        ID_Lang = request.DocumentTrad.ID_Lang,
                        Name = request.DocumentTrad.Name,
                        NomOrigineFichier = request.DocumentTrad.NomOrigineFichier,
                        Data = request.Document.Extension == ".pdf" ? request.DocumentTrad.Data : null
                    });
                }



                // Action BO Create/Update Document
                ActionsBO.Add(new ActionBO()
                {
                    Action = request.Document.ID == 0 ? Actions.Create : Actions.Update,
                    ID_Object = docToSave.ID,
                    ID_User = request.Document.ID_UserCre.HasValue ? request.Document.ID_UserCre.Value : 0, //docToSave.ID_UserCre.HasValue ? docToSave.ID_UserCre.Value: ,
                    ObjectCode = ObjectCode.DOCUMENT,
                    LinkObjects = string.Empty,
                    DateAction = DateTime.Now

                });

                var allDocEntities = _entitydocrepository.FindBy(new Specification<EntityDocument>(ed => ed.ID_Document == request.Document.ID));

                if (allDocEntities.Any())
                {
                    var deleteEntityDoc = string.Join("-", allDocEntities.Select(de => string.Format("{0} {1}", de.EntityName, de.AgencyName == null ? string.Empty : de.AgencyName)).ToList());
                    _entitydocrepository.RemoveAll(allDocEntities);

                    // ActionBO Delete/Update Entity Document affectation
                    ActionsBO.Add(new ActionBO()
                    {
                        Action = Actions.Delete,
                        ID_Object = docToSave.ID,
                        ID_User = request.Document.ID_UserCre.HasValue ? request.Document.ID_UserCre.Value : 0,
                        ObjectCode = ObjectCode.ENTITY_DOCUMENT,
                        LinkObjects = deleteEntityDoc,
                        DateAction = DateTime.Now
                    });
                }


                var allDocProfiles = _profiledocrepository.FindAll().Where(pd => pd.ID_Document == request.Document.ID);
                var profileDocNames = new List<string>();
                if (allDocProfiles.Any())
                {

                    var deletedProfileDocId = allDocProfiles.Select(dp => dp.ID_Profile).ToList();

                    _profiledocrepository.RemoveAll(allDocProfiles);

                    foreach (var profileId in deletedProfileDocId)
                    {
                        var profileName = _profilerepository.FindBy(profileId.Value).Name;
                        profileDocNames.Add(profileName);

                    }
                    // ActionBO Delete/Update Profile Document affectation
                    ActionsBO.Add(new ActionBO()
                    {
                        Action = Actions.Delete,
                        ID_Object = docToSave.ID,
                        ID_User = request.Document.ID_UserCre.HasValue ? request.Document.ID_UserCre.Value : 0,
                        ObjectCode = ObjectCode.PROFILE_DOCUMENT,
                        LinkObjects = string.Join("-", profileDocNames),
                        DateAction = DateTime.Now
                    });
                }

                if (!string.IsNullOrEmpty(request.Affectation))
                {
                    if (request.Document.TypeAffectation == DocumentTypes.ENTITEAGENCE)
                    {
                        var affectations = request.Affectation.Split(',');

                        foreach (var aff in affectations)
                        {
                            var entiteAgence = aff.Split('|');
                            _entitydocrepository.Add(new EntityDocument()
                            {
                                EntityName = entiteAgence[0],
                                AgencyName = entiteAgence[1] == "NULL" ? null : entiteAgence[1],
                                EntityDocDate = DateTime.Now,
                                ID_Document = docToSave.ID
                            });
                        }

                        allDocEntities = _entitydocrepository.FindBy(new Specification<EntityDocument>(ed => ed.ID_Document == docToSave.ID));
                    }
                    else
                    {

                        var profiles = request.Affectation.Split(',');
                        foreach (var profile in profiles)
                        {
                            var id_profile = 0;
                            if (int.TryParse(profile, out id_profile))
                            {
                                var profileDoc = _profiledocrepository.FindAll().Where(pd => pd.ID_Document == request.Document.ID && pd.ID_Profile == id_profile).FirstOrDefault();

                                _profiledocrepository.Add(new ProfileDocument()
                                {
                                    ID_Profile = id_profile,
                                    ID_Document = docToSave.ID,
                                    Date = DateTime.Now,
                                });
                            }

                        }

                        allDocProfiles = _profiledocrepository.FindAll().Where(pd => pd.ID_Document == docToSave.ID);

                        var deletedProfileDocId = allDocProfiles.Select(dp => dp.ID_Profile).ToList();
                        profileDocNames = new List<string>();
                        foreach (var profileId in deletedProfileDocId)
                        {
                            var profileName = _profilerepository.FindBy(profileId.Value).Name;
                            profileDocNames.Add(profileName);

                        }
                    }



                    // ActionBO Create Entity / Profile Document affectation
                    ActionsBO.Add(new ActionBO()
                    {
                        Action = Actions.Create,
                        ID_Object = docToSave.ID,
                        ID_User = request.Document.ID_UserCre.HasValue ? request.Document.ID_UserCre.Value : 0,
                        ObjectCode = request.Document.TypeAffectation == DocumentTypes.ENTITEAGENCE ? ObjectCode.ENTITY_DOCUMENT : ObjectCode.PROFILE_DOCUMENT,
                        LinkObjects = request.Document.TypeAffectation == DocumentTypes.ENTITEAGENCE ?
                             string.Join("-", allDocEntities.Select(de => string.Format("{0} {1}", de.EntityName, de.AgencyName == null ? string.Empty : de.AgencyName)).ToList()) :
                             string.Join("-", profileDocNames),
                        DateAction = DateTime.Now
                    });
                }



                _histoActionBO = new HistoActionBO(base.uow, ActionsBO);
                _histoActionBO.SaveHisto();
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "Save",
                    ServiceName = "DocumentService",

                }, ex);
                throw ex;
            }
        }
        public GetAllDocumentNameResponse GetAllName(GetAllDocumentNameRequest request)
        {

            var response = new GetAllDocumentNameResponse();
            try
            {
                var docs = request.ExactMatch ? _doclangrepository.FindBy(new Specification<DocumentLang>(doc => doc.Name == request.Search && doc.ID_Lang == request.IdLang))
                    : _doclangrepository.FindBy(new Specification<DocumentLang>(doc => doc.Name.Contains(request.Search) && doc.ID_Lang == request.IdLang));

                response = new GetAllDocumentNameResponse()
                {
                    Names = docs.OrderBy(d => d.Name).Select(doc => new NameDTO()
                    {
                        ID = doc.ID_Document,
                        Name = doc.Name
                    }).ToList()
                };

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "GetAll",
                    ServiceName = "DocumentService",

                }, ex);
                throw ex;
            }
        }

        public CheckDocumentNameResponse CheckName(CheckDocumentNameRequest request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                return new CheckDocumentNameResponse()
                {
                    Exist = false,
                    Document = null
                };
            }

            try
            {
                return new CheckDocumentNameResponse()
                {
                    Exist = false,
                    Document = new DocumentDTO()
                };
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "CheckName",
                    ServiceName = "DocumentService",

                }, ex);
                throw ex;
            }
        }

        public GetProfileDocumentResponse GetProfileDocument(GetProfileDocumentRequest request)
        {
            try
            {
                var profiles = _profiledocrepository.FindBy(new Specification<ProfileDocument>(d => d.ID_Document == request.DocumentID));
                return new GetProfileDocumentResponse()
                {
                    Affectation = new DocumentAffectationDTO()
                    {
                        AffectationType = DocumentTypes.ENTITEAGENCE,
                        AffectedTo = profiles.Any() ? profiles.Select(e => new
                        {
                            ProfileId = e.ID_Profile,
                            DocumentId = e.ID_Document,
                            ProfileName = e.Profile.Name, //_profilerepository.FindBy(e.ID_Profile.Value).Name
                        }).Select(e => string.Format("{0}", e.ProfileId)).ToList() : new List<string>()
                    }

                };

            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "GetProfileDocument",
                    ServiceName = "DocumentService",

                }, ex);
                throw ex;
            }
        }

        public GetProfileDocumentResponse GetProfileIdDocument(GetProfileDocumentRequest request)
        {
            try
            {
                var profiles = _profiledocrepository.FindBy(new Specification<ProfileDocument>(d => d.ID_Document == request.DocumentID));
                return new GetProfileDocumentResponse()
                {
                    Affectation = new DocumentAffectationDTO()
                    {
                        AffectationType = DocumentTypes.ENTITEAGENCE,
                        AffectedTo = profiles.Any() ? profiles.Select(e => new
                        {
                            ProfileId = e.ID_Profile,
                            DocumentId = e.ID_Document,
                            ProfileName = e.Profile.Name, //_profilerepository.FindBy(e.ID_Profile.Value).Name
                        }).Select(e => string.Format("{0}", e.ProfileId)).ToList() : new List<string>()
                    }

                };

            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "GetProfileDocument",
                    ServiceName = "DocumentService",

                }, ex);
                throw ex;
            }
        }

        public GetEntityDocumentResponse GetEntityDocument(GetEntityDocumentRequest request)
        {
            try
            {
                var entities = _entitydocrepository.FindBy(new Specification<EntityDocument>(d => d.ID_Document == request.DocumentID));
                return new GetEntityDocumentResponse()
                {
                    Affectation = new DocumentAffectationDTO()
                    {
                        AffectationType = DocumentTypes.ENTITEAGENCE,
                        AffectedTo = entities.Any() ? entities.Select(e => !string.IsNullOrEmpty(e.AgencyName) ? string.Format("{0}|{1}", e.EntityName, e.AgencyName) : string.Format("{0}|NULL", e.EntityName)).ToList() : new List<string>()
                    }

                };

            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "GetEntityDocument",
                    ServiceName = "DocumentService",

                }, ex);
                throw ex;
            }
        }

        public SaveCategoryResponse SaveCategory(SaveCategoryRequest request)
        {
            var response = new SaveCategoryResponse();
            try
            {
                var catToSave = new DocumentCategory();
                var catTrad = new DocumentCategoryLang()
                {
                    ID_DocumentCategory = request.Category.ID,
                    ID_Lang = request.CategoryTrad.ID_Lang,
                    Name = request.CategoryTrad.Name,
                };
                catToSave.ID = request.Category.ID;
                catToSave.Id = request.Category.ID;
                catToSave.OrdreCategory = request.Category.OrdreCategory;

                catToSave.DocumentCategoryLang.Add(catTrad);

                if (request.Category.ID == 0)
                {
                    _doccategoryrepository.Save(catToSave);
                }
                else
                {
                    _doccategoryrepository.Update(catToSave);
                }


                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "SaveCategory",
                    ServiceName = "DocumentService",

                }, ex);
                throw ex;
            }
        }
        public bool IsCategoryNameExist(SaveCategoryRequest request)
        {
            return false;
        }
        public GetCategoryResponse GetCategory(GetCategoryRequest request)
        {
            var response = new GetCategoryResponse();
            try
            {
                var categ = _doccategoryrepository.FindBy(request.Id);
                if (categ != null)
                {
                    response = new GetCategoryResponse()
                    {
                        Category = categ,
                        CategoryTrad = _doccategorylangrepository.FindBy(new Specification<DocumentCategoryLang>(dcl => dcl.ID_Lang == request.IdLang && dcl.ID_DocumentCategory == request.Id)).FirstOrDefault(),
                        DefaultTrad = _doccategorylangrepository.FindBy(new Specification<DocumentCategoryLang>(dcl => dcl.ID_Lang != request.IdLang && dcl.ID_DocumentCategory == request.Id)).FirstOrDefault(),
                        IsDeleted = !this.IsAttachedDocument(categ.ID)
                    };

                }
                else
                {
                    response = new GetCategoryResponse()
                    {
                        Category = new DocumentCategory()
                        {
                            //Name = string.Empty,
                            OrdreCategory = (_doccategoryrepository.Count() + 1) * 10
                        },

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
                    MethodName = "GetCategory",
                    ServiceName = "DocumentService",

                }, ex);
                throw ex;
            }
        }

        public DeleteCategoryResponse DeleteCategory(DeleteCategoryRequest request)
        {
            var response = new DeleteCategoryResponse();
            try
            {
                var categToDelete = _doccategoryrepository.FindBy(request.Id);
                var catelangToDelete = _doccategorylangrepository.FindBy(new Specification<DocumentCategoryLang>(dcl => dcl.ID_DocumentCategory == request.Id));
                if (categToDelete != null)
                {
                    if (catelangToDelete.Any())
                    {
                        _doccategorylangrepository.RemoveAll(catelangToDelete);
                    }
                    _doccategoryrepository.Remove(categToDelete);
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "DeleteCategory",
                    ServiceName = "DocumentService",

                }, ex);
                throw ex;
            }
        }

        public GetDocumentVersionsResponse GetVersions(GetDocumentVersionsRequest request)
        {
            var response = new GetDocumentVersionsResponse();
            try
            {

                var version = _docversionrepository.FindBy(request.Id);
                if (version != null)
                {
                    var versionlang = _docversionlangrepository.FindBy(new Specification<DocumentVersionLang>(dvl => dvl.ID_DocumentVersion == version.ID && dvl.ID_Lang == request.IdLang)).FirstOrDefault();
                    response = new GetDocumentVersionsResponse()
                    {
                        Version = new DocumentVersionDTO()
                        {
                            ID = version.Id,
                            ID_Document = version.ID_Document,
                            ID_UserCre = version.ID_UserCre,
                            UserName = string.Empty,
                            IsMajor = version.IsMajor,
                            DateCre = version.DateCre.ToString(),
                            ContentType = version.Extension,
                            Extension = version.Extension,
                            Data = versionlang == null ? null : versionlang.Data,
                            Name = versionlang == null ? string.Empty : versionlang.Name,
                            NomOrigineFichier = versionlang == null ? string.Empty : versionlang.NomOrigineFichier,
                            Version = version.Version,
                        }
                    };

                }
                else
                {
                    response = new GetDocumentVersionsResponse()
                    {
                        Version = new DocumentVersionDTO()


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
                    MethodName = "GetVersion",
                    ServiceName = "DocumentService",

                }, ex);
                throw ex;
            }
        }

        public GetDocumentVersionResponse GetDocumentVersion(GetDocumentVersionRequest request)
        {
            return new GetDocumentVersionResponse()
            {
                NewVersion = this.GetNextVersion(request.Id, request.IsMajorVersion),

            };
        }

        public ExistDocumentVersionLangResponse ExistDocumentVersionLang(ExistDocumentVersionLangRequest request)
        {
            bool isLangVersionned = false;
            var versionTrad = new DocumentVersionLang();
            if (_docversionrepository.FindBy(new Specification<DocumentVersion>(dv => dv.ID_Document == request.ID_Document && dv.Version == request.Version)).Any())
            {
                var docversion = _docversionrepository.FindBy(new Specification<DocumentVersion>(dv => dv.ID_Document == request.ID_Document && dv.Version == request.Version)).FirstOrDefault();
                isLangVersionned = _docversionlangrepository.FindBy(new Specification<DocumentVersionLang>(dvl => dvl.ID_DocumentVersion == docversion.ID && dvl.ID_Lang == request.ID_Lang)).Any();
                versionTrad = _docversionlangrepository.FindBy(new Specification<DocumentVersionLang>(dvl => dvl.ID_DocumentVersion == docversion.ID && dvl.ID_Lang == request.ID_Lang)).FirstOrDefault();
            }

            return new ExistDocumentVersionLangResponse()
            {
                Result = isLangVersionned,
                VersionTrad = null
            };
        }

        private bool IsDocLangVersionned(int docId, int langId, string version)
        {
            bool isLangVersionned = false;
            var versionTrad = new DocumentVersionLang();
            if (_docversionrepository.FindBy(new Specification<DocumentVersion>(dv => dv.ID_Document == docId && dv.Version == version)).Any())
            {
                var docversion = _docversionrepository.FindBy(new Specification<DocumentVersion>(dv => dv.ID_Document == docId && dv.Version == version)).Select(v => v.ID).ToList();
                isLangVersionned = _docversionlangrepository.FindBy(new Specification<DocumentVersionLang>(dvl => docversion.Contains(dvl.ID_DocumentVersion) && dvl.ID_Lang == langId)).Any();
                versionTrad = _docversionlangrepository.FindBy(new Specification<DocumentVersionLang>(dvl => docversion.Contains(dvl.ID_DocumentVersion) && dvl.ID_Lang == langId)).FirstOrDefault();
            }

            return isLangVersionned;
        }

        public GetAllSubCategoryResponse GetAllSubcategory(GetAllSubCategoryRequest request)
        {
            try
            {
                var query = this._subcategrepository.FindAll()
                 .Select(sc =>
                 {
                     var categoryTrad = _doccategorylangrepository.FindBy(new Specification<DocumentCategoryLang>(dc => dc.ID_DocumentCategory == sc.ID_DocumentCategory && dc.ID_Lang == request.IdLang)).FirstOrDefault();
                     var defaultCategoryTrad = _doccategorylangrepository.FindBy(new Specification<DocumentCategoryLang>(dc => dc.ID_DocumentCategory == sc.ID_DocumentCategory && dc.ID_Lang != request.IdLang)).FirstOrDefault();
                     return new SubCategoryDTO()
                     {
                         ID = sc.ID,
                         ID_Category = sc.ID_DocumentCategory,
                         OrdreCategory = _doccategoryrepository.FindBy(sc.ID_DocumentCategory).OrdreCategory.Value,
                         CategoryName = categoryTrad == null ? defaultCategoryTrad.Name : categoryTrad.Name,
                         SubCategoryTrad = _subcateglangrepository.FindBy(new Specification<SubCategoryLang>(scl => scl.ID_SubCategory == sc.ID && scl.ID_Lang == request.IdLang)).FirstOrDefault(),
                         DefaultTrad = _subcateglangrepository.FindBy(new Specification<SubCategoryLang>(scl => scl.ID_SubCategory == sc.ID && scl.ID_Lang != request.IdLang)).FirstOrDefault(),
                     };
                 })
                 .OrderBy(sc => sc.OrdreCategory);

                return new GetAllSubCategoryResponse()
                {
                    SubCategories = query.ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "GetAllSubcategory",
                    ServiceName = "DocumentService",

                }, ex);
                throw ex;
            }
        }

        public GetSubCategoryResponse GetSubcategory(GetSubCategoryRequest request)
        {
            var response = new GetSubCategoryResponse();
            try
            {
                var subcateg = this._subcategrepository.FindBy(request.Id);

                if (subcateg != null)
                {
                    var categoryTrad = _doccategorylangrepository.FindBy(new Specification<DocumentCategoryLang>(dc => dc.ID_DocumentCategory == subcateg.ID_DocumentCategory && dc.ID_Lang == request.IdLang)).FirstOrDefault();
                    var defaultCategoryTrad = _doccategorylangrepository.FindBy(new Specification<DocumentCategoryLang>(dc => dc.ID_DocumentCategory == subcateg.ID_DocumentCategory && dc.ID_Lang != request.IdLang)).FirstOrDefault();
                    response = new GetSubCategoryResponse()
                    {
                        SubCategory = new SubCategoryDTO()
                        {
                            ID = subcateg.ID,
                            ID_Category = subcateg.ID_DocumentCategory,
                            CategoryName = categoryTrad == null ? defaultCategoryTrad.Name : categoryTrad.Name,
                            Ordre = subcateg.Ordre,
                            SubCategoryTrad = _subcateglangrepository.FindBy(new Specification<SubCategoryLang>(scl => scl.ID_SubCategory == request.Id && scl.ID_Lang == request.IdLang)).FirstOrDefault(),
                            DefaultTrad = _subcateglangrepository.FindBy(new Specification<SubCategoryLang>(scl => scl.ID_SubCategory == request.Id && scl.ID_Lang != request.IdLang)).FirstOrDefault(),
                        },

                    };

                }
                else
                {
                    response = new GetSubCategoryResponse()
                    {
                        SubCategory = new SubCategoryDTO()
                        {
                            ID = 0,
                            ID_Category = 0,
                            CategoryName = string.Empty,
                            Ordre = 1,
                        }
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
                    MethodName = "GetSubcategory",
                    ServiceName = "DocumentService",

                }, ex);
                throw ex;
            }
        }

        public SaveSubCategoryResponse SaveSubcategory(SaveSubCategoryRequest request)
        {
            var response = new SaveSubCategoryResponse();
            try
            {
                var subcatToSave = new SubCategory();
                var subcategTrad = new SubCategoryLang()
                {
                    Id = request.SubCategoryTrad.Id,
                    ID_SubCategory = request.SubCategoryTrad.ID_SubCategory,
                    ID_Lang = request.SubCategoryTrad.ID_Lang,
                    Name = request.SubCategoryTrad.Name
                };

                subcatToSave.ID_DocumentCategory = request.SubCategory.ID_DocumentCategory;

                subcatToSave.ID = request.SubCategory.ID;
                subcatToSave.Id = request.SubCategory.ID;
                subcatToSave.Ordre = request.SubCategory.Ordre;


                subcatToSave.SubCategoryLang.Add(subcategTrad);

                if (request.SubCategory.ID == 0)
                {
                    _subcategrepository.Save(subcatToSave);
                }
                else
                {
                    _subcategrepository.Update(subcatToSave);
                    if (request.IsCategoryChanged)
                    {
                        var docToReloc = _docrepository.FindBy(new Specification<Document>(doc => doc.ID_SubCategory == subcatToSave.ID));
                        foreach (var doc in docToReloc)
                        {
                            doc.Id = doc.ID;
                            doc.ID_Category = request.SubCategory.ID_DocumentCategory;
                        }
                        _docrepository.UpdateAll(docToReloc);
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "SaveSubCategoryResponse",
                    ServiceName = "DocumentService",

                }, ex);
                throw ex;
            }
        }

        public DeleteSubCategoryResponse DeleteSubcategory(DeleteSubCategoryRequest request)
        {
            var response = new DeleteSubCategoryResponse();
            try
            {
                var subcategToDelete = _subcategrepository.FindBy(request.Id);
                var subcatelangToDelete = _subcateglangrepository.FindBy(new Specification<SubCategoryLang>(scl => scl.ID_SubCategory == request.Id));
                if (subcatelangToDelete != null)
                {
                    if (subcatelangToDelete.Any())
                    {
                        _subcateglangrepository.RemoveAll(subcatelangToDelete);
                    }
                    _subcategrepository.Remove(subcategToDelete);
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "DeleteSubcategory",
                    ServiceName = "DocumentService",

                }, ex);
                throw ex;
            }
        }

    }
}
