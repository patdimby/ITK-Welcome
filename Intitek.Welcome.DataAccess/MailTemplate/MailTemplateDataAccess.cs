using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.UnitOfWork;
using Intitek.Welcome.Repository;

namespace Intitek.Welcome.DataAccess
{
    public class MailTemplateDataAccess : Repository<MailTemplate, int>, IMailTemplateDataAccess
    {
        public MailTemplateDataAccess(IUnitOfWork uow) : base(uow)
        {

        }      
    }
}
