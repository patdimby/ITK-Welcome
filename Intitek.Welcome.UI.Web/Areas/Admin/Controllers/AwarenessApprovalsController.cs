using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Intitek.Welcome.Service.Back;
using Intitek.Welcome.UI.Web.Admin.Models;
using Intitek.Welcome.UI.Web.Infrastructure;
using Intitek.Welcome.UI.Resources;
using Intitek.Welcome.Infrastructure.Log;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;

namespace Intitek.Welcome.UI.Web.Areas.Admin.Controllers
{
    public class AwarenessApprovalsController : CommunController
    {
        private readonly IApprobationService _approbationService;
        private readonly ISensibilisationService _sensibilisationService;
        private readonly IHistoADService _histoADService;

        public AwarenessApprovalsController()
        {
            _approbationService = new ApprobationService(new FileLogger());
            _sensibilisationService = new SensibilisationService(new FileLogger());
            _histoADService = new HistoADService(new FileLogger());
        }
        // GET: Admin/AwarenessApprovals
        public ActionResult Index()
        {
            AwarenessApprovalsResponseViewModel aam = new AwarenessApprovalsResponseViewModel();
            aam.Months = _histoADService.getAllMonths();
            return View(aam);
        }
        /// <summary>
        /// Execute generate excel
        /// </summary>
        /// <param name="model"></param>
        /// <param name="SubmitButton"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(true)]
        public ActionResult GenerateDocs(GetAwarenessApprovalsRequest awarenessApprovalsRequest)
        {
            if ("DocValidated".Equals(awarenessApprovalsRequest.SubmitButton))
            {
                return this._ExcelSensibilisation(awarenessApprovalsRequest);
            }
            else if ("DocApproved".Equals(awarenessApprovalsRequest.SubmitButton))
            {
                return this._ExcelApprobation(awarenessApprovalsRequest);
            }
            return null;
            
        }
        /// <summary>
        /// Export data Sensibilisation to EXCEL
        /// </summary>
        /// <param name="model"></param>
        private FileDownloadResult _ExcelSensibilisation(GetAwarenessApprovalsRequest awarenessApprovalsRequest)
        {
            //var fileName = Resource.sensibilisation_title+"-" + awarenessApprovalsRequest.Month + ".xlsx";
            var fileName = Resource.sensibilisation_title+"_" + awarenessApprovalsRequest.Month.Replace("-", string.Empty) + ".xlsx";
            var toGenerateListDocs = _sensibilisationService.ListSensibilisationsDocs(awarenessApprovalsRequest);
            var toGenerateListDocsPerUser = _sensibilisationService.ListSensibilisationsDocsPerUser(awarenessApprovalsRequest);
            var toGenerateListUsers = _sensibilisationService.ListSensibilisationsUsers(awarenessApprovalsRequest);
            try
            {
                ExcelPackage HndExcel = new ExcelPackage(); // Handle on EXCEL
                int X, Y,   // Position in sheet
                StartTable;  // First line of table

                // Create workbook
                ExcelWorksheet HndXls = HndExcel.Workbook.Worksheets.Add(Resource.sensibilisation_workbookname);
                // sets the header
                X = Y = 1;
                List<string> titles = new List<string>();
                titles.Add(Resource.sensibilisation_entity);
                titles.Add(Resource.sensibilisation_agency);
                titles.Add(Resource.sensibilisation_engineer);
                titles.Add(Resource.sensibilisation_mail);
                titles.Add(Resource.sensibilisation_active_inactive);
                titles.Add(Resource.sensibilisation_present_not_present);

                //Use the resource files
                /* List<string[]> documentsInResource = new List<string[]>();
                 documentsInResource.Add(new string[2] { Resource.sensibilisation_corruption, "Sensibilisation aux risques de corruption" });
                 documentsInResource.Add(new string[2] { Resource.sensibilisation_environment, "Sensibilisation Environnementale (ISO 14001)" });
                 documentsInResource.Add(new string[2] { Resource.sensibilisation_quality, "Sensibilisation Qualité (ISO 9001)" });
                 documentsInResource.Add(new string[2] { Resource.sensibilisation_safety_business, "Sensibilisation Sécurité - Consultant Métier (ISO 27001)" });
                 documentsInResource.Add(new string[2] { Resource.sensibilisation_safety_common, "Sensibilisation Sécurité - Tronc commun (ISO 27001)" });

                 foreach (var document in toGenerateListDocs)
                 {
                     foreach(var docRessource in documentsInResource)
                     {
                         if (docRessource[1] == document.Nom_Document)
                         {
                             titles.Add(docRessource[0] +" - "+document.Version);
                         }                     
                     }   
                 }*/
                //or
                foreach (var document in toGenerateListDocs)
                {
                    titles.Add(document.Nom_Document + " - " + document.Version);
                }

                //Set the header
                for (X = 1; X <= titles.Count(); X++)
                {
                    HndXls.Cells[Y, X].Value = titles.ElementAt(X-1);
                    HndXls.Cells[Y, X].Style.Font.Bold = true;
                    if (X > 6)
                    {
                        HndXls.Cells[Y, X].Style.Numberformat.Format = "dd/mm/yyyy";
                        HndXls.Cells[Y, X].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    }
                }
               
                Y++;
               
                //the body
                foreach(var user in toGenerateListUsers)
                {
                    HndXls.Cells[Y, 1].Value = user.Entité;
                    HndXls.Cells[Y, 2].Value = user.Agence;
                    HndXls.Cells[Y, 3].Value = user.FullName;
                    HndXls.Cells[Y, 4].Value = user.Email;
                    HndXls.Cells[Y, 5].Value = user.Actif==1?Resource.sensibilisation_approbation_active: Resource.sensibilisation_approbation_inactive;
                    HndXls.Cells[Y, 6].Value = user.Present==1?Resource.sensibilisation_approbation_present: Resource.sensibilisation_approbation_not_present;
                    X = 7;
                    var currentUserDocs = toGenerateListDocsPerUser.FindAll(u => u.ID_User == user.ID_User);
                    bool isColumnSet = false;
                    foreach(var doc in toGenerateListDocs)
                    {
                        
                        foreach(var docUser in currentUserDocs)
                        {
                            if (doc.ID_Document == docUser.ID_Document)
                            {
                                isColumnSet = true;
                                if(docUser.Resultat!= "N/A" && docUser.Resultat != "NULL" && docUser.Resultat != "" && docUser.Resultat!=null)
                                {
                                    string[] rDate = docUser.Resultat.Split('-');
                                    HndXls.Cells[Y, X].Style.Numberformat.Format = "dd/mm/yyyy";
                                    //DATE(Year,Month,Day)
                                    //HndXls.Cells[Y, X].Formula = "=DATE(" + rDate[0] + "," + rDate[1] + "," + rDate[2] + ")";
                                    HndXls.Cells[Y, X].Value = "" + rDate[2] + "/" + rDate[1] + "/" + rDate[0] + "";
                                    HndXls.Cells[Y, X].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                }
                                else
                                {
                                    HndXls.Cells[Y, X].Style.Numberformat.Format = "dd/mm/yyyy";
                                    HndXls.Cells[Y, X].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    HndXls.Cells[Y, X].Value = docUser.Resultat != "NULL" ? docUser.Resultat : "";
                                }

                            }
                        }
                        if (!isColumnSet)
                        {
                            HndXls.Cells[Y, X].Value = "";
                        }
                        isColumnSet = false;
                        X++;
                    }
                    Y++;
                }
                //adjust column width
                for (int i = 1; i <= titles.Count(); i++)
                {
                    HndXls.Column(i).AutoFit();
                }
                // Push response
                MemoryStream hndMemoryFile = new MemoryStream();

                // Prepare content disposition depending of browser
                HndExcel.SaveAs((Stream)hndMemoryFile);
                FileDownloadResult downResult = new FileDownloadResult(DocType.EXCEL, fileName, hndMemoryFile);
                return downResult;
            }
            catch(Exception Ex)
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
        /// <summary>
        /// Export data Approbation to EXCEL
        /// </summary>
        /// <param name="model"></param>
        private FileDownloadResult _ExcelApprobation(GetAwarenessApprovalsRequest awarenessApprovalsRequest)
        {
            var fileName = Resource.approbation_title+"_" + awarenessApprovalsRequest.Month.Replace("-", string.Empty) + ".xlsx";
            var toGenerateListDocs = _approbationService.ListApprobationsDocs(awarenessApprovalsRequest);
            var toGenerateListDocsPerUser = _approbationService.ListApprobationsDocsPerUser(awarenessApprovalsRequest);
            var toGenerateListUsers = _approbationService.ListApprobationsUsers(awarenessApprovalsRequest);
            try
            {
                ExcelPackage HndExcel = new ExcelPackage(); // Handle on EXCEL
                int X, Y,   // Position in sheet
                StartTable;  // First line of table

                // Create workbook
                ExcelWorksheet HndXls = HndExcel.Workbook.Worksheets.Add(Resource.approbation_workbookname);
                // sets the header
                X = Y = 1;
                List<string> titles = new List<string>();
                titles.Add(Resource.approbation_entity);
                titles.Add(Resource.approbation_agency);
                titles.Add(Resource.approbation_engineer);
                titles.Add(Resource.approbation_mail);
                titles.Add(Resource.approbation_active_inactive);
                titles.Add(Resource.approbation_present_not_present);

                //Use the resource files
                /* List<string[]> documentsInResource = new List<string[]>();
                 documentsInResource.Add(new string[2] { approbation_admin_charter, "Charte administrateur" });
                 documentsInResource.Add(new string[2] { Resource.approbation_practical_guide, "Guide pratique de classification et manipulation des documents" });
                
                 foreach (var document in toGenerateListDocs)
                 {
                     foreach(var docRessource in documentsInResource)
                     {
                         if (docRessource[1] == document.Nom_Document)
                         {
                             titles.Add(docRessource[0] +" - "+document.Version);
                         }                     
                     }   
                 }*/
                //or
                foreach (var document in toGenerateListDocs)
                {
                    titles.Add(document.Nom_Document + " - " + document.Version);
                }

                //Set the header
                for (X = 1; X <= titles.Count(); X++)
                {
                    HndXls.Cells[Y, X].Value = titles.ElementAt(X - 1);
                    HndXls.Cells[Y, X].Style.Font.Bold = true;
                    if (X > 6)
                    {
                        HndXls.Cells[Y, X].Style.Numberformat.Format = "dd/mm/yyyy";
                        HndXls.Cells[Y, X].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    }
                }

                Y++;

                //the body
                foreach (var user in toGenerateListUsers)
                {
                    HndXls.Cells[Y, 1].Value = user.Entité;
                    HndXls.Cells[Y, 2].Value = user.Agence;
                    HndXls.Cells[Y, 3].Value = user.FullName;
                    HndXls.Cells[Y, 4].Value = user.Email;
                    HndXls.Cells[Y, 5].Value = user.Actif == 1 ? Resource.sensibilisation_approbation_active : Resource.sensibilisation_approbation_inactive;
                    HndXls.Cells[Y, 6].Value = user.Present == 1 ? Resource.sensibilisation_approbation_present : Resource.sensibilisation_approbation_not_present;
                    X = 7;
                    var currentUserDocs = toGenerateListDocsPerUser.FindAll(u => u.ID_User == user.ID_User);
                    bool isColumnSet = false;
                    foreach (var doc in toGenerateListDocs)
                    {

                        foreach (var docUser in currentUserDocs)
                        {
                            if (doc.ID_Document == docUser.ID_Document)
                            {
                                isColumnSet = true;
                                if (docUser.Resultat != "N/A" && docUser.Resultat != "NULL" && docUser.Resultat != "" && docUser.Resultat != null)
                                {
                                    string[] rDate = docUser.Resultat.Split('-');
                                    HndXls.Cells[Y, X].Style.Numberformat.Format = "dd/mm/yyyy";
                                    //DATE(Year,Month,Day)
                                    //HndXls.Cells[Y, X].Formula = "=DATE(" + rDate[0] + "," + rDate[1] + "," + rDate[2] + ")";
                                    HndXls.Cells[Y, X].Value = "" + rDate[2] + "/" + rDate[1] + "/" + rDate[0] + "";
                                    HndXls.Cells[Y, X].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                }
                                else
                                {
                                    HndXls.Cells[Y, X].Style.Numberformat.Format = "dd/mm/yyyy";
                                    HndXls.Cells[Y, X].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    HndXls.Cells[Y, X].Value = docUser.Resultat != "NULL" ? docUser.Resultat : "";
                                }
                            }
                        }
                        if (!isColumnSet)
                        {
                            HndXls.Cells[Y, X].Value = "";
                        }
                        isColumnSet = false;
                        X++;
                    }
                    Y++;
                }
                //adjust column width
                for (int i = 1; i <= titles.Count(); i++)
                {
                    HndXls.Column(i).AutoFit();
                }
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
      
    }
}