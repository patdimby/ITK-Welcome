using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intitek.Welcome.Service.Front;
using Intitek.Welcome.Domain;

namespace Intitek.Welcome.UI.ViewModels
{
    public class UserQcmViewModel
    {
        public int DocumentID { get; set; }
        public string DocumentVersion { get; set; }
        public int QcmID { get; set; }
        public int QcmScoreMinimal { get; set; }
        public int NbQuestions { get; set; }
        public int UserID { get; set; }
        public DateTime DateCre { get; set; }
        public List<int> Reponses { get; set; }


    }
}
