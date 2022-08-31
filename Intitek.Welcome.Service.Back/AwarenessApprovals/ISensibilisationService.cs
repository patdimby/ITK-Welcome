using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.Service.Back
{
    public interface ISensibilisationService
    {
        List<SensibilisationDTO> ListSensibilisations(GetAwarenessApprovalsRequest request);
        List<SensibilisationDTO> ListSensibilisationsUsers(GetAwarenessApprovalsRequest request);
        List<SensibilisationDTO> ListSensibilisationsDocsPerUser(GetAwarenessApprovalsRequest request);
        List<SensibilisationDTO> ListSensibilisationsDocs(GetAwarenessApprovalsRequest request);
    }
}
