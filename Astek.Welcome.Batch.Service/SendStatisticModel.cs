using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astek.Welcome.Batch.Service
{
    public class SendStatisticModel
    {
        public string EntityName { get; set; }
        public string AgencyName { get; set; }
        public string PercentItemsRead { get; set; }
        public string ReadItemsCount { get; set; }
        public string ItemsCount { get; set; }
        public string PercentItemsApproved { get; set; }
        public string ApprovedItemsCount { get; set; }
        public string ItemsToApproveCount { get; set; }
        public string PercentItemsTested { get; set; }
        public string TestedPassedItemsCount { get; set; }
        public string ItemsToTestCount { get; set; }
    }
}
