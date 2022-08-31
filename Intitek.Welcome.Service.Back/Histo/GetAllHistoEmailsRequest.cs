using System;

namespace Intitek.Welcome.Service.Back
{
    public class GetAllHistoEmailsRequest
    {
        public GridMvcRequest GridRequest { get; set; }
        public DateTime? LimitDate { get; set; }
    }
}
