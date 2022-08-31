using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Excel = Microsoft.Office.Interop.Excel;
using System.Diagnostics;
using OfficeOpenXml;

namespace Intitek.Welcome.Service.Back
{
    public class ExcelExportHelper
    {
        public static string ExcelContentType => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        public static byte[] ExportExcel(QcmDTO qcmFR, QcmDTO qcmEN, ExcelExportInfo excelExportInfo)
        {
            byte[] result = null;

            FileInfo file = new FileInfo(excelExportInfo.TemplatePath);
            using (ExcelPackage package = new ExcelPackage(file))
            {
                var workbook = package.Workbook;
                var workSheet = workbook.Worksheets.First();

                int row = 3;
                int questionNumber = 1;
                package.Workbook.CalcMode = ExcelCalcMode.Manual;
                workSheet.Cells[1, 2].Value = qcmFR.QcmTrad?.QcmName;
                foreach (var question in qcmFR.Questions)
                {
                    if (!String.IsNullOrEmpty(question.QuestionTrad?.TexteQuestion))
                    {
                        workSheet.Cells[row, 1].Value = questionNumber;

                        workSheet.Cells[row, 2].Value = question.QuestionTrad?.TexteQuestion;

                        foreach (var reponse in question.Reponses)
                        {
                            workSheet.Cells[row, 3].Value = reponse.ReponseTrad?.Texte;

                            if (reponse.Reponse.IsRight)
                            {
                                workSheet.Cells[row, 4].Value = "X";
                            }
                            row++;
                        }
                        workSheet.Cells[row, 2].Value = question.QuestionTrad?.TexteJustification;
                    }
                    questionNumber++;
                    row++;
                }

                // Second lang
                workSheet = workbook.Worksheets.ElementAt(1);

                row = 3;
                questionNumber = 1;
                package.Workbook.CalcMode = ExcelCalcMode.Manual;
                workSheet.Cells[1, 2].Value = qcmEN.QcmTrad?.QcmName;
                foreach (var question in qcmEN.Questions)
                {
                    if (!String.IsNullOrEmpty(question.QuestionTrad?.TexteQuestion))
                    {
                        workSheet.Cells[row, 1].Value = questionNumber;

                        workSheet.Cells[row, 2].Value = question.QuestionTrad?.TexteQuestion;

                        foreach (var reponse in question.Reponses)
                        {
                            workSheet.Cells[row, 3].Value = reponse.ReponseTrad?.Texte;

                            if (reponse.Reponse.IsRight)
                            {
                                workSheet.Cells[row, 4].Value = "X";
                            }
                            row++;
                        }
                        workSheet.Cells[row, 2].Value = question.QuestionTrad?.TexteJustification;
                    }
                    questionNumber++;
                    row++;
                }
                package.Workbook.Calculate();
                result = package.GetAsByteArray();
            }
            return result;
        }

        private static decimal GetExcelDecimalValueForDate(DateTime date)
        {
            DateTime start = new DateTime(1900, 1, 1);
            TimeSpan diff = date - start;
            return diff.Days + 2;
        }

        private static ExcelRange ConvertDateNullableFormat(ExcelRange cell, DateTime? date, string customFormat = null)
        {
            if (date != null)
            {
                cell = ConvertDateFormat(cell, date.Value, customFormat);
            }
            else
            {
                cell.Value = string.Empty;
            }
            return cell;
        }

        private static ExcelRange ConvertDateFormat(ExcelRange cell, DateTime date, string customFormat = null)
        {
            cell.Value = GetExcelDecimalValueForDate(date);
            if (!string.IsNullOrWhiteSpace(customFormat))//MMM yy
            {
                cell.Style.Numberformat.Format = customFormat;
            }
            else
            {
                cell.Style.Numberformat.Format = "dd/MM/yyyy";
            }
            return cell;
        }

        private static void RunMacro(object oApp, object[] oRunArgs)
        {
            oApp.GetType().InvokeMember("Run", BindingFlags.Default | BindingFlags.InvokeMethod, null, oApp, oRunArgs);
        }

