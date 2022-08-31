using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Service.Back;
using Intitek.Welcome.UI.ViewModels.Admin;
using Intitek.Welcome.UI.Web.Admin.Models;
using Intitek.Welcome.UI.Resources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Intitek.Welcome.UI.Web.Infrastructure;

namespace Intitek.Welcome.UI.Web.Areas.Admin.Controllers
{
    public class DocumentController : CommunController
    {
        private readonly IDocumentService _docService;
        private readonly IQcmService _qcmService;
        private readonly IUserService _userService;
        private readonly IEntiteService _entiteService;
        private readonly IProfilService _profilService;
        private readonly ILangService _langService;
        private FileLogger _logger;

        public DocumentController()
        {
            _logger = new FileLogger();
            _docService = new DocumentService(new FileLogger());
            _qcmService = new QcmService(new FileLogger());
            _userService = new UserService(new FileLogger());
            _entiteService = new EntiteService(new FileLogger());
            _profilService = new ProfilService(new FileLogger());
            _langService = new LangService(new FileLogger());

            
        }

        public ActionResult SearchCategories()
        {
            var categoryModels = _docService.GetAllCategory(new GetAllCategoryRequest() {
                IdLang = GetIdLang(),
                IdDefaultLang = GetDefaultLang()
            });
            var viewCategoryModels = AutoMapperConfigAdmin.Mapper.Map<List<CategoryViewModel>>(categoryModels);
            viewCategoryModels.Add(new CategoryViewModel() { ID = -1, Name = Resources.Resource.NoCategory });
            return Json(new { Items = viewCategoryModels });
        }
        public ActionResult SearchSousCategories()
        {
            int idLang = GetIdLang();
            int idDefaultLang = GetDefaultLang();
            var subcategoryModels = _docService.GetAllSubCategory(idLang, idDefaultLang);
            var viewSubCategoryModels = AutoMapperConfigAdmin.Mapper.Map<List<SubCategoryViewModel>>(subcategoryModels);
            viewSubCategoryModels.Add(new SubCategoryViewModel() { ID = -1, ID_DocumentCategory = -1, Name = Resources.Resource.NoSubCategory });
            return Json(new { Items = viewSubCategoryModels });
        }
        public ActionResult SearchVersions()
        {
            var versions = _docService.GetAllVersion();
            return Json(new { Items = versions });
        }

        // GET: Admin/Document
        private GridMvcRequest ReplaceFiltre(GridMvcRequest gridRequest)
        {
            if (gridRequest != null)
            {
                if (gridRequest.Filtres != null)
                {
                    int i = 0;
                    string newFiltre = "";
                    foreach (string filtre in gridRequest.Filtres)
                    {
                        newFiltre = filtre;
                        if (filtre.IndexOf("Approbation") != -1 || filtre.IndexOf("Test") != -1)
                        {
                            newFiltre = newFiltre.Replace("true", "1");
                            newFiltre = newFiltre.Replace("false", "0");
                        }
                        gridRequest.Filtres[i] = newFiltre;
                        i++;
                    }
                }
            }
            return gridRequest;
        }

        public ActionResult Index()
        {
            string nameGrid = "docGrid";
            GridMvcRequest initrequest = GridBORequest.GetRequestGrid(Request, nameGrid, null);
            GridMvcRequest request = base.GetGridRequestSession(initrequest);
            GetAllDocumentRequest docRequest = new GetAllDocumentRequest() { GridRequest = this.ReplaceFiltre(request) };
            docRequest.IdLang = GetIdLang();
            docRequest.IdDefaultLang = GetDefaultLang();
            var total = _docService.GetAllCount(docRequest);
            var documents = _docService.GetAll(docRequest).Documents;
            var docViewModels = AutoMapperConfigAdmin.Mapper.Map<List<DocumentViewModel>>(documents);
            var grid = new GridBO<DocumentViewModel>(request, docViewModels, total, request.Limit);
            return View(grid);
        }

