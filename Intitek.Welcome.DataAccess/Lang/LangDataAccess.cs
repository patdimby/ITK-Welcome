using Intitek.Welcome.Domain;
using Intitek.Welcome.Repository;
using Intitek.Welcome.Infrastructure.UnitOfWork;

namespace Intitek.Welcome.DataAccess
{
    public class LangDataAccess : Repository<Lang, int>, ILangDataAccess
    {
        public LangDataAccess(IUnitOfWork uow) : base(uow)
        {

        }
    }
}
