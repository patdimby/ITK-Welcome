using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.UnitOfWork;
using Intitek.Welcome.Repository;

namespace Intitek.Welcome.DataAccess.QCM
{
    public class UserQcmDataAccess : Repository<UserQcm, int>, IUserQcmDataAccess
    {
        public UserQcmDataAccess(IUnitOfWork uow) : base(uow)
        {

        }
    }
}
