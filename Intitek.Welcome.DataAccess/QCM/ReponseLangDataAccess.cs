using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.UnitOfWork;
using Intitek.Welcome.Repository;

namespace Intitek.Welcome.DataAccess
{
    public class ReponseLangDataAccess : Repository<ReponseLang, int>, IReponseLangDataAccess
    {
        public ReponseLangDataAccess(IUnitOfWork uow) : base(uow)
        {

        }
    }
}
