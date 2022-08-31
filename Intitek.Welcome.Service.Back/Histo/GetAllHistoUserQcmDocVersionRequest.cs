using System;

namespace Intitek.Welcome.Service.Back
{
    public class GetAllHistoUserQcmDocVersionRequest
    {
        public GridMvcRequest GridRequest { get; set; }
        public DateTime? LimitDate { get; set; }
        public int IdLang { get; set; }
        public int IdDefaultLang { get; set; }
    }
}
