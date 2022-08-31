using Grid.Mvc.Ajax.Helpers;
using GridMvc;
using GridMvc.Filtering;
using GridMvc.Pagination;
using Intitek.Welcome.Infrastructure.Helpers;
using Intitek.Welcome.Service.Back;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GridMvc.Html;
using Intitek.Welcome.UI.ViewModels;

namespace Intitek.Welcome.UI.Web.Admin.Models
{
    public static class GridBORequest
    {
        private static string PAGESIZE_COOKIE_BO = "pagesizeBO";
        private static string INPUT_SEARCH = "filtre";
        private static string CATEGORIES_COLLAPSE = "categories";
        public static string GetParameterName(string nameGrid)
        {
            return nameGrid + "-page";
        }
        public static string GetParameterPagesize(string nameGrid)
        {
            return nameGrid + "-pagesize";
        }
        public static GridMvcRequest GetRequestGrid(HttpRequestBase Request, string nameGrid, string defaultOrderColum)
        {
            int pageSize, pageIndex, orderDirection;
            int.TryParse(Request.QueryString[GetParameterName(nameGrid)], out pageIndex);
            int.TryParse(Request.QueryString[GetParameterPagesize(nameGrid)], out pageSize);
            string orderColumn = Request.QueryString[QueryStringGridSettingsProvider.GetColumnQueryParameterName(nameGrid)];
            int.TryParse(Request.QueryString[QueryStringGridSettingsProvider.GetDirectionQueryParameterName(nameGrid)], out orderDirection);
            //Trier par defaut
            if (string.IsNullOrEmpty(orderColumn) && !string.IsNullOrEmpty(defaultOrderColum))
            {
                orderColumn = defaultOrderColum;
                orderDirection = 0;
            }
            //Filtre
            string[] filters = Request.QueryString.GetValues(QueryStringFilterSettings.DefaultTypeQueryParameter);
            if (pageSize == 0)
            {
                pageSize = 10;
                if (Utils.GetCookies(PAGESIZE_COOKIE_BO) != null)
                {
                    pageSize = Int32.Parse(Utils.GetCookies(PAGESIZE_COOKIE_BO));
                }
            }
            else
            {
                int cookPageSize = 0;
                if (Utils.GetCookies(PAGESIZE_COOKIE_BO) != null)
                {
                    cookPageSize = Int32.Parse(Utils.GetCookies(PAGESIZE_COOKIE_BO));
                }
                if (cookPageSize != pageSize)
                {
                    Utils.SetCookies(PAGESIZE_COOKIE_BO, pageSize, DateTime.MaxValue);
                }
            }
            if (pageIndex == 0) pageIndex = 1;
            string search = Request.QueryString[INPUT_SEARCH];
            string categories = Request.QueryString[CATEGORIES_COLLAPSE];
            return new GridMvcRequest() { GridName=nameGrid, Page = pageIndex, Limit = pageSize, OrderColumn = orderColumn, SortDirection = orderDirection, Search = search, Filtres = filters, Categories=categories };
        }
    }
    public class GridBO<T> : Grid<T> where T : class
    {
        private static int MaxDisplayedPages = 10;
        private bool _hasProcessors = false;
        public string Search { get; set; }
        public string Categories { get; set; }
        public List<int> CategorieCollapseds {
            get {
                if (!string.IsNullOrEmpty(this.Categories))
                {
                    return this.Categories.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Int32.Parse(x)).ToList();
                }
                return null;
            }
        }
        /// <summary>
        /// Plusieurs grid afficher dans une page
        /// </summary>
        /// <param name="indexGrid"></param>
        /// <param name="items"></param>
        public GridBO(GridMvcRequest gridRequest, IEnumerable<T> items, int? total, int pagesize)
        {
            BeforeItems = items.AsQueryable();
            string nameGrid = gridRequest.GridName;
            this.Search = gridRequest.Search;
            this.Categories = gridRequest.Categories;
            this._hasProcessors = total.HasValue ? false : true;

            //set up sort and filter settings use in session
            _settings = new SessionGridSettingsProvider(gridRequest);
            this.InternalInit(this._hasProcessors);
            RenderOptions = new GridRenderOptions();
            this.RenderOptions.GridName = nameGrid;

            var pager = new GridPager();
            pager.ParameterName = GridBORequest.GetParameterName(nameGrid);
            pager.ParameterPagesize = GridBORequest.GetParameterPagesize(nameGrid);

            pager.MaxDisplayedPages = MaxDisplayedPages;
            pager.TemplateName = "_AjaxGridPager";
            if (pagesize > 0)
                pager.PageSize = pagesize;
            else
                this.EnablePaging = false;

            this.Pager = pager;
            if (!this._hasProcessors)
            {
                this.SetItemsCount(total.Value);
            }
            ApplyGridSettings();
        }
        /// <summary>
        /// Si total==null, le Grid gère le tri, le filtre, et la pagination aussi
        /// </summary>
        /// <param name="nameGrid"></param>
        /// <param name="items"></param>
        /// <param name="total"></param>
        /// <param name="pagesize"></param>
        public GridBO(string nameGrid, IEnumerable<T> items, int? total, int pagesize)
            : base(nameGrid, items, total.HasValue? false : true)
        {
            this._hasProcessors = total.HasValue ? false : true;
            //Grid Named
            this.RenderOptions.GridName = nameGrid;
            //this.RenderOptions.ShowGridItemsCount = false;
            var pager = new GridPager();
            pager.ParameterName = GridBORequest.GetParameterName(nameGrid);
            pager.ParameterPagesize = GridBORequest.GetParameterPagesize(nameGrid);
            pager.MaxDisplayedPages = MaxDisplayedPages;
            pager.TemplateName = "_AjaxGridPager";
            if (pagesize > 0)
                pager.PageSize = pagesize;
            else
                this.EnablePaging = false;
            this.Pager = pager;
            if (!this._hasProcessors)
            {
                this.SetItemsCount(total.Value);
            }              
        }
       
        /// <sum
        private void SetItemsCount(int itemsCount)
        {
            this.ItemsCount = itemsCount;
            ((GridPager)this.Pager).ItemsCount = itemsCount;
            this.PagerProcessor = new PagerGridItemsProcessorItemsCount<T>(this.Pager);
        }
        public int NbColumns
        {
            get
            {
                return this.Columns.Count();
            }
        }
        public string ToJson(string gridPartialViewName, Controller controller)
        {
            var htmlHelper = new AjaxGridHtmlHelpers();
            return htmlHelper.RenderPartialViewToString(gridPartialViewName, this, new ViewDataDictionary(), controller);
        }

        public string ToJson(string gridPartialViewName, Object model, Controller controller)
        {
            var htmlHelper = new AjaxGridHtmlHelpers();
            return htmlHelper.RenderPartialViewToString(gridPartialViewName, model, new ViewDataDictionary(), controller);
        }
        public string ToJson(string gridPartialViewName, ViewDataDictionary viewData, Controller controller)
        {
            var htmlHelper = new AjaxGridHtmlHelpers();
            return htmlHelper.RenderPartialViewToString(gridPartialViewName, this, viewData, controller);
        }

    }
}