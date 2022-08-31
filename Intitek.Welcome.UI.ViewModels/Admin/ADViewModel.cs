using Intitek.Welcome.UI.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.UI.ViewModels.Admin
{
    public class ADViewModel
    {
        public int ID { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "name")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "address")]
        public string Address { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "domain")]
        public string Domain { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "user_login")]
        public string Username { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "login_Password")]
        public string Password { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "confirmation")]
        public string ConfirmPassword { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "toBeSynchronized")]
        public bool ToBeSynchronized { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "lastSynchronization")]
        public DateTime? LastSynchronized { get; set; }
    }
}
