using Intitek.Welcome.Domain;
using Intitek.Welcome.Repository;
using Intitek.Welcome.Infrastructure.UnitOfWork;

namespace Intitek.Welcome.DataAccess
{
    public class EntityDocumentDataAccess : Repository<EntityDocument, int>, IEntityDocumentDataAccess
    {
        public EntityDocumentDataAccess(IUnitOfWork uow) : base(uow)
        {

        }
    }
}
