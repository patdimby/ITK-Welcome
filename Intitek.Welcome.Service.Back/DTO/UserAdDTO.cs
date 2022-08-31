using System;

namespace Intitek.Welcome.Service.Back
{
    public class UserAdDTO
    {
        public string FirstName { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public Nullable<int> Status { get; set; }
        public string EntityName { get; set; }
        public string AgencyName { get; set; }
        public string Country { get; set; }
        public string Type { get; set; }
        public string Email { get; set; }
        public int ID_AD { get; set; }
        public bool IsBlackListed { get; set; }
        public string Departement { get; set; }
        public string Division { get; set; }
        public string Pays { get; set; }
        public string Plaque { get; set; }
        public DateTime EntryDate { get; set; }
        public DateTime? ExitDate { get; set; }
    }
}
