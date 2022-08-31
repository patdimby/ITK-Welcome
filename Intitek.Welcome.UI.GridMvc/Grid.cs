using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GridMvc.Columns;
using GridMvc.DataAnnotations;
using GridMvc.Filtering;
using GridMvc.Html;
using GridMvc.Pagination;
using GridMvc.Resources ;
using GridMvc.Sorting;
using GridMvc.Utility;

namespace GridMvc
{
    /// <summary>
    ///     Grid.Mvc base class
    /// </summary>
    public class Grid<T> : GridBase<T>, IGrid where T : class
    {
        private IGridAnnotaionsProvider _annotaions;
        private IColumnBuilder<T> _columnBuilder;
        private GridColumnCollection<T> _columnsCollection;
        private FilterGridItemsProcessor<T> _currentFilterItemsProcessor;
        private SortGridItemsProcessor<T> _currentSortItemsProcessor;

        private int _displayingItemsCount = -1; // count of displaying items (if using pagination)
        private bool _enablePaging;
        private IGridPager _pager;

        private IGridItemsProcessor<T> _pagerProcessor;
        protected IGridSettingsProvider _settings;

        public Grid(): base()
        {
        }
        public Grid(IEnumerable<T> items)
            : this(items.AsQueryable())
        {
        }

        public Grid(IQueryable<T> items)
            : base(items)
        {
            #region init default properties
            //set up sort settings:
            _settings = new QueryStringGridSettingsProvider();
            this.InternalInit(true);
            #endregion
            RenderOptions = new GridRenderOptions();

            ApplyGridSettings();
        }
        /// <summary>
        /// Plusieurs grid afficher dans une page
        /// </summary>
        /// <param name="indexGrid"></param>
        /// <param name="items"></param>
        public Grid(int indexGrid, IEnumerable<T> items, bool hasTransformProcessor=true)
           : base(items.AsQueryable())
        {
            #region init default properties
            _settings = new QueryStringGridSettingsProvider(indexGrid);
            this.InternalInit(hasTransformProcessor);
            #endregion
            RenderOptions = new GridRenderOptions();
            ApplyGridSettings();
        }
        /// <summary>
        /// Plusieurs grid afficher dans une page
        /// </summary>
        /// <param name="indexGrid"></param>
        /// <param name="items"></param>
        public Grid(string gridName, IEnumerable<T> items, bool hasTransformProcessor = true)
           : base(items.AsQueryable())
        {
            //set up sort settings:
            _settings = new QueryStringGridSettingsProvider(gridName);
            this.InternalInit(hasTransformProcessor);
            RenderOptions = new GridRenderOptions();

            ApplyGridSettings();
        }
        protected void InternalInit(bool hasTransformProcessor)
        {
            #region init default properties
            Sanitizer = new Sanitizer();
            EmptyGridText = Strings.DefaultGridEmptyText;
            Language = Strings.Lang;
            //c'est le grid qui gère le tri et les filtres
            if (hasTransformProcessor)
            {
                _currentSortItemsProcessor = new SortGridItemsProcessor<T>(this, _settings.SortSettings);
                _currentFilterItemsProcessor = new FilterGridItemsProcessor<T>(this, _settings.FilterSettings);
                AddItemsPreProcessor(_currentFilterItemsProcessor);
                InsertItemsProcessor(0, _currentSortItemsProcessor);
            }

            _annotaions = new GridAnnotaionsProvider();

            #endregion

            //Set up column collection:
            _columnBuilder = new DefaultColumnBuilder<T>(this, _annotaions);
            _columnsCollection = new GridColumnCollection<T>(_columnBuilder, _settings.SortSettings);
        }
        /// <summary>
        ///     Grid columns collection
        /// </summary>
        public IGridColumnCollection<T> Columns
        {
            get { return _columnsCollection; }
        }

        /// <summary>
        ///     Sets or get default value of sorting for all adding columns
        /// </summary>
        public bool DefaultSortEnabled
        {
            get { return _columnBuilder.DefaultSortEnabled; }
            set { _columnBuilder.DefaultSortEnabled = value; }
        }

