using Intitek.Welcome.Domain;

namespace Intitek.Welcome.Service.Back
{
    public class ExistDocumentVersionLangResponse
    {
        public bool Result { get; set; }
        public int IdLang { get; set; }
        public DocumentVersionLang VersionTrad { get; set; }
    }
}
