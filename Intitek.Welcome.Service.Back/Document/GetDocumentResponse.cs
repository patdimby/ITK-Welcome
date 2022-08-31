using System.Collections.Generic;
using Intitek.Welcome.Domain;

namespace Intitek.Welcome.Service.Back
{
    public class GetDocumentResponse
    {
        public string DocumentTitle { get; set; }
        public Document Document { get; set; }
        public DocumentLang DocumentTrad { get; set; }
        public DocumentLang DefaultDocumentTrad { get; set; }
        public List<DocumentCategoryDTO> Categories { get; set; }
        public List<SubCategoryDTO> SubCategories { get; set; }
        public List<DocumentVersionDTO> Versions { get; set; }
    }
}
