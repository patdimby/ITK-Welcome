using System.Collections.Generic;

namespace Intitek.Welcome.Service.Back
{
    public class GetEntityRequest
    {
        public int IdLang { get; set; }
        public int IdDefaultLang { get; set; }
        public int ID_Document { get; set; }
        public string EntityName { get; set; }
        public string AgencyName { get; set; }
        public GridMvcRequest Request { get; set; }
        public List<DocCheckState> DocsAffected { get; set; }
    }
}
