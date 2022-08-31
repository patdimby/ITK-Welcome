using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.Infrastructure.Histo
{
    public class BatchBO
    {
        public long ID { get; set; }
        public int ID_Batch { get; set; }
        public System.DateTime Start { get; set; }
        public Nullable<System.DateTime> Finish { get; set; }
        public Nullable<int> ReturnCode { get; set; }
        public string Message { get; set; }
    }
}
