using Intitek.Welcome.Infrastructure.Helpers;
using System;

namespace Intitek.Welcome.Service.Front
{
    public class ColumnFilter
    {
        private const string FilterDataDelimeter = "__";
        public string GetOperator(int index, string filterValue)
        {
            string Field = this.ColumnName;
            string ret = "";
            if ("ID_Category".Equals(this.ColumnName) && "-1".Equals(filterValue))
            {
                ret = "(ID_Category== null OR ID_Category==0)";
                return ret;
            }
            else if ("ID_SubCategory".Equals(this.ColumnName) && "-1".Equals(filterValue))
            {
                ret = "(ID_SubCategory== null OR ID_SubCategory==0)";
                return ret;
            }
            else if ("Name".Equals(this.ColumnName) && !string.IsNullOrEmpty(filterValue))
            {
                Field = "EdmxFunction.RemoveAccent(Name)";
                this.FilterValue = Utils.RemoveAccent(filterValue);
            }
            switch (this.FilterType)
            {
                case GridFilterType.Equals:
                    ret = Field + "== @" + index;
                    break;
                case GridFilterType.LessThan:
                    ret = Field + "< @" + index;
                    break;
                case GridFilterType.GreaterThan:
                    ret = Field + "> @" + index;
                    break;
                case GridFilterType.LessThanOrEquals:
                    ret = Field + "<= @" + index;
                    break;
                case GridFilterType.GreaterThanOrEquals:
                    ret = Field + ">= @" + index;
                    break;
                case GridFilterType.Contains:
                    ret = Field + ".Contains(@" + index + ")";
                    break;
                case GridFilterType.StartsWith:
                    ret = Field + ".StartsWith(@" + index + ")";
                    break;
                case GridFilterType.EndsWidth:
                    ret = Field + ".EndsWith(@" + index + ")";
                    break;
                default:
                    break;
            }
            return ret;
        }
        public string Where(int value)
        {
            string ret = "";
            switch (this.ColumnName)
            {
                case "FiltreRead":
                    switch (value)
                    {
                        case 1:
                            ret = "!IsBoolRead";
                            break;
                        case 2:
                            ret = "IsBoolRead";
                            break;
                        default:
                            break;
                    }
                    break;
                case "FiltreApproved":
                    switch (value)
                    {
                        case 1:
                            ret = "(Approbation.HasValue AND Approbation.Value == 1 AND !IsBoolApproved)";
                            break;
                        case 2:
                            //ret = "(Approbation.HasValue AND Approbation.Value == 1 AND IsBoolApproved)";
                            ret = "(IsBoolApproved)";
                            break;
                        case 3:
                            //ret = "(Approbation.HasValue==false OR Approbation.Value == 0)";
                            ret = "(!IsBoolApproved)";
                            break;
                        default:
                            break;
                    }
                    break;
                case "FiltreTested":
                    switch (value)
                    {
                        case 1:
                            ret = "(Test.HasValue AND Test.Value == 1 AND !IsBoolTested)";
                            break;
                        case 2:
                            //ret = "(Test.HasValue AND Test.Value == 1 AND IsBoolTested)";
                            ret = "(IsBoolTested)";
                            break;
                        case 3:
                            //ret = "(Test.HasValue==false OR Test.Value == 0)";
                            ret = "(!IsBoolTested)";
                            break;
                        default:
                            break;
                    }
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
            LessThanOrEquals = 8,
            IN_INT32 = 9
        };
        public string ColumnName { get; set; }

        public GridFilterType FilterType { get; set; }

        public string FilterValue { get; set; }

        public static ColumnFilter CreateColumnFilter(string queryParameterValue)
        {
            if (string.IsNullOrEmpty(queryParameterValue))
                return null;

            string[] data = queryParameterValue.Split(new[] { FilterDataDelimeter }, StringSplitOptions.RemoveEmptyEntries);
            if (data.Length != 3)
                return null;
            GridFilterType type;
            if (!Enum.TryParse(data[1], true, out type))
                type = GridFilterType.Equals;

            return new ColumnFilter { ColumnName = data[0], FilterType = type, FilterValue = data[2] };
        }
  
    }

}
