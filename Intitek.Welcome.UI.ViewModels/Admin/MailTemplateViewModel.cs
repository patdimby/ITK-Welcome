using Intitek.Welcome.Service.Back;
using Intitek.Welcome.UI.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Intitek.Welcome.UI.ViewModels.Admin
{
    public class MailTemplateViewModel
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "name")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "comment")]
        public string Comment { get; set; }

        public string Object { get; set; }

        [AllowHtml]
        public string Content { get; set; }

        public List<MailKeywordsDTO> MailKeywords { get; set; }
        public List<SubCategoryViewModel> SubCategories { get; set; }
        public List<CategoryViewModel> Categories { get; set; }
        public List<int> SelectedCategories { get; set; }
        public List<int> SelectedSubCategories { get; set; }
        public List<string> CategorySubCategories { get; set; }
        public bool IsGlobal { get; set; }
        public bool IsDocNoCategory { get; set; }
        public bool IsDocNoSubCategory { get; set; }
    }
}
