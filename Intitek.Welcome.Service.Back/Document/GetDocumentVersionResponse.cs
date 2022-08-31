using Intitek.Welcome.Domain;

namespace Intitek.Welcome.Service.Back
{
    public class GetDocumentVersionResponse
    {
        public string NewVersion { get; set; }

        public bool IsLangVersionned { get; set; }
        public DocumentVersionLang VersionTrad { get; set; }
    }
}
