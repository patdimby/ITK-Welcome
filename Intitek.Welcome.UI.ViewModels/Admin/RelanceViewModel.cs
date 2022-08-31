using Intitek.Welcome.Domain;
using Intitek.Welcome.Service.Back;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.UI.ViewModels.Admin
{
    public class RelanceViewModel
    {
        public ProfilDTO Profile { get; set; }
        public UserViewModel User { get; set; }
        public List<MailTemplateDTO> ListTemplate { get; set; }
        public string EntityName { get; set; }
        public string AgencyName { get; set; }
    }
}
