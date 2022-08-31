using Intitek.Welcome.Domain;
using Intitek.Welcome.Repository;
using Intitek.Welcome.Infrastructure.UnitOfWork;

namespace Intitek.Welcome.DataAccess
{
    public class DocumentVersionDataAccess : Repository<DocumentVersion, long>, IDocumentVersionDataAccess
    {
        public DocumentVersionDataAccess(IUnitOfWork uow) : base(uow)
        {

        }
    }
}
