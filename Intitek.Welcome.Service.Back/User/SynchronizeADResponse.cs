using Intitek.Welcome.Domain;
using System.Collections.Generic;

namespace Intitek.Welcome.Service.Back
{
    public class SynchronizeADResponse
    {
        public int Result { get; set; }
        public int Count { get; set; }

        public List<UserAdDTO> UsersADToSync { get; set; }
        public List<UserAdDTO> UsersADMailDiscarded { get; set; }
        public List<UserAdDTO> BlackListedUsers { get; set; }
        public List<IntitekUser> UsersToDelete { get; set; }
        public List<IntitekUser> UsersToUpdate { get; set; }
        public List<IntitekUser> UsersToAdd { get; set; }
        public List<string> CRUDErrors { get; set; }
        public List<string> SyntheseADErrors { get; set; }
        public string ErrorMessage { get; set; }
        public SynchronizeADResponse()
        {
            this.CRUDErrors = new List<string>();
            this.SyntheseADErrors = new List<string>();
        }

    }
}
