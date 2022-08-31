using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using GridMvc.Filtering;
using GridMvc.Sorting;
using GridMvc.Utility;

namespace GridMvc.Columns
{
    public interface IGridColumn<T> : IGridColumn, IColumn<T>, ISortableColumn<T>, IFilterableColumn<T>, IResizableColumn<T>
    {
    }

    public interface IGridColumn : ISortableColumn, IFilterableColumn, IResizableColumn
    {
        IGrid ParentGrid { get; }
    }

    /// <summary>
    ///     fluent interface for grid column
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IColumn<T>
    {
        /// <summary>
        ///     Set gridColumn title
        /// </summary>
        /// <param name="title">Title text</param>
        IGridColumn<T> Titled(string title);

        IGridColumn<T> Titled(Expression<Func<T, dynamic>> titleField);

        /// <summary>
        ///     Need to encode the content of the gridColumn
        /// </summary>
        /// <param name="encode">Yes/No</param>
        IGridColumn<T> Encoded(bool encode);

        /// <summary>
        /// Format html
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        IGridColumn<T> IsHtml(bool html);

        /// <summary>
        ///     Set the column as editable on edit
        /// </summary>
        /// <param name="encode">Yes/No</param>
        IGridColumn<T> DisableEditable(bool editable);

        /// <summary>
        ///     Sanitize column value from XSS attacks
        /// </summary>
        /// <param name="sanitize">If true values from this column will be sanitized</param>
        IGridColumn<T> Sanitized(bool sanitize);

        /// <summary>
        ///     Allow globaleShearch in the column
        /// </summary>
        /// <param name="allowed">If true the colums will affected by globale search</param>
        IGridColumn<T> GlobaleSearch(bool allowed);

        IGridColumn<T> Fixed(bool isFixed);

        IGridColumn<T> SetHeaderClass(string cssClass);

        IGridColumn<T> SetSortColumnHtmlContent(string htmlContent);
        IGridColumn<T> SetIsSortedDescColumnHtmlContent(string htmlContent);
        IGridColumn<T> SetIsSortedAscColumnHtmlContent(string htmlContent);
        IGridColumn<T> setFilterHtmlContent(string htmlContent);

        /// <summary>
        ///     Show toltip when height is upper than defined height
        /// </summary>
        /// <param name="height">height</param>
        IGridColumn<T> SetTextMaxRow(int maxRow);

        /// <summary>
        ///     Sets the width of the column
        /// </summary>
        IGridColumn<T> SetWidth(string width);

        /// <summary>
        ///     Sets the width of the column in pizels
        /// </summary>
        IGridColumn<T> SetWidth(int width);
        IGridColumn<T> SetWidth(int width, bool isminmax);
        IGridColumn<T> SetWidth(int width, int miwidth, int maxwidth);
        IGridColumn<T> SetWidth(string width, string miwidth, string maxwidth);
        IGridColumn<T> SetMinWidth(string width);
        IGridColumn<T> SetDataPriority(int priority);

        /// <summary>
        ///     Sets the width of the column in pizels
        /// </summary>
        IGridColumn<T> SetMinWidth(int width);
        IGridColumn<T> SetMaxWidth(string width);

        /// <summary>
        ///     Sets the width of the column in pizels
        /// </summary>
        IGridColumn<T> SetMaxWidth(int width);

        IGridColumn<T> SetColspan(int colspan);
        IGridColumn<T> SetOnclick(string onclick);
        IGridColumn<T> SetOnDblclick(string onclick);

        /// <summary>
        ///     Specify additional css class of the column
        /// </summary>
        IGridColumn<T> Css(string cssClasses);

        /// <summary>
        ///     Setup the custom rendere for property
        /// </summary>
        IGridColumn<T> RenderValueAs(Func<T, string> constraint);

        /// <summary>
        ///     Format column values with specified text pattern
        /// </summary>
        IGridColumn<T> Format(string pattern);

        /// <summary>
        ///     Supply a custom filter option that is executed
        /// </summary>
        IGridColumn<T> SetCustomFilter(Expression<Func<T, string, bool>> expression);

        IGridColumn<T> SelectListFilter(IEnumerable<GridSelectListItem> selectList);
        IGridColumn<T> MultiCheckboxListFilter(IEnumerable<GridSelectListItem> selectList);
    }

    public interface IColumn
    {
        /// <summary>
        ///     Columns title
        /// </summary>
        string Title { get; }

        /// <summary>
        ///     Internal name of the gridColumn
        /// </summary>
        string Name { get; set; }

        /// <summary>
        ///     The selector of the column
        /// </summary>
        string Selector { get; set; }

        /// <summary>
        ///     Width of the column
        /// </summary>
        string Width { get; set; }
        string MinWidth { get; set; }
        string MaxWidth { get; set; }

