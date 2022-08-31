using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using GridMvc.Columns;

namespace GridMvc
{
    public class GridHeaderRenderer : GridStyledRenderer, IGridColumnHeaderRenderer
    {
        private const string ThClass = "grid-header";

        private readonly List<IGridColumnHeaderRenderer> _additionalRenders = new List<IGridColumnHeaderRenderer>();

        public GridHeaderRenderer()
        {
            AddCssClass(ThClass);
        }

        public IHtmlString Render(IGridColumn column)
        {
            string cssStyles = GetCssStylesString();
            if (column.DataPriority > 0)
            {
                this.AddCssClass("priority-"+ column.DataPriority);
            }
            string cssClass = GetCssClassesString();

            if (!string.IsNullOrWhiteSpace(column.Width))
                cssStyles = string.Concat(cssStyles, " width:", column.Width, ";").Trim();
            if (!string.IsNullOrWhiteSpace(column.MinWidth))
                cssStyles = string.Concat(cssStyles, " min-width:", column.MinWidth, ";").Trim();
            if (!string.IsNullOrWhiteSpace(column.MaxWidth))
                cssStyles = string.Concat(cssStyles, " max-width:", column.MaxWidth, ";").Trim();

            var builder = new TagBuilder("th");
            builder.Attributes.Add("data-name", !String.IsNullOrEmpty(column.Selector) ? column.Selector : column.Name);
            
            if (!string.IsNullOrWhiteSpace(cssClass))
                builder.AddCssClass(cssClass);
            if (!string.IsNullOrWhiteSpace(cssStyles))
                builder.MergeAttribute("style", cssStyles);
            if (!column.ResizeEnabled)
            {
                builder.MergeAttribute("data-noresizable", "1");
            }
            builder.InnerHtml = RenderAdditionalContent(column);

            return MvcHtmlString.Create(builder.ToString());
        }

        protected virtual string RenderAdditionalContent(IGridColumn column)
        {
            if (_additionalRenders.Count == 0) return string.Empty;
            var sb = new StringBuilder();
            foreach (IGridColumnHeaderRenderer gridColumnRenderer in _additionalRenders)
            {
                sb.Append(gridColumnRenderer.Render(column));
            }
            return sb.ToString();
        }

        public void AddAdditionalRenderer(IGridColumnHeaderRenderer renderer)
        {
            if (_additionalRenders.Contains(renderer))
                throw new InvalidOperationException("This renderer already exist");
            _additionalRenders.Add(renderer);
        }

        public void InsertAdditionalRenderer(int position, IGridColumnHeaderRenderer renderer)
        {
            if (_additionalRenders.Contains(renderer))
                throw new InvalidOperationException("This renderer already exist");
            _additionalRenders.Insert(position, renderer);
        }
    }
}