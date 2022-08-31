using Intitek.Welcome.Domain;

namespace Intitek.Welcome.Service.Back
{
    public class SaveCategoryRequest
    {
        public DocumentCategory Category { get; set; }
        public DocumentCategoryLang CategoryTrad { get; set; }
    }
}
