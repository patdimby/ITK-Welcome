using Intitek.Welcome.Domain;

namespace Intitek.Welcome.Service.Front
{
    public interface IUserService
    {
        GetIntitekUserByLoginResponse GetIntitekUserByLogin(string username);

        GetIntitekUserByEmailResponse GetIntitekUserByEmail(string email, bool isactive);

        IntitekUser GetById(int id);
    }
}
