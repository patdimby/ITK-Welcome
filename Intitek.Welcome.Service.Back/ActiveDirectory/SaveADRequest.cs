using Intitek.Welcome.Domain;

namespace Intitek.Welcome.Service.Back
{
    public class SaveADRequest
    {
        public AD AD { get; set; }      
        public string PwdKey { get; set; }
        public string PwdIV { get; set; }
        public bool fromBatch { get; set; }
    }
}
