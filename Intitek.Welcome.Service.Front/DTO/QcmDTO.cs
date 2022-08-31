using Intitek.Welcome.Domain;
using System.Collections.Generic;

namespace Intitek.Welcome.Service.Front
{
    public class QcmDTO
    {

        public Qcm Qcm { get; set; }
        public QcmLang QcmTrad { get; set; }
        public QcmLang DefaultTrad {get; set;}
        public List<QuestionDTO> Questions { get; set; }
      
    }
}
