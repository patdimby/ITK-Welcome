using Intitek.Welcome.Domain;
using System.Collections.Generic;

namespace Intitek.Welcome.Service.Back
{
    public class SaveMailTemplateRequest
    {
        public MailTemplate MailTemplate { get; set; }
        public List<string> CategorySubCategories { get; set; }
    }
}
