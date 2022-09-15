using System;
using System.IO;
using System.Collections.Generic;
using System.Web.Mvc;

using Intitek.Welcome.Service.Front;
using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.UI.ViewModels;
using Intitek.Welcome.UI.Web.Models;
using GridMvc;
using GridMvc.Filtering;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.Helpers;
using System.Web;
using Intitek.Welcome.UI.Web.Infrastructure;
using Intitek.Welcome.UI.Web.Filters;
using System.Security.Cryptography;
using System.Linq;


namespace Intitek.Welcome.UI.Web.Controllers
{
    [UrlRestrictAccessFilter]
    public class UserDocumentController : CommunController
    {
        private readonly string INPUT_SEARCH = "filtre";
        private readonly string PAGESIZE_COOKIE = "pagesize";
        private readonly IWelcomeService _welcomeService;
        private readonly IDocumentService _docService;
        private readonly IUserDocumentService _userDocumentService;
        private readonly ILangService _langService;
        private readonly Service.Back.IUserService _userService;
        private readonly Service.Back.IStatsService _statsService;
        private Service.Back.ManagerStats _managerStats;       

        private FileLogger _logger;
        public int SpyId { get; set; }

        public UserDocumentController()
        {
            _logger = new FileLogger();
            _welcomeService = new WelcomeService(new FileLogger());
            _docService = new DocumentService(new FileLogger());
            _userDocumentService = new UserDocumentService(new FileLogger());
            _langService = new LangService(new FileLogger());
            _userService = new Service.Back.UserService(new FileLogger());
            _statsService = new Service.Back.StatsService(new FileLogger());

        }
        // GET: UserDocument
        private GetUserDocumentRequest GeRequestGrid(int userId, int indexGrid)
        {
            string nameGrid = "grid" + indexGrid;
            int pageSize, pageIndex, orderDirection;
            int.TryParse(Request.QueryString[DocumentGrid.GetParameterName(indexGrid)], out pageIndex);
            int.TryParse(Request.QueryString[DocumentGrid.GetParameterPagesize(indexGrid)], out pageSize);
            string orderColumn = Request.QueryString[QueryStringGridSettingsProvider.GetColumnQueryParameterName(indexGrid)];
            int.TryParse(Request.QueryString[QueryStringGridSettingsProvider.GetDirectionQueryParameterName(indexGrid)], out orderDirection);
            //Trier par defaut
            if (string.IsNullOrEmpty(orderColumn))
            {
                orderColumn = "Name";
                orderDirection = 0;
            }
            //Filtre
            string[] filters = Request.QueryString.GetValues(QueryStringFilterSettings.DefaultTypeQueryParameter);
            if (pageSize == 0)
            {
                pageSize = 10;
                if (Utils.GetCookies(PAGESIZE_COOKIE + "_" + indexGrid) != null)
                {
                    pageSize = Int32.Parse(Utils.GetCookies(PAGESIZE_COOKIE + "_" + indexGrid));
                }
            }
            else
            {
                int cookPageSize = 0;
                if (Utils.GetCookies(PAGESIZE_COOKIE + "_" + indexGrid) != null)
                {
                    cookPageSize = Int32.Parse(Utils.GetCookies(PAGESIZE_COOKIE + "_" + indexGrid));
                }
                if (cookPageSize != pageSize)
                {
                    Utils.SetCookies(PAGESIZE_COOKIE + "_" + indexGrid, pageSize, DateTime.MaxValue);
                }
            }
            if (pageIndex == 0) pageIndex = 1;
            string search = Request.QueryString[nameGrid + "_" + INPUT_SEARCH];
            return new GetUserDocumentRequest() { GridName = nameGrid, UserID = userId, Page = pageIndex, Limit = pageSize, OrderColumn = orderColumn, SortDirection = orderDirection, Search = search, Filtres = filters };
        }

       

