using Intitek.Welcome.Infrastructure.Domain;
using Intitek.Welcome.Domain;
using System.Collections.Generic;

namespace Intitek.Welcome.DataAccess
{
    public interface IApprobationDataAccess : IRepository<IntitekUser, int>
    {
        IEnumerable<f_ListApprobations_Result> ListAll(string requestMonth);
        IEnumerable<ApprobationUserDTO> ListAllUsers(string requestMonth);
        IEnumerable<ApprobationDocPerUserDTO> ListAllDocsPerUser(string requestMonth);
        IEnumerable<ApprobationDocDTO> ListAllDocs(string requestMonth);
    }
}
