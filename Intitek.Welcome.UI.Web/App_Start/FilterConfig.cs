using Intitek.Welcome.Infrastructure.Attributes;
using Intitek.Welcome.UI.Web.Filters;
using System.Web.Mvc;

namespace Intitek.Welcome.UI.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new AuthorizeAttribute());
            filters.Add(new HandleErrorAttribute());
            filters.Add(new NoCacheAttribute());
            filters.Add(new CustomExceptionHandlerFilter());
        }
    }
}
