using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.UnitOfWork;
using Intitek.Welcome.Repository;

namespace Intitek.Welcome.DataAccess
{
    public class HistoActionsDataAccess : Repository<HistoActions, long>, IHistoActionsDataAccess
    {
        public HistoActionsDataAccess (IUnitOfWork uow) : base(uow)
        {

        }
    }
}
