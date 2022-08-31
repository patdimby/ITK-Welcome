using System;

namespace Intitek.Welcome.Service.Back
{
    public class HistoBatchsDTO
    {
        public DateTime DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public string BatchProgName { get; set; }
        public string Description { get; set; }
        public int? ReturnCode { get; set; }
        public string Message { get; set; }
    }
}
