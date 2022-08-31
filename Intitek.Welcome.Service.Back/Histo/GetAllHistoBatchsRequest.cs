using System;

namespace Intitek.Welcome.Service.Back
{
    public class GetAllHistoBatchsRequest
    {
        public GridMvcRequest GridRequest { get; set; }
        public DateTime? LimitDate { get; set; }
    }
}