        public ActionResult Index(int? id=null)
        {
            var userId = GetUserIdConnected();
            var concerned = GetUserIdConnected();
            var userConnected = GetUserConnected();
            
            //STATISTIQUES           
            var isReadOnly = false;
            if (id != null || TempData["readOnly"] != null || TempData["id"] != null)
            {
                if (id != null)
                {
                    var c = (int)id;
                    if(c != userId)
                    {
                        SpyId = c;
                        userId = c;
                        var user = _userService.GetUser(userId);
                        if (user != null)
                        {
                            userConnected.NomPrenom = user.FullName;
                            userConnected.FirstName = user.FirstName;
                            userId = user.ID;
                            userConnected.ID = userId;
                            ViewBag.ViewAs = user.FullName;
                        }
                        TempData["readOnly"] = true;
                        TempData["id"] = userId;
                        isReadOnly = true;
                    }
                    
                }
                if (TempData["readOnly"] != null && id == null)
                {
                    var b = (bool)TempData["readOnly"];
                    if (b)
                    {
                        TempData["readOnly"] = true;                       
                        isReadOnly = true;
                    }
                }
                if (TempData["id"] != null && id == null)
                {
                    var i = (int)TempData["id"];
                    if (i > 0)
                    {
                        if(i == userId)
                        {
                            TempData["readOnly"] = null;
                            TempData["id"] = null;
                            isReadOnly = false;
                        }
                        else
                        {
                            TempData["readOnly"] = true;
                            isReadOnly = true;
                            userId = i;
                            var user = _userService.GetUser(userId);
                            TempData["id"] = userId;
                            if (user != null)
                            {
                                userConnected.NomPrenom = user.FullName;
                                userConnected.FirstName = user.FirstName;
                                userId = user.ID;
                                userConnected.ID = userId;
                                ViewBag.ViewAs = user.FullName;
                            }
                        }
                        
                    }
                }
            }
            else
            {
                TempData["readOnly"] = false;
                isReadOnly = false;
                TempData["id"] = 0;
                userId = GetUserIdConnected();
                userConnected = GetUserConnected();
            }

           

            var _culture = Request.Cookies["_culture"] != null ? Request.Cookies["_culture"].Value : (Request.UserLanguages != null && Request.UserLanguages.Length > 0 ?
                        Request.UserLanguages[0] : "fr-FR");
            var idLang = GetIdLang();
            var defaulltIdLang = GetDefaultLang();
            var welcomeMessage = _welcomeService.GetWelcomeMessageByLang(_culture.Substring(0, 2));

            //Liste Document
            int indexGrid = 1;
            GetUserDocumentRequest initrequest1 = this.GeRequestGrid(userId, indexGrid);
            GetUserDocumentRequest request1 = base.GetGridRequestSession(initrequest1);
            request1.UserID = userId;
            request1.IDLang = idLang;
            request1.IDDefaultLang = defaulltIdLang ;
            var codeLangDefault = _langService.Get(new GetLangRequest() { Id = defaulltIdLang }).CodeLangue;

            //DOCUMENTS INFORMATIFS
            indexGrid = 2;
            GetUserDocumentRequest initrequest2 = this.GeRequestGrid(userId, indexGrid);
            GetUserDocumentRequest request2 = base.GetGridRequestSession(initrequest2);
            request2.UserID = userId;
            request2.IDLang = idLang;
            request2.IDDefaultLang = defaulltIdLang;

            //DOCUMENTS DÉJÀ LUS ET REVUS
            indexGrid = 3;
            GetUserDocumentRequest initrequest3 = this.GeRequestGrid(userId, indexGrid);
            GetUserDocumentRequest request3 = base.GetGridRequestSession(initrequest3);
            request3.IDLang = idLang;
            request3.UserID = userId;
            request3.IDDefaultLang = defaulltIdLang;

            GetUserDocumentResponse response = _docService.GetAllListDocumentByUser(request1, request2, request3);
            //DOCUMENTS NÉCESSITANT UNE ACTION DE VOTRE PART
            //var totalNAction = _docService.GetAllDocumentByUserLoginCount(request1);
            //var docNActions = _docService.GetAllDocumentByUserLogin(request1);
            var totalNAction = response.NbActionDocuments;
            var docNActions = response.LstActionDocuments;
            var userDocNActionViewModels = AutoMapperConfig.Mapper.Map<List<DocumentViewModel>>(docNActions);
            var grid1 = new DocumentGrid(request1, userDocNActionViewModels, totalNAction, request1.Limit);

            //var totalInformatif = _docService.GetNoActionDocumentByUserLoginCount(request2);
            //var docInformatifs = _docService.GetNoActionDocumentByUserLogin(request2);
            var totalInformatif = response.NbInformatifDocuments;
            var docInformatifs = response.LstInformatifDocuments;
            var userInfoViewModels = AutoMapperConfig.Mapper.Map<List<DocumentViewModel>>(docInformatifs);
            var grid2 = new DocumentGrid(request2, userInfoViewModels, totalInformatif, request2.Limit);

            //var totalRead = _docService.GetReadedDocumentByUserLoginCount(request3);
            //var docReads = _docService.GetReadedDocumentByUserLogin(request3);
            var totalRead = response.NbReadDocuments;
            var docReads = response.LstReadDocuments;
            var userDocReadViewModels = AutoMapperConfig.Mapper.Map<List<DocumentViewModel>>(docReads);
            var grid3 = new DocumentGrid(request3, userDocReadViewModels, totalRead, request3.Limit);

            

           _managerStats = new Service.Back.ManagerStats(_statsService, _userService)
            {
                Id = userId
            };
            if(_userService.GetManagerList(userId).Count > 0)
            {                              
                _managerStats.InitRequest();
                foreach (var m in _managerStats.Collaborateurs)
                {
                    
                    m.Response = _docService.GetListDocumentByUser(m.UserId, 1);
                    m.ActionsCount = m.Response.ActionsCount;
                    m.NbActionDocuments = m.Response.LstActionDocuments.Count;
                    m.NbReadDocuments = m.Response.LstReadDocuments.Count;
                }
                _managerStats.GetRetails();               
            }
            grid1.IsReadOnly= isReadOnly;
            grid2.IsReadOnly = isReadOnly;
            grid3.IsReadOnly = isReadOnly;
            grid1.Pager.IsReadOnly = isReadOnly;
            grid2.Pager.IsReadOnly = isReadOnly;
            grid3.Pager.IsReadOnly = isReadOnly;
            //
            WelcomeViewModel wm = new WelcomeViewModel()
            {
                IsReadOnly = isReadOnly,
                WelcomeMessage = string.IsNullOrEmpty(welcomeMessage) ? "" : welcomeMessage
                    .Replace("#full_name#", userConnected.NomPrenom).Replace("#first_name#", userConnected.FirstName),
                Grid1 = grid1,
                Grid2 = grid2,
                Grid3 = grid3,               
                StatModel = _managerStats,
                HistoUserQcms = _docService.FindAllUserQcmSuccess(userConnected.ID, 10, idLang, defaulltIdLang),
                CodeLangue =_culture.Substring(0,2),
                DefaultCodeLangue = codeLangDefault
            };
            
            return View(wm);
        }



