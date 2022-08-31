using Intitek.Welcome.UI.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.UI.ViewModels.Admin
{
    public class HistoUserQcmViewModel
    {
        public long ID { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "utilisateur")]
        public string Username { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "document")]
        public string DocName { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "mcq")]
        public string QcmName { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "version")]
        public string Version { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "score")]
        public int Score { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "minimalScore")]
        public int ScoreMinimal { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "gridHdDateHeure")]
        public DateTime DateAction { get; set; }
    }
}
