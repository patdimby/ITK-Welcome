using Intitek.Welcome.DataAccess.Welcome;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.UnitOfWork;
using Intitek.Welcome.Repository;

namespace Intitek.Welcome.DataAccess
{
    public class WelcomeDataAccess : Repository<WelcomeMessage, int>, IWelcomeDataAccess
    {
        public WelcomeDataAccess(IUnitOfWork uow) : base(uow)
        {

        }
    }
}
