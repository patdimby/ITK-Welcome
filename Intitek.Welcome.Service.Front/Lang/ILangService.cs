namespace Intitek.Welcome.Service.Front
{
    public interface ILangService
    {
        GetLangResponse Get(GetLangRequest request);
        GetAllLangResponse GetAll(GetAllLangRequest request);
    }
}
