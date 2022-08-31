using Intitek.Welcome.Domain;
using System.Collections.Generic;

namespace Intitek.Welcome.Service.Back
{
    public class MailTemplateDTO
    {
        public const string TemplateRemindEntities = "TemplateRemindEntities";
        public const string TemplateRemindEmployees = "TemplateRemindEmployees";
        public int Id { get; set; }
        
        public string Name { get; set; }
        public string Comment { get; set; }
        public string Object { get; set; }
        public string Content { get; set; }
        public bool IsGlobal { get; set; }
        public bool IsDocNoCategory { get; set; }
        public bool IsDocNoSubCategory { get; set; }

        public List<DocumentCategory> Categories { get; set; }
        public List<SubCategory> SubCategories { get; set; }
    }
   
}
