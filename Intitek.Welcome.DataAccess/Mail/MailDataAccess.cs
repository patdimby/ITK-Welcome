using Intitek.Welcome.Domain;
using Intitek.Welcome.Repository;
using Intitek.Welcome.Infrastructure.UnitOfWork;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Core.Objects;

namespace Intitek.Welcome.DataAccess
{
    public class MailDataAccess : Repository<IntitekUser, int>, IMailDataAccess
    {
        public MailDataAccess(IUnitOfWork uow) : base(uow)
        {

        }
        public string SwapEmail(string oldEmail, string newEmail)
        {
            ObjectParameter objectParameter = new ObjectParameter("Retour", typeof(string));
            this.Context.p_SwapEmails( oldEmail,  newEmail, objectParameter);
            return objectParameter.Value.ToString();
        
         }
        
    }
}
