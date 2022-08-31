using Intitek.Welcome.Domain;

namespace Intitek.Welcome.Service.Back
{
    public class SaveBlackListRequest
    {
        public BlackList BlackList { get; set; }
        public string Id { get; set; }
    }
}
