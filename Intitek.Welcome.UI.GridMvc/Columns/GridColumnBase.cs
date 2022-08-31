using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using GridMvc.Filtering;
using GridMvc.Sorting;
using GridMvc.Utility;

namespace GridMvc.Columns
{
    public abstract class GridColumnBase<T> : IGridColumn<T>
    {
        protected Func<T, string> ValueConstraint;
        protected string ValuePattern;

        #region IGridColumn<T> Members

        public bool EncodeEnabled { get; protected set; }
        public bool HtmlEnabled { get; protected set; }
        public bool EditDisabled { get; set; }
        public bool SanitizeEnabled { get; set; }
        public bool GlobaleSearchEnabled { get; set; }
        public bool IsFixed { get; set; }
        public string HeaderClass { get; set; }
        public string SortColumnHtmlContent { get; set; }
        public string SortedDescSortColumnHtmlContent { get; set; }
        public string SortedAscColumnHtmlContent { get; set; }
        public string FilterColumnHtmlContent { get; set; }
        public int? TextMaxRow { get; set; }
        public string dropdownOptionString { get; set; }

        public string Width { get; set; }
        public string MinWidth { get; set; }
        public string MaxWidth { get; set; }
        public int Colspan { get; set; }
        public string Onclick { get; set; }
        public string OnDblclick { get; set; }
        public int DataPriority { get; set; }
        public bool SortEnabled { get; protected set; }

        public string Title { get; set; }

        public string Name { get; set; }

        public string Selector { get; set; }

        public bool IsSorted { get; set; }
        public GridSortDirection? Direction { get; set; }

        public IGridColumn<T> Titled(string title)
        {
            Title = title;
            return this;
        }

        public IGridColumn<T> Titled(Expression<Func<T, dynamic>> titleField)
        {
            Title = PropertiesHelper.GetDisplayName(titleField);
            return this;
        }

        public IGridColumn<T> Encoded(bool encode)
        {
            EncodeEnabled = encode;
            return this;
        }
        public IGridColumn<T> IsHtml(bool html)
        {
            HtmlEnabled = html;
            return this;
        }
        public IGridColumn<T> DisableEditable(bool editable)
        {
            EditDisabled = editable;
            return this;
        }

        IGridColumn<T> IColumn<T>.SetWidth(string width)
        {
            Width = width;
            return this;
        }
        IGridColumn<T> IColumn<T>.SetWidth(string width, string minwidth, string maxwidth)
        {
            Width = width;
            if (!string.IsNullOrEmpty(minwidth))
                MinWidth = minwidth;
            if (!string.IsNullOrEmpty(maxwidth))
                MaxWidth = maxwidth;
            return this;
        }
        IGridColumn<T> IColumn<T>.SetWidth(int width)
        {
            Width = width.ToString(CultureInfo.InvariantCulture) + "px";
            return this;
        }
        IGridColumn<T> IColumn<T>.SetWidth(int width, bool isminmax)
        {
            Width = width.ToString(CultureInfo.InvariantCulture) + "px";
            if (isminmax)
            {
                MinWidth = width.ToString(CultureInfo.InvariantCulture) + "px"; 
                MaxWidth = width.ToString(CultureInfo.InvariantCulture) + "px"; 
            }
            return this;
        }
        IGridColumn<T> IColumn<T>.SetWidth(int width, int minwidth, int maxwidth)
        {
            Width = width.ToString(CultureInfo.InvariantCulture) + "px";
            if(minwidth>0)
                MinWidth = minwidth.ToString(CultureInfo.InvariantCulture) + "px";
            if (maxwidth > 0)
                MaxWidth = maxwidth.ToString(CultureInfo.InvariantCulture) + "px";
            return this;
        }
        IGridColumn<T> IColumn<T>.SetMinWidth(string width)
        {
            MinWidth = width;
            return this;
        }

        IGridColumn<T> IColumn<T>.SetMinWidth(int width)
        {
            MinWidth = width.ToString(CultureInfo.InvariantCulture) + "px";
            return this;
        }
        IGridColumn<T> IColumn<T>.SetMaxWidth(string width)
        {
            MaxWidth = width;
            return this;
        }

