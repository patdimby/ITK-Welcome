using System.Collections.Generic;

namespace Intitek.Welcome.Service.Back
{
    public class GetAllUserADResponse
    {
        public bool IsOK { get; set; }
        public string ErrMessage { get; set; }
        public List<UserLDAP> Users { get; set; }
        public List<string> DiscardedUsers { get; set; }
    }
}
