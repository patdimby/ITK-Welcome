using System.Collections.Generic;

namespace Intitek.Welcome.Service.Back
{
    public class GetAllBlackListResponse
    {
        public List<BlackListDTO> BlackLists { get; set; }
        public List<CityEntityBlacklistDTO> CityEntityBlacklists { get; set; }
    }
}
