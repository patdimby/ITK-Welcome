using Intitek.Welcome.Domain;
using Intitek.Welcome.Repository;
using Intitek.Welcome.Infrastructure.UnitOfWork;

namespace Intitek.Welcome.DataAccess
{
    public class UserQcmDataAccess : Repository<UserQcm, int>, IUserQcmDataAccess
    {
        public UserQcmDataAccess(IUnitOfWork uow) : base(uow)
        {

        }
    }
}
