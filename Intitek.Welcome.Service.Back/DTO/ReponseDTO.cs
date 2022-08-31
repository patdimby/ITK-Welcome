using Intitek.Welcome.Domain;

namespace Intitek.Welcome.Service.Back
{
    public class ReponseDTO
    {
        public Reponse Reponse { get; set; }
        public ReponseLang ReponseTrad { get; set; }
        public ReponseLang DefaultTrad { get; set; }
        public int IdLang { get; set; }
        public bool IsTrad { get; set; }
    }
}
