using Intitek.Welcome.Domain;

namespace Intitek.Welcome.Service.Front
{
    public class  GetDocumentResponse
    {
        public Document Document { get; set; }
        public DocumentLang DocumentTrad { get; set; }
        public DocumentLang DefaultDocumentTrad { get; set; }
    }
}
