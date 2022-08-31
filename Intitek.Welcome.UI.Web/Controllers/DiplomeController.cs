using Intitek.Welcome.Infrastructure.Helpers;
using System;
using System.IO;
using System.Web.Mvc;
using Intitek.Welcome.Service.Front;
using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.UI.Web.Infrastructure;
using PdfSharp.Pdf;
using Intitek.Welcome.UI.Web.Filters;

namespace Intitek.Welcome.UI.Web.Controllers
{
    [UrlRestrictAccessFilter]
    public class DiplomeController : CommunController
    {
        private readonly IUserService _userService;
        private readonly IDocumentService _documentService;
        private readonly ILangService _langService;

        public DiplomeController()
        {
            _userService = new UserService(new FileLogger());
            _documentService = new DocumentService(new FileLogger());
            _langService = new LangService(new FileLogger());

        }
        /// <summary>
        /// Get pdf diplome en utilisant iTextSharp (template .html)
        /// </summary>
        /// <returns></returns>
       
        public ActionResult Index()
        {
            string rrr = Request.QueryString["rrr"];
            var codeLangDefault = _langService.Get(new GetLangRequest() { Id = GetDefaultLang() }).CodeLangue;
            var param = HtmlExtensions.DecryptURL(rrr);
            int doc = -1;
            int id = -1;
            DiplomeDTO dto = new DiplomeDTO();
            if (!string.IsNullOrEmpty(param))
            {
                string[] query = param.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                string paramDoc = query[0].Replace("doc=", string.Empty);
                string paramID = query[1].Replace("ID=", string.Empty);
                Int32.TryParse(paramDoc, out doc);
                Int32.TryParse(paramID, out id);
                //var histoUserQcm = _documentService.FindByUserQcm(id);
                var histoUserQcm = _documentService.FindByHistoUserQcm(id);
                dto.Date = histoUserQcm.DateFin;
            }
            var userId = this.GetUserIdConnected();
            var user = _userService.GetById(userId);
            // var userLDAP = _adService.ActiveDirectoryUserByLogin(user.Username);
            dto.ID = doc;
            dto.Name = user.Username;
            dto.FullName = GetUserNomPrenomConnected();
            
            var _culture = Request.Cookies["_culture"] != null ? Request.Cookies["_culture"].Value : (Request.UserLanguages != null && Request.UserLanguages.Length > 0 ?
                       Request.UserLanguages[0] : "fr-FR");
            dto.CodeLangue = _culture.Substring(0, 2);
            dto.CodeDefaultLangue = codeLangDefault;
            PdfDocument HndPdfDoc; // Handle on PDF Document

            // Get dipmoma
            HndPdfDoc = C_MakeDiploma.MakeDiploma(dto);
            // Convert to stream
            byte[] FileBytes;   // Bytes of PDF file
            using (MemoryStream HndStream = new MemoryStream())
            {
                HndPdfDoc.Save(HndStream, true);
                FileBytes = HndStream.ToArray();
            }
            HndPdfDoc.Dispose();
            return File(FileBytes, "application/pdf");
        }
    }
}