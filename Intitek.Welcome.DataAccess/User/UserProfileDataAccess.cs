using Intitek.Welcome.Domain;
using Intitek.Welcome.Repository;
using Intitek.Welcome.Infrastructure.UnitOfWork;

namespace Intitek.Welcome.DataAccess
{
    public class UserProfileDataAccess : Repository<ProfileUser, int>, IUserProfileDataAccess
    {
        public UserProfileDataAccess(IUnitOfWork uow) : base(uow)
        {

        }
    }
}
