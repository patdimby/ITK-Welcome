using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace Grid.Mvc.Ajax.Helpers
{
    public class AjaxGridHtmlHelpers : IHtmlHelpers
    {
        public string RenderPartialViewToString(string viewName, object model, ViewDataDictionary viewBagData, Controller controller)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = controller.ControllerContext.RouteData.GetRequiredString("action");

            controller.ViewData.Model = model;
            if (viewBagData != null)
            {
                foreach (KeyValuePair<string, object> thisViewBagItem in viewBagData)
                {
                    controller.ViewData.Add(thisViewBagItem);
                }
            }

            using (var sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                var result = sw.GetStringBuilder().ToString();
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}