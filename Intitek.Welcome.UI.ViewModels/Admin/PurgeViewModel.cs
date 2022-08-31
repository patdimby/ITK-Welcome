using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.UI.ViewModels.Admin
{
    public class PurgeViewModel
    {
        public string LimitDateString { get; set; }

        public int HistoActionsCount { get; set; }
        public int HistoEmailsCount { get; set; }
        public int HistoBatchsCount { get; set; }
        public int HistoUserQcmCount { get; set; }

        public int Total
        {
            get
            {
                return HistoActionsCount + HistoEmailsCount + HistoBatchsCount + HistoUserQcmCount;
            }
        }
    }
}