        /// <summary>
        ///     Set or get default value of filtering for all adding columns
        /// </summary>
        public bool DefaultFilteringEnabled
        {
            get { return _columnBuilder.DefaultFilteringEnabled; }
            set { _columnBuilder.DefaultFilteringEnabled = value; }
        }
        public bool DefaultResizeEnabled
        {
            get { return _columnBuilder.DefaultResizeEnabled; }
            set { _columnBuilder.DefaultResizeEnabled = value; }
        }

        public GridRenderOptions RenderOptions { get; set; }

        public string Named
        {
            get { return this.RenderOptions.GridName; }
        }

        /// <summary>
        ///     Provides settings, using by the grid
        /// </summary>
        public override IGridSettingsProvider Settings
        {
            get { return _settings; }
            set
            {
                _settings = value;
                _currentSortItemsProcessor.UpdateSettings(_settings.SortSettings);
                _currentFilterItemsProcessor.UpdateSettings(_settings.FilterSettings);
            }
        }

        /// <summary>
        ///     Items, displaying in the grid view
        /// </summary>
        IEnumerable<object> IGrid.ItemsToDisplay
        {
            get { return GetItemsToDisplay(); }
        }
        
        #region IGrid Members

        /// <summary>
        ///     Count of current displaying items
        /// </summary>
        public virtual int DisplayingItemsCount
        {
            get
            {
                if (_displayingItemsCount >= 0)
                    return _displayingItemsCount;
                _displayingItemsCount = GetItemsToDisplay().Count();
                return _displayingItemsCount;
            }
        }

        /// <summary>
        ///     Enable or disable paging for the grid
        /// </summary>
        public bool EnablePaging
        {
            get { return _enablePaging; }
            set
            {
                if (_enablePaging == value) return;
                _enablePaging = value;
                if (_enablePaging)
                {
                    if (_pagerProcessor == null)
                        _pagerProcessor = new PagerGridItemsProcessor<T>(Pager);
                    AddItemsProcessor(_pagerProcessor);
                }
                else
                {
                    RemoveItemsProcessor(_pagerProcessor);
                }
            }
        }
        public string Language { get; set; }

        public IGridItemsProcessor<T> PagerProcessor
        {
            get { return _pagerProcessor; }
            set { _pagerProcessor = value; }
        }

        /// <summary>
        ///     Gets or set Grid column values sanitizer
        /// </summary>
        public ISanitizer Sanitizer { get; set; }

        /// <summary>
        ///     Manage pager properties
        /// </summary>
        public IGridPager Pager
        {
            get { return _pager ?? (_pager = new GridPager()); }
            set { _pager = value; }
        }

        IGridColumnCollection IGrid.Columns
        {
            get { return Columns; }
        }

        #endregion

        /// <summary>
        ///     Applies data annotations settings
        /// </summary>
        protected void ApplyGridSettings()
        {
            GridTableAttribute opt = _annotaions.GetAnnotationForTable<T>();
            if (opt == null) return;
            EnablePaging = opt.PagingEnabled;
            if (opt.PageSize > 0)
                Pager.PageSize = opt.PageSize;

            if (opt.PagingMaxDisplayedPages > 0 && Pager is GridPager)
            {
                (Pager as GridPager).MaxDisplayedPages = opt.PagingMaxDisplayedPages;
            }
        }

        /// <summary>
        ///     Methods returns items that will need to be displayed
        /// </summary>
        protected internal virtual IEnumerable<T> GetItemsToDisplay()
        {
            PrepareItemsToDisplay();
            return AfterItems;
        }

        /// <summary>
        ///     Generates columns for all properties of the model
        /// </summary>
        public virtual void AutoGenerateColumns()
        {
            //TODO add support order property
            PropertyInfo[] properties = typeof (T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo pi in properties)
            {
                if (pi.CanRead)
                    Columns.Add(pi);
            }
        }

    }
}