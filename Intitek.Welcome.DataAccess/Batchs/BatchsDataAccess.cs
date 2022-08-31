using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.UnitOfWork;
using Intitek.Welcome.Repository;

namespace Intitek.Welcome.DataAccess
{
    public class BatchsDataAccess : Repository<Batchs, int>, IBatchsDataAccess
    {
        public BatchsDataAccess(IUnitOfWork uow) : base(uow)
        {

        }
    }
}
