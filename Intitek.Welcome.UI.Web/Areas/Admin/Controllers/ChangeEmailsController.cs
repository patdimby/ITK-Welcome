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
    public class ChangeEmailsController : CommunController
    {
        private readonly IChangeEmailsService _changeEmailsService;
        private ILogger _logger;
        public ChangeEmailsController()
        {
            this._logger = new FileLogger();
            _changeEmailsService = new ChangeEmailsService(this._logger);
        }
        // GET: Admin/ChangeEmails
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult GenerateResult()
        {
            List<ChangeEmailsViewModel> excelData = new List<ChangeEmailsViewModel>();
            if (Request.Files.Count > 0)
            {
                try
                {
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                       
                        excelData = readExcelData(file);
                    }
                    if (excelData != null && excelData.Count > 0 )
                    {
                        return Json(excelData, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(false, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        public List<ChangeEmailsViewModel> readExcelData(HttpPostedFileBase fileBase)
        {
            using (var package = new ExcelPackage(fileBase.InputStream))
            {
                try
                {
                    List<ChangeEmailsViewModel> excelData = new List<ChangeEmailsViewModel>();
                    // get the first worksheet in the workbook
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                    for (int row = 2; row<=worksheet.Dimension.End.Row; row++)
                    {
                        if (worksheet.Cells[row, 1].Value != null)
                        {
                            string matchEmail = _changeEmailsService.SwapEmail(worksheet.Cells[row, 1].Value.ToString().Trim(), worksheet.Cells[row, 2].Value.ToString().Trim());
                            // do something with worksheet.Cells[row, col].Value
                            excelData.Add(new ChangeEmailsViewModel()
                            {
                                PreviousEmail = worksheet.Cells[row, 1].Value.ToString().Trim(),
                                NewEmail = worksheet.Cells[row, 2].Value.ToString().Trim(),
                                Result = matchEmail 
                            });
                        }
                    }
                    return excelData;

                }
                catch (Exception ex)
                {
                    this._logger.Error(ex.Message);
                    this._logger.Error(ex.StackTrace);
                    return null;
                }
            } 
        }

        

      

       
    }
}