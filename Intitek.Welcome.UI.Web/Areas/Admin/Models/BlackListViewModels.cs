using Intitek.Welcome.UI.ViewModels.Admin;
using Intitek.Welcome.UI.Web.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Intitek.Welcome.UI.Web.Areas.Admin.Models
{
    public class BlackListViewModels
    {
        public GridBO<CityEntityBlacklistViewModel> GridCityEntityBlacklist { get; set; }
        public GridBO<BlackListViewModel> GridBlackList { get; set; }
    }
}