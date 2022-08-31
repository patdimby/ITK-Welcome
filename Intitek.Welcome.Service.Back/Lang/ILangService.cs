namespace Intitek.Welcome.Service.Back
{
    public interface ILangService
    {
        GetLangResponse Get(GetLangRequest request);
        GetAllLangResponse GetAll(GetAllLangRequest request);
    }
}
