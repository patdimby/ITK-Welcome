using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.UI.ViewModels
{
    public class UserReponseQcmViewModel
    {
        public int QcmId { get; set; }
        public int QuestionId { get; set; }
        public int QuestionOrder { get; set; }
        public string TexteQuestion { get; set; }
        public string TexteJustification { get; set; }
        public string UserReponse { get; set; }
        public bool IsCorrect { get; set; }
    }
}
