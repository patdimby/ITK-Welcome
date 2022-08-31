using System;

namespace Intitek.Welcome.Service.Back
{
    public class DocumentCategoryDTO
    {
        public int ID { get; set; }
        public int ID_Lang { get; set; }
        public string Name { get; set; }
        public bool IsDefaultLangName { get; set; }
        public Nullable<int> OrdreCategory { get; set; }
    }
}
