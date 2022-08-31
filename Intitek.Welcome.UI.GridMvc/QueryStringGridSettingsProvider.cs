using GridMvc.Filtering;
using GridMvc.Sorting;

namespace GridMvc
{
    /// <summary>
    ///     Provider of grid settings, based on query string parameters
    /// </summary>
    public class QueryStringGridSettingsProvider : IGridSettingsProvider
    {
        protected QueryStringFilterSettings _filterSettings;
        protected QueryStringSortSettings _sortSettings;

        public QueryStringGridSettingsProvider()
        {
            _sortSettings = new QueryStringSortSettings();
            //add additional header renderer for filterable columns:
            _filterSettings = new QueryStringFilterSettings();
        }
        /// <summary>
        /// ajout code 17/01/2020
        /// </summary>
        /// <param name="indexGrid"></param>
        public QueryStringGridSettingsProvider(int indexGrid)
        {
            _sortSettings = new QueryStringSortSettings();
            _sortSettings.ColumnQueryParameterName = GetColumnQueryParameterName(indexGrid);
            _sortSettings.DirectionQueryParameterName = GetDirectionQueryParameterName(indexGrid);
            //add additional header renderer for filterable columns:
            _filterSettings = new QueryStringFilterSettings();
        }
        public QueryStringGridSettingsProvider(string gridName)
        {
            _sortSettings = new QueryStringSortSettings();
            _sortSettings.ColumnQueryParameterName = gridName + "-column";
            _sortSettings.DirectionQueryParameterName = gridName + "-dir";
            //add additional header renderer for filterable columns:
            _filterSettings = new QueryStringFilterSettings();
        }
        public static string GetColumnQueryParameterName(int indexGrid)
        {
            return "grid" + indexGrid + "-column";
        }
        public static string GetDirectionQueryParameterName(int indexGrid)
        {
            return "grid" + indexGrid + "-dir";
        }
        public static string GetColumnQueryParameterName(string gridName)
        {
            return gridName + "-column";
        }
        public static string GetDirectionQueryParameterName(string gridName)
        {
            return gridName + "-dir";
        }

        #region IGridSettingsProvider Members

        public IGridSortSettings SortSettings
        {
            get { return _sortSettings; }
        }

        public IGridFilterSettings FilterSettings
        {
            get { return _filterSettings; }
        }

        public IGridColumnHeaderRenderer GetHeaderRenderer()
        {
            var headerRenderer = new GridHeaderRenderer();
            //MOD 29/01/2020 -Convertir AddAdditionalRenderer de QueryStringSortColumnHeaderRenderer et QueryStringFilterColumnHeaderRenderer
            headerRenderer.AddAdditionalRenderer(new QueryStringSortColumnHeaderRenderer(_sortSettings));
            headerRenderer.AddAdditionalRenderer(new QueryStringFilterColumnHeaderRenderer(_filterSettings));
            return headerRenderer;
        }

        #endregion
    }
}