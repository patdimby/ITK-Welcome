using Intitek.Welcome.Domain;
using Intitek.Welcome.Repository;
using Intitek.Welcome.Infrastructure.UnitOfWork;
using System.Data.Entity;

namespace Intitek.Welcome.DataAccess
{
    public class DocumentDataAccess : Repository<Document, int>, IDocumentDataAccess
    {
        public DocumentDataAccess(IUnitOfWork uow) : base(uow)
        {
        }
        public Database Database
        {
            get { return this.Context.Database; }
        }
    }
}
