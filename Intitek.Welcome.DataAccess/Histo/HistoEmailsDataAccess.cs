using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.UnitOfWork;
using Intitek.Welcome.Repository;

namespace Intitek.Welcome.DataAccess
{
    public class HistoEmailsDataAccess : Repository<HistoEmails, int>, IHistoEmailsDataAccess
    {
        public HistoEmailsDataAccess(IUnitOfWork uow) : base(uow)
        {

        }
    }
}
