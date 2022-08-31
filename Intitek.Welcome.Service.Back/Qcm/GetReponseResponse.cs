using Intitek.Welcome.Domain;

namespace Intitek.Welcome.Service.Back
{
    public class GetReponseResponse
    {
        public Reponse Reponse { get; set; }
        public ReponseLang ReponseTrad { get; set; }
        public ReponseLang DefaultTrad { get; set; }
    }
}
