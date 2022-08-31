using Intitek.Welcome.Domain;

namespace Intitek.Welcome.Service.Back
{
    public class GetCategoryResponse
    {
        public DocumentCategory Category { get; set; }
        public DocumentCategoryLang CategoryTrad { get; set; }
        public DocumentCategoryLang DefaultTrad { get; set; }
        public bool IsDeleted { get; set; }
    }
}
