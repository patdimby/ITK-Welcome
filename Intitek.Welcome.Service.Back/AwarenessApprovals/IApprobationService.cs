using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.Service.Back
{
    public interface IApprobationService
    {
        List<ApprobationDTO> ListApprobations(GetAwarenessApprovalsRequest request);
        List<ApprobationDTO> ListApprobationsUsers(GetAwarenessApprovalsRequest request);
        List<ApprobationDTO> ListApprobationsDocsPerUser(GetAwarenessApprovalsRequest request);
        List<ApprobationDTO> ListApprobationsDocs(GetAwarenessApprovalsRequest request);
    }
}
