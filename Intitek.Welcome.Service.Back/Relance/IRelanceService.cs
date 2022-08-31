namespace Intitek.Welcome.Service.Back
{
    public interface IRelanceService
    {
        bool Execute(GetRelanceRequest request, out string Message);
    }
}
