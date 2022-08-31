using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.Service.Back
{
    public class GetAwarenessApprovalsRequest
    {
        public string SubmitButton { get; set; }
        public DateTime? MonthToGenerate { get; set; }
        public string Month { get; set; }
    }
}
