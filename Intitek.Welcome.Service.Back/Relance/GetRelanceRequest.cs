using System.Collections.Generic;

namespace Intitek.Welcome.Service.Back
{
    public class GetRelanceRequest
    {
        public int IdLang { get; set; }
        public int IdDefaultLang { get; set; }
        public int MailTemplateID { get; set; }
        public int ProfilID { get; set; }
        public int UserID { get; set; }
        public string EntityName { get; set; }
        public string AgencyName { get; set; }
        public MailTemplateDTO MailTemplate { get; set; }
        public List<UserDTO> LstUsers { get; set; }
    }
}
