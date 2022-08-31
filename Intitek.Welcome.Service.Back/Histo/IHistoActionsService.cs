using System.Collections.Generic;
using System.Resources;

namespace Intitek.Welcome.Service.Back
{
    public interface IHistoActionsService
    {
        int GetAllCount(GetAllHistoActionsRequest request);
        GetAllHistoActionsResponse GetAll();
        GetAllHistoActionsResponse GetAll(GetAllHistoActionsRequest request);
        List<string> GetAllObjectCodes();
        List<string> GetAllActions();
        List<string> GetAllActionsFullNames(ResourceManager rm);
        DeleteHistoActionsResponse Delete(DeleteHistoActionsRequest request);
    }
}
