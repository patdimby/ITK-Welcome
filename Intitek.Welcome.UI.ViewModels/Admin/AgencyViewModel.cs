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
    public class AgencyViewModel
    {
        public int ID { get; set; }
        [Display(ResourceType = typeof(Resource), Name = "agency_Name")]
        public string Name { get; set; }
        public int ToTested { get; set; }
        public int DocumentTested { get; set; }
        public int ToRead { get; set; }
        public int DocumentRead { get; set; }
        public int ToApproved { get; set; }
        public int DocumentApproved { get; set; }
        [IgnoreDataMember]
        public int Total {
            get {
                return this.ToRead + this.ToTested + this.ToApproved;
            }
        }
    }
}
