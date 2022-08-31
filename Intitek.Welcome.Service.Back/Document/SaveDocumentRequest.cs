using Intitek.Welcome.Domain;

namespace Intitek.Welcome.Service.Back
{
    public class SaveDocumentRequest
    {
        public Document Document { get; set; }
        public DocumentLang DocumentTrad { get; set; }
        public string Affectation { get; set; }
    }
}
