using Intitek.Welcome.Domain;
using Intitek.Welcome.Repository;
using Intitek.Welcome.Infrastructure.UnitOfWork;

namespace Intitek.Welcome.DataAccess.User
{
    public class ADDataAccess : Repository<AD, int>, IADDataAccess
    {
        public ADDataAccess(IUnitOfWork uow) : base(uow)
        {

        }
    }
}
