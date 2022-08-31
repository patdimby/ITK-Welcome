using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.Helpers;
using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Service.Back;
using Intitek.Welcome.UI.Resources;
using Intitek.Welcome.UI.ViewModels.Admin;
using Intitek.Welcome.UI.Web.Admin.Models;
using OfficeOpenXml;
using OfficeOpenXml.Table;

namespace Intitek.Welcome.UI.Web.Areas.Admin.Controllers
{
    public class UserController : CommunController
    {
        private const string DEFAULT_ENTITY = "Intitek";
        private readonly IUserService _userService;
        private readonly IImportManagerService _importManagerService;
        private readonly IProfilService _profilService;
        private readonly IEntiteService _entiteService;
        private readonly IMailTemplateService _mailTemplateService;
        private readonly IRelanceService _relanceService;

        public string EntityNameSession
        {
            get
            {
                if (Session["EntityNameSessionForUser"] != null)
                    return (string)Session["EntityNameSessionForUser"];
                return null;
            }
            set { Session["EntityNameSessionForUser"] = value; }
        }
        public string AgencyNameSession
        {
            get
            {
                if (Session["AgencyNameSessionForUser"] != null)
                    return (string)Session["AgencyNameSessionForUser"];
                return null;
            }
            set { Session["AgencyNameSessionForUser"] = value; }
        }

        public string EntryDateSession
        {
            get
            {
                if (Session["EntryDateSessionForUser"] != null)
                    return Convert.ToString(Session["EntryDateSessionForUser"]);
                return null;
            }
            set { Session["EntryDateSessionForUser"] = value; }
        }

        public string ExitDateSession
        {
            get
            {
                if (Session["ExitDateSessionForUser"] != null)
                    return Convert.ToString(Session["ExitDateSessionForUser"]);
                return null;
            }
            set { Session["ExitDateSessionForUser"] = value; }
        }

        public int ActifSession
        {
            get
            {
                if (Session["ActifSessionForUser"] != null)
                    return (int)Session["ActifSessionForUser"];
                return 0;
            }
            set { Session["ActifSessionForUser"] = value; }
        }
        public int ActivitySession
        {
            get
            {
                if (Session["ActivitySessionForUser"] != null)
                    return (int)Session["ActivitySessionForUser"];
                return 0;
            }
            set { Session["ActivitySessionForUser"] = value; }
        }
        public List<DocCheckState> SessionAffectUserProfil
        {
            get
            {
                if (Session["SessionAffectUserProfil"] != null)
                    return (List<DocCheckState>)Session["SessionAffectUserProfil"];
                return null;
            }
            set { Session["SessionAffectUserProfil"] = value; }
        }

        public UserController()
        {
            _userService = new UserService(new FileLogger());
            _profilService = new ProfilService(new FileLogger());
            _entiteService = new EntiteService(new FileLogger());
            _mailTemplateService = new MailTemplateService(new FileLogger());
            _relanceService = new RelanceService(new FileLogger());
            _importManagerService = new ImportManagerService(new FileLogger());
        }

