using System;
using System.Collections.Generic;

namespace Intitek.Welcome.Service.Back
{
    public class GetUserRequest
    {
        public int Id { get; set; }
        public int IdLang { get; set; }
        public int IdDefaultLang { get; set; }
        public string EntityName { get; set; }
        public string AgencyName { get; set; }
        public int Activity { get; set; }
        public int Actif { get; set; }
        public bool Refresh { get; set; }
        public GridMvcRequest Request { get; set; }
        public List<DocCheckState> DocsAffected { get; set; }
        public DateTime? ExitDate { get; set; }
        public DateTime? EntryDate { get; set; }
    }
}
