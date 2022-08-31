using Intitek.Welcome.Infrastructure.Domain;
using Intitek.Welcome.Domain;
using System;

namespace Intitek.Welcome.DataAccess
{
    public interface IUserDocumentDataAccess : IRepository<UserDocument, int>
    {
        Boolean CheckDocUserGrant(int idDocument, int idUser);
    }
}
