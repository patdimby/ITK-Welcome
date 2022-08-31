using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web;

namespace GridMvc.Filtering
{
    /// <summary>
    ///     Structure that specifies filter settings for each column
    /// </summary>
    [DataContract]
    public struct ColumnFilterValue
    {
        //[DataMember(Name = "columnName")]
        public string ColumnName { get; internal set; }

        [DataMember(Name = "filterType")] 
        public GridFilterType FilterType { get; internal set; }

        public string FilterValue { get; internal set; }

        [DataMember(Name = "filterValue")]
        internal string FilterValueEncoded
        {
            get { return HttpUtility.UrlEncode(FilterValue); }
            set { FilterValue = value; }
        }

        public static ColumnFilterValue Null
        {
            get { return default(ColumnFilterValue); }
        }

       

        public static bool operator ==(ColumnFilterValue a, ColumnFilterValue b)
        {
            return a.ColumnName == b.ColumnName && a.FilterType == b.FilterType && a.FilterValue == b.FilterValue;
        }

        public static bool operator !=(ColumnFilterValue a, ColumnFilterValue b)
        {
            return a.ColumnName != b.ColumnName || a.FilterType != b.FilterType || a.FilterValue != b.FilterValue;
        }

        public override bool Equals(object obj)
        {
            return obj != null
                   && obj is ColumnFilterValue
                   && this == ((ColumnFilterValue)obj);
        }

        public override int GetHashCode()
        {
            var hashCode = 80300311;
            hashCode = hashCode * -1521134295 + this.ColumnName.GetHashCode();
            hashCode = hashCode * -1521134295 + FilterType.GetHashCode() ;
            hashCode = hashCode * -1521134295 + (this.FilterValue!=null ? this.FilterValue.GetHashCode() :0 );
            return hashCode;
        }
    }
}