        IGridColumn<T> IColumn<T>.SetMaxWidth(int width)
        {
            MaxWidth = width.ToString(CultureInfo.InvariantCulture) + "px";
            return this;
        }
        public IGridColumn<T> SetColspan(int colspan)
        {
            Colspan = colspan;
            return this;
        }
        public IGridColumn<T> SetOnclick(string onclick)
        {
            Onclick = onclick;
            return this;
        }
        public IGridColumn<T> SetOnDblclick(string onclick)
        {
            OnDblclick = onclick;
            return this;
        }
        IGridColumn<T> IColumn<T>.SetDataPriority(int priority)
        {
            DataPriority = priority;
            return this;
        }
        public IGridColumn<T> Css(string cssClasses)
        {
            if (string.IsNullOrEmpty(cssClasses))
                return this;
            var headerStyledRender = HeaderRenderer as GridStyledRenderer;
            if (headerStyledRender != null)
                headerStyledRender.AddCssClass(cssClasses);

            var cellStyledRender = CellRenderer as GridStyledRenderer;
            if (cellStyledRender != null)
                cellStyledRender.AddCssClass(cssClasses);
            return this;
        }


        public IGridColumn<T> RenderValueAs(Func<T, string> constraint)
        {
            ValueConstraint = constraint;
            return this;
        }

        public IGridColumn<T> Format(string pattern)
        {
            ValuePattern = pattern;
            return this;
        }

        public abstract IGrid ParentGrid { get; }

        public virtual IGridColumn<T> Sanitized(bool sanitize)
        {
            SanitizeEnabled = sanitize;
            return this;
        }

        public virtual IGridColumn<T> GlobaleSearch(bool allowed)
        {
            GlobaleSearchEnabled = allowed;
            return this;
        }

        public virtual IGridColumn<T> Fixed(bool isFixed)
        {
            IsFixed = isFixed;
            return this;
        }

        public virtual IGridColumn<T> SetHeaderClass(string cssClass)
        {
            HeaderClass = cssClass;
            return this;
        }

        public virtual IGridColumn<T> SetSortColumnHtmlContent(string htmlContent)
        {
            SortColumnHtmlContent = htmlContent;
            return this;
        }

        public virtual IGridColumn<T> SetIsSortedDescColumnHtmlContent(string htmlContent)
        {
            SortedDescSortColumnHtmlContent = htmlContent;
            return this;
        }

        public virtual IGridColumn<T> SetIsSortedAscColumnHtmlContent(string htmlContent)
        {
            SortedAscColumnHtmlContent = htmlContent;
            return this;
        }

        public virtual IGridColumn<T> setFilterHtmlContent(string htmlContent)
        {
            FilterColumnHtmlContent = htmlContent;
            return this;
        }

        public virtual IGridColumn<T> SetTextMaxRow(int maxRow)
        {
            TextMaxRow = maxRow;
            return this;
        }

        public virtual IGridColumn<T> ToDropdownOnEdit(string optionJson)
        {
            dropdownOptionString = optionJson;
            return this;
        }

        public IGridColumn<T> SetInitialFilter(GridFilterType type, string value)
        {
            var filter = new ColumnFilterValue
                {
                    FilterType = type,
                    FilterValue = value,
                    ColumnName = Name
                };
            InitialFilterSettings = filter;
            return this;
        }

        public abstract IGridColumn<T> SortInitialDirection(GridSortDirection direction);

        public abstract IGridColumn<T> ThenSortBy<TKey>(Expression<Func<T, TKey>> expression);
        public abstract IGridColumn<T> ThenSortByDescending<TKey>(Expression<Func<T, TKey>> expression);

        public abstract IEnumerable<IColumnOrderer<T>> Orderers { get; }
        public abstract IGridColumn<T> Sortable(bool sort);

        public abstract IGridColumnHeaderRenderer HeaderRenderer { get; set; }
        public abstract IGridCellRenderer CellRenderer { get; set; }
        public abstract IGridCell GetCell(object instance);

        public abstract bool FilterEnabled { get; set; }
        public abstract bool ResizeEnabled { get; set; }

        public ColumnFilterValue InitialFilterSettings { get; set; }

        public abstract IGridColumn<T> Filterable(bool showColumnValuesVariants);
        public abstract IGridColumn<T> Resizable(bool showColumnValuesVariants);


        public abstract IGridColumn<T> SetFilterWidgetType(string typeName);
        public abstract IGridColumn<T> SetFilterWidgetType(string typeName, object widgetData);


        public abstract IColumnFilter<T> Filter { get; }
        public abstract string FilterWidgetTypeName { get; }
        public object FilterWidgetData { get; protected set; }
        
        #endregion

        public abstract IGridCell GetValue(T instance);

        public abstract IGridColumn<T> SetCustomFilter(Expression<Func<T, string, bool>> expression);

        public abstract IGridColumn<T> SelectListFilter(IEnumerable<GridSelectListItem> selectList);
        public abstract IGridColumn<T> MultiCheckboxListFilter(IEnumerable<GridSelectListItem> selectList);


    }
}