using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Intitek.Welcome.Infrastructure.Helpers
{
    public static class Utils
    {
        public static string UpperFirst(string strD)
        {
            string sBeginPattern = @"[a-z]{1}";
            Match match = new Regex(sBeginPattern).Match(strD);
            char[] replaceChars = strD.ToCharArray();
            replaceChars[match.Index] = char.Parse(match.Value.ToUpper());
            return new string(replaceChars);
        }
        /// <summary>
        /// Date de la forme 05 Mai 2019
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>

        public static string GetDateFr(DateTime date, bool hour)
        {
            string patternFR = "d MMMM yyyy";
            if(hour)
                patternFR = "d MMMM yyyy à HH\\h mm";
            CultureInfo francais = CultureInfo.GetCultureInfo("fr-FR");
            string strD = date.ToString(patternFR, francais);
            return UpperFirst(strD);
        }
         public static void SetCookies(string name, object value, DateTime expires)
        {
            HttpContext.Current.Response.Cookies[name].Value = value+"";
            HttpContext.Current.Response.Cookies[name].Expires = expires;
        }
        public static string GetCookies(string name)
        {
            if (HttpContext.Current.Request.Cookies[name] != null)
            {
                return HttpContext.Current.Request.Cookies[name].Value;
            }
            return null;
        }

        public static string StripText(string texte, int length)
        {
            var strippedText = string.Empty;
            strippedText = texte.Length > length ? string.Format("{0} ...", texte.Substring(0, 127)) : texte;
            return strippedText;
        }
        public static bool FileExist(string file)
        {
            return System.IO.File.Exists(file);
        }
        /// <summary>
        /// Supprimer l 'accent et convert la chaine en minuscule
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveAccent(string str)
        {
            if (null == str) return null;
            str = str.Replace("&amp;", "&").Replace("&#233;", "e").Replace("&#39;", "'");
            var chars = str
                .Normalize(NormalizationForm.FormD)
                .ToCharArray()
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray();

            var ret = new string(chars).Normalize(NormalizationForm.FormC);
            return ret.ToLower();
        }
        public static bool IsValidEmail(string email)
        {
            return new EmailAddressAttribute().IsValid(email);
        }
    }
}
