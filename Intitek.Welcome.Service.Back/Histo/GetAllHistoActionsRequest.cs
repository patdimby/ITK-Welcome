using System;

namespace Intitek.Welcome.Service.Back
{
    public class GetAllHistoActionsRequest
    {
        public GridMvcRequest GridRequest { get; set; }
        public DateTime? LimitDate { get; set; }
        public int IdLang { get; set; }
    }
}
