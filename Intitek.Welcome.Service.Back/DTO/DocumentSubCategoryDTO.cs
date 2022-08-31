using System;

namespace Intitek.Welcome.Service.Back
{
    public class DocumentSubCategoryDTO
    {
        public int ID { get; set; }
        public int ID_DocumentCategory { get; set; }
        public string NameCategory { get; set; }
        public string Name { get; set; }
        public bool IsDefaultLangName { get; set; }
        public bool IsDefaultLangNameCategory { get; set; }
        public Nullable<int> OrdreSubCategory { get; set; }
        public Nullable<int> OrdreCategory { get; set; }
    }
}
