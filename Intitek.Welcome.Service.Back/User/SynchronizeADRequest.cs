using Intitek.Welcome.Domain;
using System.Collections.Generic;

namespace Intitek.Welcome.Service.Back
{
    public class SynchronizeADRequest
    {
        public List<UserAdDTO> ADUsers { get; set; }
        public List<UserAdDTO> ADBlackListedUsers { get; set; }
        public List<EntityAgencyDTO> BlackListedAgencies { get; set; }
        public int Id_AD { get; set; }
        public string Name_AD { get; set; }
    }
}