       [HandleError(View = "~/Views/Shared/AjaxError.cshtml")]
        public ActionResult AjaxDocGrid()
        {
            var defaulltIdLang = GetDefaultLang();
            var userId = GetUserIdConnected();
            string gridName = Request.QueryString["grid"];
            int indexGrid = System.Int32.Parse(gridName.Replace("grid", ""));
            GetUserDocumentRequest request = this.GeRequestGrid(userId, indexGrid);
            request.IDLang = GetIdLang();
            request.IDDefaultLang = defaulltIdLang;
            var codeLangDefault = _langService.Get(new GetLangRequest() { Id = defaulltIdLang }).CodeLangue;
            base.SetGridRequestSession(request, "Index");
            int total = 0;
            DocumentGrid grid = null;
            switch (indexGrid)
            {
                case 1:
                    var totalNAction = _docService.GetAllDocumentByUserLoginCount(request);
                    var docNActions = _docService.GetAllDocumentByUserLogin(request);
                    var userDocNActionViewModels = AutoMapperConfig.Mapper.Map<List<DocumentViewModel>>(docNActions);
                    grid = new DocumentGrid(request, userDocNActionViewModels, totalNAction, request.Limit);
                    total = totalNAction;
                    break;
                case 2:
                    var totalInformatif = _docService.GetNoActionDocumentByUserLoginCount(request);
                    var docInformatifs = _docService.GetNoActionDocumentByUserLogin(request);
                    var userInfoViewModels = AutoMapperConfig.Mapper.Map<List<DocumentViewModel>>(docInformatifs);
                    grid = new DocumentGrid(request, userInfoViewModels, totalInformatif, request.Limit);
                    total = totalInformatif;
                    break;
                case 3:
                    var totalRead = _docService.GetReadedDocumentByUserLoginCount(request);
                    var docReads = _docService.GetReadedDocumentByUserLogin(request);
                    var userDocReadViewModels = AutoMapperConfig.Mapper.Map<List<DocumentViewModel>>(docReads);
                    grid = new DocumentGrid(request, userDocReadViewModels, totalRead, request.Limit);
                    total = totalRead;
                    break;
                default:
                    break;
            }
            bool bInformatif = false;
            if (indexGrid == 2)
            {
                bInformatif = true;
            }
            if (grid != null)
            {
                return Json(new { Html = grid.ToJson("_DocumentGrid", new ViewDataDictionary() { { "Informatif", bInformatif }, { "DefaultCodeLangue", codeLangDefault } }, this), total }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Message = "Error on grid" });
        }

