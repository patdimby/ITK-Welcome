using Intitek.Welcome.Domain;
using Intitek.Welcome.Repository;
using Intitek.Welcome.Infrastructure.UnitOfWork;

namespace Intitek.Welcome.DataAccess
{
    public class UserQcmReponseDataAccess : Repository<UserQcmReponse, long>, IUserQcmReponseDataAccess
    {
        public UserQcmReponseDataAccess(IUnitOfWork uow) : base(uow)
        {

        }
    }
}
