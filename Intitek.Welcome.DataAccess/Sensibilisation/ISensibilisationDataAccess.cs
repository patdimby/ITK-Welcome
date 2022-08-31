using Intitek.Welcome.Infrastructure.Domain;
using Intitek.Welcome.Domain;
using System.Collections.Generic;

namespace Intitek.Welcome.DataAccess
{
    public interface ISensibilisationDataAccess : IRepository<IntitekUser, int>
    {
        IEnumerable<f_ListSensibilisations_Result> ListAll(string requestMonth);
        IEnumerable<SensibilisationUserDTO> ListAllUsers(string requestMonth);
        IEnumerable<SensibilisationDocPerUserDTO> ListAllDocsPerUser(string requestMonth);
        IEnumerable<SensibilisationDocDTO> ListAllDocs(string requestMonth);
    }
}
