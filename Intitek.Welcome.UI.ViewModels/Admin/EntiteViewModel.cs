using Intitek.Welcome.UI.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.UI.ViewModels.Admin
{
    public class EntiteViewModel
    {
        public int ID { get; set; }
        [Display(ResourceType = typeof(Resource), Name = "entity_Name")]
        public string Name { get; set; }
    }
}
