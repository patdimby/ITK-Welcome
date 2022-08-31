namespace Intitek.Welcome.Service.Front
{
    public class UserDocumentDTO
    {
        public int DocumentId { get; set; }
        public int UserId { get; set; }
        public bool IsRead { get; set; }
        public bool IsApproved { get; set; }
        public bool IsTested { get; set; }
    }
}
