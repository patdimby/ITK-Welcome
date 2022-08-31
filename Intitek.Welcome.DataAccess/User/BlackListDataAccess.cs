using Intitek.Welcome.Domain;
using Intitek.Welcome.Repository;
using Intitek.Welcome.Infrastructure.UnitOfWork;
namespace Intitek.Welcome.DataAccess
{
    public class BlackListDataAccess : Repository<BlackList, int>, IBlackListDataAccess
    {
        public BlackListDataAccess(IUnitOfWork uow) : base(uow)
        {

        }
    }
}
