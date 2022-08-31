using Intitek.Welcome.UI.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.UI.ViewModels
{
    public class SubCategoryViewModel
    {
        public int ID { get; set; }
        public int ID_DocumentCategory { get; set; }
        public string NameCategory { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "doc_SubCategories")]
        public string Name { get; set; }
        public int? OrdreSubCategory { get; set; }

    }
}
