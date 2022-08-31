using Intitek.Welcome.Domain;
using Intitek.Welcome.Repository;
using Intitek.Welcome.Infrastructure.UnitOfWork;
namespace Intitek.Welcome.DataAccess
{
    public class CityEntityBlackListedDataAccess : Repository<CityEntityBlackListed, int>, ICityEntityBlackListedDataAccess
    {
        public CityEntityBlackListedDataAccess(IUnitOfWork uow) : base(uow)
        {

        }
    }
}