        public static int GetWeekOfYear(DateTime date)
        {
            var day = (int)CultureInfo.CurrentCulture.Calendar.GetDayOfWeek(date);
            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(date.AddDays(4 - (day == 0 ? 7 : day)), CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        public static QcmDTO LoadQCMFromExcel(string filePath, string fileName, int idLang = 1)
        {
            QcmDTO qcm = new QcmDTO();
            object oMissing = System.Reflection.Missing.Value;
            // Create an instance of Microsoft Excel
            Excel.Application oExcel = new Excel.Application();

            // Make it visible
            oExcel.Visible = false;

            // Define Workbooks
            Excel.Workbooks oBooks = oExcel.Workbooks;
            Excel._Workbook oBook = null;
            try
            {


                string path = Path.GetDirectoryName(filePath) + "\\" + fileName;
                //Open the file, using the 'path' variable
                oBook = oBooks.Open(path, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing);

                // FR
                Excel.Worksheet xlRange = oBook.Sheets[idLang];
                if (Convert.ToString(xlRange.Cells[1, 2].Value2) != null)
                {
                    qcm.Id = 0;
                    qcm.Name = Convert.ToString(xlRange.Cells[1, 2].Value2);
                    qcm.QcmTrad = new Domain.QcmLang()
                    {
                        ID_Qcm = 0,
                        ID_Lang = idLang,
                        QcmName = Convert.ToString(xlRange.Cells[1, 2].Value2),
                    };
                    List<QuestionDTO> questions = new List<QuestionDTO>();
                    for (int i = 3; i <= xlRange.UsedRange.Rows.Count; i++)
                    {
                        if (Convert.ToString(xlRange.Cells[i, 1].Value2) != null)
                        {
                            var question = new QuestionDTO();
                            question.IdLang = idLang;

                            var questionDomain = new Domain.Question();
                            questionDomain.Id = 0;
                            questionDomain.Id_Qcm = 0;
                            questionDomain.inactif = 0;

                            question.Question = questionDomain;

                            var questionTrad = new Domain.QuestionLang();
                            questionTrad.Id = 0;
                            questionTrad.ID_Lang = idLang;
                            questionTrad.ID_Question = 0;
                            questionTrad.TexteQuestion = Convert.ToString(xlRange.Cells[i, 2].Value2);

                            question.QuestionTrad = questionTrad;

                            question.Reponses = new List<ReponseDTO>();
                            for (int j = i; j <= xlRange.UsedRange.Rows.Count; j++)
                            {
                                if (Convert.ToString(xlRange.Cells[j, 3].Value2) == null)
                                {
                                    Debug.WriteLine("~~~~~~~~~~~~~~~~~~~~~~ row = " + j);
                                    question.QuestionTrad.TexteJustification = Convert.ToString(xlRange.Cells[j, 2].Value2);
                                    questions.Add(question);
                                    i = j;
                                    break;
                                }
                                else
                                {
                                    var reponse = new ReponseDTO();

                                    // Reponse
                                    var reponseDomain = new Domain.Reponse();
                                    reponseDomain.ID_Question = 0;
                                    reponseDomain.Id = 0;
                                    reponseDomain.IsRight = (xlRange.Cells[j, 4] != null && xlRange.Cells[j, 4].Value2 != null && xlRange.Cells[j, 4].Value2.ToString() == "X") ? true : false;

                                    Debug.WriteLine("Ligne " + j + ", IsRight = " + reponseDomain.IsRight);
                                    reponse.Reponse = reponseDomain;

                                    // FR
                                    var reponseTrad = new Domain.ReponseLang();
                                    reponseTrad.ID_Lang = idLang;
                                    reponseTrad.ID_Reponse = 0;
                                    reponseTrad.Texte = Convert.ToString(xlRange.Cells[j, 3].Value2);

                                    reponse.ReponseTrad = reponseTrad;

                                    question.Reponses.Add(reponse);
                                }

                            }
                        }
                    }
                    qcm.Questions = questions;
                    qcm.NbQuestions = qcm.Questions.Count;
                    qcm.NoteMinimal = qcm.Questions.Count * 80 / 100;
                    qcm.Inactif = false;
                }

                // Quit Excel and clean up.
                oBook.Close(false, oMissing, oMissing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oBook);
                oBook = null;
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oBooks);
                oBooks = null;
                oExcel.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oExcel);
                oExcel = null;

                //Garbage collection
                GC.Collect();

                return qcm;

            }
            catch (Exception ex)
            {
                // Quit Excel and clean up.
                oBook?.Close(false, oMissing, oMissing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oBook);
                oBook = null;
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oBooks);
                oBooks = null;
                oExcel.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oExcel);
                oExcel = null;

                //Garbage collection
                GC.Collect();
                Debug.WriteLine(ex.StackTrace);
                throw ex;
            }
        }

        public static QcmDTO LoadEN(string filePath, string fileName)
        {
            QcmDTO qcm = new QcmDTO();
            object oMissing = System.Reflection.Missing.Value;
            // Create an instance of Microsoft Excel
            Excel.Application oExcel = new Excel.Application();

            // Make it visible
            oExcel.Visible = false;

            // Define Workbooks
            Excel.Workbooks oBooks = oExcel.Workbooks;
            Excel._Workbook oBook = null;
            try
            {


                string path = Path.GetDirectoryName(filePath) + "\\" + fileName;
                //Open the file, using the 'path' variable
                oBook = oBooks.Open(path, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing);

                // FR
                Excel.Worksheet xlRange = oBook.Sheets[2];
                if (Convert.ToString(xlRange.Cells[1, 2].Value2) != null)
                {
                    qcm.Id = 0;
                    qcm.Name = Convert.ToString(xlRange.Cells[1, 2].Value2);
                    qcm.QcmTrad = new Domain.QcmLang()
                    {
                        ID_Qcm = 0,
                        ID_Lang = 2,
                        QcmName = Convert.ToString(xlRange.Cells[1, 2].Value2),
                    };
                }
                // Quit Excel and clean up.
                oBook.Close(false, oMissing, oMissing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oBook);
                oBook = null;
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oBooks);
                oBooks = null;
                oExcel.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oExcel);
                oExcel = null;

                //Garbage collection
                GC.Collect();

                return qcm;
            }
            catch (Exception ex)
            {
                // Quit Excel and clean up.
                oBook?.Close(false, oMissing, oMissing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oBook);
                oBook = null;
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oBooks);
                oBooks = null;
                oExcel.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oExcel);
                oExcel = null;

                //Garbage collection
                GC.Collect();
                Debug.WriteLine(ex.StackTrace);
                throw ex;
            }

        }
    }


    public class ExcelExportInfo
    {
        public string TemplatePath { get; set; }
        public string UserNameCreation { get; set; }
    }



}
