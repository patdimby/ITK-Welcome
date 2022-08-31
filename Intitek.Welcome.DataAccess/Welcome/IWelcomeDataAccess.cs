using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.Domain;

namespace Intitek.Welcome.DataAccess.Welcome
{
    public interface IWelcomeDataAccess : IRepository<WelcomeMessage, int>
    {
    }
}