        public ActionResult Show()
        {
            var _userID = GetUserIdConnected();           
            
            if(TempData["id"] != null)
            {
                int p = (int)TempData["id"];
                TempData["readOnly"] = true;
                TempData["id"] = p;
                _userID = p;
            }
            else
            {
                string s = Request.UrlReferrer.AbsolutePath;
                string[] arr = s.Split('/');
                if (s.Length > 0)
                {
                    var n = arr[arr.Length - 1];
                    try
                    {
                        int p = int.Parse(n);
                        TempData["readOnly"] = true;
                        TempData["id"] = p;
                        _userID = p;
                    }
                    catch
                    {
                        TempData["readOnly"] = false;
                        TempData["id"] = _userID;
                    }

                }
            }
            string rrr = Request.QueryString["rrr"];
            var param = HtmlExtensions.DecryptURL(rrr);

            int doc = -1;
            if (!string.IsNullOrEmpty(param))
            {
                param = param.Replace("doc=", string.Empty);
                Int32.TryParse(param, out doc);
            }
            var idLang = GetIdLang();
            var defaulltIdLang = GetDefaultLang();
            var codeLangDefault = string.Empty;           
            GetDocumentRequest request = new GetDocumentRequest() { Id = doc, IdLang = idLang, DefaulltIdLang = defaulltIdLang };

            var response = _docService.GetDocument(request);
            var trad = response.DocumentTrad;
            if (response.DocumentTrad == null)
            {
                trad = response.DefaultDocumentTrad;
                codeLangDefault = _langService.Get(new GetLangRequest() { Id = response.DefaultDocumentTrad.ID_Lang }).CodeLangue;
                //pdfFile = string.Format("{0}_{1}.pdf", response.Document.ID, codeLangDefault);
            }

            var userDocumentViewModel = new UserDocumentViewModel()
            {
                UserID = _userID,
                DocumentID = doc,
                DocumentVersion = response.Document.Version,
                DocumentTitle = trad.Name,
                QcmID = response.Document.IdQcm == null ? 0 : response.Document.IdQcm.Value,
                IsNoActionRequired = response.Document.IsNoActionRequired,
                IsDocumentToRead = !response.Document.IsNoActionRequired,
                IsDocumentToApprouve = response.Document.Approbation == 1,
                IsDocumentToTest = response.Document.Test == 1,
                IsMagazine = response.Document.isMagazine
            };
            var userDocumentResponse = _userDocumentService.GetUserDocument(new GetUserDocumentRequest()
            {
                DocumentID = doc,
                UserID = _userID
            });

            if (userDocumentResponse.UserDocument != null)
            {
                userDocumentViewModel.DocumentID = userDocumentResponse.UserDocument.DocumentId;
                userDocumentViewModel.UserID = userDocumentResponse.UserDocument.UserId;
                userDocumentViewModel.IsRead = userDocumentResponse.UserDocument.IsRead;
                userDocumentViewModel.IsApproved = userDocumentResponse.UserDocument.IsApproved;
                userDocumentViewModel.IsTested = userDocumentResponse.UserDocument.IsTested;
            }

           
            if (".pdf".Equals(response.Document.Extension, StringComparison.InvariantCultureIgnoreCase))
            {
                var pdfPath = Path.Combine(Request.PhysicalApplicationPath, "pdf");

                string pdfFile = string.Format("{0}_{1}.pdf", response.Document.ID, response.DocumentTrad == null ? codeLangDefault : GetCodeLang());

                //string filename = HttpUtility.HtmlDecode(trad.NomOrigineFichier) + response.Document.Extension;
                var pdfFullname = string.Format("{0}\\{1}", pdfPath, pdfFile);
                if (!Directory.Exists(pdfPath))
                {
                    Directory.CreateDirectory(pdfPath);
                }
                if (System.IO.File.Exists(pdfFullname))
                {
                    System.IO.File.Delete(pdfFullname);
                }
                System.IO.File.WriteAllBytes(pdfFullname, trad.Data);

                userDocumentViewModel.PdfFile = pdfFile;
            }
            else if (".mp4".Equals(response.Document.Extension, StringComparison.InvariantCultureIgnoreCase)){
                string videoFile = string.Format("document_{0}_{1}_{2}.mp4", response.Document.ID, response.Document.Version,  GetCodeLang());
                var dirVideo = GetAppSettings("Welcome.video.directory");
                var pathVideo = System.IO.Path.Combine(dirVideo, videoFile);
                bool bVideoExist = true;
                if (!System.IO.File.Exists(pathVideo))
                {
                    videoFile = string.Format("document_{0}_{1}_{2}.mp4", response.Document.ID, response.Document.Version, codeLangDefault);
                    pathVideo = System.IO.Path.Combine(dirVideo, videoFile);
                    if (!System.IO.File.Exists(pathVideo))
                    {
                        bVideoExist = false;
                    }
                }
                if (bVideoExist)
                {
                    var rnd = new byte[4];
                    using (var rng = new RNGCryptoServiceProvider())
                        rng.GetBytes(rnd);
                    var j = Math.Abs(BitConverter.ToInt32(rnd, 0));
                    userDocumentViewModel.Url = GetAppSettings("Welcome.video.url") + "/" + videoFile+"?rr="+ j;
                }
                else
                {
                    userDocumentViewModel.Error = string.Format(Resources.Resource.VideoNotExist, pathVideo);
                    
                }
                return View("ShowVideo", userDocumentViewModel);
            }
            return View(userDocumentViewModel);
        }

