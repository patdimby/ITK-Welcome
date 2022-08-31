namespace Intitek.Welcome.Service.Back
{
    public class  GetAllDocumentRequest
    {
        public GridMvcRequest GridRequest { get; set; }
        public int IdLang { get; set; }
        public int IdDefaultLang { get; set; }
    }
}
