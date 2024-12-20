﻿using System;

namespace Intitek.Welcome.Service.Back
{
    public class ColumnFilter
    {
        private const string FilterDataDelimeter = "__";
        public string GetOperatorString(int index, string filterValue)
        {
            if(string.IsNullOrEmpty(this.Field))
                this.Field = this.ColumnName;
            string ret = "";
            switch (this.FilterType)
            {
                case GridFilterType.Equals:
                    ret = this.Field + "== @" + index;
                    break;
                case GridFilterType.LessThan:
                    ret = this.Field + "< @" + index;
                    break;
                case GridFilterType.GreaterThan:
                    ret = this.Field + "> @" + index;
                    break;
                case GridFilterType.LessThanOrEquals:
                    ret = this.Field + "<= @" + index;
                    break;
                case GridFilterType.GreaterThanOrEquals:
                    ret = this.Field + ">= @" + index;
                    break;
                case GridFilterType.Contains:
                    ret = this.Field + ".Contains(@" + index + ")";
                    break;
                case GridFilterType.StartsWith:
                    ret = this.Field + ".StartsWith(@" + index + ")";
                    break;
                case GridFilterType.EndsWidth:
                    ret = this.Field + ".EndsWith(@" + index + ")";
                    break;
                default:
                    break;
            }
            return ret;
        }
        public string GetOperatorDate(int index, string filterValue)
        {
            if (string.IsNullOrEmpty(this.Field))
                this.Field = this.ColumnName;
            string ret = "";
            switch (this.FilterType)
            {
                case GridFilterType.Equals:
                    ret = "DbFunctions.TruncateTime("+this.Field+") == @" + index;
                    break;
                case GridFilterType.LessThan:
                    ret = "DbFunctions.TruncateTime(" + this.Field + ") < @" + index;
                    break;
                case GridFilterType.GreaterThan:
                    ret = "DbFunctions.TruncateTime(" + this.Field + ") > @" + index;
                    break;
                case GridFilterType.LessThanOrEquals:
                    ret = "DbFunctions.TruncateTime(" + this.Field + ") <= @" + index;
                    break;
                case GridFilterType.GreaterThanOrEquals:
                    ret = "DbFunctions.TruncateTime(" + this.Field + ") >= @" + index;
                    break;
                default:
                    break;
            }
            return ret;
        }
        public string GetOperator(Type type, int index, string filterValue)
        {
            System.Reflection.PropertyInfo property = type.GetProperty(this.ColumnName);
            if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
            {
                return this.GetOperatorDate(index, filterValue);
            }
            else
            {
                return this.GetOperatorString(index, filterValue);
            }
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
                            ret = "Approbation.HasValue AND Approbation.Value == 1 AND !IsBoolApproved";
                            break;
                        case 2:
                            ret = "Approbation.HasValue AND Approbation.Value == 1 AND IsBoolApproved";
                            break;
                        case 3:
                            ret = "Approbation.HasValue==false OR Approbation.Value == 0";
                            break;
                        default:
                            break;
                    }
                    break;
                case "FiltreTested":
                    switch (value)
                    {
                        case 1:
                            ret = "Test.HasValue AND Test.Value == 1 AND !IsBoolTested";
                            break;
                        case 2:
                            ret = "Test.HasValue AND Test.Value == 1 AND IsBoolTested";
                            break;
                        case 3:
                            ret = "Test.HasValue==false OR Test.Value == 0";
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
        public string Field { get; set; }

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
        public override bool Equals(object obj)
        {
            return obj != null
                   && obj is ColumnFilter
                   && this.ColumnName == ((ColumnFilter)obj).ColumnName;
        }
        public override int GetHashCode()
        {
            return this.ColumnName.GetHashCode();
        }


    }

}
