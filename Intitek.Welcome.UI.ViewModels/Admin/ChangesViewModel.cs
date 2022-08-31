using Intitek.Welcome.UI.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.UI.ViewModels.Admin
{
    public class ChangesViewModel
    {
        public long ID { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "gridHdDateHeure")]
        public DateTime DateAction { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "action")]
        public string Action { get; set; }

        public int ID_IntitekUser { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "idObject")]
        public int ID_Object { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "description")]
        public string Description { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "document")]
        public string ObjectName { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "linkedObjects")]
        public string LinkedObjects { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "data")]
        public string ObjectCode { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "utilisateur")]
        public string Username { get; set; }
    }
}
