using System;

namespace Intitek.Welcome.Service.Back
{
    public class EntityDocumentDTO
    {
        public int ID { get; set; }
        public string EntityName { get; set; }
        public string AgencyName { get; set; }
        public Nullable<System.DateTime> EntityDocDate { get; set; }
        public Nullable<int> ID_Document { get; set; }

    }
}
