using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.UI.ViewModels.Admin
{
    public class WelcomeMessageViewModel
    {
        public int ID_Lang { get; set; }

        //Message de bienvenue
        public string WelcomeMessage { get; set; }

        [AllowHtml]
        public string WelcomeMessageHtml { get; set; }
    }
}