        public ActionResult ShowFromLink()
        {
            //Request.Cookies["Window_Scroll_Top"].Value ="-1";
            //Request.Cookies["UserDocument_Grid_HASH"].Value ="gridBO";
            string rrr = Request.QueryString["rrr"];
            var param = HtmlExtensions.DecryptURL(rrr);
            int doc = -1;
            if (!string.IsNullOrEmpty(param))
            {
                param = param.Replace("doc=", string.Empty);
                Int32.TryParse(param, out doc);
            }
            var idLang = GetIdLang();
            var defaulltIdLang = GetDefaultLang();
            var codeLangDefault = string.Empty;
            var _userID = GetUserIdConnected();
            
            //check if the user has access to the doc
            Boolean authorised= _userDocumentService.CheckDocUserGrant(doc, _userID);
            if (!authorised)
            {
                return NoAccessDocument();
            }

            GetDocumentRequest request = new GetDocumentRequest() { Id = doc, IdLang = idLang, DefaulltIdLang = defaulltIdLang };

            var response = _docService.GetDocument(request);
            var trad = response.DocumentTrad;
            if (response.DocumentTrad == null)
            {
                trad = response.DefaultDocumentTrad;
                codeLangDefault = _langService.Get(new GetLangRequest() { Id = response.DefaultDocumentTrad.ID_Lang }).CodeLangue;
                //pdfFile = string.Format("{0}_{1}.pdf", response.Document.ID, codeLangDefault);
            }

            var userDocumentViewModel = new UserDocumentViewModel()
            {
                UserID = _userID,
                DocumentID = doc,
                DocumentVersion = response.Document.Version,
                DocumentTitle = trad.Name,
                QcmID = response.Document.IdQcm == null ? 0 : response.Document.IdQcm.Value,
                IsNoActionRequired = response.Document.IsNoActionRequired,
                IsDocumentToRead = !response.Document.IsNoActionRequired,
                IsDocumentToApprouve = response.Document.Approbation == 1,
                IsDocumentToTest = response.Document.Test == 1,
                IsMagazine = response.Document.isMagazine
            };
            // set not magazine
            //userDocumentViewModel.IsMagazine = false;
            var userDocumentResponse = _userDocumentService.GetUserDocument(new GetUserDocumentRequest()
            {
                DocumentID = doc,
                UserID = _userID
            });

            if (userDocumentResponse.UserDocument != null)
            {
                userDocumentViewModel.DocumentID = userDocumentResponse.UserDocument.DocumentId;
                userDocumentViewModel.UserID = userDocumentResponse.UserDocument.UserId;
                userDocumentViewModel.IsRead = userDocumentResponse.UserDocument.IsRead;
                userDocumentViewModel.IsApproved = userDocumentResponse.UserDocument.IsApproved;
                userDocumentViewModel.IsTested = userDocumentResponse.UserDocument.IsTested;
            }


            if (".pdf".Equals(response.Document.Extension, StringComparison.InvariantCultureIgnoreCase))
            {
                var pdfPath = Path.Combine(Request.PhysicalApplicationPath, "pdf");

                string pdfFile = string.Format("{0}_{1}.pdf", response.Document.ID, response.DocumentTrad == null ? codeLangDefault : GetCodeLang());

                //string filename = HttpUtility.HtmlDecode(trad.NomOrigineFichier) + response.Document.Extension;
                var pdfFullname = string.Format("{0}\\{1}", pdfPath, pdfFile);
                if (!Directory.Exists(pdfPath))
                {
                    Directory.CreateDirectory(pdfPath);
                }
                if (System.IO.File.Exists(pdfFullname))
                {
                    System.IO.File.Delete(pdfFullname);
                }
                System.IO.File.WriteAllBytes(pdfFullname, trad.Data);

                userDocumentViewModel.PdfFile = pdfFile;
            }
            else if (".mp4".Equals(response.Document.Extension, StringComparison.InvariantCultureIgnoreCase))
            {
                string videoFile = string.Format("document_{0}_{1}_{2}.mp4", response.Document.ID, response.Document.Version, GetCodeLang());
                var dirVideo = GetAppSettings("Welcome.video.directory");
                var pathVideo = System.IO.Path.Combine(dirVideo, videoFile);
                bool bVideoExist = true;
                if (!System.IO.File.Exists(pathVideo))
                {
                    videoFile = string.Format("document_{0}_{1}_{2}.mp4", response.Document.ID, response.Document.Version, codeLangDefault);
                    pathVideo = System.IO.Path.Combine(dirVideo, videoFile);
                    if (!System.IO.File.Exists(pathVideo))
                    {
                        bVideoExist = false;
                    }
                }
                if (bVideoExist)
                {
                    var rnd = new byte[4];
                    using (var rng = new RNGCryptoServiceProvider())
                        rng.GetBytes(rnd);
                    var j = Math.Abs(BitConverter.ToInt32(rnd, 0));
                    userDocumentViewModel.Url = GetAppSettings("Welcome.video.url") + "/" + videoFile + "?rr=" + j;
                }
                else
                {
                    userDocumentViewModel.Error = string.Format(Resources.Resource.VideoNotExist, pathVideo);

                }
                return View("ShowVideo", userDocumentViewModel);
            }
            return View("Show",userDocumentViewModel);
        }
        public ActionResult NoAccessDocument()
        {
            return View("NoAccessDocument");
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult ValidateAndTest(UserDocumentViewModel model)
        {
            var request = new UpdateUserDocumentRequest()
            {
                UserDocument = new UserDocumentDTO()
                {
                    DocumentId = model.DocumentID,
                    UserId = model.UserID,
                    IsRead = model.IsRead,
                    IsApproved = model.IsApproved,
                    IsTested = model.IsTested
                }

            };

            var response = _userDocumentService.UpdateUserDocument(request);

            return Json(new { success = true, data = model }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetFileFromDatabase(int documentID, Boolean inline)
        {
            int iddLang = GetIdLang();
            var defaulltIdLang = GetDefaultLang();
            var codeLangDefault = _langService.Get(new GetLangRequest() { Id = defaulltIdLang }).CodeLangue;
            var response = _docService.GetDocument(new GetDocumentRequest() { Id = documentID, IdLang = iddLang, DefaulltIdLang = defaulltIdLang });
            Document doc = response.Document;
            var request = new UpdateUserDocumentRequest()
            {
                UserDocument = new UserDocumentDTO()
                {
                    DocumentId = doc.ID,
                    UserId = GetUserIdConnected(),
                    IsRead = true
                }

            };
            if (!_userDocumentService.IsReadUserDocument(request))
            {
                _userDocumentService.UpdateUserDocument(request);
            }

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
                string videoFile = string.Format("document_{0}_{1}.mp4", doc.ID, GetCodeLang());
                var dirVideo = GetAppSettings("Welcome.video.directory");
                var pahVideo = System.IO.Path.Combine(dirVideo, videoFile);
                bool bVideoExist = true;
                if (!System.IO.File.Exists(pahVideo))
                {
                    videoFile = string.Format("document_{0}_{1}_{2}.mp4", doc.ID, doc.Version, codeLangDefault);
                    pahVideo = System.IO.Path.Combine(dirVideo, videoFile);
                    if (!System.IO.File.Exists(pahVideo))
                    {
                        bVideoExist = false;
                    }
                }
                if (bVideoExist)
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
                    var error = string.Format("Video {0} n'existe pas sur le disque", pahVideo);
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
            else
            {
                var trad = response.DocumentTrad == null && iddLang != defaulltIdLang ? response.DefaultDocumentTrad : response.DocumentTrad;
                output.WriteAsync(trad.Data, 0, trad.Data.Length);
                FileStreamResult outputAsync = new FileStreamResult(output, "application/pdf");
                output.Position = 0;
                return outputAsync;
            }
        }
        /// <summary>
        /// Télécharger le document informatif sans modifier les actions
        /// </summary>
        /// <param name="documentID"></param>
        /// <param name="inline"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult DownoloadDocInfo(int documentID, Boolean inline)
        {
            int iddLang = GetIdLang();
            var defaulltIdLang = GetDefaultLang();
            var codeLangDefault = _langService.Get(new GetLangRequest() { Id = defaulltIdLang }).CodeLangue;
            var response = _docService.GetDocument(new GetDocumentRequest() { Id = documentID, IdLang = iddLang, DefaulltIdLang = defaulltIdLang });
            Document doc = response.Document;
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
                string videoFile = string.Format("document_{0}_{1}.mp4", doc.ID, GetCodeLang());
                var dirVideo = GetAppSettings("Welcome.video.directory");
                var pahVideo = System.IO.Path.Combine(dirVideo, videoFile);
                bool bVideoExist = true;
                if (!System.IO.File.Exists(pahVideo))
                {
                    videoFile = string.Format("document_{0}_{1}_{2}.mp4", doc.ID, doc.Version, codeLangDefault);
                    pahVideo = System.IO.Path.Combine(dirVideo, videoFile);
                    if (!System.IO.File.Exists(pahVideo))
                    {
                        bVideoExist = false;
                    }
                }
                if (bVideoExist)
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
                    var error = string.Format("Video {0} n'existe pas sur le disque", pahVideo);
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
            else
            {
                var trad = response.DocumentTrad == null && iddLang != defaulltIdLang ? response.DefaultDocumentTrad : response.DocumentTrad;
                output.WriteAsync(trad.Data, 0, trad.Data.Length);
                FileStreamResult outputAsync = new FileStreamResult(output, "application/pdf");
                output.Position = 0;
                return outputAsync;
            }
        }

    }
    
}