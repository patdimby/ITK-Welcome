using Intitek.Welcome.Domain;
using System.Collections.Generic;

namespace Intitek.Welcome.Service.Back
{
    public class QuestionDTO
    {
        public Question Question { get; set; }
        public QuestionLang QuestionTrad { get; set; }
        public QuestionLang DefaultTrad { get; set; }
        public int IdLang { get; set; }
        public bool IsTrad { get; set; }
        public bool IsMultipleReponse { get; set; }
        public List<ReponseDTO> Reponses { get; set; }
    }
}
