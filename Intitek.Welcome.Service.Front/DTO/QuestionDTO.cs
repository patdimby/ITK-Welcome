using System.Collections.Generic;
using Intitek.Welcome.Domain;

namespace Intitek.Welcome.Service.Front
{
    public class QuestionDTO
    {
        public Question Question { get; set; }
        public QuestionLang QuestionTrad { get; set; }
        public QuestionLang DefaultTrad { get; set; }
        public bool IsMultipleReponse {get; set; }
        public List<ReponseDTO> Reponses { get; set; }
    }
}
