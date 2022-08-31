using System;

namespace Intitek.Welcome.Service.Back
{
    public class UserLDAP
    {
        public string Login { get; set; }
        public string FirstName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Zone { get; set; }
        public string Country { get; set; }
        public string Entity { get; set; }
        public string Agency { get; set; }
        public string Type { get; set; }
        public bool IsBlackListed { get; set; }
        public string Department { get; set; }
        public string Division { get; set; }
        public DateTime EntryDate { get; set; }
        public LocationLDAP Location { get; set; }
    }
}