        [HandleError(View = "~/Areas/Admin/Views/Shared/AjaxError.cshtml")]
        public ActionResult AjaxDocGrid()
        {
            string nameGrid = "docGrid";
            GridMvcRequest request = GridBORequest.GetRequestGrid(Request, nameGrid, null);
            base.SetGridRequestSession(request, "Index");
            GetAllDocumentRequest docRequest = new GetAllDocumentRequest() { GridRequest = this.ReplaceFiltre(request) };
            docRequest.IdLang = GetIdLang();
            docRequest.IdDefaultLang = GetDefaultLang();
            var total = _docService.GetAllCount(docRequest);
            var documents = _docService.GetAll(docRequest).Documents;
            var docViewModels = AutoMapperConfigAdmin.Mapper.Map<List<DocumentViewModel>>(documents);
            var grid = new GridBO<DocumentViewModel>(request, docViewModels, total, request.Limit);
            return Json(new { Html = grid.ToJson("_DocumentGrid", this), total }, JsonRequestBehavior.AllowGet);
        }
         public ActionResult Edit(int Id = 0, int IdLang = 0)
        {
            var codeLang = GetCodeLang();
            var lang = _langService.Get(new GetLangRequest() { Code = codeLang }).Langue;
            var langues = _langService.GetAll(new GetAllLangRequest()).Langues;

            var response = _docService.Get(new GetDocumentRequest()
            {
                Id = Id,
                IdLang = IdLang == 0 ? lang.ID : IdLang,
                DefaulltIdLang = GetDefaultLang()
            });
            var documentViewModel = new DocumentViewModel();

            if (response.Document != null)
            {
                documentViewModel.DocumentTitle = response.DocumentTitle;
                documentViewModel.IdLang = lang.ID;
                documentViewModel.ID = response.Document.ID;
                documentViewModel.Name = response.DocumentTrad == null ? string.Empty : response.DocumentTrad.Name;
                documentViewModel.IsDefaultLangName = response.DocumentTrad == null;
                documentViewModel.IsMajor = response.Document.isMajor;
                documentViewModel.ID_Category = response.Document.ID_Category;
                documentViewModel.ID_SubCategory = response.Document.ID_SubCategory;
                documentViewModel.IsNoActionRequired = response.Document.IsNoActionRequired;
                documentViewModel.Approbation = response.Document.Approbation;
                documentViewModel.Test = response.Document.Test;
                documentViewModel.TypeAffectation = response.Document.TypeAffectation.Trim();
                documentViewModel.Commentaire = response.Document.Commentaire;
                documentViewModel.NomOrigineFichier = response.Versions.FirstOrDefault(v => v.ID_Lang == lang.ID && v.Version == response.Document.Version) == null ? string.Empty : string.Format("{0}{1}", response.Versions.FirstOrDefault(v => v.ID_Lang == lang.ID).NomOrigineFichier, response.Document.Extension);
                documentViewModel.Extension = response.Document.Extension;
                documentViewModel.Version = response.Document.Version;
                documentViewModel.Inactif = response.Document.Inactif.Value;
                documentViewModel.PhaseEmployee = response.Document.PhaseEmployee;
                documentViewModel.PhaseOnboarding = response.Document.PhaseOnboarding;
                documentViewModel.ReadBrowser = response.Document.ReadBrowser;
                documentViewModel.ReadDownload = response.Document.ReadDownload;
                documentViewModel.IsMetier = response.Document.isMetier;
                documentViewModel.IsStructure = response.Document.isStructure;
                documentViewModel.IsMagazine = response.Document.isMagazine;
                documentViewModel.CreatedBy = string.Empty;
                if (response.Document.ID_UserCre.HasValue) {
                    var iUser = _userService.Get(new GetUserRequest() { Id = response.Document.ID_UserCre.Value }).User;
                    documentViewModel.CreatedBy = iUser != null ? iUser.FullName : string.Empty;
                }
                documentViewModel.CreationDate = response.Document.DateCre.HasValue ? response.Document.DateCre.Value.ToString() : string.Empty;

                documentViewModel.ModifiedBy = string.Empty;
                if (response.Document.ID_UserUpd.HasValue)
                {
                    var iUser = _userService.Get(new GetUserRequest() { Id = response.Document.ID_UserUpd.Value }).User;
                    documentViewModel.ModifiedBy = iUser != null ? iUser.FullName : string.Empty;
                }
                documentViewModel.ModificationDate = response.Document.DateUpd.HasValue ? response.Document.DateUpd.Value.ToString() : string.Empty;

                documentViewModel.DeletedBy = string.Empty;
                if (response.Document.ID_UserDel.HasValue)
                {
                    var iUser = _userService.Get(new GetUserRequest() { Id = response.Document.ID_UserDel.Value }).User;
                    documentViewModel.DeletedBy = iUser != null ? iUser.FullName : string.Empty;
                }
                documentViewModel.DeletionDate = response.Document.DateDel.HasValue ? response.Document.DateDel.Value.ToString() : string.Empty;

                documentViewModel.Categories = response.Categories;
                var categories = response.Categories.OrderBy(c => c.OrdreCategory).Select(
                    cat => new CategorySubcategoryViewModel()
                    {
                        ID_Category = cat.ID,
                        Category = cat.Name,
                        ID_SubCategory = -1,
                        Subcategory = string.Empty
                    }).ToList();
                var subcategories = response.SubCategories.OrderBy(c => c.Ordre).Select(                    
                    scat => new CategorySubcategoryViewModel(){
                        ID_Category = scat.ID_Category,
                        Category = scat.CategoryName,
                        ID_SubCategory = scat.ID,
                        Subcategory = scat.Name
                    }).ToList();

                categories.AddRange(subcategories);
                documentViewModel.CategorySubcategories = categories;

                documentViewModel.Versions = response.Versions.Any() ? response.Versions.Select(dv => new DocumentVersionDTO() {
                        ID = dv.ID, 
                        ID_Document = dv.ID_Document,
                        ID_UserCre = dv.ID_UserCre,
                        UserName = _userService.Get(new GetUserRequest()
                        {
                            Id = dv.ID_UserCre
                        }).User.Username,
                        IsMajor = dv.IsMajor,
                        DateCre = dv.DateCre,
                        ContentType = dv.ContentType,
                        Extension = dv.Extension,
                        Data = dv.Data,
                        Name = dv.Name,
                    NomOrigineFichier = dv.NomOrigineFichier,
                    Version = dv.Version,
                    }).ToList() : new List<DocumentVersionDTO>();

                documentViewModel.IdQcm = response.Document.IdQcm;
                documentViewModel.Qcms = _qcmService.GetAll(lang.ID).Qcms;

                var entites = _entiteService.GetAllEntity(new GetAllEntityRequest() { WithAgency = false }).Entites;
                var agencies = _entiteService.GetAllEntity(new GetAllEntityRequest() { WithAgency = true }).Entites;
                entites.AddRange(agencies);
                documentViewModel.Entites = entites;
                documentViewModel.Profiles = _profilService.GetAll()
                                        .Select(p => new ProfilDTO()
                                        {
                                            ID = p.ID,
                                            Name = p.Name
                                        }).ToList();

                var docAffectedTo = string.Empty;
                if (response.Document.TypeAffectation.Trim() == DocumentTypes.ENTITEAGENCE)
                {
                    var entityAffectation = _docService.GetEntityDocument(new GetEntityDocumentRequest() { DocumentID = Id }).Affectation;
                    if (entityAffectation.AffectedTo.Any())
                    {
                        docAffectedTo = string.Join(",", entityAffectation.AffectedTo);
                    }
                    
                }
                else
                {
                    var profileAffectation = _docService.GetProfileDocument(new GetProfileDocumentRequest() { DocumentID = Id }).Affectation;
                    if (profileAffectation.AffectedTo.Any())
                    {
                        docAffectedTo = string.Join(",", profileAffectation.AffectedTo);
                    }
                }
                documentViewModel.Affectation = docAffectedTo;
                
                if (response.DocumentTrad != null && response.DocumentTrad.Data != null)
                {
                    var pdfPath = Path.Combine(Request.PhysicalApplicationPath, "pdf");
                    string filename = HttpUtility.HtmlDecode(response.DocumentTrad.NomOrigineFichier) + response.Document.Extension;
                    var fullname = string.Format("{0}\\{1}", pdfPath, filename);
                    if (!System.IO.File.Exists(fullname))
                    {
                        if (!Directory.Exists(pdfPath))
                        {
                            Directory.CreateDirectory(pdfPath);
                        }
                        System.IO.File.WriteAllBytes(fullname, response.DocumentTrad.Data);
                    }
                }
                documentViewModel.Langues = langues;

            }
            return View(documentViewModel);
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult Edit(DocumentViewModel model)
        {
           

            var response = new SaveDocumentResponse();
            List<string> authorizedExtensionFile = new List<string> { ".pdf", ".mp4" };
            var codeLang = GetCodeLang();
            var lang = _langService.Get(new GetLangRequest() { Code = codeLang }).Langue;
            if (Request.Files.Count > 0)
            {
                var fileToUpload = Request.Files[0];
                string extension = Path.GetExtension(fileToUpload.FileName);

                if (!authorizedExtensionFile.Contains(extension) && model.ID == 0)
                {
                    model.Categories = _docService.GetAllCategory(new GetAllCategoryRequest() {
                         IdLang = 1
                    });
                    var entites = _entiteService.GetAllEntity(new GetAllEntityRequest() { WithAgency = false }).Entites;
                    var agencies = _entiteService.GetAllEntity(new GetAllEntityRequest() { WithAgency = true }).Entites;
                    entites.AddRange(agencies);
                    model.Entites = entites;
                    model.Profiles = _profilService.GetAll()
                                            .Select(p => new ProfilDTO()
                                            {
                                                ID = p.ID,
                                                Name = p.Name
                                            }).ToList();

                    model.Qcms = _qcmService.GetAll(lang.ID).Qcms;
                    model.Versions = new List<DocumentVersionDTO>();
                    return View("Edit", model);
                }

                

                byte[] fileData = null;
                using (var binaryReader = new BinaryReader(fileToUpload.InputStream))
                {
                    fileData = binaryReader.ReadBytes(fileToUpload.ContentLength);
                }

                var categorySubcategory = string.IsNullOrEmpty(model.CategorySubCategory) ? new string[]{"0", "NULL"} : model.CategorySubCategory.Split('|');
                int? categoryId = null;
                if (int.Parse(categorySubcategory[0]) > 0)
                {
                    categoryId = int.Parse(categorySubcategory[0]);
                }
                int? subCategoryId = null;
                if (categorySubcategory[1] != "NULL")
                {
                    subCategoryId = int.Parse(categorySubcategory[1]);
                }
                var request = new SaveDocumentRequest()
                {
                    Document = new Domain.Document()
                    {
                        ID = model.ID,                      
                        Version = model.Version,
                        Date = DateTime.Now,
                        Approbation = model.Approbation,
                        Test = model.Test,
                        Commentaire = model.Commentaire,
                        ContentType = fileToUpload.ContentType,
                        Extension = extension,
                        Inactif = model.Inactif,
                        TypeAffectation = model.TypeAffectation,
                        IdQcm = model.IdQcm,
                        ID_Category = categoryId,
                        ID_SubCategory = subCategoryId,
                        isMajor = model.IsMajor,
                        IsNoActionRequired = model.IsNoActionRequired,
                        isMetier = model.IsMetier,
                        isStructure = model.IsStructure,
                        ReadBrowser = model.ReadBrowser,
                        ReadDownload = model.ReadDownload,
                        PhaseEmployee = model.PhaseEmployee,
                        PhaseOnboarding = model.PhaseOnboarding,
                        ID_UserCre = GetUserIdConnected(),
                        DateCre = DateTime.Now,
                        isMagazine = model.IsMagazine
                    },
                    DocumentTrad = new DocumentLang()
                    {
                        ID_Lang = model.IdLang,
                        Name = model.Name,
                        NomOrigineFichier = string.IsNullOrEmpty(model.NomOrigineFichier) ? Path.GetFileNameWithoutExtension(fileToUpload.FileName) : Path.GetFileNameWithoutExtension(model.NomOrigineFichier),
                        Data = fileData
                    },
                    Affectation = model.Affectation
                };

               
                response = _docService.Save(request);

                if (extension == ".pdf")
                {
                    var pdfPath = Path.Combine(Request.PhysicalApplicationPath, "pdf");
                    var pdfFileName = string.Format("{0}\\{1}.pdf", pdfPath, model.ID);
                    if (System.IO.File.Exists(pdfFileName))
                    {
                        System.IO.File.Delete(pdfFileName);
                    }
                }
                else if (extension == ".mp4")
                {
                    var mp4Path = Path.Combine(Request.PhysicalApplicationPath, ConfigurationManager.AppSettings["Welcome.video.directory"]);
                    var mp4FileName = string.Format("{0}\\document_{1}_{2}_{3}.mp4", mp4Path, response.Id, response.Version, codeLang);
                    System.IO.File.WriteAllBytes(mp4FileName, fileData);
                }
                 
            }
            return RedirectToAction("Edit", "Document", new { Id = response.Id });
        }

        public ActionResult ConfirmDelete(int Id)
        {
            var response = _docService.Get(new GetDocumentRequest()
            {
                Id = Id
            });

            return PartialView("_ConfirmDelete", new ConfirmDeleteViewModel()
            {
                ID = response.Document.ID,
                CanDelete = true,
                ControllerName = "Document",
                ActionName = "Delete",
                EntityName = Resources.Resource.le_document,
                Name = string.Empty, // response.Document.Name
            });
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult Delete(int id)
        {
            var reponse = _docService.Delete(new DeleteDocumentRequest()
            {
                Id = id,
                UserId = GetUserIdConnected(),
            });

            if (Request.IsAjaxRequest())
                return Json(new { Id = id }, JsonRequestBehavior.AllowGet);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult GetTranslatedDoc(int Id, int IdLang)
        {
            var response = _docService.Get(new GetDocumentRequest()
            {
                Id = Id,
                IdLang = IdLang 
            });
            var documentViewModel = new DocumentViewModel();

            if (response.Document != null)
            {
                
                documentViewModel.IdLang = IdLang;               
                documentViewModel.Name = response.DocumentTrad == null ? string.Empty : response.DocumentTrad.Name;               
                documentViewModel.ID_Category = response.Document.ID_Category;               
                documentViewModel.NomOrigineFichier = response.DocumentTrad == null ? string.Empty : string.Format("{0}{1}", response.DocumentTrad.NomOrigineFichier, response.Document.Extension);
                documentViewModel.Extension = response.Document.Extension;
                documentViewModel.Version = response.Document.Version;

                documentViewModel.Categories = response.Categories;
                documentViewModel.Versions = response.Versions.Any() ? response.Versions.Select(dv => new DocumentVersionDTO()
                {
                    ID = dv.ID,
                    ID_Document = dv.ID_Document,
                    ID_UserCre = dv.ID_UserCre,
                    UserName = _userService.Get(new GetUserRequest()
                    {
                        Id = dv.ID_UserCre
                    }).User.Username,
                    IsMajor = dv.IsMajor,
                    DateCre = dv.DateCre,
                    ContentType = dv.ContentType,
                    Extension = dv.Extension,
                    Name = dv.Name,
                    NomOrigineFichier = dv.NomOrigineFichier,
                    Version = dv.Version,
                }).ToList() : new List<DocumentVersionDTO>();
                documentViewModel.IdQcm = response.Document.IdQcm;
               
            }
            return Json( 
                new
                {
                    doc = documentViewModel,
                    qcms = _qcmService.GetAll(IdLang).Qcms.Select(
                        qcm => new
                        {
                            IdQcm = qcm.Id,
                            QcmName = qcm.QcmTrad == null ? string.Empty : qcm.QcmTrad.QcmName
                        }
                   ).ToList()
                }, JsonRequestBehavior.AllowGet);
          
        }

        [HttpGet]
        public ActionResult GetFileFromDatabase(int documentID, Boolean inline)
        {

            var iddLang = GetIdLang();
            var defaulltIdLang = GetDefaultLang();
            var response = _docService.Get(new GetDocumentRequest() { Id = documentID, IdLang= iddLang, DefaulltIdLang= defaulltIdLang });
            var doc = response.Document;
            string filename = response.DocumentTrad == null && iddLang != defaulltIdLang ?
                HttpUtility.HtmlDecode(response.DefaultDocumentTrad.NomOrigineFichier) + doc.Extension :
                HttpUtility.HtmlDecode(response.DocumentTrad.NomOrigineFichier) + doc.Extension;
            if (inline)
                Response.AddHeader("Content-Disposition", "inline; filename=" + filename);
            else
                Response.AddHeader("Content-Disposition", "attachment; filename=" + Uri.EscapeDataString(filename));
            Response.ContentType = "application/pdf";
            //FileDownload ajax
            Response.SetCookie(new HttpCookie(FileDownloadResult.COOKIE_FILEDOWNLOAD, "true") { Path = "/", HttpOnly = true });
            MemoryStream output = new MemoryStream();
            if (!".pdf".Equals(doc.Extension, StringComparison.InvariantCultureIgnoreCase))
            {
                bool bVideoExist = true;
                string videoFile = string.Format("document_{0}_{1}_{2}.mp4", doc.ID, doc.Version, GetCodeLang());
                var dirVideo = GetAppSettings("Welcome.video.directory");
                var pahVideo = System.IO.Path.Combine(dirVideo, videoFile);
                if (!System.IO.File.Exists(pahVideo))
                {
                    bVideoExist = false;
                    var error = string.Format("Video {0} n'existe pas sur le disque", pahVideo);
                    _logger.Error(new ExceptionLogger()
                    {
                        ExceptionDateTime = DateTime.Now,
                        ExceptionMessage = error,
                        MethodName = "DownoloadDocInfo",
                        ServiceName = "UserDocumentController",

                    }, new Exception(error));
                }
                if(bVideoExist)
                {
                    Response.ContentType = "application/force-download";
                    var videos = System.IO.File.ReadAllBytes(pahVideo);
                    output.WriteAsync(videos, 0, videos.Length);
                    FileStreamResult outputAsync = new FileStreamResult(output, "application/force-download");
                    output.Position = 0;
                    return outputAsync;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                var trad = response.DocumentTrad == null && iddLang != defaulltIdLang ? response.DefaultDocumentTrad : response.DocumentTrad;
                output.WriteAsync(trad.Data, 0, trad.Data.Length);
                FileStreamResult outputAsync = new FileStreamResult(output, "application/pdf");
                output.Position = 0;
                return outputAsync;
            }
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult GetAllNames(string term, bool exact)
        {
            var response = _docService.GetAllName(new GetAllDocumentNameRequest() {
                Search = term,
                ExactMatch = exact
            });
            return Json(response.Names.Select(doc => new
            {
                label = doc.Name,
                value = doc.ID
            }).ToList());
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult CheckName(string term)
        {
            var response = _docService.CheckName(new CheckDocumentNameRequest()
            {
                Name = term
            });
            return Json(new { success = response.Exist, data = response.Document }, JsonRequestBehavior.AllowGet);

        }

        
        public ActionResult Download(int versionID, int langID)
        {
            var doc = _docService.GetVersions(new GetDocumentVersionsRequest() { Id = versionID, IdLang = langID });
            var lang = _langService.Get(new GetLangRequest() { Id = langID }).Langue;
            var fileType = doc.Version.ContentType;
            MemoryStream output = new MemoryStream();
            string filename = string.Format("{0}{1}", HttpUtility.HtmlDecode(doc.Version.NomOrigineFichier), doc.Version.Extension);
            Response.AddHeader("Content-Disposition", "attachment; filename=" + Uri.EscapeDataString(filename));
            Response.ContentType = fileType;
            if (fileType.Contains("pdf"))
            {
                output.WriteAsync(doc.Version.Data, 0, doc.Version.Data.Length);
            }
            else
            {
                string videoFile = string.Format("document_{0}_{1}_{2}.mp4", doc.Version.ID_Document, doc.Version.Version, lang.Code.Substring(0,2));
                var dirVideo = GetAppSettings("Welcome.video.directory");
                var videoPath = System.IO.Path.Combine(dirVideo, videoFile);
                if (System.IO.File.Exists(videoPath)) {
                    var videos = System.IO.File.ReadAllBytes(videoPath);
                    output.WriteAsync(videos, 0, videos.Length);
                }
                else
                {                   
                    var error = string.Format("Video {0} n'existe pas sur le disque", videoPath);
                    
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + Uri.EscapeDataString(string.Format("No-Content{0}", doc.Version.Extension)));
                    _logger.Error(new ExceptionLogger()
                    {
                        ExceptionDateTime = DateTime.Now,
                        ExceptionMessage = error,
                        MethodName = "DownoloadDocInfo",
                        ServiceName = "UserDocumentController",

                    }, new Exception(error));
                    return null;
                }
            }
            FileStreamResult outputAsync = new FileStreamResult(output, fileType);
            output.Position = 0;
            return outputAsync;
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult GetVersion(int Id,  bool IsMajorVersion)
        {
            var response = _docService.GetDocumentVersion(new GetDocumentVersionRequest()
            {
                Id = Id,
                IsMajorVersion = IsMajorVersion
            });

            return Json(new {
                success = true,
                data = response.NewVersion
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult ExistVersionTrad(int Id, int IdLang, string Version)
        {
            var codeLang = GetCodeLang();
            var lang = _langService.Get(new GetLangRequest() { Code = codeLang }).Langue;

            var response = _docService.ExistDocumentVersionLang(new ExistDocumentVersionLangRequest()
            {
                ID_Document = Id,
                ID_Lang = lang.ID,
                Version = Version
            });

            return Json(new
            {
                success = response.Result,
                data = response.VersionTrad
            }, JsonRequestBehavior.AllowGet);
        }
    }
}