        // GET: Admin/User
        public ActionResult Index(GetUserRequest allrequest)
        {
            string nameGrid = "usrGrid";
            GridMvcRequest initrequest = GridBORequest.GetRequestGrid(Request, nameGrid, "Name");
            //nbquery>0 si on click le bouton Rafrachir
            int nbQuery = Request.QueryString.Count;
            //Metre Actif =Oui par défaut
            if (nbQuery==0 && string.IsNullOrEmpty(allrequest.EntityName) && string.IsNullOrEmpty(allrequest.AgencyName) && allrequest.Actif == 0 && allrequest.Activity == 0)
            {
                if (!HasSession(initrequest, "Index"))
                {
                    allrequest.Actif = Options.True;
                }
                else
                {
                    allrequest.AgencyName = AgencyNameSession;
                    allrequest.EntityName = EntityNameSession;
                    allrequest.Actif = ActifSession;
                    allrequest.Activity = ActivitySession;
                    if(!String.IsNullOrEmpty(EntryDateSession))
                        allrequest.EntryDate =  Convert.ToDateTime(EntryDateSession);
                    if (!String.IsNullOrEmpty(ExitDateSession))
                        allrequest.ExitDate = Convert.ToDateTime(ExitDateSession);
                }
            }
            
            
            GridMvcRequest request = base.GetGridRequestSession(initrequest);          
            allrequest.Request = request;
            AgencyNameSession = allrequest.AgencyName;
            EntityNameSession = allrequest.EntityName;
            ActifSession = allrequest.Actif;
            ActivitySession = allrequest.Activity;
            EntryDateSession = allrequest.EntryDate!=null ? Convert.ToString(allrequest.EntryDate) : null;
            ExitDateSession = allrequest.ExitDate !=null ? Convert.ToString(allrequest.ExitDate) : null;

            var nbcount = _userService.ListUsersCount(allrequest);
            var users = _userService.ListUsers(allrequest);
            var viewModels = AutoMapperConfigAdmin.Mapper.Map<List<UserViewModel>>(users);
            var grid = new GridBO<UserViewModel>(request, viewModels, nbcount, request.Limit);

            UserResponseViewModel response = new UserResponseViewModel();
            response.EntityName = allrequest.EntityName;
            response.AgencyName = allrequest.AgencyName;
            response.AgencyNameList = _entiteService.AgencyByEntity(allrequest.EntityName, false, true, false);
            response.EntityNameList = new List<string>() { string.Empty };
            response.EntityNameList.AddRange(_userService.EntityNameList(false));
            response.Actif = allrequest.Actif.ToString();
            response.EntryDate = allrequest.EntryDate;
            response.ExitDate = allrequest.ExitDate;
            response.Activity = allrequest.Activity.ToString();
            response.ActifList = new List<StringOption>() { new StringOption() { Value = "", Text = Resource.All }, new StringOption() { Value= Options.True.ToString(), Text= Resource.yes}, new StringOption() { Value = Options.False.ToString(), Text = Resource.no } };
            response.ListUser = grid;
            return View(response);
        }

