using Intitek.Welcome.Service.Back;
using Intitek.Welcome.UI.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.UI.ViewModels.Admin
{
    public class ProfilViewModel
    {
        public int ID { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "prf_Name")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "assigned")]
        public bool Affecte { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "prf_Reading")]
        public int DocumentRead { get; set; }
        [Display(ResourceType = typeof(Resource), Name = "prf_Approbation")]
        public int DocumentApproved { get; set; }
        [Display(ResourceType = typeof(Resource), Name = "prf_Test")]
        public int DocumentTested { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "assigned")]
        public bool IsSessionChecked { get; set; }
        public Statistiques Statistiques { get; set; }
    }
}
