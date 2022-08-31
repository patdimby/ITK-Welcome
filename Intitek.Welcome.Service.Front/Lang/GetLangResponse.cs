using System.Runtime.Serialization;
using Intitek.Welcome.Domain;

namespace Intitek.Welcome.Service.Front
{
    public class GetLangResponse
    {
        public Lang Langue { get; set; }
        [IgnoreDataMember]
        public string CodeLangue {
            get
            {
                if (this.Langue != null)
                    return this.Langue.Code.Substring(0,2);
                return string.Empty;
            }
        }
    }
}
