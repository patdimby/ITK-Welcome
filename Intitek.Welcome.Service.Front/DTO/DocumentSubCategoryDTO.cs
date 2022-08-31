using System;

namespace Intitek.Welcome.Service.Front
{
    public class DocumentSubCategoryDTO
    {
        public int ID { get; set; }
        public int ID_DocumentCategory { get; set; }
        public string NameCategory { get; set; }
        public string Name { get; set; }
        public Nullable<int> OrdreSubCategory { get; set; }
        public Nullable<int> OrdreCategory { get; set; }
    }
}
