using Intitek.Welcome.UI.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.UI.ViewModels.Admin
{
    public class CategoryViewModel
    {
        public int ID { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "category")]
        public string Name { get; set; }
        public string DefaultName { get; set; }
        public bool IsDefaultLangName { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Order")]
        public int? OrdreCategory { get; set; }
        public int NbDocuments { get; set; }
        public bool IsDeleted { get; set; }
        public List<SubCategoryViewModel> SubCategories { get; set; }
        public string NameDisplay {
            get {
                if(string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(DefaultName))
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
