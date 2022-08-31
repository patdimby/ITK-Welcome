using System;
using System.Web;
using GridMvc.Columns;
using System.Collections.Generic;

namespace GridMvc.Html
{
    /// <summary>
    ///     Grid options for html helper
    /// </summary>
    public interface IGridHtmlOptions<T> : IHtmlString
    {
        IGridHtmlOptions<T> Columns(Action<IGridColumnCollection<T>> columnBuilder);
        /// <summary>
        /// Enable paging with default pageSize
        /// </summary>
        /// <returns></returns>
        IGridHtmlOptions<T> WithPaging();
        /// <summary>
        ///     Enable paging for grid
        /// </summary>
        /// <param name="pageSize">Setup the page size of the grid</param>
        IGridHtmlOptions<T> WithPaging(int pageSize);

        /// <summary>
        ///     Enable paging for grid
        /// </summary>
        /// <param name="pageSize">Setup the page size of the grid</param>
        /// <param name="maxDisplayedItems">Setup max count of displaying pager links</param>
        IGridHtmlOptions<T> WithPaging(int pageSize, int maxDisplayedItems);

        /// <summary>
        ///     Enable paging for grid
        /// </summary>
        /// <param name="pageSize">Setup the page size of the grid</param>
        /// <param name="maxDisplayedItems">Setup max count of displaying pager links</param>
        /// <param name="queryStringParameterName">Query string parameter name</param>
        IGridHtmlOptions<T> WithPaging(int pageSize, int maxDisplayedItems, string queryStringParameterName);
        /// <summary>
        /// Drop down page size (10,20,50,100)
        /// </summary>
        /// <returns></returns>
        IGridHtmlOptions<T> SetPageSizes(List<int> pages);

        /// <summary>
        ///     Enable sorting for all columns
        /// </summary>
        IGridHtmlOptions<T> Sortable();

        /// <summary>
        ///     Enable or disable sorting for all columns
        /// </summary>
        IGridHtmlOptions<T> Sortable(bool enable);

        /// <summary>
        ///     Enable filtering for all columns
        /// </summary>
        IGridHtmlOptions<T> Filterable();

        /// <summary>
        ///     Enable or disable filtering for all columns
        /// </summary>
        IGridHtmlOptions<T> Filterable(bool enable);

        /// <summary>
        ///     Enable resizing for all columns
        /// </summary>
        IGridHtmlOptions<T> Resizable();

        /// <summary>
        ///     Enable or disable resizing for all columns
        /// </summary>
        IGridHtmlOptions<T> Resizable(bool enable);

        /// <summary>
        ///     Define what column shows
        /// </summary>
        IGridHtmlOptions<T> SetPreference(Dictionary<int?, bool> preference);

        /// <summary>
        ///     Enable or disable client grid items selectable feature
        /// </summary>
        IGridHtmlOptions<T> Selectable(bool set);

        /// <summary>
        ///     Setup the text, which will displayed with empty items collection in the grid
        /// </summary>
        /// <param name="text">Grid empty text</param>
        IGridHtmlOptions<T> EmptyText(string text);

        /// <summary>
        ///     Setup the language of Grid.Mvc
        /// </summary>
        /// <param name="lang">SetLanguage string (example: "en", "ru", "fr" etc.)</param>
        IGridHtmlOptions<T> SetLanguage(string lang);

        /// <summary>
        ///     Setup specific row css classes
        /// </summary>
        IGridHtmlOptions<T> SetRowCssClasses(Func<T, string> contraint);

        /// <summary>
        ///     Specify Grid client name
        /// </summary>
        IGridHtmlOptions<T> Named(string gridName);

        /// <summary>
        ///     Generates columns for all properties of the model.
        ///     Use data annotations to customize columns
        /// </summary>
        IGridHtmlOptions<T> AutoGenerateColumns();

        /// <summary>
        ///     Allow grid to use multiple filters
        /// </summary>
        IGridHtmlOptions<T> WithMultipleFilters();

        /// <summary>
        ///    Allow grid to show Grid items count
        /// </summary>
        IGridHtmlOptions<T> WithGridItemsCount(string gridItemsName);

        /// <summary>
        ///    the url to load data with ajax
        /// </summary>
        IGridHtmlOptions<T> SetAjaxUrl(string url);

        /// <summary>
        ///    the placeHolder of search input
        /// </summary>
        IGridHtmlOptions<T> SetGlobaleSearchPlaceHolder(string placeHolder);

        /// <summary>
        ///    Define the html class to inserte globale shercheach input
        /// </summary>
        /// /// <param name="htmlClass">the html classe to inserte globale shercheach input</param>
        IGridHtmlOptions<T> GlobaleSearchParent(string htmlClass, bool AllowAjaxSearch = false);

        /// <summary>
        ///    Allow grid to show Grid items count
        /// </summary>
        IGridHtmlOptions<T> WithGridItemsCount();
        IGridHtmlOptions<T> SetOnClick(string onclick);
        IGridHtmlOptions<T> SetOnDblClick(string onclick);

        /// <summary>
        ///     Obviously render Grid markup
        /// </summary>
        /// <returns>Grid html layout</returns>
        string Render();
    }
}