namespace Intitek.Welcome.Service.Front
{
    public interface IWelcomeService
    {
        string GetWelcomeMessageByLang(int idLang);

        string GetWelcomeMessageByLang(string culture);
    }
}
