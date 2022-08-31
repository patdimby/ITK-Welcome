using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.UnitOfWork;
using Intitek.Welcome.Repository;

namespace Intitek.Welcome.DataAccess
{
    public class ConfigDataAccess : Repository<Config, string>, IConfigDataAccess
    {
        public ConfigDataAccess(IUnitOfWork uow) : base(uow)
        {

        }
    }
}
