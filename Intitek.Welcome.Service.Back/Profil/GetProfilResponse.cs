using System.Collections.Generic;

namespace Intitek.Welcome.Service.Back
{
    public class GetProfilResponse
    {
        public ProfilDTO Profile {get; set;}
        public List<DocumentDTO> ListDocument { get; set; }
        public List<UserDTO> ListUser { get; set; }
    }
}
