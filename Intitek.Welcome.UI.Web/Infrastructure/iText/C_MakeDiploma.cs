using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Service.Front;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.IO;

namespace Intitek.Welcome.UI.Web.Infrastructure
{
    public static class C_MakeDiploma
    {
        public const string FOLDER_TEMPLATE_DIPLOME = "~/Templates/Diplome/";


        /// <summary>
        /// Build a diploma
        /// </summary>
        /// <param name="GuGus">information about engineer and its diploma</param>
        /// <returns></returns>
        public static PdfDocument MakeDiploma(DiplomeDTO diplomeDTO)
        {
            String FileName;    // File name of PDF template
            PdfDocument HndPdfDoc; // Handle on PDF Document
            XGraphics HndGrPdf; // PDF graphical context
            int xstartNamePositionDefault = 228;
            int xstartDatePositionDefault = 450;
            // Get template
            FileName = System.Web.HttpContext.Current.Server.MapPath(FOLDER_TEMPLATE_DIPLOME) + diplomeDTO.DiplomeTemplateFilename;
            bool bFileNameExist = false;
            if (!File.Exists(FileName))
            {
                if (!diplomeDTO.CodeLangue.Equals(diplomeDTO.CodeDefaultLangue))
                {
                    FileName = System.Web.HttpContext.Current.Server.MapPath(FOLDER_TEMPLATE_DIPLOME) + diplomeDTO.DefaultTemplateFilename;
                    if (!File.Exists(FileName))
                    {
                        bFileNameExist = false;
                    }
                    else
                    {
                        bFileNameExist = true;
                    }
                }
                
            }
            else
            {
                bFileNameExist = true;
            }
            if (!bFileNameExist)
            {
                var logger = new FileLogger();
                Exception ex = new FileNotFoundException("File not found", FileName);
                logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionMessage = "File not found",
                    MethodName = "MakeDiploma",
                    ServiceName = "C_MakeDiploma",

                }, ex);
                throw ex;
            }
            PdfDocument HndPdfTemplate = PdfReader.Open(FileName, PdfDocumentOpenMode.Import);
            // Open template
            HndPdfDoc = new PdfDocument();
            HndPdfDoc.AddPage(HndPdfTemplate.Pages[0]);
            HndPdfTemplate.Close();
            HndPdfTemplate.Dispose();
            // Handle to graphic space
            HndGrPdf = XGraphics.FromPdfPage(HndPdfDoc.Pages[0]);
            // Common parameters
            XBrush HndXBrush = new XSolidBrush(XColor.FromKnownColor(XKnownColor.Black));
            XStringFormat XFormat = new XStringFormat();
            XFormat.Alignment = XStringAlignment.Center;
            XFormat.LineAlignment = XLineAlignment.Center;
            // Add name
            HndGrPdf.DrawString(diplomeDTO.NameOrFullName,
                                new XFont("Poppins Semi-Bold", (double)30),
                                HndXBrush,
                                new XRect(0, xstartNamePositionDefault, HndPdfDoc.Pages[0].Width, 50),
                                XFormat);
            // Add date
            HndGrPdf.DrawString(String.Format(Resources.Resource.Diplome_Fait_Le, string.Format("{0:dd/MM/yyyy}", diplomeDTO.Date)),
                                new XFont("Poppins", (double)10),
                                HndXBrush,
                                new XRect(0, xstartDatePositionDefault, HndPdfDoc.Pages[0].Width, 50),
                                XFormat);
            HndGrPdf.Dispose();
            return (HndPdfDoc);
        }

    }
}
