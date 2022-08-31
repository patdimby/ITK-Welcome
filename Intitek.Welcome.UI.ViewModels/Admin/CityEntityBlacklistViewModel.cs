using Intitek.Welcome.UI.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.UI.ViewModels.Admin
{
    public class CityEntityBlacklistViewModel
    {
        [Display(ResourceType = typeof(Resource), Name = "City")]
        public string City { get; set; }
        [Display(ResourceType = typeof(Resource), Name = "Entity")]
        public string Entity { get; set; }
        [Display(ResourceType = typeof(Resource), Name = "Creation")]
        public System.DateTime DateCre { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "BlackList_NbCollabInactif")]
        public int NbCollabInactif { get; set; }
        [Display(ResourceType = typeof(Resource), Name = "BlackList_NbCollabActif")]
        public int NbCollabActif { get; set; }
    }
}
