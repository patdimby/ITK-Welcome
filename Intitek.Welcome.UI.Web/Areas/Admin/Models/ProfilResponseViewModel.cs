using Intitek.Welcome.UI.Web.Admin.Models;

namespace Intitek.Welcome.UI.ViewModels.Admin
{
    public class ProfilResponseViewModel
    {
        public ProfilViewModel Profile { get; set; }
        public GridBO<DocumentViewModel> ListDocument { get; set; }
        public GridBO<UserViewModel> ListUser { get; set; }
    }
}
