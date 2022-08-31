using Intitek.Welcome.Domain;
using System.Collections.Generic;

namespace Intitek.Welcome.Service.Back
{
    public class GetUserResponse
    {
        public IntitekUser User { get; set; }
        public List<string> EntityNameList { get; set; }
        public List<string> AgencyNameList { get; set; }
        public List<UserDTO> UserList { get; set; }
    }
}
