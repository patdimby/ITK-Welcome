using Intitek.Welcome.Domain;
using Intitek.Welcome.Service.Front;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.UI.ViewModels
{
    public class QcmViewModel
    {
        public int DocumentID { get; set; }
        public string DocumentVersion { get; set; }
        public DateTime DateCre { get; set; }
        public QcmDTO Qcm { get; set; }
        public List<QuestionViewModel> Questions { get; set; }
    }
}
