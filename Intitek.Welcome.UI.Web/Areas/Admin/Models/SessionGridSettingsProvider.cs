using GridMvc;
using GridMvc.Filtering;
using GridMvc.Sorting;
using Intitek.Welcome.Service.Back;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Intitek.Welcome.UI.Web.Admin.Models
{
    public class SessionGridSettingsProvider : QueryStringGridSettingsProvider
    {
        public SessionGridSettingsProvider(GridMvcRequest gridRequest)
        {
            _sortSettings = new QueryStringSortSettings();
            _sortSettings.ColumnQueryParameterName = gridRequest.GridName + "-column";
            _sortSettings.DirectionQueryParameterName = gridRequest.GridName + "-dir";
            if (!string.IsNullOrEmpty(gridRequest.OrderColumn))
            {
                _sortSettings.ColumnName = gridRequest.OrderColumn;
                _sortSettings.Direction = GridSortDirection.Ascending;
            }
            if (gridRequest.SortDirection==1)
            {
                _sortSettings.Direction = GridSortDirection.Descending;
            }
            //add additional header renderer for filterable columns:
            _filterSettings = new QueryStringFilterSettings(gridRequest.Filtres);
        }
        

    }
}