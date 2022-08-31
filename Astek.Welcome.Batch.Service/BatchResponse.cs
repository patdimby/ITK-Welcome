using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astek.Welcome.Batch.Service
{
    public class BatchResponse
    {
        public int Result { get; set; }
        public List<string> Errors { get; set; }
        public BatchResponse()
        {
            this.Errors = new List<string>();
        }
    }
}
