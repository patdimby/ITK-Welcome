namespace Intitek.Welcome.Service.Back
{
    public class UserDocumentDTO
    {
        public string Source { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Entity { get; set; }
        public string Agency { get; set; }
        public string Email { get; set; }
        public string UserType { get; set; }
        public int DocumentID { get; set; }
        public string Name { get; set; }
        public string DocType { get; set; }
        public bool Inactif { get; set; }
        public int? IsActionRequired { get; set; }
        public int? Approbation { get; set; }
        public int? Test { get; set; }
        public int? IsRead { get; set; }
        public int? IsApproved { get; set; }
        public int? IsTested { get; set; }
    }
}
