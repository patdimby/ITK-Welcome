using System.Collections.Generic;

namespace Intitek.Welcome.Service.Back
{
    public class GetProfilRequest
    {
        //Id Profile
        public int? Id { get; set; }
        public int IdLang { get; set; }
        public int IdDefaultLang { get; set; }
        public GridMvcRequest GridRequest { get; set; }
        public List<DocCheckState> DocsAffected { get; set; }
    }
}
