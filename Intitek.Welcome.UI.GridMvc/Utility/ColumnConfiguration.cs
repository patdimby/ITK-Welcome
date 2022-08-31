using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridMvc.Filtering;

namespace GridMvc.Utility
{
    public class GridConfiguration
    {
        public string Name { get; set; }
        public string SortColumn { get; set; }
        public string SortDirection { get; set; }
        public string GlobaleSearch { get; set; }
        public bool AllowGlobaleSearch { get; set; }
        public string GlobaleSearchParentClass { get; set; }
        public bool GlobaleSearchAjax { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<KeyValuePair<int?, bool>> ColumnPreference { get; set; }
        public string AjaxUrl { get; set; }
        public string GlobaleSearchPlaceHolder { get; set; }
    }

    public class ColumnConfiguration
    {
        public string Name { get; set; }
        public string Selector { get; set; }
        public string DataType { get; set; }
        public bool GlobalSearchEnabled { get; set; }
        public int? textMaxRow { get; set; }
        public string dropdownOptionString { get; set; }
        public bool EditDisabled { get; set; }
    }
}
