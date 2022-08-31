using System;

namespace Intititek.Welcome.Service.Front
{
    public class ColumnFilter
    {
        private const string FilterDataDelimeter = "__";
        public static string GetOperator(string field, GridFilterType type, int index)
        {
            string ret = "";
            switch (type)
            {
                case GridFilterType.Equals:
                    ret = field + "== @" + index;
                    break;
                case GridFilterType.LessThan:
                    ret = field + "< @" + index;
                    break;
                case GridFilterType.GreaterThan:
                    ret = field + "> @" + index;
                    break;
                case GridFilterType.LessThanOrEquals:
                    ret = field + "<= @" + index;
                    break;
                case GridFilterType.GreaterThanOrEquals:
                    ret = field + ">= @" + index;
                    break;
                case GridFilterType.Contains:
                    ret = field + ".Contains(@" + index + ")";
                    break;
                case GridFilterType.StartsWith:
                    ret = field + ".StartsWith(@" + index + ")";
                    break;
                case GridFilterType.EndsWidth:
                    ret = field + ".EndsWith(@" + index + ")";
                    break;
                default:
                    break;
            }
            return ret;
        }

        public enum GridFilterType
        {
            Equals = 1,
            Contains = 2,
            StartsWith = 3,
            EndsWidth = 4,
            GreaterThan = 5,
            LessThan = 6,
            GreaterThanOrEquals = 7,
            LessThanOrEquals = 8
        };
        public string ColumnName { get; set; }

        public GridFilterType FilterType { get; set; }

        public string FilterValue { get; set; }

        public ColumnFilter CreateColumnData(string queryParameterValue)
        {
            if (string.IsNullOrEmpty(queryParameterValue))
                return null;

            string[] data = queryParameterValue.Split(new[] { FilterDataDelimeter }, StringSplitOptions.RemoveEmptyEntries);
            if (data.Length != 3)
                return ColumnFilterValue.Null;
            GridFilterType type;
            if (!Enum.TryParse(data[1], true, out type))
                type = GridFilterType.Equals;

            return new ColumnFilter { ColumnName = data[0], FilterType = type, FilterValue = data[2] };
        }
    }

}
