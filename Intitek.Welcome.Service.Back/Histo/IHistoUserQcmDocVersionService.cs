namespace Intitek.Welcome.Service.Back
{
    public interface IHistoUserQcmDocVersionService
    {
        int GetAllHistoUserQcmDocVersionCount(GetAllHistoUserQcmDocVersionRequest request);
        GetAllHistoUserQcmDocVersionResponse GetAllHistoUserQcmDocVersion(GetAllHistoUserQcmDocVersionRequest allRequest);
        DeleteHistoUserQcmDocVersionResponse Delete(DeleteHistoUserQcmDocVersionRequest request);
    }
}
