using System.Collections.Generic;

namespace Intitek.Welcome.Service.Back
{
    public interface IBatchService
    {
        GetBatchResponse Get(GetBatchRequest request);
        void Historize();
        List<string> GetAllProgNames();
    }
}
