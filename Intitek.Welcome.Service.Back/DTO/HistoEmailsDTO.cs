using System;

namespace Intitek.Welcome.Service.Back
{
    public class HistoEmailsDTO
    {
        public DateTime? DateAction { get; set; }
        public string TemplateName { get; set; }
        public string BatchProgName { get; set; }
        public string Username { get; set; }
        public string Message { get; set; }
    }
}
