using Grid.Mvc.Ajax.Helpers;
using GridMvc;
using GridMvc.Html;
using GridMvc.Pagination;
using Intitek.Welcome.Service.Front;
using Intitek.Welcome.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Intitek.Welcome.UI.Web.Models
{
    public class DocumentGrid : Grid<DocumentViewModel>
    {
        private static int MaxDisplayedPages = 5;
        public bool IsReadOnly { get; set; } = false;
        public string Search { get; set; }
        public bool HasFilter { get; set; }
        public DocumentGrid(GetUserDocumentRequest gridRequest, IEnumerable<DocumentViewModel> items, int total, int pagesize)
        {
            BeforeItems = items.AsQueryable();
            string nameGrid = gridRequest.GridName;
            this.Search = gridRequest.Search;
            this.HasFilter = (!string.IsNullOrEmpty(this.Search) || (gridRequest.Filtres != null && gridRequest.Filtres.Length > 0));
            //set up sort and filter settings use in session
            _settings = new SessionGridSettingsProvider(gridRequest);
            this.InternalInit(false);
            RenderOptions = new GridRenderOptions();
            this.RenderOptions.GridName = nameGrid;
            this.RenderOptions.ShowGridItemsCount = false;

            var pager = new GridPager();
            pager.ParameterName = GetParameterName(gridRequest.GridName);
            pager.ParameterPagesize = GetParameterPagesize(gridRequest.GridName);

            pager.MaxDisplayedPages = MaxDisplayedPages;
            pager.TemplateName = "_AjaxGridPager";
            if (pagesize > 0)
                pager.PageSize = pagesize;
            else
                this.EnablePaging = false;

            this.Pager = pager;

            this.SetItemsCount(total);
            ApplyGridSettings();
        }
        private void SetItemsCount(int itemsCount)
        {
            this.ItemsCount = itemsCount;
            ((GridPager)this.Pager).ItemsCount = itemsCount;
            this.PagerProcessor = new PagerGridItemsProcessorItemsCount<DocumentViewModel>(this.Pager);
        }
        public static string GetParameterName(int indexGrid)
        {
            return "grid" + indexGrid + "-page";
        }
        public static string GetParameterPagesize(int indexGrid)
        {
            return "grid" + indexGrid + "-pagesize"; 
        }
        public static string GetParameterName(string nameGrid)
        {
            return nameGrid + "-page";
        }
        public static string GetParameterPagesize(string nameGrid)
        {
            return nameGrid + "-pagesize";
        }
        /// <summary>
        /// Nombre de colonne
        /// </summary>
        public int NbColumns
        {
            get
            {
                return this.Columns.Count();
            }
        }
        public string ToJson(string gridPartialViewName, ViewDataDictionary viewData, Controller controller)
        {
            var htmlHelper = new AjaxGridHtmlHelpers();
            return htmlHelper.RenderPartialViewToString(gridPartialViewName, this, viewData, controller);
        }
        public string ToJson(string gridPartialViewName, Object model, ViewDataDictionary viewData, Controller controller)
        {
            var htmlHelper = new AjaxGridHtmlHelpers();
            return htmlHelper.RenderPartialViewToString(gridPartialViewName, model, viewData, controller);
        }
    }
}