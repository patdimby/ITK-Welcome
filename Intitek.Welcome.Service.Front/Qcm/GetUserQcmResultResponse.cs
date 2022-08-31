using Intitek.Welcome.Domain;
using System.Collections.Generic;

namespace Intitek.Welcome.Service.Front
{
    public class GetUserQcmResultResponse
    {
        public Qcm Qcm { get; set; }
    
        public List<UserQcmReponseDTO> UserReponses { get; set; }
    }
}
