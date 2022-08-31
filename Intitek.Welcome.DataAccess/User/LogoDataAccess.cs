using Intitek.Welcome.Domain;
using Intitek.Welcome.Repository;
using Intitek.Welcome.Infrastructure.UnitOfWork;

namespace Intitek.Welcome.DataAccess
{
    public class LogoDataAccess : Repository<Logos, int>, ILogoDataAccess
    {
        public LogoDataAccess(IUnitOfWork uow) : base(uow)
        {

        }
    }
}
