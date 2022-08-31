using Intitek.Welcome.UI.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.UI.ViewModels.Admin
{
    public class BlackListViewModel
    {
        public string ID { get; set; }
        [Display(ResourceType = typeof(Resource), Name = "Path")]
        public string Path { get; set; }
        [Display(ResourceType = typeof(Resource), Name = "Creation")]
        public System.DateTime DateCre { get; set; }
    }
}
