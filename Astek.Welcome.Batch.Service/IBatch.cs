using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astek.Welcome.Batch.Service
{
    public interface IBatch
    {
        BatchResponse Execute(BatchRequest request, Synthese synthese);
    }
}
