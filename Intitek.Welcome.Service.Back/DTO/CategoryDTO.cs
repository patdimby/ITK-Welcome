using Intitek.Welcome.Domain;
using System;
using System.Collections.Generic;

namespace Intitek.Welcome.Service.Back
{
    public class CategoryDTO
    {
        public int ID { get; set; }
        public Nullable<int> OrdreCategory { get; set; }
        public bool IsDeleted { get; set; }
        public DocumentCategoryLang CategoryTrad { get; set; }
        public DocumentCategoryLang DefaultTrad { get; set; }
        public int NbDocuments { get; set; }
        public string Name {
            get {
                if (CategoryTrad != null)
                    return CategoryTrad.Name;
                return DefaultTrad.Name;
            }
        }
        public bool IsDefaultLangName
        {
            get
            {
               
                return CategoryTrad == null;
            }
        }
        public List<SubCategoryDTO> Subcategories { get; set; }
    }
}
