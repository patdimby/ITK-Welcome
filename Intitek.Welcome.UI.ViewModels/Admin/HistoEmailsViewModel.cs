using Intitek.Welcome.UI.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.UI.ViewModels.Admin
{
    public class HistoEmailsViewModel
    {
        [Display(ResourceType = typeof(Resource), Name = "gridHdDateHeure")]
        public DateTime? DateAction { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "model")]
        public string TemplateName { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "batch")]
        public string BatchProgName { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "utilisateur")]
        public string Username { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "message")]
        public string Message { get; set; }
    }
}
