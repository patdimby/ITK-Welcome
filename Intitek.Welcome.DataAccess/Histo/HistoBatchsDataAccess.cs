using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.UnitOfWork;
using Intitek.Welcome.Repository;

namespace Intitek.Welcome.DataAccess
{
    public class HistoBatchsDataAccess : Repository<HistoBatchs, long>, IHistoBatchsDataAccess
    {
        public HistoBatchsDataAccess(IUnitOfWork uow) : base(uow)
        {

        }
    }
}
