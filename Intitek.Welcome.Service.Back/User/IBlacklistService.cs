namespace Intitek.Welcome.Service.Back
{
    public interface IBlacklistService
    {
        GetAllBlackListResponse GetAll();
        GetBlackListResponse GetBlackList(GetBlackListRequest request);
        DeleteBlackListResponse DeleteBlackList(DeleteBlackListRequest request);
        bool IsBlackListExist(SaveBlackListRequest request);
        SaveBlackListResponse SaveBlackList(SaveBlackListRequest request);
    }
}
