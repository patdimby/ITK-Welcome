namespace Intitek.Welcome.Service.Back
{
    public interface IHistoBatchsService
    {
        int GetAllCount(GetAllHistoBatchsRequest request);
        GetAllHistoBatchsResponse GetAll(GetAllHistoBatchsRequest request);
        DeleteHistoBatchsResponse Delete(DeleteHistoBatchsRequest request);
    }
}
