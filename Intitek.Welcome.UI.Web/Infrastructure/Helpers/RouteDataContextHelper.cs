using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Web.Routing;

namespace Intitek.Welcome.Infrastructure.Helpers
{
    public class RouteDataContextHelper : HttpContextBase
    {
        public override HttpRequestBase Request { get; }

        private RouteDataContextHelper(Uri uri)
        {
            var url = uri.GetLeftPart(UriPartial.Path);
            var qs = uri.GetComponents(UriComponents.Query, UriFormat.UriEscaped);

            Request = new HttpRequestWrapper(new HttpRequest(null, url, qs));
        }

        public static RouteValueDictionary RouteValuesFromUri(Uri uri)
        {
            return RouteTable.Routes.GetRouteData(new RouteDataContextHelper(uri)).Values;
        }
    }
}