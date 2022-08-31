using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.UI.ViewModels.Admin
{
    public class ConfirmDeleteViewModel
    {
        public object ID { get; set; }
        public bool CanDelete { get; set; }
        public bool IsDeleted { get; set; }
        public string EntityName { get; set; }
        public string Name { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }

    }
}
