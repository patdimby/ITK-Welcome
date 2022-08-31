namespace Intitek.Welcome.Service.Back
{
    public class GetAllDocumentNameRequest
    {
        public string Search { get; set; }
        public bool ExactMatch { get; set; }
        public int IdLang { get; set; }
    }
}
