namespace Intitek.Welcome.Service.Front
{
    public class UserQcmReponseDTO
    {
       
        public int UserQcmId { get; set; }
        public int UserId { get; set; }
        public int ReponseId { get; set; }
        public string Reponse { get; set; }
        public int QuestionId { get; set; }
        public QuestionDTO Question { get; set; }
        public bool IsCorrect { get; set; }
        public int RightResponseCount { get; set; }
    }
}