        int Colspan { get; set; }
        string Onclick { get; set; }
        string OnDblclick { get; set; }
        int DataPriority { get; set; }

        /// <summary>
        ///     EncodeEnabled
        /// </summary>
        bool EncodeEnabled { get; }

        bool SanitizeEnabled { get; }

        bool GlobaleSearchEnabled { get; }

        bool IsFixed { get; set; }

        string HeaderClass { get; set; }

        string SortColumnHtmlContent { get; set; }

        string SortedDescSortColumnHtmlContent { get; set; }

        string SortedAscColumnHtmlContent { get; set; }

        string FilterColumnHtmlContent { get; set; }

        bool EditDisabled { get; }

        int? TextMaxRow { get; }

        string dropdownOptionString { get; }

        IGridColumnHeaderRenderer HeaderRenderer { get; set; }
        IGridCellRenderer CellRenderer { get; set; }

        /// <summary>
        ///     Gets value of the gridColumn by instance
        /// </summary>
        /// <param name="instance">Instance of the item</param>
        IGridCell GetCell(object instance);
    }

    /// <summary>
    ///     fluent interface for grid sorted column
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISortableColumn<T> : IColumn
    {
        /// <summary>
        ///     List of column orderes
        /// </summary>
        IEnumerable<IColumnOrderer<T>> Orderers { get; }

        /// <summary>
        ///     Enable sort of the gridColumn
        /// </summary>
        /// <param name="sort">Yes/No</param>
        IGridColumn<T> Sortable(bool sort);

        /// <summary>
        ///     Setup the initial sorting direction of current column
        /// </summary>
        /// <param name="direction">Ascending / Descending</param>
        IGridColumn<T> SortInitialDirection(GridSortDirection direction);

        /// <summary>
        ///     Setup ThenBy sorting of current column
        /// </summary>
        IGridColumn<T> ThenSortBy<TKey>(Expression<Func<T, TKey>> expression);

        /// <summary>
        ///     Setup ThenByDescending sorting of current column
        /// </summary>
        IGridColumn<T> ThenSortByDescending<TKey>(Expression<Func<T, TKey>> expression);
    }

    public interface ISortableColumn : IColumn
    {
        /// <summary>
        ///     Enable sort for this column
        /// </summary>
        bool SortEnabled { get; }

        /// <summary>
        ///     Is current column sorted
        /// </summary>
        bool IsSorted { get; set; }

        /// <summary>
        ///     Sort direction of current column
        /// </summary>
        GridSortDirection? Direction { get; set; }
    }

    public interface IFilterableColumn<T>
    {
        /// <summary>
        ///     Collection of current column filter
        /// </summary>
        IColumnFilter<T> Filter { get; }

        /// <summary>
        ///     Allows filtering for this column
        /// </summary>
        /// <param name="enalbe">Enable/disable filtering</param>
        IGridColumn<T> Filterable(bool enalbe);

        /// <summary>
        ///     Set up initial filter for this column
        /// </summary>
        /// <param name="type">Filter type</param>
        /// <param name="value">Filter value</param>
        IGridColumn<T> SetInitialFilter(GridFilterType type, string value);

        /// <summary>
        ///     Specify custom filter widget type for this column
        /// </summary>
        /// <param name="typeName">Widget type name</param>
        IGridColumn<T> SetFilterWidgetType(string typeName);

        /// <summary>
        ///     Set column as dropdown on edit
        /// </summary>
        /// <param name="propertyJsonString">option en format jso</param>
        IGridColumn<T> ToDropdownOnEdit(string optionJson);

        /// <summary>
        ///     Specify custom filter widget type for this column
        /// </summary>
        /// <param name="typeName">Widget type name</param>
        /// <param name="widgetData">The data would be passed to the widget</param>
        IGridColumn<T> SetFilterWidgetType(string typeName, object widgetData);
    }
    
    public interface IFilterableColumn : IColumn
    {
        /// <summary>
        ///     Internal name of the gridColumn
        /// </summary>
        bool FilterEnabled { get; }

        /// <summary>
        ///     Initial filter settings for the column
        /// </summary>
        ColumnFilterValue InitialFilterSettings { get; set; }

        string FilterWidgetTypeName { get; }

        object FilterWidgetData { get; }
    }

    public interface IResizableColumn<T>
    {
        /// <summary>
        ///     Allows resizable for this column
        /// </summary>
        /// <param name="enalbe">Enable/disable resizing</param>
        IGridColumn<T> Resizable(bool enalbe);
    }
    public interface IResizableColumn : IColumn
    {
        /// <summary>
        ///     Internal name of the gridColumn
        /// </summary>
        bool ResizeEnabled { get; }
    }
}