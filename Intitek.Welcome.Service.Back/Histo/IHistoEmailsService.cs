namespace Intitek.Welcome.Service.Back
{
    public interface IHistoEmailsService
    {
        int GetAllCount(GetAllHistoEmailsRequest request);
        GetAllHistoEmailsResponse GetAll(GetAllHistoEmailsRequest request);
        DeleteHistoEmailsResponse Delete(DeleteHistoEmailsRequest request);
    }
}
