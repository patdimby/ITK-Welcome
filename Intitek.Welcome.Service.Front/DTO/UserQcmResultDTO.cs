using System.Collections.Generic;

namespace Intitek.Welcome.Service.Front.DTO
{
    public class UserQcmResultDTO
    {
        public UserQcmDTO Qcm { get; set; }
        public List<UserQcmReponseDTO> Results { get; set; }
    }
}
