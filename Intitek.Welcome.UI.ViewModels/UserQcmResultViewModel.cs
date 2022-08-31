using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intitek.Welcome.Service.Front;

namespace Intitek.Welcome.UI.ViewModels
{
    public class UserQcmResultViewModel
    {
        public int QcmId { get; set; }       
        public string QcmName { get; set; }
        public int QcmUserScore { get; set; }
        public decimal QcmRightPercent { get; set; }
        public decimal QcmWrongPercent { get; set; }
        public int QcmMinScore { get; set; }
        public int QcmNombreQuestions { get; set; }

        public bool IsQcmPassed { get; set; }

        public List<UserReponseQcmViewModel> UserReponses { get; set; }
    }
}
