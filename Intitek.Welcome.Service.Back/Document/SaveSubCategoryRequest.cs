using Intitek.Welcome.Domain;

namespace Intitek.Welcome.Service.Back
{
    public class SaveSubCategoryRequest
    {
        public int IdLang { get; set; }
        public bool IsCategoryChanged { get; set; }
        public SubCategory SubCategory { get; set; }
        public SubCategoryLang SubCategoryTrad { get; set; }
    }
}
