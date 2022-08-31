using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.UnitOfWork;
using Intitek.Welcome.Repository;

namespace Intitek.Welcome.DataAccess
{
    public class HistoUserQcmDocVersionDataAccess : Repository<HistoUserQcmDocVersion, long>, IHistoUserQcmDocVersionDataAccess
    {
        public HistoUserQcmDocVersionDataAccess(IUnitOfWork uow) : base(uow)
        {

        }
    }
}
