using Intitek.Welcome.UI.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.UI.ViewModels.Admin
{
    public class HistoBatchsViewModel
    {
        [Display(ResourceType = typeof(Resource), Name = "gridHdDateHeure")]
        public DateTime DateStart { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "end")]
        public DateTime? DateEnd { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "programName")]
        public string BatchProgName { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "description")]
        public string Description { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "returnCode")]
        public int? ReturnCode { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "information")]
        public string Message { get; set; }
    }
}
