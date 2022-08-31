using Intitek.Welcome.UI.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.UI.ViewModels.Admin
{
    public class SubCategoryViewModel
    {
        public int ID { get; set; }
        public int ID_Category { get; set; }
        public int ID_OldCategory { get; set; }
        public int ID_DocumentCategory { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "subcategory")]
        public string Name { get; set; }
        public string NameCategory { get; set; }
        public string DefaultName { get; set; }
        public bool IsDefaultLangName { get; set; }
        public bool IsDefaultLangNameCategory { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Order")]
        public int? Ordre { get; set; }
        public List<CategoryViewModel> Categories { get; set; }
        public int NbDocuments { get; set; }
        public bool IsDeleted { get; set; }
        public string NameDisplay
        {
            get
            {
                if (string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(DefaultName))
                {
                    return DefaultName;
                }
                return Name;
            }
        }
        public string ClassDefautName
        {
            get
            {
                if (string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(DefaultName))
                {
                    return "defaultName";
                }
                return "";
            }
        }
    }
}
