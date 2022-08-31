using Intitek.Welcome.Domain;

namespace Intitek.Welcome.Service.Front
{
    public class GetIntitekUserByLoginResponse
    {
        public IntitekUser User { get; set; }
        public string Logo { get; set; }
    }

    public class GetIntitekUserByEmailResponse
    {
        public IntitekUser User { get; set; }
        public string Logo { get; set; }
    }
}
