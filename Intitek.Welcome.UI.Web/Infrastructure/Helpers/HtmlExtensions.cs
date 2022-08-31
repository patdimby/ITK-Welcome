using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Intitek.Welcome.Infrastructure.Helpers
{
    public static class HtmlExtensions
    {
        private static readonly UTF8Encoding Encoder = new UTF8Encoding();
        public const string FOLDER_TEMPLATE_DIPLOME = "~/Templates/Diplome/";
        public static IHtmlString ProgressBarClass(this HtmlHelper htmlHelper, int Value, int Total, bool Reverse)
        {
            //Ticket 241: je propose que les barres changent de couleur avec la valeur:
            //De 0 à 20 %: vert
            //De 21 % à 70 %: orange
            // De 71 % à 100 %: rouge
            return (IHtmlString)MvcHtmlString.Create(Total == 0 ? "" : (Reverse ? 
                (Value * 100 / Total >= 20 ? (Value * 100 / Total >= 70 ? "progress-bar-success" : "progress-bar-warning") : "progress-bar-danger-inverse") : 
                (Value * 100 / Total >= 20 ? (Value * 100 / Total >= 70 ? "progress-bar-danger" : "progress-bar-warning") : "progress-bar-success")
            ));
        }
        public static IHtmlString GaugeColor(this HtmlHelper htmlHelper, int Value, int Total)
        {
            //Ticket 241: je propose que les barres changent de couleur avec la valeur:
            //De 0 à 20 %: vert
            //De 21 % à 70 %: orange
            // De 71 % à 100 %: rouge
            return (IHtmlString)MvcHtmlString.Create(Total == 0 ? "" : 
                (Value * 100 / Total >= 20 ? (Value * 100 / Total >= 70 ? "red" : "orange") : "#009c49")) ;
        }

        public static IHtmlString ProgressBarWidth(this HtmlHelper htmlHelper, int Value, int Total)
        {
            decimal valP = 0;
            if (Total>0)
                valP = Decimal.Divide(Value * 100, Total);
            //Minimum width
            if (Total > 0 && valP < 3) valP = 3;
            return (IHtmlString)MvcHtmlString.Create((Total == 0 || Value==0) ? "0%" : string.Format("{0}%", Math.Round(valP)));
        }
        public static IHtmlString Badge(this HtmlHelper htmlHelper, int qcmId, string cultureCode, string defaultCultureCode)
        {
            var uriRepBadge = "/Content/images/badges/";  
            var badgeDefault = string.Format("badge_default_{0}.png", cultureCode);
            var badge = string.Format("badge_{0}_{1}.png", qcmId, cultureCode);
            string templates = System.Web.HttpContext.Current.Server.MapPath("~"+ uriRepBadge);
            if (Utils.FileExist(templates + badge))
            {
                return (IHtmlString)MvcHtmlString.Create(uriRepBadge+ badge);
            }
            else if(!defaultCultureCode.Equals(cultureCode))
            {
                badge = string.Format("badge_{0}_{1}.png", qcmId, defaultCultureCode);
                templates = System.Web.HttpContext.Current.Server.MapPath("~" + uriRepBadge);
                if (Utils.FileExist(templates + badge))
                {
                    return (IHtmlString)MvcHtmlString.Create(uriRepBadge + badge);
                }
            }
            return (IHtmlString)MvcHtmlString.Create("");
        }

        public static bool BadgeExist(this HtmlHelper htmlHelper, int qcmId, string cultureCode)
        {
            var repBadge = "~/Content/images/badges/";
            var badge = string.Format("badge_{0}_{1}.png", qcmId, cultureCode);
            string templates = System.Web.HttpContext.Current.Server.MapPath(repBadge);
            if (Utils.FileExist(templates + badge))
            {
                return true;
            }

            return false;
        }

        public static bool TemplateFileExist(this HtmlHelper htmlHelper, int qcmId, string cultureCode)
        {
            var repBadge = FOLDER_TEMPLATE_DIPLOME;
            var badge = string.Format("diplome_{0}_{1}.pdf", qcmId, cultureCode);
            string templates = System.Web.HttpContext.Current.Server.MapPath(repBadge);
            if (Utils.FileExist(templates + badge))
            {
                return true;
            }
            return false;
        }

        public static IHtmlString TemplateFile(this HtmlHelper htmlHelper, int qcmId, string cultureCode, bool bGetDefault)
        {
            var repTempl = FOLDER_TEMPLATE_DIPLOME;
            var filename = string.Format("diplome_{0}_{1}.pdf", qcmId, cultureCode);
            string templates = System.Web.HttpContext.Current.Server.MapPath(repTempl);

            var uriTemplate = "/Templates/Diplome/";

            var templDefault = string.Format("default_sensibilisation_{0}.pdf", cultureCode);
            if (Utils.FileExist(templates + filename))
            {
                return (IHtmlString)MvcHtmlString.Create(uriTemplate + filename);
            }
            if (bGetDefault)
            {
                return (IHtmlString)MvcHtmlString.Create(uriTemplate + templDefault);
            }
            return (IHtmlString)MvcHtmlString.Create("");
        }
        public static bool VideoExistBO(this HtmlHelper htmlHelper, int docId, string docVersion, string extension, string cultureCode=null)
        {
            if (string.IsNullOrEmpty(cultureCode))
            {
                cultureCode = Culture(htmlHelper).Substring(0, 2);
            }
            if (!".pdf".Equals(extension, StringComparison.InvariantCultureIgnoreCase))
            {
                string videoFile = string.Format("document_{0}_{1}_{2}.mp4", docId, docVersion, cultureCode);
                var dirVideo = ConfigurationManager.AppSettings["Welcome.video.directory"];
                var pahVideo = System.IO.Path.Combine(dirVideo, videoFile);
                if (Utils.FileExist(pahVideo))
                {
                    return true;
                }
                return false;
            }
            return true;
        }
        public static bool VideoExistFront(this HtmlHelper htmlHelper, int docId, string extension, string defaultCultureCode, string docVersion)
        {
            var cultureCode = Culture(htmlHelper).Substring(0, 2);
            if (!".pdf".Equals(extension, StringComparison.InvariantCultureIgnoreCase))
            {
                string videoFile = string.Format("document_{0}_{1}_{2}.mp4", docId, docVersion, cultureCode);
                var dirVideo = ConfigurationManager.AppSettings["Welcome.video.directory"];
                var pahVideo = System.IO.Path.Combine(dirVideo, videoFile);
                if (Utils.FileExist(pahVideo))
                {
                    return true;
                }
                else
                {
                    videoFile = string.Format("document_{0}_{1}.mp4", docId, defaultCultureCode);
                    pahVideo = System.IO.Path.Combine(dirVideo, videoFile);
                    if (Utils.FileExist(pahVideo))
                    {
                        return true;
                    }
                }
                return false;
            }
            return true;
        }
        public static string Culture(this HtmlHelper htmlHelper)
        {
            var Request = System.Web.HttpContext.Current.Request;
            var _culture = Request.Cookies["_culture"] != null ? Request.Cookies["_culture"].Value : (Request.UserLanguages != null && Request.UserLanguages.Length > 0 ?
                       Request.UserLanguages[0] : "fr-FR");
            return _culture;
        }
        public static string EncryptURL(this HtmlHelper htmlHelper, string url)
        {
            var key = "x2pV8E8NK5Y85oHVqM0B2agPDDX9e1mJk0bJO75Hr+M="; //ConfigurationManager.AppSettings["keyPwd"];
            var iv = "qf6bYB7dJxer+CQjoVhAdQ=="; // ConfigurationManager.AppSettings["ivPwd"];
            var urlEncrypt =  EncryptionHelper.Encrypt(url, key, iv);
            return HttpServerUtility.UrlTokenEncode(Encoder.GetBytes(urlEncrypt));
        }
        public static string DecryptURL(string url)
        {
            var bytes = HttpServerUtility.UrlTokenDecode(url);
            var key = "x2pV8E8NK5Y85oHVqM0B2agPDDX9e1mJk0bJO75Hr+M="; //ConfigurationManager.AppSettings["keyPwd"];
            var iv = "qf6bYB7dJxer+CQjoVhAdQ=="; // ConfigurationManager.AppSettings["ivPwd"];
            var urlDecrypt = EncryptionHelper.Decrypt(Encoder.GetString(bytes), key, iv);
            return urlDecrypt;
        }
    }
}