        [HandleError(View = "~/Areas/Admin/Views/Shared/AjaxError.cshtml")]
        public ActionResult AjaxUserGrid(GetUserRequest allrequest)
        {
            string nameGrid = "usrGrid";
            //_adService = GetActiveDirectoryService(GetUserConnectedAD());
            GridMvcRequest request = GridBORequest.GetRequestGrid(Request, nameGrid, "Name");
            allrequest.Request = request; 
            base.SetGridRequestSession(request, "Index");
            var nbcount = _userService.ListUsersCount(allrequest);
            var users = _userService.ListUsers(allrequest);
            var viewModels = AutoMapperConfigAdmin.Mapper.Map<List<UserViewModel>>(users);
            var grid = new GridBO<UserViewModel>(request, viewModels, nbcount, allrequest.Request.Limit);
            return Json(new { Html = grid.ToJson("_UserGrid", new ViewDataDictionary { { "EntityName", allrequest.EntityName } }, this) }, JsonRequestBehavior.AllowGet);
       }
        public ActionResult ChangeAgences(string EntityName)
        {
           var agencyNameList = _entiteService.AgencyByEntity(EntityName, false, true, false);
            return Json(new { Agences = agencyNameList, success=1 }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Edit(int Id = 0)
        {
            return View(new UserViewModel()
            {
                ID = Id,
                Name = string.Empty
            });
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult Edit(UserViewModel model)
        {
            return View();
        }
        public ActionResult Affected(GetUserRequest allrequest)
        {
            SaveUserRequest userRequest = new SaveUserRequest();
            userRequest.InitAffectDocs();
            this.SessionAffectUserProfil = userRequest.DocsAffected;

            var user = _userService.Get(allrequest).User;
            IntitekUser manager = null;
            ImportManager importManager = null;
            if (user.ID_Manager != null)
            {
                manager = _userService.Get(new GetUserRequest() { Id = (int)user.ID_Manager }).User;
                if (manager.Email != null)
                {
                    importManager = _importManagerService.GetImportManager(manager);
                }
            }
            var userVm = new UserViewModel()
            {
                ID = user.ID,
                Name = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                IsOnBoarding = user.isOnBoarding,
                EmailOnBoarding = user.EmailOnBoarding,
                EntityName = user.EntityName,
                AgencyName = user.AgencyName,
                Status = user.Status.HasValue ? user.Status : 10,
                Active = user.Active,
                Activity = _userService.GetActivityUser(user.ID),
                InactivityReason = user.InactivityReason,
                InactivityStart = user.InactivityStart,
                InactivityEnd = user.InactivityEnd,
                Departement = user.Departement,
                Division = user.Division,
                Type = user.Type,
                ProfilList = _userService.ListProfileByUserId(user.ID),
                EntryDate = user.EntryDate,
                ExitDate = user.ExitDate,
                Manager = manager,
                ImportManager=importManager,
               isReader=user.isReader
            };
            userVm.IsRoot = userVm.GetIsRoot;
            string nameGrid = "prfGrid";
            allrequest.Request = GridBORequest.GetRequestGrid(Request, nameGrid, "Name");
            var profils = _userService.ListProfil(allrequest);
            var pVm = new UserResponseViewModel();
            var viewModels = AutoMapperConfigAdmin.Mapper.Map<List<ProfilViewModel>>(profils);
            var grid = new GridBO<ProfilViewModel>(nameGrid, viewModels, null, -1);
            pVm.User = userVm;
            pVm.ListProfil = grid;
            pVm.ListProfil = grid;
            return View(pVm);
        }
        [HttpPost]
        [ValidateInput(true)]
        public ActionResult AffectProfilSession(DocCheckState model)
        {
            SaveUserRequest userRequest = new SaveUserRequest();
            userRequest.DocsAffected = this.SessionAffectUserProfil;
            userRequest.ToAffectDocs(model);
            this.SessionAffectUserProfil = userRequest.DocsAffected;
            return Json(new { Success = 1 }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateInput(true)]
        public ActionResult Affected(SaveUserRequest model)
        {
            model.DocsAffected = SessionAffectUserProfil;
            _userService.Save(model);
            return RedirectToAction("Index", "User");
        }
        [HandleError(View = "~/Areas/Admin/Views/Shared/AjaxError.cshtml")]
        public ActionResult AjaxProfilGrid()
        {
            int userId = 0;
            int.TryParse(Request.QueryString["UserID"], out userId);
            string nameGrid = "prfGrid";
            GetUserRequest allrequest = new GetUserRequest() { Id = userId };
            var user = _userService.Get(allrequest).User;
            allrequest.DocsAffected = this.SessionAffectUserProfil;
            allrequest.Request = GridBORequest.GetRequestGrid(Request, nameGrid, "Name");
            var profils = _userService.ListProfil(allrequest);
            var viewModels = AutoMapperConfigAdmin.Mapper.Map<List<ProfilViewModel>>(profils);
            var grid = new GridBO<ProfilViewModel>(nameGrid, viewModels, null, -1);
            return Json(new { Html = grid.ToJson("_ProfilGrid", new ViewDataDictionary { { "UsrActive", user.Active } }, this) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DocumentState(GetUserRequest allrequest)
        {
            var user = _userService.Get(allrequest).User;
            IntitekUser manager = null;
            ImportManager importManager = null;
            if (user.ID_Manager != null)
            {
                manager = _userService.Get(new GetUserRequest() { Id = (int)user.ID_Manager }).User;
                if (manager.Email != null)
                {
                    importManager = _importManagerService.GetImportManager(manager);
                }
            }
            var userVm = new UserViewModel()
            {
                ID = user.ID,
                Name = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                IsOnBoarding = user.isOnBoarding,
                EmailOnBoarding = user.EmailOnBoarding,
                EntityName = user.EntityName,
                AgencyName = user.AgencyName,
                Status = user.Status.HasValue ? user.Status : 10,
                Active = user.Active,
                Type = user.Type,
                ProfilList = _userService.ListProfileByUserId(user.ID),
                Manager = manager,
                ImportManager=importManager
            };

            string nameGrid = "docGrid";
            allrequest.Request = GridBORequest.GetRequestGrid(Request, nameGrid, "Name");
            allrequest.IdLang = GetIdLang();
            allrequest.IdDefaultLang = GetDefaultLang();
            var total = _userService.ListDocumentReadByLoginCount(allrequest);
            var docs = _userService.ListDocumentReadByLogin(allrequest);
            var viewModels = AutoMapperConfigAdmin.Mapper.Map<List<DocumentViewModel>>(docs);
            var pVm = new UserResponseViewModel();
            var grid = new GridBO<DocumentViewModel>(nameGrid, viewModels, total, allrequest.Request.Limit);
            pVm.ListDocument = grid;
            pVm.User = userVm;
            return View(pVm);
        }
        [HandleError(View = "~/Views/Shared/AjaxError.cshtml")]
        public ActionResult AjaxDocumentState(GetUserRequest allrequest)
        {
            string nameGrid = "docGrid";
            allrequest.Request = GridBORequest.GetRequestGrid(Request, nameGrid, "Name");
            allrequest.IdLang = GetIdLang();
            allrequest.IdDefaultLang = GetDefaultLang();
            var total = _userService.ListDocumentReadByLoginCount(allrequest);
            var docs = _userService.ListDocumentReadByLogin(allrequest);
            var viewModels = AutoMapperConfigAdmin.Mapper.Map<List<DocumentViewModel>>(docs);
            var grid = new GridBO<DocumentViewModel>(nameGrid, viewModels, total, allrequest.Request.Limit);
            return Json(new { Html = grid.ToJson("_DocumentGrid", this), total }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchProfils()
        {
            var items = _profilService.GetAll();
            return Json(new { Items = items }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchAgenceByEntity(string entity)
        {
            var items = _entiteService.AgencyByEntity(entity, true, false, false);
            return Json(new { Items = items }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Relance(int Id)
        {
            var user = _userService.Get(new GetUserRequest() { Id= Id}).User;
            var userVm = new UserViewModel()
            {
                ID = user.ID,
                Name = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                EntityName = user.EntityName,
                AgencyName = user.AgencyName,
            };
            var lstMailTemplate = _mailTemplateService.GetAllTemplateRemind();
            var vm = new RelanceViewModel() { User = userVm, ListTemplate = lstMailTemplate };
            return PartialView(vm);
        }
        [HttpPost]
        [ValidateInput(true)]
        public ActionResult Relance(GetRelanceRequest request)
        {
            request.IdLang = GetIdLang();
            request.IdDefaultLang = GetDefaultLang();
            var mail = _mailTemplateService.Get(new GetMailTemplateRequest() { Id = request.MailTemplateID });
            var user = _userService.Get(new GetUserRequest() { Id = request.UserID }).User;
            var userDTO = new UserDTO()
            {
                ID = user.ID,
                Name = user.FullName,
                Status = user.Status,
                //EmailOnBoarding = user.EmailOnBoarding,
                EmailOnBoarding = user.Email,
                IsOnBoarding = user.isOnBoarding,
                EntityName = user.EntityName,
                AgencyName = user.AgencyName,
            };
            var lstDTO = new List<UserDTO>();
            if (_userService.GetActivityUser(user.ID))
            {
                lstDTO.Add(userDTO);
            }
            request.LstUsers = lstDTO;
            request.MailTemplate = mail.MailTemplate;
            string message = "";
            bool ret = _relanceService.Execute(request, out message);
            if(ret)
            {
                return Json(new { success = 1 , Message = Resource.relanceMessageSuccess }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = 0, Message= string.Format(Resource.relanceMessageFailed, "<div>"+message+"</div>") }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult LoadInactif()
        {
            ImportInactifViewModel vm = new ImportInactifViewModel();
            return View(vm);
        }
        [HttpPost]
        [ValidateInput(true)]
        public ActionResult LoadInactif(ImportInactifViewModel vm)
        {
            vm.Posted = true;
            vm.SetEmptyError();
            if (ModelState.IsValid)
            {
                List<IntitekUser> usersUpdated = new List<IntitekUser>();
                Stream fs = vm.FileUploadXls.InputStream;
                ExcelPackage package = new ExcelPackage(fs);
                ExcelWorksheets workSheets = package.Workbook.Worksheets;
                ExcelTable table = null;
                if (workSheets.Count > 0)
                {
                    ExcelWorksheet workSheet = workSheets.First();
                    if (workSheet.Tables.Any())
                    {
                        table = workSheet.Tables.FirstOrDefault();
                    }
                    else
                    {
                        table = workSheet.Tables.Add(workSheet.Dimension, "tblUsers");
                    }
                    //Get the cells based on the table address
                    List<IGrouping<int, ExcelRangeBase>> groups = table.WorkSheet.Cells[table.Address.Start.Row, table.Address.Start.Column, table.Address.End.Row, table.Address.End.Column]
                        .GroupBy(cell => cell.Start.Row)
                        .ToList();
                    //Assume first row has the column names
                    var colnames = groups.FirstOrDefault()
                        .Select((hcell, idx) => new
                        {
                            Name = hcell.Value != null ? hcell.Value.ToString() : string.Empty,
                            NameId = hcell.Value != null ? EdmxFunction.RemoveAccent(hcell.Value.ToString()) : string.Empty,
                            index = idx
                        })
                        .ToList();
                    int indexRow = 1;
                    //Nom ou Prénom ou Email obligatoires
                    var bFileCorrect = colnames.Select(x => x.NameId).Where(x => new List<string>() { "nom", "prenom", "email" }.Contains(x)).Any();
                    if (bFileCorrect)
                    {
                        bFileCorrect = colnames.Select(x => x.NameId).Where(x => x.Equals("debut")).Any();
                        if (!bFileCorrect)
                        {
                            vm.AddError(indexRow, Resource.inact_columns_required);
                        }
                    }
                    else
                    {
                        vm.AddError(indexRow, Resource.inact_columns_required);
                    }
                    //Everything after the header is data
                    //List<List<object>> rowvalues = groups
                    //    .Skip(1) //Exclude header
                    //    .Select(cg => cg.Select(c => c.Value).ToList()).ToList();

                    var datavalues = groups
                        .Skip(1) //Exclude header
                        .Select(cg => cg.Select(c=> new { ColumnIndex=c.Start.Column-1, Value=c.Value}).ToList()).ToList();
                    if (vm.HasError(indexRow))
                    {
                        return View("LoadInactif", vm);
                    }
                    indexRow = 2;
                    int nbRows = datavalues.Count;
                    int nbRowsLoaded = 0;
                    foreach (var row in datavalues)
                    {
                        bool bRowIsNotEmpty = row.Select(x=>x.Value).Where(x => x != null && !string.IsNullOrEmpty(x.ToString().Trim())).Any();
                        if (!bRowIsNotEmpty)
                        {
                            indexRow++;
                            continue;
                        }
                        UserDTO userDTO = new UserDTO();
                        var firstname = "";
                        foreach (var colname in colnames)
                        {
                            object val = null;
                            try
                            {
                                val = row.Where(x=> x.ColumnIndex== colname.index).Select(x=>x.Value).FirstOrDefault();
                            }
                            catch (SystemException) { }                           
                            switch (colname.NameId)
                            {
                                case "nom":
                                    userDTO.Name = val != null ? val.ToString().Trim() : string.Empty;
                                    break;
                                case "prenom":
                                    firstname = val != null ? val.ToString().Trim() : string.Empty;
                                    break;
                                case "email":
                                    userDTO.Email = val != null ? val.ToString().Trim() : string.Empty;
                                    if(!string.IsNullOrEmpty(userDTO.Email) && !Utils.IsValidEmail(userDTO.Email))
                                    {
                                        vm.AddError(indexRow, string.Format(Resource.inact_column_invalide, indexRow.ToString("D4"), "Email", userDTO.Email));
                                    }
                                    break;
                                case "motif":
                                    userDTO.InactivityReason = val != null ? val.ToString().Trim() : string.Empty;
                                    break;
                                case "debut":
                                    if (val != null)
                                    {
                                        if (val is DateTime)
                                        {
                                            userDTO.InactivityStart = (DateTime)val;
                                        }
                                        else
                                        {
                                            try
                                            {
                                                userDTO.InactivityStart = DateTime.ParseExact(val.ToString().Trim(), "dd/MM/yyyy", CultureInfo.GetCultureInfo("fr-FR"));
                                            }
                                            catch(Exception)
                                            {
                                                vm.AddError(indexRow, string.Format(Resource.inact_column_invalide, indexRow.ToString("D4"), "Début", val.ToString().Trim()));
                                            }

                                        }
                                    }
                                    break;
                                case "fin":
                                    if (val != null)
                                    {
                                        if (val is DateTime)
                                        {
                                            userDTO.InactivityEnd = (DateTime)val;
                                        }
                                        else
                                        {
                                            try
                                            {
                                                userDTO.InactivityEnd = DateTime.ParseExact(val.ToString().Trim(), "dd/MM/yyyy", CultureInfo.GetCultureInfo("fr-FR"));
                                            }
                                            catch (Exception)
                                            {
                                                vm.AddError(indexRow, string.Format(Resource.inact_column_invalide, indexRow.ToString("D4"), "Fin", val.ToString().Trim()));
                                            }

                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        userDTO.FullName = !string.IsNullOrEmpty(firstname) ? firstname + " " + userDTO.Name : userDTO.Name;
                        //Ligne incorrecte Nom, Prenom, Email obligatoire
                        if(string.IsNullOrEmpty(userDTO.FullName + userDTO.Email))
                        {
                            vm.AddError(indexRow, string.Format(Resource.inact_line_incorrect, indexRow.ToString("D4")));
                        }
                        //Comparaison Date debut et fin
                        if (userDTO.InactivityEnd.HasValue)
                        {
                            if (userDTO.InactivityStart.HasValue)
                            {
                                if (userDTO.InactivityStart > userDTO.InactivityEnd)
                                {
                                    vm.AddError(indexRow, string.Format(Resource.inact_start_end_invalide, indexRow.ToString("D4"), userDTO.InactivityEnd.Value.ToString("dd/MM/yyyy"), userDTO.InactivityStart.Value.ToString("dd/MM/yyyy")));
                                }
                            }
                            else
                            {
                                if (DateTime.Today > userDTO.InactivityEnd)
                                {
                                    vm.AddError(indexRow, string.Format(Resource.inact_start_end_invalide, indexRow.ToString("D4"), userDTO.InactivityEnd.Value.ToString("dd/MM/yyyy"), string.Format(Resource.inact_start_empty, DateTime.Today.ToString("dd/MM/yyyy"))));
                                }
                            }
                        }
                        List<IntitekUser> users = new List<IntitekUser>() ;
                        IntitekUser user = null;
                        if (!string.IsNullOrEmpty(userDTO.Email) && Utils.IsValidEmail(userDTO.Email))
                        {
                            user = _userService.GetIntitekUserByEmail(userDTO.Email);
                            if (user != null)
                            {
                                users.Add(user);
                            }
                        }
                        if (user == null && !string.IsNullOrWhiteSpace(userDTO.FullName))
                        {
                            var tmpusers = _userService.GetIntitekUserByFullname(userDTO.FullName);
                            if(tmpusers != null)
                            {
                                user = tmpusers.FirstOrDefault();
                                users.AddRange(tmpusers);
                            }
                               
                        }
                        //Test l'existence d'un utilisateur en base
                        if(user==null)
                        {
                            if (!string.IsNullOrEmpty(userDTO.Email) && Utils.IsValidEmail(userDTO.Email))
                            {
                                vm.AddError(indexRow, string.Format(Resource.inact_column_notexist, indexRow.ToString("D4"), "Email", userDTO.Email));
                            }
                            if (!string.IsNullOrEmpty(userDTO.FullName))
                            {
                                vm.AddError(indexRow, string.Format(Resource.inact_column_notexist, indexRow.ToString("D4"), "Fullname", userDTO.FullName));
                            }

                        }
                        if (!vm.HasError(indexRow))
                        {
                            nbRowsLoaded += 1;
                            foreach (var usr in users)
                            {
                                usr.Id = usr.ID; //en mode modification
                                usr.InactivityStart = userDTO.InactivityStart.HasValue ? userDTO.InactivityStart : DateTime.Today;
                                usr.InactivityEnd = userDTO.InactivityEnd;
                                if(userDTO.InactivityReason.Length>512)
                                    usr.InactivityReason = userDTO.InactivityReason.Substring(0, 512);
                                else
                                    usr.InactivityReason = userDTO.InactivityReason;
                                usersUpdated.Add(usr);
                            }
                        }
                        indexRow++;
                    }
                    vm.NbRowsLoaded = nbRowsLoaded;
                    vm.NbRows = nbRows;
                    _userService.RemoveAllInactivity();
                }//worksheet.count
               
                if (usersUpdated.Any())
                {
                    _userService.UpdateAll(usersUpdated);
                }

           }//ModelState.isvalid
           return View("LoadInactif", vm);
        }
        
    }
}