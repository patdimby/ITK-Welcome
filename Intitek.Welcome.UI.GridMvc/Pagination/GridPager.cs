﻿using System;
using System.Globalization;
using System.Linq;
using System.Web;
using GridMvc.Utility;
using System.Collections.Generic;

namespace GridMvc.Pagination
{
    /// <summary>
    ///     Default grid pager implementation
    /// </summary>
    public class GridPager : IGridPager
    {
        public const int DefaultMaxDisplayedPages = 5;
        public const int DefaultPageSize = 10;
        private List<int> _DefaultPageSizes = new List<int>() { 10, 20, 50, 100 };

        public const string DefaultPageQueryParameter = "grid-page";
        public const string DefaultPageSizeQueryParameter = "grid-pagesize";
        public const string DefaultPagerViewName = "_GridPager";

        private readonly HttpContext _context;
        private readonly CustomQueryStringBuilder _queryBuilder;
        private int _currentPage;
        private bool _readonly;

        private int _itemsCount;
        private int _maxDisplayedPages;
        private int _pageSize;
        private List<int> _pageSizes;

        #region ctor's

        public GridPager()
            : this(HttpContext.Current)
        {
        }

        public GridPager(HttpContext context)
        {
            _context = context ?? throw new Exception("No http context here!");
            _currentPage = -1;
            _queryBuilder = new CustomQueryStringBuilder(HttpContext.Current.Request.QueryString);

            ParameterName = DefaultPageQueryParameter;
            ParameterPagesize = DefaultPageSizeQueryParameter;
            TemplateName = DefaultPagerViewName;
            PageSize = DefaultPageSize;
            PageSizes = _DefaultPageSizes;
        }

        #endregion

        #region IGridPager members

        public int PageSize
        {
            get {
                var tmp = 0;
                string currentPageString = _context.Request.QueryString[ParameterPagesize];
                if (!int.TryParse(currentPageString, out tmp))
                    tmp = _pageSize;
                if (tmp != _pageSize)
                {
                    _pageSize = tmp;
                }
                return _pageSize;
            }
            set
            {
                _pageSize = value;
                //RecalculatePages();
            }
        }
        public List<int> PageSizes
        {
            get { return _pageSizes; }
            set {  _pageSizes = value; }
        }

        public bool IsReadOnly
        {
            get { return _readonly; }
            set { _readonly = value; }
        } 

        public int CurrentPage
        {
            get
            {
                if (_currentPage >= 0) return _currentPage;
                string currentPageString = _context.Request.QueryString[ParameterName] ?? "1";
                if (!int.TryParse(currentPageString, out _currentPage))
                    _currentPage = 1;
                if (_currentPage > PageCount)
                    _currentPage = PageCount;
                return _currentPage;
            }
            protected internal set
            {
                _currentPage = value;
                if (_currentPage > PageCount)
                    _currentPage = PageCount;
                RecalculatePages();
            }
        }

        #endregion

        /// <summary>
        ///     Query string parameter name, that determine current displaying page
        /// </summary>
        public string ParameterName { get; set; }
        public string ParameterPagesize { get; set; }
        

        /// <summary>
        ///     Total items of the initial collection
        /// </summary>
        public virtual int ItemsCount
        {
            get { return _itemsCount; }
            set
            {
                _itemsCount = value;
                RecalculatePages();
            }
        }

        public int MaxDisplayedPages
        {
            get { return _maxDisplayedPages == 0 ? DefaultMaxDisplayedPages : _maxDisplayedPages; }
            set
            {
                _maxDisplayedPages = value;
                RecalculatePages();
            }
        }

        /// <summary>
        ///     Total pages count
        /// </summary>
        public int PageCount { get; protected set; }

        public virtual void Initialize<T>(IQueryable<T> items)
        {
            ItemsCount = items.Count(); //take total items count from collection
        }

        protected virtual void RecalculatePages()
        {
            if (ItemsCount == 0)
            {
                PageCount = 0;
                return;
            }
            PageCount = (int) (Math.Ceiling(ItemsCount/(double) PageSize));

            //if (CurrentPage > PageCount)
            //    CurrentPage = PageCount;

            StartDisplayedPage = (CurrentPage - MaxDisplayedPages/2) < 1 ? 1 : CurrentPage - MaxDisplayedPages/2;
            EndDisplayedPage = (CurrentPage + MaxDisplayedPages/2) > PageCount
                                   ? PageCount
                                   : CurrentPage + MaxDisplayedPages/2;
        }

        #region View

        public int StartDisplayedPage { get; protected set; }
        public int EndDisplayedPage { get; protected set; }
        public string TemplateName { get; set; }
        public virtual string GetLinkForPage(int pageIndex)
        {
            return _queryBuilder.GetQueryStringWithParameter(ParameterName,
                                                             pageIndex.ToString(CultureInfo.InvariantCulture));
        }
        public virtual string GetLinkForPageSize(int pageSize)
        {
            return _queryBuilder.GetQueryStringWithParameter(ParameterPagesize,
                                                             pageSize.ToString(CultureInfo.InvariantCulture));
        }
        public virtual string GetLinkForPageSize(string pageSize)
        {
            return _queryBuilder.GetQueryStringWithParameter(ParameterPagesize,
                                                             pageSize.ToString(CultureInfo.InvariantCulture));
        }

        #endregion
    }
}