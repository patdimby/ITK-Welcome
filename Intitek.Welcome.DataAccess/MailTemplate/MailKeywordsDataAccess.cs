using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.UnitOfWork;
using Intitek.Welcome.Repository;

namespace Intitek.Welcome.DataAccess
{
    public class MailKeywordsDataAccess : Repository<MailKeywords, string>, IMailKeywordsDataAccess
    {
        public MailKeywordsDataAccess(IUnitOfWork uow) : base(uow)
        {

        }
    }
}
