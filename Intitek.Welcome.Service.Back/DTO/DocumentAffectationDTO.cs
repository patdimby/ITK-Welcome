using System.Collections.Generic;

namespace Intitek.Welcome.Service.Back
{
    public class DocumentAffectationDTO
    {
        public string AffectationType { get; set; }
        public List<string> AffectedTo { get; set; }
    }
}
