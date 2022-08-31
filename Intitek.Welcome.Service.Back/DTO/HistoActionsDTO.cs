using Intitek.Welcome.Domain;
using System;
using System.Collections.Generic;

namespace Intitek.Welcome.Service.Back
{
    public class HistoActionsDTO
    {
        public long ID { get; set; }
        public int ID_Object { get; set; }
        public int ID_IntitekUser { get; set; }
        public string Username { get; set; }
        public string ObjectCode { get; set; }
        public string Action { get; set; }
        public DateTime DateAction { get; set; }
        public string Description { get; set; }
        public string LinkedObjects { get; set; }
        public string ObjectName { get; set; }
        public IEnumerable<Document> QcmDocs { get; set; }
    }
}
