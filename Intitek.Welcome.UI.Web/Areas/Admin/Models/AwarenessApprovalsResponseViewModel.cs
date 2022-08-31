using Intitek.Welcome.Service.Back;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Intitek.Welcome.UI.Web.Admin.Models
{
    public class AwarenessApprovalsResponseViewModel
    {
        public DateTime CurrentDate { get; set; }
        public List<HistoADDTO> Months { get; set; }
        public AwarenessApprovalsResponseViewModel()
        {
            CurrentDate = DateTime.Now;
        }
    }
}