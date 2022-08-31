using System.Web;
using System.Web.Mvc;
using GridMvc.Columns;

namespace GridMvc
{
    public class GridCellRenderer : GridStyledRenderer, IGridCellRenderer
    {
        private const string TdClass = "grid-cell";

        public GridCellRenderer()
        {
            AddCssClass(TdClass);
        }

        public IHtmlString Render(IGridColumn column, IGridCell cell)
        {
            string cssStyles = GetCssStylesString();
            if (column.DataPriority > 0)
            {
                this.AddCssClass("priority-" + column.DataPriority);
            }
            string cssClass = GetCssClassesString();

            var builder = new TagBuilder("td");
            if (!string.IsNullOrWhiteSpace(cssClass))
                builder.AddCssClass(cssClass);
            if (!string.IsNullOrWhiteSpace(cssStyles))
                builder.MergeAttribute("style", cssStyles);
            builder.MergeAttribute("data-name", column.Name);

            if (column.Colspan > 1)
            {
                builder.MergeAttribute("colspan", column.Colspan.ToString());
            }
            if (!string.IsNullOrEmpty(column.Onclick))
            {
                builder.MergeAttribute("onclick", column.Onclick);
            }
            if (!string.IsNullOrEmpty(column.OnDblclick))
            {
                builder.MergeAttribute("ondblclick", column.OnDblclick);
            }
            builder.InnerHtml = cell.ToString();

            return MvcHtmlString.Create(builder.ToString());
        }
    }
}