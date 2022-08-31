using Intitek.Welcome.Domain;
using System;

namespace Intitek.Welcome.Service.Back
{
    public class SubCategoryDTO
    {
        public int ID { get; set; }
        public int ID_Category { get; set; }
        public string CategoryName { get; set; }
        public int OrdreCategory { get; set; }
        public Nullable<int> Ordre { get; set; }
        public int NbDocuments { get; set; }
        public bool IsDeleted { get; set; }
        public SubCategoryLang SubCategoryTrad { get; set; }
        public SubCategoryLang DefaultTrad { get; set; }
        public string Name
        {
            get
            {
                if (SubCategoryTrad != null)
                    return SubCategoryTrad.Name;
                return DefaultTrad == null ? string.Empty : DefaultTrad.Name;
            }
        }
        public bool IsDefaultLangName
        {
            get
            {
                return SubCategoryTrad == null;
            }
        }
    }
}
