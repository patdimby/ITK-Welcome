using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Intitek.Welcome.UI.Web.Infrastructure
{
    public enum DocType
    {
        PDF,
        EXCEL
    }
    public class FileDownloadResult : ActionResult
    {
        public static readonly string COOKIE_FILEDOWNLOAD = "fileDownload";
        public FileDownloadResult() { }

        public FileDownloadResult(DocType type, string fileName, MemoryStream stream)
        {
            this.MemoryStream = stream;
            this.FileDownloadName = fileName;
            if (type == DocType.EXCEL)
            {
                this.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; ;
            }
            else if (type == DocType.PDF)
            {
                this.ContentType = "application/pdf";
            }           
        }
        private string ContentType
        {
            get;
            set;
        }
        private MemoryStream MemoryStream
        {
            get;
            set;
        }

        private string FileDownloadName
        {
            get;
            set;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.Clear();
            if (!String.IsNullOrEmpty(FileDownloadName))
            {
                var ContentDisp = "";
                if (context.HttpContext.Request.Browser.Browser == "IE" && (context.HttpContext.Request.Browser.Version == "7.0" || context.HttpContext.Request.Browser.Version == "8.0"))
                    ContentDisp = "attachment; filename=" + Uri.EscapeDataString(FileDownloadName);
                else if (context.HttpContext.Request.UserAgent != null && context.HttpContext.Request.UserAgent.ToLowerInvariant().Contains("android"))
                    ContentDisp = "attachment; filename=\"" + _MakeAndroidSafeFileName(FileDownloadName) + "\"";
                else
                    ContentDisp = "attachment; filename=\"" + FileDownloadName + "\"; filename*=UTF-8''" + Uri.EscapeDataString(FileDownloadName);
                context.HttpContext.Response.AddHeader("Content-Disposition", ContentDisp);
                if (MemoryStream.Length > 0)
                {
                    MemoryStream.Seek(0, SeekOrigin.Begin);
                }
                MemoryStream.WriteTo(context.HttpContext.Response.OutputStream);
                MemoryStream.Dispose();
               
                context.HttpContext.Response.AddHeader("Cache-Control", "max-age=0");
                context.HttpContext.Response.HeaderEncoding = Encoding.UTF8;
                context.HttpContext.Response.ContentEncoding = Encoding.UTF8;
                context.HttpContext.Response.ContentType = this.ContentType;
                context.HttpContext.Response.SetCookie(new HttpCookie(COOKIE_FILEDOWNLOAD, "true") { Path = "/", HttpOnly = true });
            }
            context.HttpContext.Response.Flush();
            context.HttpContext.Response.End();
        }
        /// <summary>
        /// Helper function for filanme under Android
        /// </summary>
        private static readonly Dictionary<char, char> AndroidAllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ._-+,@£$€!½§~'=()[]{}0123456789".ToDictionary(c => c);
        private string _MakeAndroidSafeFileName(string fileName)
        {
            char[] newFileName = fileName.ToCharArray();
            for (int i = 0; i < newFileName.Length; i++)
            {
                if (!AndroidAllowedChars.ContainsKey(newFileName[i]))
                    newFileName[i] = '_';
            }
            return new string(newFileName);
        }
    }
}