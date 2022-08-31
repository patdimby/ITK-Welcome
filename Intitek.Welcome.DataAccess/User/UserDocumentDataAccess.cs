using Intitek.Welcome.Domain;
using Intitek.Welcome.Repository;
using Intitek.Welcome.Infrastructure.UnitOfWork;
using System;
using System.Linq;

namespace Intitek.Welcome.DataAccess
{
    public class UserDocumentDataAccess : Repository<UserDocument, int>, IUserDocumentDataAccess
    {
        public UserDocumentDataAccess(IUnitOfWork uow) : base(uow)
        {
            
        }
        public Boolean CheckDocUserGrant(int idDocument, int idUser)
        {
            return this.Context.Database.SqlQuery<Boolean>("SELECT dbo.f_CheckDocUserGrant(" + idDocument + "," + idUser + ") AS 'CHECK DROIT'").ToList().FirstOrDefault();
        }

    }
}
