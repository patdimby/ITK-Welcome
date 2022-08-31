using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Service.Back;
using Intitek.Welcome.UI.Resources;
using Intitek.Welcome.UI.ViewModels.Admin;
using Intitek.Welcome.UI.Web.Admin;
using Intitek.Welcome.UI.Web.Admin.Models;
using Intitek.Welcome.UI.Web.Infrastructure;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Intitek.Welcome.UI.Web.Areas.Admin.Controllers
{
    public class StatsController : CommunController
    {
        private readonly IStatsService _statsService;
        private readonly IEntiteService _entiteService;
        private readonly IUserService _userService;
        
        public StatsController()
        {
            _statsService = new StatsService(new FileLogger());
            _userService = new UserService(new FileLogger());
            _entiteService = new EntiteService(new FileLogger());
        }
        // GET: Stats
        public ActionResult Index()
        {
            int idlang = GetIdLang();
            StatsResponseViewModel vm = new StatsResponseViewModel();
            var documents = _statsService.ListDocuments(idlang, GetDefaultLang());
            vm.ListDocument = AutoMapperConfigAdmin.Mapper.Map<List<DocumentViewModel>>(documents);
            List<string> entities = _userService.EntityNameList(true);
            foreach (var entity in entities)
            {
                int nb = _userService.GetCountUserActiveByEntityName(entity);
                if (nb > 0)
                {
                    List<string> agencies = _entiteService.AgencyByEntity(entity, true, false, true);
                    List<OptionStatDTO> subList = new List<OptionStatDTO>();
                    if (agencies == null || agencies.Count == 0)
                    {
                        subList.Add(new OptionStatDTO() { Nb = nb, Key = null, Text = Resource.noAgence });
                    }
                    else
                    {
                        foreach (var agency in agencies)
                        {
                            int nbAgency = _userService.GetCountUserActiveByAgencyName(entity, agency);
                            if (nbAgency > 0)
                            {
                                string strAgency = string.IsNullOrEmpty(agency) ? Resource.noAgence : agency;
                                subList.Add(new OptionStatDTO() { Nb = nbAgency, Key = agency, Text = string.Format("{0} ({1})", strAgency, nbAgency) });
                            }
                        }
                    }
                    vm.EntityNameList.Add(new OptionStatDTO() { Nb = nb, Key = entity, Text = string.Format("{0} ({1})", entity, nb), SubList = subList });
                }

            }
            vm.EmployeesList = new List<IntOption>() { new IntOption() { Value = 0, Text = Resource.EmployeesAll }, new IntOption() { Value = Options.True, Text = Resource.EmployeesActif }, new IntOption() { Value = Options.False, Text = Resource.EmployeesInactif } };
            vm.NbDay = 7;
            vm.EndDate = DateTime.Today;
            vm.StartDate = new DateTime(2000, 1, 1);
            return View(vm);
        }
        [HttpPost]
        [ValidateInput(true)]
        public ActionResult Index(GetStatsRequest statsRequest)
        {
            int idlang = GetIdLang();
            statsRequest.IdLang = GetIdLang();
            statsRequest.IdDefaultLang = GetDefaultLang();
            StatsResponseViewModel vm = new StatsResponseViewModel();
            vm.MultiDocSelect = statsRequest.MultiDocSelect;
            vm.MultiEntitySelect = statsRequest.MultiEntitySelect;
            vm.EmployeeSelect = statsRequest.EmployeeSelect;
            var documents = _statsService.ListDocuments(idlang, GetDefaultLang());
            vm.ListDocument = AutoMapperConfigAdmin.Mapper.Map<List<DocumentViewModel>>(documents);
            List<string> entities = _userService.EntityNameList(true);
            foreach (var entity in entities)
            {
                int nb = _userService.GetCountUserActiveByEntityName(entity);
                if (nb > 0)
                {
                    List<string> agencies = _entiteService.AgencyByEntity(entity, true, false, true);
                    List<OptionStatDTO> subList = new List<OptionStatDTO>();
                    if (agencies == null || agencies.Count == 0)
                    {
                        subList.Add(new OptionStatDTO() { Nb = nb, Key = null, Text = Resource.noAgence });
                    }
                    else
                    {
                        foreach (var agency in agencies)
                        {
                            int nbAgency = _userService.GetCountUserActiveByAgencyName(entity, agency);
                            if (nbAgency > 0)
                            {
                                string strAgency = string.IsNullOrEmpty(agency) ? Resource.noAgence : agency;
                                subList.Add(new OptionStatDTO() { Nb = nbAgency, Key = agency, Text = string.Format("{0} ({1})", strAgency, nbAgency) });
                            }
                        }
                    }
                    vm.EntityNameList.Add(new OptionStatDTO() { Nb = nb, Key = entity, Text = string.Format("{0} ({1})", entity, nb), SubList = subList });
                }

            }
            vm.EmployeesList = new List<IntOption>() { new IntOption() { Value = 0, Text = Resource.EmployeesAll }, new IntOption() { Value = Options.True, Text = Resource.EmployeesActif }, new IntOption() { Value = Options.False, Text = Resource.EmployeesInactif } };
            vm.NbDay = statsRequest.NbDay;
            vm.Periode = DateTime.Today;
            vm.EndDate = statsRequest.EndDate;
            vm.StartDate = statsRequest.StartDate;
            statsRequest.Periode = vm.Periode;

            statsRequest.StatType = StatsRequestType.Agency;
            var stats = _statsService.GetStats(statsRequest, false);
            GetStatsResponse reponse = new GetStatsResponse();
            reponse.SetEntityStats(stats);
            vm.ReponseEntityNames = reponse.EntityNameList;
            int indexGrid = 0;
            foreach (var lst in reponse.DicoAgencyStats)
            {
                var grid = new GridBO<Statistiques>("agGrid_" + indexGrid, lst, null, -1);
                vm.GridStats.Add(grid);
                indexGrid++;
            }
            vm.Total = reponse.Total;
            vm.ToApproved = reponse.ToApproved;
            vm.ToTested = reponse.ToTested;
            vm.NotRead = reponse.NotRead;
            vm.NotApproved = reponse.NotApproved;
            vm.NotTested = reponse.NotTested;
            GetRythmeResponse reponseRythme = new GetRythmeResponse(statsRequest);
            reponseRythme.Documents = documents;
            reponseRythme.GetDatasets(stats);
            vm.Datasets = reponseRythme.Datasets;
            return View("Index", vm);
        }
        [HttpPost]
        [ValidateInput(true)]
        public ActionResult RythmeAjax(GetStatsRequest statsRequest)
        {
            statsRequest.IdLang = GetIdLang();
            statsRequest.IdDefaultLang = GetDefaultLang();
            var documents = _statsService.ListDocuments(GetIdLang(), GetDefaultLang());
            StatsResponseViewModel vm = new StatsResponseViewModel();
            vm.MultiDocSelect = statsRequest.MultiDocSelect;
            vm.MultiEntitySelect = statsRequest.MultiEntitySelect;
            vm.EmployeeSelect = statsRequest.EmployeeSelect;

            vm.NbDay = statsRequest.NbDay;
            vm.Periode = DateTime.Today;
            statsRequest.Periode = vm.Periode;

            statsRequest.StatType = StatsRequestType.Agency;
            var stats = _statsService.GetStats(statsRequest, false);
            GetRythmeResponse reponse = new GetRythmeResponse(statsRequest);
            reponse.Documents = documents;
            reponse.GetDatasets(stats);
            vm.Datasets = reponse.Datasets;
            return PartialView("_Chart", vm);
        }
        /// <summary>
        /// Execute stats computations
        /// </summary>
        /// <param name="model"></param>
        /// <param name="SubmitButton"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(true)]
        public ActionResult DoStats(GetStatsRequest statsRequest)
        {
            statsRequest.IdLang = GetIdLang();
            statsRequest.IdDefaultLang = GetDefaultLang();
            statsRequest.StatType = StatsRequestType.Agency;
            if ("ExcelLate".Equals(statsRequest.SubmitButton)){
                return this._Excel(statsRequest, true);
            }
            else if ("ExcelFull".Equals(statsRequest.SubmitButton)){
                return this._Excel(statsRequest, false);
            }
            return null ;
        }
        /// <summary>
        /// Execute stats computations
        /// </summary>
        /// <param name="model"></param>
        /// <param name="SubmitButton"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(true)]
        public ActionResult DoStatsDepartement(GetStatsRequest statsRequest)
        {
            statsRequest.IdLang = GetIdLang();
            statsRequest.IdDefaultLang = GetDefaultLang();
            statsRequest.StatType = StatsRequestType.Departement;
            if ("ExcelLate".Equals(statsRequest.SubmitButton))
            {
                return this._Excel(statsRequest, true);
            }
            else if ("ExcelFull".Equals(statsRequest.SubmitButton))
            {
                return this._Excel(statsRequest, false);
            }
            return null;
        }
        /// <summary>
        /// Execute stats computations
        /// </summary>
        /// <param name="model"></param>
        /// <param name="SubmitButton"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(true)]
        public ActionResult DoStatsManager(GetStatsRequest statsRequest)
        {
            statsRequest.IdLang = GetIdLang();
            statsRequest.IdDefaultLang = GetDefaultLang();
            statsRequest.StatType = StatsRequestType.Manager;
            if ("ExcelLate".Equals(statsRequest.SubmitButton))
            {
                return this._Excel(statsRequest, true);
            }
            else if ("ExcelFull".Equals(statsRequest.SubmitButton))
            {
                return this._Excel(statsRequest, false);
            }
            return null;
        }
        /// <summary>
        /// Export data to EXCEL
        /// </summary>
        /// <param name="model"></param>
        /// <param name="OnlyLatePeople"></param>
        private FileDownloadResult _Excel(GetStatsRequest statsRequest, bool onlyLatePeople)
        {
            var fileName = onlyLatePeople ? string.Format("{0}.xlsx", Resource.stat_filename_relance)
                                              : string.Format("{0}.xlsx", Resource.stat_filename_all);
            switch (statsRequest.StatType)
            {
                case StatsRequestType.Departement:
                    fileName = onlyLatePeople ? string.Format("{0}.xlsx", Resource.stat_filename_relance+"-direction")
                                              : string.Format("{0}.xlsx", Resource.stat_filename_all + "-direction");
                    break;
                case StatsRequestType.Manager:
                    fileName = onlyLatePeople ? string.Format("{0}.xlsx", Resource.stat_filename_relance + "-manager")
                                              : string.Format("{0}.xlsx", Resource.stat_filename_all + "-manager");
                    break;
                default:
                    break;
            }
            var stats = _statsService.GetEngineerList(statsRequest, onlyLatePeople);
            ExcelRange XlsRange;    // Excel range
            try
            {
                ExcelPackage HndExcel = new ExcelPackage(); // Handle on EXCEL
                int X, Y,   // Position in sheet
                    StartTable;  // First line of table

                // Create workbook
                ExcelWorksheet HndXls = HndExcel.Workbook.Worksheets.Add(Resource.stat_workbookname);

                // Add title and date
                X = Y = 1;
                HndXls.Cells[Y, 1].Value = Resource.stat_title;
                HndXls.Cells[Y, 1].Style.Font.UnderLine = true;
                HndXls.Cells[Y, 1].Style.Font.Bold = true;
                HndXls.Cells[Y, 1].Style.Font.Size = 20;
                Y++;
                HndXls.Cells[Y, 1].Value = Resource.stat_Employees + " :";
                HndXls.Cells[Y, 1].Style.Font.UnderLine = true;
                HndXls.Cells[Y, 1].Style.Font.Bold = true;
                HndXls.Cells[Y, 2].Value = onlyLatePeople ? Resource.stat_ToRelance
                                                          : Resource.stat_All;
                Y++;
                HndXls.Cells[Y, 1].Value = Resource.stat_Date + " :";
                HndXls.Cells[Y, 1].Style.Font.UnderLine = true;
                HndXls.Cells[Y, 1].Style.Font.Bold = true;
                HndXls.Cells[Y, 2].Value = DateTime.Now.ToShortDateString();
                // Declare filters on documents
                X = 1;
                Y++;
                HndXls.Cells[Y, 1].Value = Resource.stat_Documents+" :";
                HndXls.Cells[Y, 1].Style.Font.UnderLine = true;
                HndXls.Cells[Y, 1].Style.Font.Bold = true;
                Y++;
                if (statsRequest.MultiDocSelect != null)
                {
                    var documents = _statsService.ListDocumentsByDocs(statsRequest.IdLang, statsRequest.IdDefaultLang, statsRequest.MultiDocSelect.ToList());
                    foreach (var doc in documents)
                    {
                        HndXls.Cells[Y++, 2].Value = System.Net.WebUtility.HtmlDecode(doc.Name);
                    }
                }
                

                // Declare filters on entities
                Y++;
                switch (statsRequest.StatType)
                {
                    case StatsRequestType.Agency:
                        HndXls.Cells[Y, 1].Value = Resource.stat_Entities + " :";
                        break;
                    case StatsRequestType.Departement:
                        HndXls.Cells[Y, 1].Value = Resource.stat_Departement + " :";
                        break;
                    case StatsRequestType.Manager:
                        HndXls.Cells[Y, 1].Value = Resource.stat_Manager + " :";
                        break;
                    default:
                        break;
                }
                HndXls.Cells[Y, 1].Style.Font.UnderLine = true;
                HndXls.Cells[Y, 1].Style.Font.Bold = true;
                Y++;
                switch (statsRequest.StatType)
                {
                    case StatsRequestType.Agency:
                        if (statsRequest.MultiEntitySelect != null)
                        {
                            foreach (string entityAg in statsRequest.MultiEntitySelect)
                            {
                                string[] tab = entityAg.Split('|');
                                string entity = tab[0];
                                string agency = tab[1];
                                HndXls.Cells[Y++, 2].Value = entity + " / " + agency;
                            }
                        }
                        break;
                    case StatsRequestType.Departement:
                        if (statsRequest.MultiDepartementSelect != null)
                        {
                            foreach (string dep in statsRequest.MultiDepartementSelect)
                            {
                                var depart = dep;
                                if (Constante.NO_DIRECTION_ID.Equals(dep))
                                    depart = Resource.NoDepartement;
                                HndXls.Cells[Y++, 2].Value = depart;
                            }
                        }
                        break;
                    case StatsRequestType.Manager:
                        if (statsRequest.MultiManagerSelect != null)
                        {
                            var managerList = _userService.ManagerList();
                            foreach (string manager in statsRequest.MultiManagerSelect)
                            {
                                string[] tab = manager.Split('|');
                                string departement = tab[0];
                                int id_manager = Int32.Parse(tab[1]);
                                string division = managerList.Where(x => x.Departement.Equals(departement) && x.ID_Manager.HasValue && x.ID_Manager == id_manager).Select(x => x.Division).FirstOrDefault();
                                HndXls.Cells[Y++, 2].Value = string.Format("{0} ({1})", division, departement);
                            }
                        }
                        break;
                    default:
                        break;
                }
                
                    

                // Globals Statistics
                Int32 AllOk = 0, ReadOk = 0, ApprovedOk = 0, TestOk = 0, NbEngineer; // Totals
                NbEngineer = stats.Count;
                foreach (var stat in stats)
                {
                    ReadOk += stat.NotRead==0 ? 1 : 0;
                    ApprovedOk += stat.NotApproved == 0 ? 1 : 0;
                    TestOk += stat.NotTested== 0 ? 1 : 0;
                    AllOk += (stat.NotRead == 0 && stat.NotApproved == 0 && stat.NotTested == 0) ? 1 : 0 ;
                }
                Y++;
                HndXls.Cells[Y, 1].Value = Resource.stat_stats_general + " :";
                HndXls.Cells[Y, 1].Style.Font.UnderLine = true;
                HndXls.Cells[Y, 1].Style.Font.Bold = true;

                Y++;
                HndXls.Cells[Y, 2, Y, 5].Merge = true;
                HndXls.Cells[Y, 2].Value = Resource.stat_nbCollabTotal;
                HndXls.Cells[Y, 2].Style.Font.Bold = true;
                HndXls.Cells[Y, 6, Y, 8].Merge = true;
                //Nb collab
                HndXls.Cells[Y, 6].Value = NbEngineer;
                HndXls.Cells[Y, 6].Style.Font.Bold = true;
                Y++;
                HndXls.Cells[Y, 2, Y, 5].Merge = true;
                HndXls.Cells[Y, 2].IsRichText = true;
                var ExcelRichText = HndXls.Cells[Y, 2].RichText.Add(Resource.stat_nbParcours_Sensibilisation);
                ExcelRichText.Bold = true;
                HndXls.Cells[Y, 2].RichText.Add("\r\n");
                HndXls.Cells[Y, 2].Style.WrapText = true;
                ExcelRichText = HndXls.Cells[Y, 2].RichText.Add("("+ Resource.stat_read_approve_qcmSuccess + ")");
                ExcelRichText.Bold = false;
                ExcelRichText.Italic = true;
                ExcelRichText.Size = 8;
                HndXls.Cells[Y, 6, Y, 8].Merge = true;
                //Effectués l'intégralité du parcours
                HndXls.Cells[Y, 6].Value = AllOk;
                HndXls.Cells[Y, 6].Style.Font.Bold = true;

                Y++;
                HndXls.Cells[Y, 2, Y, 5].Merge = true;
                HndXls.Cells[Y, 2].Value = Resource.stat_percent_collab_sensibilise;
                HndXls.Cells[Y, 2].Style.Font.Bold = true;
                HndXls.Cells[Y, 6, Y, 8].Merge = true;
                HndXls.Cells[Y, 6].Formula = String.Format("=F{0}/F{1}", Y - 1, Y - 2);
                HndXls.Cells[Y, 6].Style.Numberformat.Format = "0%";
                HndXls.Cells[Y, 6].Style.Font.Bold = true;
                HndXls.Cells[Y, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                //Poucentage
                if (NbEngineer > 0)
                {
                    HndXls.Cells[Y, 6].Style.Fill.BackgroundColor.SetColor((AllOk * 100f / NbEngineer) > 95 ? Color.Green : Color.Red);
                }

                HndXls.Cells[Y - 2, 2, Y, 8].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                HndXls.Cells[Y - 2, 2, Y, 8].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                HndXls.Cells[Y - 2, 2, Y, 8].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                HndXls.Cells[Y - 2, 2, Y, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                HndXls.Cells[Y - 2, 2, Y, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                HndXls.Cells[Y - 2, 2, Y, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                HndXls.Cells[Y - 2, 2, Y, 5].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(248, 203, 173));
                HndXls.Cells[Y - 2, 2, Y, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                HndXls.Cells[Y - 2, 2, Y, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                // Detailled statistics
                Y += 2;
                HndXls.Cells[Y, 1].Value = Resource.stat_detail_etape_parcours_sensibilisation + " :";
                HndXls.Cells[Y, 1].Style.Font.UnderLine = true;
                HndXls.Cells[Y, 1].Style.Font.Bold = true;

                Y++;
                HndXls.Cells[Y, 2, Y, 5].Merge = true;
                HndXls.Cells[Y, 2].Value = Resource.stat_collab_lu_documents_propose;
                HndXls.Cells[Y, 2].Style.Font.Bold = true;
                HndXls.Cells[Y, 6, Y, 7].Merge = true;
                HndXls.Cells[Y, 6].Value = String.Format("{0} / {1}", ReadOk, NbEngineer);
                HndXls.Cells[Y, 6].Style.Font.Bold = true;
                if (NbEngineer > 0)
                    HndXls.Cells[Y, 8].Value = 1f * ReadOk / NbEngineer;
                HndXls.Cells[Y, 8].Style.Font.Bold = true;
                HndXls.Cells[Y, 8].Style.Numberformat.Format = "0%";

                Y++;
                HndXls.Cells[Y, 2, Y, 5].Merge = true;
                HndXls.Cells[Y, 2].Value = Resource.stat_collab_approved_documents_approve;
                HndXls.Cells[Y, 2].Style.Font.Bold = true;
                HndXls.Cells[Y, 6, Y, 7].Merge = true;
                HndXls.Cells[Y, 6].Value = String.Format("{0} / {1}", ApprovedOk, NbEngineer);
                HndXls.Cells[Y, 6].Style.Font.Bold = true;
                if (NbEngineer > 0)
                    HndXls.Cells[Y, 8].Value = 1f * ApprovedOk / NbEngineer;
                HndXls.Cells[Y, 8].Style.Font.Bold = true;
                HndXls.Cells[Y, 8].Style.Numberformat.Format = "0%";

                Y++;
                HndXls.Cells[Y, 2, Y, 5].Merge = true;
                HndXls.Cells[Y, 2].Value = Resource.stat_collab_success_qcm_propose;
                HndXls.Cells[Y, 2].Style.Font.Bold = true;
                HndXls.Cells[Y, 6, Y, 7].Merge = true;
                HndXls.Cells[Y, 6].Value = String.Format("{0} / {1}", TestOk, NbEngineer);
                HndXls.Cells[Y, 6].Style.Font.Bold = true;
                if (NbEngineer > 0)
                    HndXls.Cells[Y, 8].Value = 1f * TestOk / NbEngineer;
                HndXls.Cells[Y, 8].Style.Font.Bold = true;
                HndXls.Cells[Y, 8].Style.Numberformat.Format = "0%";

                HndXls.Cells[Y - 2, 2, Y, 8].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                HndXls.Cells[Y - 2, 2, Y, 8].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                HndXls.Cells[Y - 2, 2, Y, 8].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                HndXls.Cells[Y - 2, 2, Y, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                HndXls.Cells[Y - 2, 2, Y, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                HndXls.Cells[Y - 2, 2, Y, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                HndXls.Cells[Y - 2, 2, Y, 5].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(248, 203, 173));
                HndXls.Cells[Y - 2, 2, Y, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                HndXls.Cells[Y - 2, 2, Y, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                // Legend
                Y += 2;
                HndXls.Cells[Y, 1].Value = Resource.stat_Legend;
                HndXls.Cells[Y, 1].Style.Font.UnderLine = true;
                HndXls.Cells[Y, 1].Style.Font.Bold = true;
                Y++;
                HndXls.Cells[Y, 2, Y, 5].Merge = true;
                HndXls.Cells[Y, 2].Value = Resource.stat_collab_notfinish_sensibilisation;
                HndXls.Cells[Y, 2, Y, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                HndXls.Cells[Y, 2, Y, 5].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                HndXls.Cells[Y, 2, Y, 5].Style.Border.BorderAround(ExcelBorderStyle.Medium);

                // Engineer list
                Y += 2;
                HndXls.Cells[Y, 1].Value = Resource.stat_consultants + " :";
                HndXls.Cells[Y, 1].Style.Font.UnderLine = true;
                HndXls.Cells[Y, 1].Style.Font.Bold = true;
                Y++;
                StartTable = Y;

                X = 1;
                switch (statsRequest.StatType)
                {
                    case StatsRequestType.Agency:
                        HndXls.Cells[Y, X++].Value = Resource.stat_Entity;// HndXls.Cells[Y, 1].Value
                        HndXls.Cells[Y, X++].Value = Resource.stat_Agence; //HndXls.Cells[Y, 2].Value
                        break;
                    case StatsRequestType.Departement:
                        HndXls.Cells[Y, X++].Value = Resource.Departement;
                        HndXls.Cells[Y, X++].Value = Resource.stat_Entity;                      
                        break;
                    case StatsRequestType.Manager:
                        HndXls.Cells[Y, X++].Value = Resource.Manager;
                        HndXls.Cells[Y, X++].Value = Resource.Departement;
                        HndXls.Cells[Y, X++].Value = Resource.stat_Entity;
                        break;
                    default:
                        break;
                }              
                HndXls.Cells[Y, X++].Value = Resource.stat_Ingenieur;
                HndXls.Cells[Y, X++].Value = Resource.stat_lastDate;
                HndXls.Cells[Y, X++].Value = Resource.stat_document_restant_consult;
                HndXls.Cells[Y, X++].Value = Resource.stat_document_restant_approve;
                HndXls.Cells[Y, X++].Value = Resource.stat_qcm_restant_success; 
                HndXls.Cells[Y, X].Value = Resource.stat_Taux_finalisation;//X=8
                HndXls.Cells[Y, 1, Y, X].Style.Fill.PatternType = ExcelFillStyle.Solid;//X=8
                HndXls.Cells[Y, 1, Y, X].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(242, 242, 242));//X=8
                foreach (var stat in stats)
                {
                    Y++;
                    X = 1;
                    switch (statsRequest.StatType)
                    {
                        case StatsRequestType.Agency:
                            HndXls.Cells[Y, X++].Value = stat.EntityName;
                            HndXls.Cells[Y, X++].Value = stat.AgencyName;
                            //HndXls.Cells[Y, X++].Value = stat.UserId +":"+ stat.Name;
                            break;
                        case StatsRequestType.Departement:
                            HndXls.Cells[Y, X++].Value = stat.Departement;
                            HndXls.Cells[Y, X++].Value = stat.EntityName;                           
                            break;
                        case StatsRequestType.Manager:
                            HndXls.Cells[Y, X++].Value = stat.Division;
                            HndXls.Cells[Y, X++].Value = stat.Departement;
                            HndXls.Cells[Y, X++].Value = stat.EntityName;
                            break;
                        default:
                            break;
                    }
                    
                    HndXls.Cells[Y, X++].Value = stat.Name; //Nom du collaborateur
                    var lastDate = "";
                    if(stat.LastDate.HasValue && !stat.LastDate.Value.Equals(DateTime.MinValue)){
                        lastDate = stat.LastDate.ToString();
                    }
                    HndXls.Cells[Y, X++].Value = lastDate;
                    HndXls.Cells[Y, X++].Value = String.Format("{0} / {1}", stat.NotRead, stat.ToRead);
                    HndXls.Cells[Y, X++].Value = stat.ToApproved >0 ? String.Format("{0} / {1}", stat.NotApproved, stat.ToApproved): string.Empty;
                    HndXls.Cells[Y, X++].Value = stat.ToTested > 0 ? String.Format("{0} / {1}", stat.NotTested, stat.ToTested) : string.Empty;
                    // Rate 
                    float Rate = 0f;
                    float RateRead = 0f;
                    float RateApproved = 0f;
                    float RateTested = 0f;

                    if (stat.ToRead>0 || stat.ToApproved > 0 || stat.ToTested > 0)
                    {

                        if (stat.ToRead > 0)
                        {
                            RateRead += 0.50f * (stat.ToRead - stat.NotRead) / stat.ToRead;
                        }
                        if (stat.ToApproved > 0)
                        {
                            RateApproved += 0.30f * (stat.ToApproved - stat.NotApproved) / stat.ToApproved;
                        }
                        else
                        {
                            RateApproved += 0.30f;
                        }                            
                        if (stat.ToTested > 0)
                        {
                            RateTested += 0.20f * (stat.ToTested - stat.NotTested) / stat.ToTested;
                        }
                        else
                        {
                            RateTested += 0.20f;
                        }
                        Rate = RateRead + RateApproved + RateTested;
                        HndXls.Cells[Y, X].Value = Rate;
                        HndXls.Cells[Y, X].Style.Numberformat.Format = "0%";
                        // Alert if rate <>100%
                        if (Math.Abs(Rate - 1f) > 0.0001 || Math.Abs(Rate - 1f) < -0.0001)
                        {
                            HndXls.Cells[Y, 1, Y, X].Style.Fill.PatternType = ExcelFillStyle.Solid;//X=8
                            HndXls.Cells[Y, 1, Y, X].Style.Fill.BackgroundColor.SetColor(Color.Yellow);//X=8
                        }
                    }
                    else
                    {
                        HndXls.Cells[Y, X].Value = "n/a";
                    }
                    HndXls.Cells[Y, 5, Y, X].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;//X=8
                }

                // Date forme
                var dateplage = "D:D";
                var borderPlage = "A{0}:H{1}";
                if (statsRequest.StatType == StatsRequestType.Manager)
                {
                    dateplage = "E:E";
                    borderPlage = "A{0}:I{1}";
                }
                XlsRange = HndXls.Cells[dateplage];
                XlsRange.Style.Numberformat.Format = "dd/mm/yyyy";

                // Border
                XlsRange = HndXls.Cells[String.Format(borderPlage, StartTable, Y)];
                XlsRange.Style.Border.BorderAround(ExcelBorderStyle.Thick);
                XlsRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                XlsRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                XlsRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                XlsRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                XlsRange.AutoFitColumns();

                // Push response
                MemoryStream hndMemoryFile = new MemoryStream();
                
                // Prepare content disposition depending of browser
                HndExcel.SaveAs((Stream)hndMemoryFile);
                FileDownloadResult downResult = new FileDownloadResult(DocType.EXCEL, fileName, hndMemoryFile);                   
                return downResult;
            }
            catch (Exception Ex)
            {
                this.Response.Clear();
                this.Response.SetCookie(new HttpCookie(FileDownloadResult.COOKIE_FILEDOWNLOAD, "false")
                {
                    Path = "/",
                    HttpOnly = true
                });
                this.Response.Write("Création du fichier impossible : " + Ex.Message);
                this.Response.Flush();
                this.Response.End();
                return null;
            }
        }
        public ActionResult Rythme()
        {
            int idlang = GetIdLang();
            StatsResponseViewModel vm = new StatsResponseViewModel();
            vm.NbDay = 7;
            var documents = _statsService.ListDocuments(idlang, GetDefaultLang());
            vm.ListDocument = AutoMapperConfigAdmin.Mapper.Map<List<DocumentViewModel>>(documents);
            List<string> entities = _userService.EntityNameList(true);
            foreach (var entity in entities)
            {
                int nb = _userService.GetCountUserActiveByEntityName(entity);
                if (nb > 0)
                {
                    List<string> agencies = _entiteService.AgencyByEntity(entity, true, false, true);
                    List<OptionStatDTO> subList = new List<OptionStatDTO>();
                    if (agencies == null || agencies.Count == 0)
                    {
                        subList.Add(new OptionStatDTO() { Nb = nb, Key = null, Text = Resource.noAgence });
                    }
                    else
                    {
                        foreach (var agency in agencies)
                        {
                            int nbAgency = _userService.GetCountUserActiveByAgencyName(entity, agency);
                            if (nbAgency > 0)
                            {
                                string strAgency = string.IsNullOrEmpty(agency) ? Resource.noAgence : agency;
                                subList.Add(new OptionStatDTO() { Nb = nbAgency, Key = agency, Text = string.Format("{0} ({1})", strAgency, nbAgency) });
                            }
                        }
                    }
                    vm.EntityNameList.Add(new OptionStatDTO() { Nb = nb, Key = entity, Text = string.Format("{0} ({1})", entity, nb), SubList = subList });
                }

            }
            vm.EmployeesList = new List<IntOption>() { new IntOption() { Value = 0, Text = Resource.EmployeesAll }, new IntOption() { Value = Options.True, Text = Resource.EmployeesActif }, new IntOption() { Value = Options.False, Text = Resource.EmployeesInactif } };
            return View(vm);
        }
        [HttpPost]
        [ValidateInput(true)]
        public ActionResult Rythme(GetStatsRequest statsRequest)
        {
            int idlang = GetIdLang();
            statsRequest.IdLang = GetIdLang();
            statsRequest.IdDefaultLang = GetDefaultLang();
            StatsResponseViewModel vm = new StatsResponseViewModel();
            vm.MultiDocSelect = statsRequest.MultiDocSelect;
            vm.MultiEntitySelect = statsRequest.MultiEntitySelect;
            vm.EmployeeSelect = statsRequest.EmployeeSelect;
            var documents = _statsService.ListDocuments(idlang, GetDefaultLang());
            vm.ListDocument = AutoMapperConfigAdmin.Mapper.Map<List<DocumentViewModel>>(documents);
            vm.EmployeesList = new List<IntOption>() { new IntOption() { Value = 0, Text = Resource.EmployeesAll }, new IntOption() { Value = Options.True, Text = Resource.EmployeesActif }, new IntOption() { Value = Options.False, Text = Resource.EmployeesInactif } };
            List<string> entities = _userService.EntityNameList(true);
            foreach (var entity in entities)
            {
                int nb = _userService.GetCountUserActiveByEntityName(entity);
                if (nb > 0)
                {
                    List<string> agencies = _entiteService.AgencyByEntity(entity, true, false, true);
                    List<OptionStatDTO> subList = new List<OptionStatDTO>();
                    if (agencies == null || agencies.Count == 0)
                    {
                        subList.Add(new OptionStatDTO() { Nb = nb, Key = null, Text = Resource.noAgence });
                    }
                    else
                    {
                        foreach (var agency in agencies)
                        {
                            int nbAgency = _userService.GetCountUserActiveByAgencyName(entity, agency);
                            if (nbAgency > 0)
                            {
                                string strAgency = string.IsNullOrEmpty(agency) ? Resource.noAgence : agency;
                                subList.Add(new OptionStatDTO() { Nb = nbAgency, Key = agency, Text = string.Format("{0} ({1})", strAgency, nbAgency) });
                            }
                        }
                    }
                    vm.EntityNameList.Add(new OptionStatDTO() { Nb = nb, Key = entity, Text = string.Format("{0} ({1})", entity, nb), SubList = subList });
                }

            }
            vm.NbDay = statsRequest.NbDay;
            vm.Periode = DateTime.Today;
            statsRequest.Periode = vm.Periode;
            //statsRequest.Periode = new DateTime(2020,5,13);
            statsRequest.StatType = StatsRequestType.Agency;
            var stats = _statsService.GetStats(statsRequest, false);
            GetRythmeResponse reponse = new GetRythmeResponse(statsRequest);
            reponse.Documents = documents;
            reponse.GetDatasets(stats);           
            vm.Datasets = reponse.Datasets;
            return View(vm);
        }
       
       
        public ActionResult Departement()
        {
            int idlang = GetIdLang();
            StatsResponseViewModel vm = new StatsResponseViewModel();
            var documents = _statsService.ListDocuments(idlang, GetDefaultLang());
            vm.ListDocument = AutoMapperConfigAdmin.Mapper.Map<List<DocumentViewModel>>(documents);
            List<string> departments= _userService.DepartementNameList(true);
            vm.DepartementNameList = new List<OptionStatDTO>();
            foreach (var departement in departments)
            {
                if (!string.IsNullOrEmpty(departement))
                {
                    int nb = _userService.GetCountUserActiveByDepartementName(departement);
                    if (nb > 0)
                    {
                        string strDepartement = departement.Equals(Constante.NO_DIRECTION_ID) ? Resource.NoDepartement : departement;
                        vm.DepartementNameList.Add(new OptionStatDTO() { Nb = nb, Key = departement, Text = string.Format("{0} ({1})", strDepartement, nb) });
                    }
                }
            }
            vm.EmployeesList = new List<IntOption>() { new IntOption() { Value = 0, Text = Resource.EmployeesAll }, new IntOption() { Value = Options.True, Text = Resource.EmployeesActif }, new IntOption() { Value = Options.False, Text = Resource.EmployeesInactif } };
            vm.NbDay = 7;
            vm.EndDate = DateTime.Today;
            vm.StartDate = new DateTime(2000, 1, 1);
            return View(vm);
        }
        [HttpPost]
        [ValidateInput(true)]
        public ActionResult Departement(GetStatsRequest statsRequest)
        {
            int idlang = GetIdLang();
            statsRequest.IdLang = GetIdLang();
            statsRequest.IdDefaultLang = GetDefaultLang();
            StatsResponseViewModel vm = new StatsResponseViewModel();
            vm.MultiDocSelect = statsRequest.MultiDocSelect;
            vm.MultiDepartementSelect = statsRequest.MultiDepartementSelect;
            vm.EmployeeSelect = statsRequest.EmployeeSelect;
            var documents = _statsService.ListDocuments(idlang, GetDefaultLang());
            vm.ListDocument = AutoMapperConfigAdmin.Mapper.Map<List<DocumentViewModel>>(documents);
            vm.DepartementNameList = new List<OptionStatDTO>();
            List<string> departments = _userService.DepartementNameList(true);
            foreach (var departement in departments)
            {
                if (!string.IsNullOrEmpty(departement))
                {
                    int nb = _userService.GetCountUserActiveByDepartementName(departement);
                    if (nb > 0)
                    {
                        string strDepartement = departement.Equals(Constante.NO_DIRECTION_ID) ? Resource.NoDepartement : departement;
                        vm.DepartementNameList.Add(new OptionStatDTO() { Nb = nb, Key = departement, Text = string.Format("{0} ({1})", strDepartement, nb) });
                    }
                }
            }
            vm.EmployeesList = new List<IntOption>() { new IntOption() { Value = 0, Text = Resource.EmployeesAll }, new IntOption() { Value = Options.True, Text = Resource.EmployeesActif }, new IntOption() { Value = Options.False, Text = Resource.EmployeesInactif } };

            vm.NbDay = statsRequest.NbDay;
            vm.Periode = DateTime.Today;
            vm.EndDate = statsRequest.EndDate;
            vm.StartDate = statsRequest.StartDate;
            statsRequest.Periode = vm.Periode;

            statsRequest.StatType = StatsRequestType.Departement;
            var stats = _statsService.GetStats(statsRequest, false);
            GetStatsResponse reponse = new GetStatsResponse();
            reponse.SetDepartementStats(stats);
            GetRythmeResponse reponseRythme = new GetRythmeResponse(statsRequest);
            reponseRythme.Documents = documents;
            reponseRythme.GetDatasets(stats);
            vm.Datasets = reponseRythme.Datasets;

            vm.ReponseEntityNames = reponse.DepartementList;
            if(reponse.DepartementList!=null && reponse.DepartementList.Any())
            {
                var grid = new GridBO<Statistiques>("depGrid", reponse.DicoAgencyStats[0], null, -1);
                vm.GridStats.Add(grid);
            }        
            vm.Total = reponse.Total;
            vm.NotRead = reponse.NotRead;
            vm.NotApproved = reponse.NotApproved;
            vm.NotTested = reponse.NotTested;
            vm.ToApproved = reponse.ToApproved;
            vm.ToTested = reponse.ToTested;
            return View("Departement", vm);
        }
        [HttpPost]
        [ValidateInput(true)]
        public ActionResult RythmeDepartementAjax(GetStatsRequest statsRequest)
        {
            statsRequest.IdLang = GetIdLang();
            statsRequest.IdDefaultLang = GetDefaultLang();
            var documents = _statsService.ListDocuments(GetIdLang(), GetDefaultLang());
            StatsResponseViewModel vm = new StatsResponseViewModel();
            vm.MultiDocSelect = statsRequest.MultiDocSelect;
            vm.MultiDepartementSelect = statsRequest.MultiDepartementSelect;
            vm.EmployeeSelect = statsRequest.EmployeeSelect;

            vm.NbDay = statsRequest.NbDay;
            vm.Periode = DateTime.Today;
            statsRequest.Periode = vm.Periode;

            statsRequest.StatType = StatsRequestType.Departement;
            var stats = _statsService.GetStats(statsRequest, false);
            GetRythmeResponse reponse = new GetRythmeResponse(statsRequest);
            reponse.Documents = documents;
            reponse.GetDatasets(stats);
            vm.Datasets = reponse.Datasets;
            return PartialView("_Chart", vm);
        }

        public ActionResult Manager()
        {
            int idlang = GetIdLang();
            StatsResponseViewModel vm = new StatsResponseViewModel();
            var documents = _statsService.ListDocuments(idlang, GetDefaultLang());
            vm.ListDocument = AutoMapperConfigAdmin.Mapper.Map<List<DocumentViewModel>>(documents);
            List<DepartementDTO> managers = _userService.ManagerList();
            List<string> departements = managers.Where(x => x.ID_Manager.HasValue).GroupBy(x => x.Departement).Select(x => x.Key).ToList();
            vm.ManagerList = new List<OptionStatDTO>();
            foreach (var departement in departements)
            {
                int nb = _userService.GetCountUserActiveByIDManagerByDepartement(departement);
                if (nb > 0)
                {
                    var tmp_managers = managers.Where(x => x.Departement == departement).GroupBy(x => new { x.ID_Manager, x.Division }).Select(x => new { ID_Manager = x.Key.ID_Manager, Division = x.Key.Division }).ToList();
                    //ID_Manager est la clé, et Division =Fullname ou Email de l'utilisateur ID_Manager
                    var managerIDs = tmp_managers.Select(x => x.ID_Manager).Distinct().ToList();
                    //dans la base 
                    List<OptionStatDTO> subList = new List<OptionStatDTO>();
                    foreach (var managerID in managerIDs)
                    {
                        int nbManager = _userService.GetCountUserActiveByIDManager(departement, managerID.Value);
                        if (nbManager > 0)
                        {
                            var manager = tmp_managers.Where(x => x.ID_Manager == managerID.Value).FirstOrDefault();
                            string key = string.Format("{0}|{1}", departement, managerID.Value);
                            subList.Add(new OptionStatDTO() { Nb = nbManager, Key = key, Text = string.Format("{0} ({1})", manager.Division, nbManager) });
                        }
                    }
                    vm.ManagerList.Add(new OptionStatDTO() { Nb = nb, Key = departement, Text = string.Format("{0} ({1})", departement, nb), SubList = subList });
                }
            }
            vm.EmployeesList = new List<IntOption>() { new IntOption() { Value = 0, Text = Resource.EmployeesAll }, new IntOption() { Value = Options.True, Text = Resource.EmployeesActif }, new IntOption() { Value = Options.False, Text = Resource.EmployeesInactif } };
            vm.NbDay = 7;
            vm.EndDate = DateTime.Today;
            vm.StartDate = new DateTime(2000, 1, 1);
            return View(vm);
        }
       

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult Manager(GetStatsRequest statsRequest)
        {
            int idlang = GetIdLang();
            statsRequest.IdLang = GetIdLang();
            statsRequest.IdDefaultLang = GetDefaultLang();
            StatsResponseViewModel vm = new StatsResponseViewModel(statsRequest)
            {
                MultiDocSelect = statsRequest.MultiDocSelect,
                MultiManagerSelect = statsRequest.MultiManagerSelect,
                EmployeeSelect = statsRequest.EmployeeSelect,
                NbDay = statsRequest.NbDay,
                Periode = DateTime.Today,
                EndDate = statsRequest.EndDate,
                StartDate = statsRequest.StartDate,
                ManagerList = new List<OptionStatDTO>()
            };            

            statsRequest.Periode = vm.Periode;

            var documents = _statsService.ListDocuments(idlang, GetDefaultLang());

            vm.ListDocument = AutoMapperConfigAdmin.Mapper.Map<List<DocumentViewModel>>(documents);

            List<DepartementDTO> managers = _userService.ManagerList();
            List<string> departements = managers.Where(x => x.ID_Manager.HasValue).GroupBy(x => x.Departement).Select(x => x.Key).ToList();
                      
           foreach (var departement in departements)
            {
                int nb = _userService.GetCountUserActiveByIDManagerByDepartement(departement);
                if (nb > 0)
                {
                   var tmp_managers = managers.Where(x => x.Departement == departement).GroupBy(x => new { x.ID_Manager, x.Division }).Select(x => new { ID_Manager = x.Key.ID_Manager, Division = x.Key.Division }).ToList();
                    //ID_Manager est la clé, et Division =Fullname ou Email de l'utilisateur ID_Manager
                    var y = tmp_managers.Where(x => x.ID_Manager == 0).FirstOrDefault();
                    var managerIDs = tmp_managers.Select(x => x.ID_Manager).Distinct().ToList();
                    //dans la base 
                    List<OptionStatDTO> subList = new List<OptionStatDTO>();
                    foreach (var managerID in managerIDs)
                    {
                        int nbManager = _userService.GetCountUserActiveByIDManager(departement, managerID.Value);
                        if (nbManager > 0)
                        {
                            var manager = tmp_managers.Where(x => x.ID_Manager == managerID.Value).FirstOrDefault();
                            string key = string.Format("{0}|{1}", departement, managerID.Value);
                            subList.Add(new OptionStatDTO() { Nb = nbManager, Key = key, Text = string.Format("{0} ({1})", manager.Division, nbManager) });
                        }
                    }
                    vm.ManagerList.Add(new OptionStatDTO() { Nb = nb, Key = departement, Text = string.Format("{0} ({1})", departement, nb), SubList= subList });
                }
            }

            vm.EmployeesList = new List<IntOption>() { new IntOption() { Value = 0, Text = Resource.EmployeesAll }, new IntOption() { Value = Options.True, Text = Resource.EmployeesActif }, new IntOption() { Value = Options.False, Text = Resource.EmployeesInactif } };
                       

            statsRequest.StatType = StatsRequestType.Manager;
                       
            var stats = _statsService.GetStats(statsRequest, false);
          
            GetStatsResponse reponse = new GetStatsResponse();
            reponse.SetManagerStats(stats);
            vm.SetResponse(reponse);

            int indexGrid = 0;
            foreach (var lst in reponse.DicoAgencyStats)
            {
                var grid = new GridBO<Statistiques>("agGrid_" + indexGrid, lst, null, -1);
                vm.GridStats.Add(grid);
                indexGrid++;
            }

            GetRythmeResponse reponseRythme = new GetRythmeResponse(statsRequest);
            reponseRythme.Documents = documents;
            reponseRythme._Lststat = stats;
            reponseRythme.CorrectDataSet();
            reponseRythme.GetDatasets(stats);
            vm.Datasets = reponseRythme.Datasets;

           

            return View("Manager", vm);
        }       
      

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult RythmeManagerAjax(GetStatsRequest statsRequest)
        {
            statsRequest.IdLang = GetIdLang();
            statsRequest.IdDefaultLang = GetDefaultLang();
            var documents = _statsService.ListDocuments(GetIdLang(), GetDefaultLang());
            StatsResponseViewModel vm = new StatsResponseViewModel();
            vm.MultiDocSelect = statsRequest.MultiDocSelect;
            vm.MultiManagerSelect = statsRequest.MultiManagerSelect;
            vm.EmployeeSelect = statsRequest.EmployeeSelect;

            vm.NbDay = statsRequest.NbDay;
            vm.Periode = DateTime.Today;
            statsRequest.Periode = vm.Periode;

            statsRequest.StatType = StatsRequestType.Manager;
            var stats = _statsService.GetStats(statsRequest, false);
            GetRythmeResponse reponse = new GetRythmeResponse(statsRequest);
            reponse.Documents = documents;
            reponse.GetDatasets(stats);
            vm.Datasets = reponse.Datasets;
            return PartialView("_Chart", vm);
        }
        public ActionResult Test()
        {
            return PartialView();
        }

    }
}