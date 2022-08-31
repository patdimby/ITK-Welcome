using System;
using System.Web.Mvc;
using Intitek.Welcome.Infrastructure.Log;

namespace Intitek.Welcome.UI.Web.Filters
{

    public class CustomExceptionHandlerFilter : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            string messageError = "Une erreur système s'est produite." + Environment.NewLine + " Veuillez contacter votre administrateur et/ou réessayer ultérieurement.";
            if (filterContext.Exception is ApplicationException)
            {
                messageError = filterContext.Exception.Message;
            }

            FileLogger fileLogger = new FileLogger();
            fileLogger.Error(new ExceptionLogger()
                    {
                        ExceptionMessage = filterContext.Exception.Message,
                        ExceptionStackTrack = filterContext.Exception.StackTrace,
                        ServiceName = filterContext.RouteData.Values["controller"].ToString(),
                        MethodName = filterContext.RouteData.Values["action"].ToString(),
                        ExceptionDateTime = DateTime.Now
                    },
                    filterContext.Exception);

        }
    }
}