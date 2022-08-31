using System.Web;

namespace Intitek.Welcome.Infrastructure.Helpers
{
    public static class Utilities
    {

        /// <summary>
        /// PR : return boolean true/false according to browser is supported or not
        /// Here : IE is not supported
        /// </summary>
        /// <returns>boolean</returns>
        public static bool IsBrowserNotSupported()
        {
            return HttpContext.Current.Request != null &&
                   (HttpContext.Current.Request.UserAgent.Contains("MSIE") ||
                   HttpContext.Current.Request.UserAgent.Contains("Trident"));
        }
    }
}