using System.Linq;

namespace Intitek.Welcome.Service.Back
{
    public interface IADService
    {
        GetAllADResponse GetAll();
        GetAllADResponse GetAll(GetAllADRequest allRequest);
        IQueryable<ADDTO> GetAllADAsQueryable(GetAllADRequest allRequest);
        GetADResponse Get(GetADRequest request);
        SaveADResponse Save(SaveADRequest request);
        DeleteADResponse Delete(DeleteADRequest request);
    }
}
